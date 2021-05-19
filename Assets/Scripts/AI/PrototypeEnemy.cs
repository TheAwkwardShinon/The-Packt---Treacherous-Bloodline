using System.Collections;
using System.Collections.Generic;
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

		private Vector2 _currentVelocity;
		private int _facingDirection;
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

		private bool _jump;
		#endregion

		#endregion

		#region methods

		private void Awake()
        {
			_rb = gameObject.GetComponent<Rigidbody2D>();
			_col = gameObject.GetComponent<Collider2D>();
		}

        // Start is called before the first frame update
        void Start()
		{
			//starting timers
			_lastDirectionChangeTime = 0f;
			_changeDirectionTime = 2f;
			_lastStandStillTime = 0f;
			_standStillTime = 1f;

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

			FSMState wanderJump = new FSMState();
			wanderJump.enterActions.Add(StartJumping);
			wanderJump.stayActions.Add(Jump);
			wanderJump.exitActions.Add(StopJumping);

			FSMTransition MustJumpTrans = new FSMTransition(MustJump);
			FSMTransition WalkTimeoutTrans = new FSMTransition(MustStandStill);
			wanderWalk.AddTransition(MustJumpTrans, wanderJump);
			wanderWalk.AddTransition(WalkTimeoutTrans, wanderIdle);

			FSMTransition LandedTrans = new FSMTransition(Landed);
			wanderJump.AddTransition(LandedTrans, wanderWalk);

			FSMTransition IdleTimeoutTrans = new FSMTransition(MustNotStandStill);
			wanderIdle.AddTransition(IdleTimeoutTrans, wanderWalk);

			FSM fsmWander = new FSM(wanderWalk);

			FSMState wander = new FSMState();
			wander.stayActions.Add(fsmWander.Update);
			#endregion

			#region seek fsm
			FSMState seekWalk = new FSMState();
			wanderWalk.enterActions.Add(StartWalking);
			wanderWalk.stayActions.Add(Avoid);
			wanderWalk.stayActions.Add(Walk);
			wanderWalk.exitActions.Add(StopWalking);

			FSM fsmSeek = new FSM(seekWalk);

			FSMState seek = new FSMState();
			wander.stayActions.Add(fsmSeek.Update);
			#endregion

			//global fsm
			FSMTransition playerNearTrans = new FSMTransition(PlayerNear);
			FSMTransition hitByPlayerTrans = new FSMTransition(HitByPlayer);
			
			wander.AddTransition(playerNearTrans, seek);
			wander.AddTransition(hitByPlayerTrans, seek);

			_fsm = new FSM(wander);

			StartCoroutine(Patrol());
		}

        public override void SimulateOwner()
        {
			_currentVelocity = _rb.velocity;
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

			Debug.Log("[PROTOTYPE] range " + _avoidRange);

			Gizmos.DrawRay(transform.position, -transform.right * _avoidRange);
		}

        #region fsm enter actions
        private void StartIdle()
		{
			_rb.velocity = new Vector2(0, _currentVelocity.y);
			//inizio animazione?
		}

		private void StartJumping()
		{
			//inizio animazione?
		}

		private void StartWalking()
		{
			//inizio animazione?
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
			//Debug.Log("[PROTOTYPE] walking");

			_rb.velocity = new Vector2(_movementSpeed * _facingDirection, _currentVelocity.y);
		}

		private void CheckIfMustWalk()
		{
			if (Time.time >= _lastStandStillTime + _standStillTime)
			{
				Debug.Log("[PROTOTYPE] flipping");

				Flip();
				_lastDirectionChangeTime = Time.time;
				_standStill = false;

				//generate also a randomic change direction time and stand still time
				_changeDirectionTime = Random.Range(_minChangeDirectionTime, _maxChangeDirectionTime);
				_standStillTime = Random.Range(_minStandStillTime, _maxStandStillTime);
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
				Debug.Log("[PROTOTYPE] avoid flipping");

				Flip();
				//_lastDirectionChangeTime = Time.time;
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

		private bool PlayerNear()
		{
			return false;
		}

		private bool HitByPlayer()
		{
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
