using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace ThePackt
{
	public class PrototypeEnemy : Enemy
	{
		#region variables

		[SerializeField] private float _reactionTime;
		[SerializeField] private float _movementSpeed;
		[SerializeField] private float _jumpVelocity;
		[SerializeField] private float _avoidRange;
		[SerializeField] private float _perceptionRange;

		private Vector2 _currentVelocity;
		private int _facingDirection;
		private Collider2D[] _nearPlayers;
		private Rigidbody2D _rb;
		private Collider2D _col;

		#region wandering
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
		[SerializeField] private float _seekThreshold;
		private bool _jump;
		private BoltEntity _target;
		private float _xDiff;

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
        void Start()
		{
			_facingDirection = 1;
			if (IsFacingLeft())
            {
				_facingDirection = -1;
			}

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
			seekIdle.stayActions.Add(CheckIfTargetIsFar);
			seekIdle.exitActions.Add(StopIdle);

			FSMState seekWalk = new FSMState();
			seekWalk.enterActions.Add(StartWalking);
			seekWalk.stayActions.Add(CheckIfTargetIsReached);
			seekWalk.stayActions.Add(Walk);
			seekWalk.exitActions.Add(StopWalking);

			FSMTransition TargetReachedTrans = new FSMTransition(MustStandStill);
			seekWalk.AddTransition(TargetReachedTrans, seekIdle);

			FSMTransition TargetFarTrans = new FSMTransition(MustNotStandStill);
			seekIdle.AddTransition(TargetFarTrans, seekWalk);

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
			Gizmos.color = Color.red;
			Gizmos.DrawRay(transform.position, -transform.right * _avoidRange);

			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, _attackRange);

			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, _perceptionRange);

			Gizmos.color = Color.magenta;
			if(Math.Abs(transform.rotation.y % 180) == 1)
            {
				Gizmos.DrawRay(transform.position + new Vector3(_seekThreshold, -0.1f, 0), transform.right * _seekThreshold * 2);
			}
            else
            {
				Gizmos.DrawRay(transform.position - new Vector3(_seekThreshold, 0.1f, 0), transform.right * _seekThreshold * 2);
			}
		}

        #region fsm enter actions
        private void StartIdle()
		{
			Debug.Log("[PROTOTYPE] idle");

			_rb.velocity = new Vector2(0, _currentVelocity.y);
			//inizio animazione?
		}

		private void StartJumping()
		{
			//inizio animazione?
		}

		private void StartWalking()
		{
			Debug.Log("[PROTOTYPE] walking");

			//inizio animazione?
		}

		private void StartWandering()
		{
			Debug.Log("[PROTOTYPE] wandering");

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
			Debug.Log("[PROTOTYPE] seeking");

			_standStill = false;
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

				//Debug.Log("[PROTOTYPE] time: " + Time.time + "    last change: " + _lastDirectionChangeTime + "     change time " + _changeDirectionTime);
				if (Time.time >= _lastDirectionChangeTime + _changeDirectionTime)
				{
					_standStill = true;
					_lastStandStillTime = Time.time;
				}
			}
		}

		private void Walk()
		{
			if(NoTarget())
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
				if(targetDirection != _facingDirection)
                {
					Flip();
                }

				_rb.velocity = new Vector2(_movementSpeed * targetDirection, _currentVelocity.y);
			}
		}

		private void CheckIfMustWalk()
		{
			//Debug.Log("[PROTOTYPE] time: " + Time.time + "   last stand: " + _lastStandStillTime + "     stand time " + _standStillTime);
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
			Debug.Log("[PROTOTYPE] jumping");

			_rb.velocity = new Vector2(_currentVelocity.x, _jumpVelocity);
		}

		private void Avoid()
		{
			RaycastHit2D[] hits = new RaycastHit2D[1];
			ContactFilter2D filter = new ContactFilter2D();
			//filter.SetLayerMask(LayerMask.GetMask("Ground", "Wall"));
			filter.SetLayerMask(LayerMask.GetMask("Ground", "Wall", "EnemyInvisibleWall", "Objectives"));

			int numHits = _col.Cast(-transform.right, filter, hits, _avoidRange, true);

			if (numHits > 0)
            {
				//Debug.Log("[PROTOTYPE] avoid flipping");

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

			if(_damageMap.Count > 0)
            {
				var sortedDict = from entry in _damageMap orderby entry.Value descending select entry;

				foreach(var entry in sortedDict)
                {
					if (_room != null)
					{
						if (_room.CheckIfPlayerIsInRoom(entry.Key) || entry.Key.IsAttached)
						{
							_target = entry.Key;
							break;
						}
					}
					else
					{
						if (MainQuest.Instance.CheckIfPlayerIsInRoom(entry.Key) || entry.Key.IsAttached)
						{
							_target = entry.Key;
							break;
						}
					}
				}
			}

			Debug.Log("[PROTOTYPE] target: " + _target.NetworkId);
		}

		private void CheckIfTargetIsReached()
		{
			if(Math.Abs(_xDiff) <= _seekThreshold)
            {
				Debug.Log("[PROTOTYPE] target reached");
				_standStill = true;
            }
		}

		private void CheckIfTargetIsFar()
		{
			if (Math.Abs(_xDiff) > _seekThreshold)
			{
				Debug.Log("[PROTOTYPE] target far");
				_standStill = false;
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
		#endregion

		#region fsm conditions
		private bool MustStandStill()
		{
			//Debug.Log("[PROTOTYPE] must stand " + _standStill);
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

		private bool Landed()
		{
			return false;
		}

		private bool NoTarget()
		{
			if(_room != null)
            {
				if (!_room.CheckIfPlayerIsInRoom(_target) || !_target.IsAttached)
				{
					_target = null;
				}
			}
            else
            {
				if (!MainQuest.Instance.CheckIfPlayerIsInRoom(_target) || !_target.IsAttached)
				{
					_target = null;
				}
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
				Debug.Log("[PROTOTYPE] player near");

				_target = _nearPlayers[0].gameObject.GetComponent<Player>().entity;

				return true;
			}

			return false;
		}

		private bool HitByPlayer()
		{
			if(_lastAttacker != null)
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

		private bool IsFacingLeft()
        {
			if(transform.right.x > 0)
            {
				return true;
            }

			return false;
        }
        #endregion

        #endregion
    }
}
