using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ThePackt
{
	public delegate void SpecificAttack();
	public delegate void CheckSpecificRange();

	public class BaseEnemy : Enemy
	{
		#region variables

		[Header("Specific")]
		[SerializeField] private float _reactionTime;
		[SerializeField] private float _jumpVelocity;
		[SerializeField] private float _jumpCooldown; //1 for prototype and 3 for mage
		[SerializeField] private float _avoidRange;
		[SerializeField] private float _perceptionRange;

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
		private bool _standStill;
		#endregion

		#region seek
		[Header("Seek")]
		[SerializeField] private float _seekThreshold;
		[SerializeField] private float _waypointThreshold;
		[SerializeField] private float _unreachabilityTime;
		[SerializeField] private float _reachabilityRestoreTime;
		[SerializeField] private float _keepSameTargetTime;
		private bool _jump;
		protected BoltEntity _target;
		protected Utils.NullableVector3 _targetWaypoint;
		private bool _targetCanBeSet;
		private Platform _nextPlatform;
		private Platform _currentPlatform;
		private Platform _lastTargetPlatform;
		private Utils.NullableVector2 _jumpPoint;
		private Vector2 _jumpPlatformVelocity;
		private int[][] _adjacencyMatrix;
		private float _lastTargetChangedTime;
		private float _xDiff;
		private float _yDiff;
		private float _dist;
		protected bool _attack;
		protected float _lastAttackTime;
		private float _lastJumpTime;
		#endregion

		#endregion

		#region methods

		protected override void Awake()
		{
			base.Awake();

			_adjacencyMatrix = Constants.ADJACENCYMATRIX;

			_nearPlayers = new Collider2D[6];
		}

		// Start is called before the first frame update
		public override void Attached()
		{
			base.Attached();

			#region wander fsm
			FSMState wanderIdle = new FSMState();
			wanderIdle.enterActions.Add(StartIdle);
			wanderIdle.stayActions.Add(CheckIfMustWalk);
			wanderIdle.exitActions.Add(StopIdle);

			FSMState wanderWalk = new FSMState();
			wanderWalk.enterActions.Add(StartWalking);
			wanderWalk.stayActions.Add(CheckIfMustStandStill);
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

			FSMTransition StunTrans = new FSMTransition(IsStunned);
			FSMTransition WalkTimeoutTrans = new FSMTransition(MustStandStill);
			wanderWalk.AddTransition(StunTrans, wanderIdle);
			wanderWalk.AddTransition(WalkTimeoutTrans, wanderIdle);

			FSMTransition IdleTimeoutTrans = new FSMTransition(MustNotStandStill);
			wanderIdle.AddTransition(IdleTimeoutTrans, wanderWalk);

			FSM fsmWander = new FSM(wanderWalk);

			FSMState wander = new FSMState();
			wander.enterActions.Add(StartWandering);
			wander.stayActions.Add(fsmWander.Update);
			#endregion

			#region seek fsm
			FSMState seekIdle = new FSMState();
			seekIdle.enterActions.Add(StartIdle);
			seekIdle.stayActions.Add(CheckIfTargetIsInRange);
			seekIdle.stayActions.Add(CheckIfTargetIsFar);
			seekIdle.exitActions.Add(StopIdle);

			FSMState seekWalk = new FSMState();
			seekWalk.enterActions.Add(StartWalking);
			seekWalk.stayActions.Add(CheckIfTargetIsInRange);
			seekWalk.stayActions.Add(CheckTargetPlatform);
			seekWalk.stayActions.Add(CheckIfIShouldJump);
			seekWalk.stayActions.Add(CheckIfTargetIsReached);
			seekWalk.stayActions.Add(Walk);
			seekWalk.exitActions.Add(StopWalking);

			FSMState seekAttack = new FSMState();
			seekAttack.enterActions.Add(StartAttacking);
			seekAttack.stayActions.Add(Attack);
			seekAttack.exitActions.Add(StopAttacking);

			FSMState seekJump = new FSMState();
			seekJump.enterActions.Add(StartJumping);
			seekJump.stayActions.Add(Jump);
			seekJump.exitActions.Add(StopJumping);

			FSMTransition TargetInRangeTrans = new FSMTransition(MustAttack);
			FSMTransition TargetReachedTrans = new FSMTransition(MustStandStill);
			FSMTransition TargetAboveTrans = new FSMTransition(MustJump);
			//we must check if the player is in range before checking if is reached, so it will go idle only if it is not in range
			seekWalk.AddTransition(StunTrans, seekIdle);
			seekWalk.AddTransition(TargetInRangeTrans, seekAttack);
			seekWalk.AddTransition(TargetAboveTrans, seekJump);
			seekWalk.AddTransition(TargetReachedTrans, seekIdle);

			FSMTransition TargetFarTrans = new FSMTransition(MustNotStandStill);
			seekIdle.AddTransition(TargetInRangeTrans, seekAttack);
			seekIdle.AddTransition(TargetFarTrans, seekWalk);
			seekIdle.AddTransition(TargetAboveTrans, seekJump);

			FSMTransition AttackEndTrans = new FSMTransition(AttackFinished);
			seekAttack.AddTransition(AttackEndTrans, seekIdle);
			//seekAttack.AddTransition(AttackEndTrans, seekWalk);

			FSMTransition JumpEndTrans = new FSMTransition(JumpFinished);
			seekJump.AddTransition(JumpEndTrans, seekIdle);
			//seekJump.AddTransition(JumpEndTrans, seekWalk);

			FSM fsmSeek = new FSM(seekWalk);

			FSMState seek = new FSMState();
			seek.enterActions.Add(StartSeeking);
			seek.stayActions.Add(SelectTarget);
			seek.stayActions.Add(fsmSeek.Update);
			#endregion

			//global fsm
			FSMTransition playerNearTrans = new FSMTransition(PlayerNear);
			FSMTransition hitByPlayerTrans = new FSMTransition(HitByPlayer);
			wander.AddTransition(playerNearTrans, seek);
			wander.AddTransition(hitByPlayerTrans, seek);

			FSMTransition NoTargetTrans = new FSMTransition(NoTarget);
			seek.AddTransition(NoTargetTrans, wander);

			_fsm = new FSM(wander);

			StartCoroutine(Patrol());
		}

		public override void SimulateOwner()
		{
			_currentVelocity = _rb.velocity;

			_isGrounded = IsGrounded();

			if (_target)
			{
				_xDiff = _target.GetComponent<Player>().transform.position.x - transform.position.x;
				_yDiff = _target.GetComponent<Player>().transform.position.y - transform.position.y;
				_dist = Vector2.Distance(_target.transform.position, transform.position);

                if (_target.GetComponent<Player>()._isGrounded)
                {
					BoxCollider2D targetCollider = _target.GetComponent<BoxCollider2D>();
					_lastTargetPlatform = GetPlatform(targetCollider);
				}

                if (_isGrounded)
                {
					_currentPlatform = GetPlatform((BoxCollider2D) _col);
				}
			}
		}

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
		private void StartIdle()
		{
			Debug.Log("[BASEENEMY] idle");

			_rb.velocity = new Vector2(0, _currentVelocity.y);
			//start animation?
		}

		private void StartJumping()
		{
			Debug.Log("[BASEENEMY] jumping");

			//start animation?
		}

		private void StartWalking()
		{
			Debug.Log("[BASEENEMY] walking");

			//start animation?
		}

		private void StartAttacking()
		{
			Debug.Log("[BASEENEMY] attacking");

			_rb.velocity = new Vector2(0, _currentVelocity.y);

			//start animation?
		}

		private void StartWandering()
		{
			Debug.Log("[BASEENEMY] wandering");

			_lastAttacker = null;
			_targetWaypoint = null;
			_lastTargetHitTime = 0f;
			_targetCanBeSet = true;
			_nextPlatform = null;
			_currentPlatform = null;
			_jumpPoint = null;

			Debug.Log("[TRGT] now in wandering, target can be changed");

			_lastDirectionChangeTime = 0f;
			_changeDirectionTime = 2f;
			_lastStandStillTime = 0f;
			_standStillTime = 1f;
			_standStill = false;
		}

		private void StartSeeking()
		{
			Debug.Log("[BASEENEMY] seeking");

			_standStill = false;
			_attack = false;
			_jump = false;
		}
		#endregion

		#region fsm stay actions

		#region wander actions
		private void CheckIfMustStandStill()
		{
			if (_lastDirectionChangeTime == 0)
			{
				//if it is the first time, do not flip

				_lastDirectionChangeTime = Time.time;
			}
			else
			{
				//if it is not the first time and enough time from the last flip elapsed, stop

				if (Time.time >= _lastDirectionChangeTime + _changeDirectionTime)
				{
					_standStill = true;
					_lastStandStillTime = Time.time;
				}
			}
		}

		private void Walk()
		{
			if (_target == null || !_isGrounded || _jump)
			{
				Debug.LogWarning("eeeee " + _jumpPlatformVelocity.x + "  " + _jumpPlatformVelocity.y);
				//if there is not a target go in the current direction
				if (_slowed)
				{
					_rb.velocity = new Vector2(_slowedSpeed * _facingDirection, _currentVelocity.y);
				}
				else
				{
					_rb.velocity = new Vector2(_jumpPlatformVelocity.x * _facingDirection, _currentVelocity.y);
				}

				if (_nextPlatform != null && Math.Abs(_nextPlatform.GetPlatformCenter().x - _col.bounds.center.x) <= _waypointThreshold)
                {
					_rb.velocity = new Vector2(0f, _currentVelocity.y); ;
				}
			}
			else
			{
				//if there is a target seek it

				//if the platform center has a higher x it means he is at my right
				int targetDirection = 1;
				if (_nextPlatform != null && _currentPlatform != null && _jumpPoint != null)
				{
					targetDirection = (_jumpPoint.vector.x - transform.position.x) > 0 ? 1 : -1;
				}
				else if(_nextPlatform != null && _currentPlatform != null)
				{
					if (_currentPlatform.left.position.y > _nextPlatform.left.position.y)
                    {
						targetDirection = (_nextPlatform.GetPlatformCenter().x - transform.position.x) > 0 ? 1 : -1;
					}
				}
				else
					targetDirection = _xDiff > 0 ? 1 : -1;

				//flip if the target direction differ from the facing direction
				if (targetDirection != _facingDirection)
				{
					Flip();
				}

				if (!_attack)
				{
					if (_slowed)
					{
						_rb.velocity = new Vector2(_slowedSpeed * targetDirection, _currentVelocity.y);
					}
					else
					{
						_rb.velocity = new Vector2(_movementSpeed * targetDirection, _currentVelocity.y);
					}
				}
			}
		}

		private void CheckIfMustWalk()
		{
			if (Time.time >= _lastStandStillTime + _standStillTime && !_stunned)
			{
				Flip();
				_lastDirectionChangeTime = Time.time;
				_standStill = false;

				//generate also a randomic change direction time and stand still time
				_changeDirectionTime = UnityEngine.Random.Range(_minChangeDirectionTime, _maxChangeDirectionTime);
				_standStillTime = UnityEngine.Random.Range(_minStandStillTime, _maxStandStillTime);
			}
		}

		private void Jump()
		{
			_rb.velocity = new Vector2(_jumpPlatformVelocity.x * _facingDirection, _jumpPlatformVelocity.y); ;
			_lastJumpTime = Time.time;

			_jump = false;
		}

		private void Avoid()
		{
			RaycastHit2D[] hits = new RaycastHit2D[1];
			ContactFilter2D filter = new ContactFilter2D();
			
			filter.SetLayerMask(LayerMask.GetMask("Ground", "Wall", "EnemyInvisibleWall"));

			int numHits = _col.Cast(-transform.right, filter, hits, _avoidRange, true);

			if (numHits > 0)
			{
				Flip();
			}
            else
            {
				Vector2 _workspace = _col.bounds.center + Vector3.down * _col.bounds.size.y * 0.5f + Vector3.right * _facingDirection * _avoidRange;
				RaycastHit2D groundInFront = Physics2D.Raycast(_workspace, -transform.up, 0.1f, LayerMask.GetMask("Ground", "EnemyInvisibleGround"));

				if (!groundInFront)
                {
					Flip();
				}
			}
		}
		#endregion

		#region seek actions

		private void SelectTarget()
		{
			//sense near players if some player have never been sensed
			if(_damageMap.Count < 6)
            {
				ContactFilter2D filter = new ContactFilter2D();
				filter.SetLayerMask(LayerMask.GetMask("Players"));
				int nearPlayersNum = Physics2D.OverlapCircle(transform.position, _perceptionRange, filter, _nearPlayers);
				foreach (var col in _nearPlayers)
				{
					if (col)
					{
						BoltEntity ent = col.gameObject.GetComponent<Player>().entity;
						if (!_damageMap.ContainsKey(ent))
						{
							_damageMap.Add(ent, 0);
						}
					}
				}
			}

			//remove from the unreachable ones the targets whose unreachability has to be restored
			List<BoltEntity> toRemove = new List<BoltEntity>();
			foreach(var entry in _unreachableTargets)
            {
				if (Time.time >= entry.Value + _reachabilityRestoreTime)
				{
					Debug.Log("[TRGT] target " + entry.Key.NetworkId + " now reachable");

					toRemove.Add(entry.Key);
				}
			}

			foreach(var elem in toRemove)
				_unreachableTargets.Remove(elem);

			//if target is unreachable for a certain amount of time, the target can be changed and the current target is added to the unreachable ones
			if (Time.time >= _lastTargetHitTime + _unreachabilityTime)
			{
				Debug.Log("[TRGT] target unreachable");
				_targetCanBeSet = true;
				_lastTargetHitTime = Time.time;

				if (_unreachableTargets.ContainsKey(_target))
				{
					_unreachableTargets[_target] = Time.time;
				}
				else
				{
					_unreachableTargets.Add(_target, Time.time);
				}
			}

			//if the target cannot be currently changed, check if the time that must pass after the target can change has passed
			//the target can be set anyway if the current target is not in room or is dead
			bool inRoomAlive = IsInRoomAndAlive(_target);
			if (!_targetCanBeSet && (Time.time >= _keepSameTargetTime + _lastTargetChangedTime || !inRoomAlive))
			{
				_targetCanBeSet = true;

				Debug.Log("[TRGT] can be changed: " + inRoomAlive);
			}

			if (_damageMap.Count > 0 && _targetCanBeSet)
			{
				//if the target can now be changed, pick the one that dealt more damage among the ones that are:
				//- still in the room
				//- still alive
				//- not in the unreachable list

				var sortedDict = from entry in _damageMap orderby entry.Value descending select entry;

				bool set = false;
				foreach (var entry in sortedDict)
				{
					if (IsInRoomAndAlive(entry.Key) && entry.Key && !_unreachableTargets.ContainsKey(entry.Key))
					{ 
						SetTarget(entry.Key);
						set = true;
						break;
					}
				}

				if(!set && !inRoomAlive)
                {
					//if no target was set, pick from the unreachable ones hoping the picked one is now reachable
					Debug.Log("[TRGT] picking from unreachable ones");

					set = false;
					foreach (var ent in _unreachableTargets.Keys)
					{
						if (IsInRoomAndAlive(ent))
						{
							_unreachableTargets.Remove(ent);
							SetTarget(ent);
							set = true;
							break;
						}
					}

					//if no target was set, it means no target is available. So set it to null, so that the transition will make the state change to wandering 
					if (!set)
					{
						Debug.Log("[TRGT] not set and not alive and in room");

						_target = null;
					}
				}
			}

			//Debug.Log("[BASEENEMY] target: " + _target.NetworkId);
		}

		private void Attack()
		{
			_specificAttack();

			_attack = false;
		}

		private void CheckIfTargetIsReached()
		{
			//Math.Abs(_xDiff) <= _seekThreshold
			if (_dist <= _seekThreshold && IsInRoomAndAlive(_target))
			{
				Debug.Log("[BASEENEMY] target reached");
				_standStill = true;
			}
		}

		private void CheckIfTargetIsFar()
		{
			//Math.Abs(_xDiff) > _seekThreshold
			if (_dist > _seekThreshold)
			{
				Debug.Log("[BASEENEMY] target far");
				_standStill = false;
			}
		}

		private void CheckIfIShouldJump()
		{

			if (Time.time >= _lastJumpTime + _jumpCooldown && _target && IsGrounded())
			{
				if(_nextPlatform != null && _currentPlatform != null && _jumpPoint != null && Math.Abs(_jumpPoint.vector.x - _col.bounds.center.x) <= _waypointThreshold)
                {
					int targetDirection;
					float centerX = _nextPlatform.GetPlatformCenter().x;
					targetDirection = (centerX - transform.position.x) > 0 ? 1 : -1;

					//flip if the target direction differ from the facing direction
					if (targetDirection != _facingDirection)
					{
						Flip();
					}

					float yMax = 0f;
					float range = 0f;
					if (_currentPlatform.left.position.y < _nextPlatform.left.position.y)
                    {
						yMax = (_nextPlatform.left.position.y + (_col.bounds.size.y * 1f)) - _col.bounds.center.y;

						//float arrivalX = _nextPlatform.left.position.x > _col.bounds.center.x ? _nextPlatform.left.position.x - _col.bounds.size.x : _nextPlatform.right.position.x + _col.bounds.size.x;
						float arrivalX = _nextPlatform.left.position.x > _col.bounds.center.x ? _nextPlatform.left.position.x + _col.bounds.size.x : _nextPlatform.right.position.x - _col.bounds.size.x;
						range = Math.Abs(arrivalX - _col.bounds.center.x);
					}
                    else
                    {
						yMax = (_nextPlatform.left.position.y + (_col.bounds.size.y * 2f)) - _col.bounds.center.y;
						range = Math.Abs(centerX - _col.bounds.center.x);
					}

					float vY = Mathf.Sqrt(yMax * -(Physics2D.gravity.y * 2));
					float vX = (range * -Physics2D.gravity.y) / (2 * vY);

					Debug.LogWarning("bbbb " + vX + "  " + vY);
					_jumpPlatformVelocity = new Vector2(vX, vY);

					_jump = true;
				}

				/*
				Debug.Log("[PATH] target above " + Physics2D.gravity.y);

				Vector3Int myCell = _groundTilemap.WorldToCell(transform.position);
				Debug.Log("[PATH] my cell: " + myCell + "  " + _groundTilemap.HasTile(myCell));
				Vector3Int targetCell = _groundTilemap.WorldToCell(_target.transform.position);
				Debug.Log("[PATH] target cell: " + targetCell + "  " + _groundTilemap.HasTile(targetCell));

				float jumpRange = (_movementSpeed * _jumpVelocity * 2) / -Physics2D.gravity.y;
				float maxHeight = (_jumpVelocity * _jumpVelocity) / -(Physics2D.gravity.y * 2);

				Bounds colBounds = GetComponent<BoxCollider2D>().bounds;
				float startHeight = colBounds.center.y - colBounds.extents.y;

				Bounds targetColBounds = _target.GetComponent<BoxCollider2D>().bounds;
				float targetHeight = targetColBounds.center.y - targetColBounds.extents.y;

				Debug.Log("[PATH] height " + (startHeight + maxHeight));

				if (startHeight + maxHeight > targetHeight)
                {
					if (transform.position.x + jumpRange >= _target.transform.position.x)
					{
						RaycastHit2D[] hits = new RaycastHit2D[1];
						ContactFilter2D filter = new ContactFilter2D();

						filter.SetLayerMask(LayerMask.GetMask("Ground", "Wall", "EnemyInvisibleWall"));

						int numHits = _col.Cast(transform.up, filter, hits, _target.transform.position.y - transform.position.y, true);

						if (numHits == 0)
						{
							_jump = true;
						}
					}
				}

				*/
			}

			if (!_jump)
			{
				Vector2 _workspace = _col.bounds.center + Vector3.down * (_col.bounds.size.y * 0.5f + 0.1f);
				Collider2D hit = Physics2D.OverlapBox(_workspace, new Vector3(_col.bounds.size.x, 0.005f, 0f), 0f, LayerMask.GetMask("Players", "Enemies", "Objectives"));
				if (hit)
				{
					_jumpPlatformVelocity = new Vector2(_currentVelocity.x, _jumpVelocity);
					_jump = true;
				}
			}
		}


		private void CheckTargetPlatform()
		{
            if (IsGrounded())
            {
				if(_lastTargetPlatform != null)
                {
					Debug.LogWarning("ppppp target " + _lastTargetPlatform.num);
				}
				if (_currentPlatform != null)
				{
					Debug.LogWarning("ppppp current " + _currentPlatform.num);
				}

				if (_currentPlatform != null && _lastTargetPlatform != null && _lastTargetPlatform.num != _currentPlatform.num)
				{
					_nextPlatform = GetPlatform(_adjacencyMatrix[_currentPlatform.num][_lastTargetPlatform.num]);

					Debug.LogWarning("ppppp next " + _nextPlatform.num);

					if (_currentPlatform.left.position.y == _nextPlatform.left.position.y)
					{
						_jumpPoint = new Utils.NullableVector2(_currentPlatform.GetPlatformCenter());
					}
					else if(_currentPlatform.left.position.y < _nextPlatform.left.position.y)
					{
						_jumpPoint = new Utils.NullableVector2(_currentPlatform.GetPlatformCenter());

						/*
						float jumpRange = (_movementSpeed * _jumpVelocity * 2) / -Physics2D.gravity.y;
						Vector2 nextPlatformCenter = _nextPlatform.GetPlatformCenter();

						int sign = (nextPlatformCenter.x - _col.bounds.center.x) > 0 ? -1 : 1;
						float jumpPointX = nextPlatformCenter.x + (sign * jumpRange);
						jumpPointX = jumpPointX > _currentPlatform.right.position.x - _col.bounds.size.x / 2 - 0.1f ? _currentPlatform.right.position.x - _col.bounds.size.x/2 - 0.1f : jumpPointX;
						jumpPointX = jumpPointX < _currentPlatform.left.position.x + _col.bounds.size.x / 2 + 0.1f ? _currentPlatform.left.position.x + _col.bounds.size.x / 2 + 0.1f : jumpPointX;

						_jumpPoint = new Utils.NullableVector2(new Vector2(jumpPointX, _currentPlatform.left.position.y));

						RaycastHit2D hit = Physics2D.BoxCast(new Vector2(_jumpPoint.vector.x + _col.bounds.size.x / 2, _jumpPoint.vector.y + _col.bounds.size.y / 2), new Vector2(_col.bounds.size.x, _jumpPoint.vector.y - _nextPlatform.left.position.y), 0f, Vector2.up, LayerMask.GetMask("Ground", "Wall", "EnemyInvisibleWall"));

                        if (hit)
						{ 

							sign = -sign;
							jumpPointX = nextPlatformCenter.x + (sign * jumpRange);
							jumpPointX = jumpPointX > _currentPlatform.right.position.x - _col.bounds.size.x / 2 - 0.1f ? _currentPlatform.right.position.x - _col.bounds.size.x / 2 - 0.1f : jumpPointX;
							jumpPointX = jumpPointX < _currentPlatform.left.position.x + _col.bounds.size.x / 2 + 0.1f ? _currentPlatform.left.position.x + _col.bounds.size.x / 2 + 0.1f : jumpPointX;

							Debug.LogWarning("ppppp hit " + jumpPointX);

							_jumpPoint = new Utils.NullableVector2(new Vector2(jumpPointX, _currentPlatform.left.position.y));
						}
						*/
					}
                    else
                    {
						_jumpPoint = null;

					}
				}
				else
				{
					_nextPlatform = null;
					_jumpPoint = null;
				}
			}
            else
            {
				//_nextPlatform = null;
				_jumpPoint = null;
			}
		}
		
		private void CheckIfTargetIsGrounded()
		{
			if (Time.time >= _lastJumpTime + _jumpCooldown && _target)
			{
				bool targetGrounded = _target.GetComponent<Player>()._isGrounded;
				
				if (!targetGrounded && IsGrounded())
				{
					Debug.Log("[BASEENEMY] target not grounded");
					_jump = true;
				}
			}
		}

		private void CheckIfTargetIsInRange()
		{
			if (IsInRoomAndAlive(_target))
			{
				_checkSpecificRange();
			}
		}

		#endregion

		#endregion

		#region fsm exit actions
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

		#region fsm conditions
		private bool MustStandStill()
		{
			//Debug.Log("[BASEENEMY] must stand " + _standStill);
			return _standStill;
		}

		private bool IsStunned()
		{ 
			return _stunned;
		}

		private bool MustNotStandStill()
		{
			return !_standStill && !_stunned;
		}

		private bool MustJump()
		{
			return _jump && !_stunned && !_slowed;
		}

		private bool JumpFinished()
		{
			return !_jump && IsLanded();
		}

		private bool MustAttack()
		{
			return _attack && !_stunned;
		}

		private bool AttackFinished()
		{
			return !_attack;
		}

		private bool NoTarget()
		{
			Debug.Log("[TRGT] qui qui" + _target == null);
			return _target == null;
		}

		private bool PlayerNear()
		{ 
			ContactFilter2D filter = new ContactFilter2D();
			filter.SetLayerMask(LayerMask.GetMask("Players"));

			int nearPlayersNum = Physics2D.OverlapCircle(transform.position, _perceptionRange, filter, _nearPlayers);

			Debug.Log("[BASEENEMY] checking near " + nearPlayersNum);
			if (nearPlayersNum > 0)
			{
				BoltEntity nearPlayer = _nearPlayers[0].gameObject.GetComponent<Player>().entity;
				if (IsInRoomAndAlive(nearPlayer))
				{
					Debug.Log("[BASEENEMY] player near");

					SetTarget(nearPlayer);
					return true;
				}
			}

			return false;
		}

		private bool HitByPlayer()
		{
			if (_lastAttacker != null && IsInRoomAndAlive(_lastAttacker))
			{
				SetTarget(_lastAttacker);

				return true;
			}

			return false;
		}
		#endregion

		#region others
		private void Flip()
		{
			_facingDirection *= -1;
			transform.Rotate(0f, 180f, 0f);
		}

		private Platform GetPlatform(BoxCollider2D collider)
        {
			Vector2 rightCorner = new Vector2(collider.bounds.center.x + (collider.bounds.size.x / 2) + 0.1f, collider.bounds.center.y - (collider.bounds.size.y / 2));
			var hit = Physics2D.Raycast(rightCorner, new Vector2(0f, -1f), 0.1f, LayerMask.GetMask("Ground", "EnemyInvisibleGround"));

			Vector2 position = Vector2.zero;
            if (hit)
            {
				position = hit.point;
			}
            else
            {
				Vector2 leftCorner = new Vector2(collider.bounds.center.x - (collider.bounds.size.x / 2) - 0.1f, collider.bounds.center.y - (collider.bounds.size.y / 2));
				hit = Physics2D.Raycast(leftCorner, new Vector2(0f, -1f), 0.1f, LayerMask.GetMask("Ground", "EnemyInvisibleGround"));

				if (hit)
                {
					position = hit.point;
				}
			}

			foreach(var platform in _platforms)
            {
				if (Math.Abs(position.y - platform.left.position.y) <= 0.01f && position.x + 0.01f >= platform.left.position.x && position.x - 0.01f <= platform.right.position.x)
                {
					return platform;
                }
            }

			return null;
        }

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
		
		public void RegisterTargetHit(BoltEntity ent)
		{
			if (_target = ent)
			{
				_lastTargetHitTime = Time.time;
			}
		}

		protected bool IsInRoomAndAlive(BoltEntity plyr)
		{
			if (_room != null)
			{
				if (_room.CheckIfPlayerIsInRoom(plyr) && plyr.IsAttached)
				{
					return true;
				}
			}
			else
			{
				if (MainQuest.Instance.CheckIfPlayerIsInRoom(plyr) && plyr.IsAttached)
				{
					return true;
				}
			}

			return false;
		}

		protected bool IsGrounded()
		{
			Vector2 _workspace = _col.bounds.center + Vector3.down * _col.bounds.size.y * 0.5f;
			return Physics2D.OverlapBox(_workspace, new Vector3(_col.bounds.size.x - 0.01f, 0.1f, 0f), 0f, LayerMask.GetMask("Ground", "EnemyInvisibleGround"));
		}

		private bool IsLanded()
		{
			Vector2 _workspace = _col.bounds.center + Vector3.down * _col.bounds.size.y * 0.5f;
			return Physics2D.OverlapBox(_workspace, new Vector3(_col.bounds.size.x - 0.01f, 0.1f, 0f), 0f, LayerMask.GetMask("Players", "Ground", "Objectives", "Enemies"));
		}
        #endregion

        #region setters
		private void SetTarget(BoltEntity ent)
        {
			_targetCanBeSet = false;
			_lastTargetChangedTime = Time.time;

			if (!_damageMap.ContainsKey(ent))
			{
				_damageMap.Add(ent, 0);
			}

			Debug.Log("[TRGT] changed to " + ent.NetworkId);

			_target = ent;
		}
	#endregion

	#endregion
}
}
