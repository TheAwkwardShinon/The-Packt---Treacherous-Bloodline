using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
	public class PrototypeEnemy : Enemy
	{
		[SerializeField] private float _reactionTime = 3f;
		[SerializeField] private float _movementSpeed = 3f;

		private Vector2 _currentVelocity;
		private Rigidbody2D _rb;

		[SerializeField] private float _minChangeDirectionTime;
		[SerializeField] private float _maxChangeDirectionTime;
		[SerializeField] private float _minStandStillTime;
		[SerializeField] private float _maxStandStillTime;
		private float _lastDirectionChangeTime;
		private float _changeDirectionTime;
		private float _lastStandStillTime;
		private float _standStillTime;
		private bool _standStill;
		private int _facingDirection;

        #region methods

        private void Awake()
        {
			_rb = gameObject.GetComponent<Rigidbody2D>();
        }

        // Start is called before the first frame update
        void Start()
		{
			_lastDirectionChangeTime = 0f;
			_changeDirectionTime = 2f;
			_lastStandStillTime = 0f;
			_standStillTime = 1f;

			_facingDirection = 1;
			if (IsFacingLeft())
            {
				_facingDirection = -1;
			}

			FSMState wander = new FSMState();
			wander.stayActions.Add(Walk);
			wander.stayActions.Add(Avoid);

			FSMState seek = new FSMState();
			seek.stayActions.Add(CheckIfTargetDied);

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

        #region fsm actions
        private void Walk()
		{
			if(_lastDirectionChangeTime == 0)
            {
				//if it is the first time, do not flip

				_lastDirectionChangeTime = Time.time;
			}
            else
            {
				//if it is not the first time and enough time from the last flip elapsed, stop

				if (!_standStill && Time.time >= _lastDirectionChangeTime + _changeDirectionTime)
				{
					_standStill = true;
					_lastStandStillTime = Time.time;

					//Debug.Log("[PROTOTYPE] standing still");
				}

				//if it is not the first time and enough time from when it stopped elapsed, flip

				if (_standStill && Time.time >= _lastStandStillTime + _standStillTime)
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

            if (!_standStill)
            {
				//Debug.Log("[PROTOTYPE] moving");

				//move
				_rb.velocity = new Vector2(_movementSpeed * _facingDirection, _currentVelocity.y);
			}
		}

		private void Avoid()
		{

		}

		private void CheckIfTargetDied()
        {

        }
        #endregion

        #region fsm conditions
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
