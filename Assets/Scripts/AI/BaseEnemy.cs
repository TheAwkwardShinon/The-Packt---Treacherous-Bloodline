using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ThePackt
{
	public delegate void SpecificAttack();
	public delegate bool CheckSpecificRange();

	public class BaseEnemy : Enemy
	{
		#region variables

		[Header("Specific")]
		[SerializeField] private float _reactionTime;
		[SerializeField] private float _jumpVelocity;
		[SerializeField] private float _jumpCooldown; //1 for prototype and 3 for mage
		[SerializeField] private float _avoidRange;
		[SerializeField] private float _perceptionRange;
		[SerializeField] private float _targetHealthWeight;

		private Vector2 _currentVelocity;
		private Collider2D[] _nearPlayers;
		private bool _isGrounded;

		protected SpecificAttack _specificAttack;
		protected CheckSpecificRange _checkSpecificRange;

		#region wandering
		[Header("Wandering")]
		[SerializeField] private float _minChangeDirectionTime;
		[SerializeField] private float _maxChangeDirectionTime;
		[SerializeField] private float _minStandStillTime;
		[SerializeField] private float _maxStandStillTime;
		private float _lastDirectionChangeTime;
		private float _changeDirectionTime;
		private float _lastStandStillTime;
		private float _standStillTime;
		#endregion

		#region seek
		[Header("Seek")]
		[SerializeField] private float _seekThreshold;
		[SerializeField] private float _jumpPointThreshold;
		[SerializeField] private float _unreachabilityTime;
		[SerializeField] private float _reachabilityRestoreTime;
		[SerializeField] private float _keepSameTargetTime;
		private float _xDiff;
		protected bool _attacking;
		protected float _lastAttackTime;
		private float _lastJumpTime;
		private bool _checkSpecificRangeResult;

		#region target selection
		protected BoltEntity _target;
		private bool _targetCanBeSet;
		private float _lastTargetChangedTime;
		#endregion

		#region pathfinding
		private Platform _nextPlatform;
		private Platform _currentPlatform;
		private Platform _lastTargetPlatform;
		private Utils.NullableVector2 _jumpPoint;
		private Vector2 _jumpNeededVelocity;
		private int[][] _adjacencyMatrix;
		#endregion
		#endregion

		#endregion

		#region methods

		protected override void Awake()
		{
			base.Awake();

			_nearPlayers = new Collider2D[6];
		}

		//define the FSM
		public override void Attached()
		{
			base.Attached();

			#region wander fsm
			//if idle in wandering check if the time to be idle ended and if so change state to wandering walk (at the transition the walk timer 
			//starts)
			FSMState wanderIdle = new FSMState();
			wanderIdle.enterActions.Add(StartWanderIdle);
			wanderIdle.exitActions.Add(StopIdle);

			//if walking in wandering walk in front and change direction to avoid walls or pits. then check if the time to be walking ended
			//and if so change state to wandering idle (at the transition the idle timer starts)
			FSMState wanderWalk = new FSMState();
			wanderWalk.enterActions.Add(StartWanderWalking);
			wanderWalk.stayActions.Add(Walk);
			wanderWalk.stayActions.Add(Avoid);
			wanderWalk.exitActions.Add(StopWalking);

			/*
			FSMState wanderJump = new FSMState();
			wanderJump.enterActions.Add(StartJumping);
			wanderJump.stayActions.Add(Jump);
			wanderJump.exitActions.Add(StopJumping);

			FSMTransition MustJumpTrans = new FSMTransition(MustJump);
			wanderWalk.AddTransition(MustJumpTrans, wanderJump);

			FSMTransition LandedTrans = new FSMTransition(Landed);
			wanderJump.AddTransition(LandedTrans, wanderWalk);
			*/

			//switch to idle also if stunned
			FSMTransition WanderStunTrans = new FSMTransition(IsStunned, new FSMAction[] { StunnedReaction });
			FSMTransition WalkTimeoutTrans = new FSMTransition(MustStandStill, new FSMAction[]{ WalkTimeoutReaction });
			wanderWalk.AddTransition(WanderStunTrans, wanderIdle);
			wanderWalk.AddTransition(WalkTimeoutTrans, wanderIdle);

			FSMTransition IdleTimeoutTrans = new FSMTransition(MustNotStandStill, new FSMAction[] { IdleTimeoutReaction });
			wanderIdle.AddTransition(IdleTimeoutTrans, wanderWalk);

			FSM fsmWander = new FSM(wanderWalk);

			//the wander superstate just updates the inner FSM
			FSMState wander = new FSMState();
			wander.enterActions.Add(StartWandering);
			wander.stayActions.Add(fsmWander.Update);
			#endregion

			#region seek fsm
			//if idle while seeking flip towards the target is needed and check if the target is in attack range and the enemy can attack it,
			//if so pass to the attack state. Otherwise if the target is not in range or the enemy attack could not reach it pass to the
			//walk state. Otherwise check if under the enemy's feet there is an anemy or a player, if so pass to the jumping state to jump away 
			FSMState seekIdle = new FSMState();
			seekIdle.enterActions.Add(StartSeekIdle);
			seekIdle.stayActions.Add(FlipIfNeeded);
			seekIdle.stayActions.Add(CheckSpecificRangeResult);
			seekIdle.exitActions.Add(StopIdle);

			//if walking while seeking check the target platform and do pathfinding if needed, then walk in the direction needed for the
			//pathfinding. if the target is in attack range and the enemy can attack it pass to the attack state, otherwise check
			//if the enemy should pass to the jump state because is near a jump point or because under the enemy's feet there
			//is an enemy or a player. Otherwise check if the target is in range and the enemy attack could reach it, but the attack is still
			//in cooldown, if so pass to the idle state 
			FSMState seekWalk = new FSMState();
			seekWalk.enterActions.Add(StartSeekWalking);
			seekWalk.stayActions.Add(CheckTargetPlatform);
			seekWalk.stayActions.Add(Walk);
			seekWalk.stayActions.Add(CheckSpecificRangeResult);
			seekWalk.exitActions.Add(StopWalking);

			//if attacking while seeking just flip towards target, attack once then go to idle state
			FSMState seekAttack = new FSMState();
			seekAttack.enterActions.Add(StartAttacking);
			seekAttack.stayActions.Add(FlipIfNeeded);
			seekAttack.stayActions.Add(Attack);
			seekAttack.exitActions.Add(StopAttacking);

			//if jumping while seeking just jump once and keep always the same velocity until landed, then go to idle state
			FSMState seekJump = new FSMState();
			seekJump.enterActions.Add(StartJumping);
			seekJump.stayActions.Add(Jump);
			seekJump.exitActions.Add(StopJumping);

			//switch to idle also if stunned
			FSMTransition TargetInRangeTrans = new FSMTransition(IsTargetInRange);
			FSMTransition TargetReachedTrans = new FSMTransition(IsTargetNear);
			FSMTransition StunTrans = new FSMTransition(IsStunned);
			//the jump velocity is prepared when the transition to jump is triggered
			FSMTransition JumpTrans = new FSMTransition(ShouldIJump, new FSMAction[] { JumpPrepare });
			FSMTransition JumpAwayTrans = new FSMTransition(ShouldIJumpAway, new FSMAction[] { JumpAwayPrepare });
			seekWalk.AddTransition(StunTrans, seekIdle);
			seekWalk.AddTransition(TargetInRangeTrans, seekAttack);
			seekWalk.AddTransition(JumpTrans, seekJump);
			seekWalk.AddTransition(JumpAwayTrans, seekJump);
			seekWalk.AddTransition(TargetReachedTrans, seekIdle);

			FSMTransition TargetFarTrans = new FSMTransition(MustReachTarget);
			seekIdle.AddTransition(TargetInRangeTrans, seekAttack);
			seekIdle.AddTransition(TargetFarTrans, seekWalk);
			seekIdle.AddTransition(JumpAwayTrans, seekJump);

			FSMTransition AttackEndTrans = new FSMTransition(AttackFinished);
			seekAttack.AddTransition(AttackEndTrans, seekIdle);

			FSMTransition JumpEndTrans = new FSMTransition(JumpFinished);
			seekJump.AddTransition(JumpEndTrans, seekIdle);

			FSM fsmSeek = new FSM(seekWalk);

			//the seek superstate just updates the inner FSM
			FSMState seek = new FSMState();
			seek.enterActions.Add(StartSeeking);
			seek.stayActions.Add(SelectTarget);
			seek.stayActions.Add(fsmSeek.Update);
			#endregion

			//global fsm that starts from the wandering state and goes to seek state when there is a target. When there is no more a target
			//it returns to wandering
			//the target is set when the transition to seeking is triggered
			FSMTransition playerNearTrans = new FSMTransition(PlayerNear);
			FSMTransition hitByPlayerTrans = new FSMTransition(HitByPlayer, new FSMAction[] { HitByPlayerReaction });
			wander.AddTransition(playerNearTrans, seek);
			wander.AddTransition(hitByPlayerTrans, seek);

			FSMTransition NoTargetTrans = new FSMTransition(NoTarget);
			seek.AddTransition(NoTargetTrans, wander);

			_fsm = new FSM(wander);

			//the coroutine updates the FSM every _reactionTime 
			StartCoroutine(Patrol());
		}

		public override void SimulateOwner()
		{
			_currentVelocity = _rb.velocity;

			_isGrounded = IsGrounded();

			//if in seek state
			if (_target)
			{
				_xDiff = _target.GetComponent<Player>().transform.position.x - transform.position.x;

				//update the target's grounded state variable
                if (_target.IsAttached && !_target.IsOwner)
                {
					_target.GetComponent<Player>().CheckIfGrounded();
				}

				//find the target current platform
                if (_target.GetComponent<Player>()._isGrounded)
                {
					BoxCollider2D targetCollider = _target.GetComponent<BoxCollider2D>();
					_lastTargetPlatform = GetPlatform(targetCollider);
				}

				//find the enemy's current platform
				if (_isGrounded)
                {
					_currentPlatform = GetPlatform((BoxCollider2D) _col);
				}
			}
		}

		///<summary>
		///updates the FSM each _reactionTime
		///</summary>
		public IEnumerator Patrol()
		{
			while (true)
			{
				_fsm.Update();
				yield return new WaitForSeconds(_reactionTime);
			}
		}

		/*
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			if (Math.Abs(transform.rotation.y % 180) == 1)
			{
				Gizmos.DrawRay(transform.position - new Vector3(0, -0.1f, 0), -transform.right * _avoidRange);
			}
			else
			{
				Gizmos.DrawRay(transform.position + new Vector3(0, 0.1f, 0), -transform.right * _avoidRange);
			}
			
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, _attackRange);

			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, _perceptionRange);

			Gizmos.color = Color.blue;
			if (Math.Abs(transform.rotation.y % 180) == 1)
			{
				Gizmos.DrawRay(transform.position + new Vector3(_seekThreshold, -0.1f, 0), transform.right * _seekThreshold * 2);
			}
			else
			{
				Gizmos.DrawRay(transform.position - new Vector3(_seekThreshold, 0.1f, 0), transform.right * _seekThreshold * 2);
			}

			Gizmos.color = Color.magenta;
			Gizmos.DrawRay(transform.position + new Vector3(0, _col.bounds.size.y, 0), -transform.right * 3);

			Gizmos.color = Color.white;
			Vector2 start = _col.bounds.center + Vector3.down * _col.bounds.size.y * 0.5f + Vector3.right * _facingDirection * _avoidRange;
			Gizmos.DrawRay(start, -transform.up * 0.1f);
		}
		*/

		#region fsm enter actions 
		//methods run when entering the corresponding a state

		private void StartWanderIdle()
		{
			Debug.Log("[BASEENEMY] idle");

			_rb.velocity = new Vector2(0, _currentVelocity.y);

			//start animation?
		}

		private void StartSeekIdle()
		{
			Debug.Log("[BASEENEMY] idle");

			_rb.velocity = new Vector2(0, _currentVelocity.y);

			if(_target) 
				CheckSpecificRangeResult();

			//start animation?
		}

		private void StartJumping()
		{
			Debug.Log("[BASEENEMY] jumping");

			if (_nextPlatform != null && _currentPlatform != null && _jumpPoint != null)
			{
				//face towards the next platform if i have one
				float centerX = _nextPlatform.GetPlatformCenter().x;
				int targetDirection = (centerX - transform.position.x) > 0 ? 1 : -1;

				//flip if the target direction differ from the facing direction
				if (targetDirection != _facingDirection)
					Flip();
			}

			_rb.velocity = new Vector2(_jumpNeededVelocity.x * _facingDirection, _jumpNeededVelocity.y);
			_lastJumpTime = Time.time;

			//start animation?
		}

		private void StartWanderWalking()
		{
			Debug.Log("[BASEENEMY] walking");

			//if it is the first update in wandering, do not flip
			if (_lastDirectionChangeTime == 0)
				_lastDirectionChangeTime = Time.time;

			//start animation?
		}

		private void StartSeekWalking()
		{
			Debug.Log("[BASEENEMY] walking");

			if (_target)
				CheckSpecificRangeResult();

			//start animation?
		}

		private void StartAttacking()
		{
			Debug.Log("[BASEENEMY] attacking");

			_attacking = true;
			_rb.velocity = new Vector2(0, _currentVelocity.y);

			//start animation?
		}

		private void StartWandering()
		{
			Debug.Log("[BASEENEMY] wandering");
			Debug.Log("[TRGT] now in wandering, target can be changed");

			//reset all target related variables
			_lastAttacker = null;
			_lastTargetHitTime = 0f;
			_targetCanBeSet = true;
			_nextPlatform = null;
			_currentPlatform = null;
			_jumpPoint = null;

			//set the wandering related variables to their initial values
			_lastDirectionChangeTime = 0f;
			_changeDirectionTime = 2f;
			_lastStandStillTime = 0f;
			_standStillTime = 1f;
		}

		private void StartSeeking()
		{
			Debug.Log("[BASEENEMY] seeking");

			//set the adjacency matrix for pathfinding based on the type of room
			if (_room)
				_adjacencyMatrix = Constants.ADJACENCYMATRIXENEMYQUEST;
			else
				_adjacencyMatrix = Constants.ADJACENCYMATRIXMAINQUEST;
		}
		#endregion

		#region fsm stay actions

		#region wander actions

		///<summary>
		///changes facing direction if needed and then walks straigth forward. used for both wandering and seeking
		///</summary>
		private void Walk()
		{
			//if there is not a target (so we are wandering) go in the current direction
			if (!_target)
				if (_slowed)
					_rb.velocity = new Vector2(_slowedSpeed * _facingDirection, _currentVelocity.y);
				else
					_rb.velocity = new Vector2(_movementSpeed * _facingDirection, _currentVelocity.y);
			else
			{
				//if there is a target seek it

				//check if what i must reach is at my right or at my left. if its center has a higher x it means it is at my right
				//if i'm not facing in the right direction flip

				int targetDirection = 1;
				//if i did pathfinding and i have to jump on a higher platform or at the same level i have a jump point to reach
				if (_nextPlatform != null && _currentPlatform != null && _jumpPoint != null)
                {
					targetDirection = (_jumpPoint.vector.x - transform.position.x) > 0 ? 1 : -1;
				}
				else if (!_isGrounded)
				{
					//if i'm in air keep the same direction
					targetDirection = _facingDirection;
				}
				else if(_nextPlatform != null && _currentPlatform != null && _jumpPoint == null)
				{
					//if i did pathfinding and i have to jump on a lower platform i just face towards the next platform
					targetDirection = (_nextPlatform.GetPlatformCenter().x - transform.position.x) > 0 ? 1 : -1;
				}
                else
                {
					//otherwise just face towards the target
					targetDirection = _xDiff > 0 ? 1 : -1;
				}

				//flip if the target direction differ from the facing direction
				if (targetDirection != _facingDirection)
				{
					Flip();
				}

				//then i just walk in the facing direction if i'm not jumping or attacking in the next update
				if (_slowed)
					_rb.velocity = new Vector2(_slowedSpeed * targetDirection, _currentVelocity.y);
				else
					_rb.velocity = new Vector2(_movementSpeed * targetDirection, _currentVelocity.y);
			}
		}

		///<summary>
		///checks if there is an obstacle or a pit forward, if so makes the enemy flip
		///</summary>
		private void Avoid()
		{
			//cast the collider forward to check for obstacles
			RaycastHit2D[] hits = new RaycastHit2D[1];
			ContactFilter2D filter = new ContactFilter2D();
			filter.SetLayerMask(LayerMask.GetMask("Ground", "Wall", "EnemyInvisibleWall"));
			int numHits = _col.Cast(-transform.right, filter, hits, _avoidRange, true);

			if (numHits > 0)
				Flip();
            else
            {
				//cast a ray forward and down to check for pits
				Vector2 _workspace = _col.bounds.center + Vector3.down * _col.bounds.size.y * 0.5f + Vector3.right * _facingDirection * _avoidRange;
				RaycastHit2D groundInFront = Physics2D.Raycast(_workspace, -transform.up, 0.1f, LayerMask.GetMask("Ground", "EnemyInvisibleGround"));

				if (!groundInFront)
                {
					Flip();
				}
			}
		}

		///<summary>
		///assigns to _checkSpecificRangeResult the result of the delegate _checkSpecificRange
		///</summary>
		private void CheckSpecificRangeResult()
		{
			_checkSpecificRangeResult = _checkSpecificRange();
		}
		#endregion

		#region seek actions

		///<summary>
		///select the target to seek based on the reachability, the damage inflicted by the players and their life points. The target can be
		///changed only each _keepSameTargetTime or if the current is not in room or not alive
		///</summary>
		private void SelectTarget()
		{
			//sense near players if some players have never been sensed
			if(_damageMap.Count < 6)
            {
				ContactFilter2D filter = new ContactFilter2D();
				filter.SetLayerMask(LayerMask.GetMask("Players"));
				int nearPlayersNum = Physics2D.OverlapCircle(transform.position, _perceptionRange, filter, _nearPlayers);
				foreach (var col in _nearPlayers)
					if (col)
					{
						BoltEntity ent = col.gameObject.GetComponent<Player>().entity;
						if (!_damageMap.ContainsKey(ent))
							_damageMap.Add(ent, 0);
					}
			}

			//remove from the unreachable ones the targets whose unreachability has to be restored
			List<BoltEntity> toRemove = new List<BoltEntity>();
			foreach(var entry in _unreachableTargets)
				if (Time.time >= entry.Value + _reachabilityRestoreTime)
					toRemove.Add(entry.Key);
			foreach(var elem in toRemove)
				_unreachableTargets.Remove(elem);

			//if i couldn't hit the current target for a certain amount of time (_unreachabilityTime), the target can be changed
			//and the current target is added to the unreachable ones
			bool inRoomAlive = IsInRoomAndAlive(_target);
			if (Time.time >= _lastTargetHitTime + _unreachabilityTime)
			{
				Debug.Log("[TRGT] target unreachable, target can be changed");
				_targetCanBeSet = true;
				_lastTargetHitTime = Time.time;

				if (_unreachableTargets.ContainsKey(_target))
					_unreachableTargets[_target] = Time.time;
				else
					_unreachableTargets.Add(_target, Time.time);
			}
            else
            {
				//otherwise, check if the time that must pass after the target can be changed has passed 
				//(_keepSameTargetTime). the target can be set anyway if the current target is not in room or is dead
				if (!_targetCanBeSet && (Time.time >= _keepSameTargetTime + _lastTargetChangedTime || !inRoomAlive))
				{
					_targetCanBeSet = true;
					Debug.Log("[TRGT] can be changed now");
				}
			}

			//remove the players that are not alive anymore from the _damageMap
			toRemove = new List<BoltEntity>();
			foreach (var entry in _damageMap)
				if (!entry.Key.IsAttached)
					toRemove.Add(entry.Key);
			foreach (var elem in toRemove)
				_damageMap.Remove(elem);

			if (_targetCanBeSet && _damageMap.Count > 0)
			{
				bool set = false;

				//if the target can now be changed, pick the player that has the highest score: dealt damage + lost life points and that is:
				//- still in the room
				//- still alive
				//- not in the unreachable list
				var sortedDict = from entry in _damageMap orderby (entry.Value + _targetHealthWeight * entry.Key.GetComponent<Player>().GetLostHealth()) descending select entry;
				foreach (var entry in sortedDict)
					if (entry.Key && IsInRoomAndAlive(entry.Key) && !_unreachableTargets.ContainsKey(entry.Key))
					{ 
						SetTarget(entry.Key);
						set = true;
						break;
					}

				//if no player satisfied the requirements keep the current if in room and alive
				if (!set && !inRoomAlive)
                {
					//otherwise iter the unreachable ones from the least recenly inserted until a player alive and in room is found, pick
					//it and hope it is now reachable
					foreach (var ent in _unreachableTargets.Keys)
						if (IsInRoomAndAlive(ent))
						{
							_unreachableTargets.Remove(ent);
							SetTarget(ent);
							set = true;
							break;
						}

					//if no target was set, it means no target is available at all. So set it to null, so that the transition will
					//make the state change to wandering 
					if (!set && !IsInRoomAndAlive(_target))
						_target = null;
				}
			}
		}

		///<summary>
		///attacks by executing the _specificAttack delegates that will perform the specific attack for the specific enemy
		///</summary>
		private void Attack()
		{
			_specificAttack();
			_attacking = false;
		}

		///<summary>
		///flips the enemy towards the target if it is not already facing in that direction
		///</summary>
		private void FlipIfNeeded()
		{
			int targetDirection = _xDiff > 0 ? 1 : -1;
			if (targetDirection != _facingDirection)
			{
				Flip();
			}
		}

		///<summary>
		///makes the enemy stop going forward while falling if the next platform's center is near
		///</summary>
		private void Jump()
		{
			if (_nextPlatform != null && Math.Abs(_nextPlatform.GetPlatformCenter().x - _col.bounds.center.x) <= _jumpPointThreshold)
			{
				_rb.velocity = new Vector2(0f, _currentVelocity.y);
			}
		}

		///<summary>
		///returns true if the enemy is grounded and target is in room, but not in attack range or the attack could not reach it,, 
		///false otherwise
		///</summary>
		private bool IsTargetFar()
		{
            if (_isGrounded && IsInRoomAndAlive(_target))
            {
				return !_checkSpecificRangeResult;
				//return !_checkSpecificRange();
			}

			return false;
		}

		///<summary>
		///returns true if the enemy is grounded, the target is in room, in attack range and the attack could reach it, false otherwise
		///</summary>
		private bool IsTargetNear()
		{
			if (_isGrounded && IsInRoomAndAlive(_target))
			{
				return _checkSpecificRangeResult;
				//return _checkSpecificRange();
			}

			return false;
		}

		///<summary>
		///returns true if the target is not more distant than _jumpPointThreshold to the jump point, if the jump is not on cooldown and if 
		///the enemy is grounded
		///</summary>
		private bool ShouldIJump()
		{
			if (!_stunned && !_slowed && _target && _isGrounded && Time.time >= _lastJumpTime + _jumpCooldown)
			{
				if(_nextPlatform != null && _currentPlatform != null && _jumpPoint != null && Math.Abs(_jumpPoint.vector.x - _col.bounds.center.x) <= _jumpPointThreshold)
                {
					return true;
				}
			}

			return false;
		}

		///<summary>
		///returns true if the target is on top of another enemy or on a player
		///</summary>
		private bool ShouldIJumpAway()
        {
			if(!_stunned && !_slowed)
            {
				//if i'm not already jumping at the next update check under the feet if there is an enemy or a player
				Vector2 _workspace = _col.bounds.center + Vector3.down * (_col.bounds.size.y * 0.5f + 0.1f);
				Collider2D hit = Physics2D.OverlapBox(_workspace, new Vector3(_col.bounds.size.x, 0.005f, 0f), 0f, LayerMask.GetMask("Players", "Enemies"));
				if (hit)
				{
					return true;
				}
			}

			return false;
		}

		///<summary>
		///compare the target platform and the enemy platform and if they are different do pathfinding
		///</summary>
		private void CheckTargetPlatform()
		{
			//if i'm on the ground
			if (_isGrounded)
            {
				//if the target is on another platform
				if (_currentPlatform != null && _lastTargetPlatform != null && _lastTargetPlatform.num != _currentPlatform.num)
				{
					//get the next platform to reach in the path to the target platform using the adjacency matrix
					_nextPlatform = GetPlatform(_adjacencyMatrix[_currentPlatform.num][_lastTargetPlatform.num]);

					//if the next platform is above pick the jump point
					if (_currentPlatform.left.position.y <= _nextPlatform.left.position.y)
						_jumpPoint = new Utils.NullableVector2(_currentPlatform.GetPlatformCenter());
                    else
						_jumpPoint = null;
				}
				else
				{
					_nextPlatform = null;
					_jumpPoint = null;
				}
			}
            else
				_jumpPoint = null;
		}

		///<summary>
		///returns true if the enemy is grounded, not stunned, the attack is not in cooldown and the target is in room, in 
		///attack range and the attack could reach it
		///</summary>
		private bool IsTargetInRange()
		{
			//if i'm not stunned, the attacj is not on cooldown and the target is in room and alive
			if (!_stunned && _isGrounded && (Time.time >= _lastAttackTime + _attackRate) && IsInRoomAndAlive(_target))
			{
				return _checkSpecificRangeResult;
				//return _checkSpecificRange();
			}

			return false;
		}
		#endregion

		#endregion

		#region fsm exit actions
		//methods run when exiting the corresponding a state

		private void StopIdle()
		{
			//end animation?
		}

		private void StopJumping()
		{
			//end animation?
		}

		private void StopWalking()
		{
			//end animation?
		}

		private void StopAttacking()
		{
			//end animation?
		}
		#endregion

		#region transitions actions
		//methods run when the corresponding transition is triggered

		///<summary>
		///sets the target as the last attacker
		///</summary>
		private void HitByPlayerReaction()
		{
			SetTarget(_lastAttacker);
		}

		///<summary>
		///starts a random idle timer
		///</summary>
		private void WalkTimeoutReaction()
		{
			_lastStandStillTime = Time.time;

			//generate a random stand still time
			_standStillTime = UnityEngine.Random.Range(_minStandStillTime, _maxStandStillTime);
		}

		///<summary>
		///starts a idle timer with duration qual to _stunTime
		///</summary>
		private void StunnedReaction()
		{
			_lastStandStillTime = Time.time;

			//generate a random stand still time
			_standStillTime = _stunTime;
		}

		///<summary>
		///makes the enemy flip and starts a random walk timer
		///</summary>
		private void IdleTimeoutReaction()
		{
			Flip();
			_lastDirectionChangeTime = Time.time;

			//generate a random change direction time
			_changeDirectionTime = UnityEngine.Random.Range(_minChangeDirectionTime, _maxChangeDirectionTime);
		}

		///<summary>
		///sets the velocity components needed for the jump
		///</summary>
		private void JumpPrepare()
		{
			float yMax;
			float range;
			//if the current platform is lower than the next one
			if (_currentPlatform.left.position.y < _nextPlatform.left.position.y)
			{
				//the max height to reach with the jump is 1 size of the collider above the next platform
				yMax = (_nextPlatform.left.position.y + (_col.bounds.size.y * 1f)) - _col.bounds.center.y;

				//float arrivalX = _nextPlatform.left.position.x > _col.bounds.center.x ? _nextPlatform.left.position.x - _col.bounds.size.x : _nextPlatform.right.position.x + _col.bounds.size.x;
				//the range to reach with the jump is 2 size of the collider inside the platform
				float arrivalX = _nextPlatform.left.position.x > _col.bounds.center.x ? _nextPlatform.left.position.x + _col.bounds.size.x * 1.75f : _nextPlatform.right.position.x - _col.bounds.size.x * 1.75f;
				range = Math.Abs(arrivalX - _col.bounds.center.x);
			}
			else
			{
				//if the current platform is at the same height as the next one

				//the max height to reach with the jump is 1.5 size of the collider above the next platform
				yMax = (_nextPlatform.left.position.y + (_col.bounds.size.y * 1.5f)) - _col.bounds.center.y;
				//the range to reach with the jump is the next platform's center
				range = Math.Abs(_nextPlatform.GetPlatformCenter().x - _col.bounds.center.x);
			}

			//with the projectile trajectory formulas determine the needed velocity components
			float vY = Mathf.Sqrt(yMax * -(Physics2D.gravity.y * 2));
			float vX = (range * -Physics2D.gravity.y) / (2 * vY);
			_jumpNeededVelocity = new Vector2(vX, vY);
		}

		///<summary>
		///sets the velocity components needed for the jump and makes the enemy flip to a random direction
		///</summary>
		private void JumpAwayPrepare()
		{
			_jumpNeededVelocity = new Vector2(_movementSpeed, _jumpVelocity);

			//also flip to a random direction
			int targetDirection = UnityEngine.Random.Range(0, 2);
			if (targetDirection == 0)
				targetDirection--;
			if (targetDirection != _facingDirection)
				Flip();
		}
		#endregion

		#region fsm conditions
		//conditions that trigger transitions based on how the state variables were setted

		///<summary>
		///returns true if the walk timer timeouted
		///</summary>
		private bool MustStandStill()
		{ 
			if (Time.time >= _lastDirectionChangeTime + _changeDirectionTime)
			{
				return true;
			}

			return false;
		}

		///<summary>
		///returns true if the idle timer timeouted
		///</summary>
		private bool MustNotStandStill()
		{
			if (Time.time >= _lastStandStillTime + _standStillTime)
			{
				return true;
			}

			return false;
		}

		private bool IsStunned()
		{
			return _stunned;
		}

		private bool MustReachTarget()
		{
			return IsTargetFar() && !_stunned;
		}

		private bool JumpFinished()
		{
			return IsLanded();
		}

		private bool AttackFinished()
		{
			return !_attacking;
		}

		private bool NoTarget()
		{
			return _target == null;
		}

		///<summary>
		///returns true and sets as target the first player in room in the sensing range if there is one, false otherwise
		///</summary>
		private bool PlayerNear()
		{ 
			//find near players
			ContactFilter2D filter = new ContactFilter2D();
			filter.SetLayerMask(LayerMask.GetMask("Players"));
			int nearPlayersNum = Physics2D.OverlapCircle(transform.position, _perceptionRange, filter, _nearPlayers);

			if (nearPlayersNum > 0)
			{
				//pick as target the first one that is in room and alive
				BoltEntity nearPlayer = _nearPlayers[0].gameObject.GetComponent<Player>().entity;
				if (IsInRoomAndAlive(nearPlayer))
				{
					SetTarget(nearPlayer);
					return true;
				}
			}

			return false;
		}

		///<summary>
		///returns true if a player in the room hit the enemy, false otherwise
		///</summary>
		private bool HitByPlayer()
		{
			if (_lastAttacker != null && IsInRoomAndAlive(_lastAttacker))
			{
				return true;
			}

			return false;
		}
		#endregion

		#region others

		///<summary>
		///switches the facing direction
		///</summary>
		private void Flip()
		{
			_facingDirection *= -1;
			transform.Rotate(0f, 180f, 0f);
		}

		///<summary>
		///returns the platform on which the passed collider is standing
		///</summary>
		private Platform GetPlatform(BoxCollider2D collider)
        {
			//find the right bottom corner of the collider and cast a short ray down from there
			Vector2 rightCorner = new Vector2(collider.bounds.center.x + (collider.bounds.size.x / 2) + 0.1f, collider.bounds.center.y - (collider.bounds.size.y / 2));
			var hit = Physics2D.Raycast(rightCorner, new Vector2(0f, -1f), 0.2f, LayerMask.GetMask("Ground", "EnemyInvisibleGround"));

			//if it hits some ground use the hit point to determine the platform
			Vector2 position = Vector2.zero;
            if (hit)
				position = hit.point;
            else
            {
				//otherwise find the left bottom corner of the collider and cast a short ray down from there
				Vector2 leftCorner = new Vector2(collider.bounds.center.x - (collider.bounds.size.x / 2) - 0.1f, collider.bounds.center.y - (collider.bounds.size.y / 2));
				hit = Physics2D.Raycast(leftCorner, new Vector2(0f, -1f), 0.2f, LayerMask.GetMask("Ground", "EnemyInvisibleGround"));
			
				//if it hits some ground use the hit point to determine the platform
				if (hit)
					position = hit.point;
			}

			//find the platform on which the hit point is
			foreach (var platform in _platforms)
            {
				if (Math.Abs(position.y - platform.left.position.y) <= 0.01f && position.x + 0.01f >= platform.left.position.x && position.x - 0.01f <= platform.right.position.x)
                {
					return platform;
                }
            }

			return null;
        }

		///<summary>
		///returns the platform with the passed number
		///</summary>
		private Platform GetPlatform(int num)
		{
			foreach (var platform in _platforms)
			{
				if (platform.num == num)
				{
					return platform;
				}
			}

			return null;
		}

		///<summary>
		///set _lastTargetHitTime to the current time if the passed entity is the target
		///</summary>
		public void RegisterTargetHit(BoltEntity ent)
		{
			if (_target == ent)
			{
				_lastTargetHitTime = Time.time;
			}
		}

		///<summary>
		///returns true if the passed entity is inside the room and is still alive, false otherwise
		///</summary>
		protected bool IsInRoomAndAlive(BoltEntity plyr)
		{
			if (_room != null)
            {
				if (plyr && plyr.IsAttached && _room.CheckIfPlayerIsInRoom(plyr))
					return true;
			}
			else
				if (plyr && plyr.IsAttached && MainQuest.Instance.CheckIfPlayerIsInRoom(plyr))
					return true;

			return false;
		}

		///<summary>
		///returns true if the enemy is on the ground, false otherwise
		///</summary>
		protected bool IsGrounded()
		{
			//check for ground under the enemy's feet
			Vector2 _workspace = _col.bounds.center + Vector3.down * _col.bounds.size.y * 0.5f;
			return Physics2D.OverlapBox(_workspace, new Vector3(_col.bounds.size.x - 0.01f, 0.1f, 0f), 0f, LayerMask.GetMask("Ground", "EnemyInvisibleGround"));
		}

		///<summary>
		///returns true if the enemy is on the ground, on players or on other enemies, false otherwise
		///</summary>
		private bool IsLanded()
		{
			//check for ground, players or enemies under the enemy's feet
			Vector2 _workspace = _col.bounds.center + Vector3.down * _col.bounds.size.y * 0.5f;
			var hits = Physics2D.OverlapBoxAll(_workspace, new Vector2(_col.bounds.size.x - 0.01f, 0.1f), 0f, LayerMask.GetMask("Players", "Ground", "Enemies", "EnemyInvisibleGround"));

			//if there is at least one hit that is not myself return true
            foreach (var hit in hits)
				if(hit != _col)
					return true;

			return false;
		}

		///<summary>
		///sets the target to the passed entity, also registering the time and adding the entity to the damage map if needed
		///</summary>
		private void SetTarget(BoltEntity ent)
		{
			Debug.Log("[TRGT] target set and cannot be changed");
			_targetCanBeSet = false;
			_target = ent;

			_lastTargetChangedTime = Time.time;
			_lastTargetHitTime = Time.time;

			if (!_damageMap.ContainsKey(ent))
			{
				_damageMap.Add(ent, 0);
			}
		}

		#endregion
		#endregion
	}
}
