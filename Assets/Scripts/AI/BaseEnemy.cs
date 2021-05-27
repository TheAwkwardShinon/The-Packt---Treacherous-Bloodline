using System;
using System.Collections;
using System.Linq;
using UnityEngine;

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
		[SerializeField] private float _jumpCooldown;
		[SerializeField] private float _avoidRange;
		[SerializeField] private float _perceptionRange;

		private Vector2 _currentVelocity;
		private Collider2D[] _nearPlayers;
		private Rigidbody2D _rb;
		private Collider2D _col;

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
		[SerializeField] private float _unreachabilityTime;
		private bool _jump;
		protected BoltEntity _target;
		private BoltEntity _prevTarget;
		private float _xDiff;
		private float _yDiff;
		protected bool _attack;
		protected float _lastAttackTime;
		private float _lastJumpTime;
        #endregion

        #endregion

        #region methods

        private void Awake()
		{
			_rb = gameObject.GetComponent<Rigidbody2D>();
			_col = gameObject.GetComponent<Collider2D>();
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

			FSMTransition WalkTimeoutTrans = new FSMTransition(MustStandStill);
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
			seekWalk.stayActions.Add(CheckIfTargetIsAbove);
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
			seekJump.AddTransition(JumpEndTrans, seekWalk);

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

			if (_target)
			{
				_xDiff = _target.GetComponent<Player>().transform.position.x - transform.position.x;
				_yDiff = _target.GetComponent<Player>().transform.position.y - transform.position.y;
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
			//
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
			//_col.bounds.size.y
		}

		#region fsm enter actions
		private void StartIdle()
		{
			Debug.Log("[BASEENEMY] idle");

			_rb.velocity = new Vector2(0, _currentVelocity.y);
			//inizio animazione?
		}

		private void StartJumping()
		{
			Debug.Log("[BASEENEMY] jumping");

			//inizio animazione?
		}

		private void StartWalking()
		{
			Debug.Log("[BASEENEMY] walking");

			//inizio animazione?
		}

		private void StartAttacking()
		{
			Debug.Log("[BASEENEMY] attacking");

			_rb.velocity = new Vector2(0, _currentVelocity.y);

			//inizio animazione?
		}

		private void StartWandering()
		{
			Debug.Log("[BASEENEMY] wandering");

			_lastAttacker = null;
			//_target = null;

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

				//Debug.Log("[BASEENEMY] time: " + Time.time + "    last change: " + _lastDirectionChangeTime + "     change time " + _changeDirectionTime);
				if (Time.time >= _lastDirectionChangeTime + _changeDirectionTime)
				{
					_standStill = true;
					_lastStandStillTime = Time.time;
				}
			}
		}

		private void Walk()
		{
			if (_target == null)
			{
				//if there is not a target go in the current direction

				_rb.velocity = new Vector2(_movementSpeed * _facingDirection, _currentVelocity.y);
			}
			else
			{
				//if there is a target seek it

				//if the player has a higher x it means he is at my right
				int targetDirection = _xDiff > 0 ? 1 : -1;
				//flip if the target direction differ from the facing direction
				if (targetDirection != _facingDirection)
				{
					Flip();
				}

				if (!_attack)
				{
					_rb.velocity = new Vector2(_movementSpeed * targetDirection, _currentVelocity.y);
				}
			}
		}

		private void CheckIfMustWalk()
		{
			//Debug.Log("[BASEENEMY] time: " + Time.time + "   last stand: " + _lastStandStillTime + "     stand time " + _standStillTime);
			if (Time.time >= _lastStandStillTime + _standStillTime)
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
			_rb.velocity = new Vector2(_currentVelocity.x, _jumpVelocity);
			_lastJumpTime = Time.time;

			_jump = false;
		}

		private void Avoid()
		{
			RaycastHit2D[] hits = new RaycastHit2D[1];
			ContactFilter2D filter = new ContactFilter2D();
			//filter.SetLayerMask(LayerMask.GetMask("Ground", "Wall"));
			filter.SetLayerMask(LayerMask.GetMask("Ground", "Wall", "EnemyInvisibleWall"));

			int numHits = _col.Cast(-transform.right, filter, hits, _avoidRange, true);

			if (numHits > 0)
			{
				//Debug.Log("[BASEENEMY] avoid flipping");

				Flip();
				//_lastDirectionChangeTime = Time.time;
			}
		}
		#endregion

		#region seek actions

		private void SelectTarget()
		{
			/*
            if (_lastAttacker)
            {
				_target = _lastAttacker;
			}
			*/

			/*
			if(_hitTimeMap.ContainsKey(_target))
            {
				if(Time.time >= _hitTimeMap[_target] + _unreachabilityTime)
				{
					Debug.Log("[BASEENEMY] target unreachable");
					_prevTarget = _target;
					_target = null;
				}
				else
				{
					_prevTarget = null;
				}
			}
			*/

			if (_damageMap.Count > 0)
			{
				var sortedDict = from entry in _damageMap orderby entry.Value descending select entry;

				foreach (var entry in sortedDict)
				{
					if (IsInRoomAndAlive(entry.Key) && entry.Key)
					{
						_target = entry.Key;
						break;
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
			if (Math.Abs(_xDiff) <= _seekThreshold && IsInRoomAndAlive(_target))
			{
				Debug.Log("[BASEENEMY] target reached");
				_standStill = true;
			}
		}

		private void CheckIfTargetIsFar()
		{
			if (Math.Abs(_xDiff) > _seekThreshold)
			{
				Debug.Log("[BASEENEMY] target far");
				_standStill = false;
			}
		}

		private void CheckIfTargetIsAbove()
		{
			if (Time.time >= _lastJumpTime + _jumpCooldown)
			{
				float targetY = _target.GetComponent<Player>().transform.position.y;
				//_target.GetComponent<Player>()._isGrounded
				if ((targetY > transform.position.y + _col.bounds.size.y) && IsGrounded())
				{
					Debug.Log("[BASEENEMY] target above");
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
			//fine animazione?
		}

		private void StopJumping()
		{
			//fine animazione?
		}

		private void StopWalking()
		{
			//fine animazione?
		}

		private void StopAttacking()
		{
			//fine animazione?
		}
		#endregion

		#region fsm conditions
		private bool MustStandStill()
		{
			//Debug.Log("[BASEENEMY] must stand " + _standStill);
			return _standStill;
		}

		private bool MustNotStandStill()
		{
			return !_standStill;
		}

		private bool MustJump()
		{
			return _jump;
		}

		private bool JumpFinished()
		{
			return !_jump && IsLanded();
		}

		private bool MustAttack()
		{
			return _attack;
		}

		private bool AttackFinished()
		{
			return !_attack;
		}

		private bool NoTarget()
		{
			if (!IsInRoomAndAlive(_target))
			{
				_target = null;
			}

			return _target == null;
		}

		private bool PlayerNear()
		{
			ContactFilter2D filter = new ContactFilter2D();
			filter.SetLayerMask(LayerMask.GetMask("Players"));

			int nearPlayersNum = Physics2D.OverlapCircle(transform.position, _perceptionRange, filter, _nearPlayers);

			if (nearPlayersNum > 0)
			{
				BoltEntity nearPlayer = _nearPlayers[0].gameObject.GetComponent<Player>().entity;
				if (IsInRoomAndAlive(nearPlayer))
				{
					Debug.Log("[BASEENEMY] player near");

					_target = _nearPlayers[0].gameObject.GetComponent<Player>().entity;
					return true;
				}
			}

			return false;
		}

		private bool HitByPlayer()
		{
			if (_lastAttacker != null && IsInRoomAndAlive(_lastAttacker))
			{
				_target = _lastAttacker;

				return true;
			}

			return false;
		}
		#endregion

		#region others
		private void Flip()
		{
			_facingDirection *= -1;
			transform.Rotate(0.0f, 180.0f, 0.0f);
		}

		protected bool IsInRoomAndAlive(BoltEntity plyr)
		{
			if (_room != null)
			{
				if (_room.CheckIfPlayerIsInRoom(plyr) && plyr.IsAttached)
				{
					return false;
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

		#endregion
	}
}
