using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ThePackt.Werewolf;


namespace ThePackt{
    public class MovementController : MonoBehaviour
    {
        #region variables
        private Werewolf _player;
        private Rigidbody2D _rb;
        #endregion

        #region methods
        private void Start()
        {
            _player = GetComponent<Werewolf>();
            _rb = GetComponent<Rigidbody2D>();
        }

        /* method that perform the movement action */
        public void Moving()
        {
            UpdateSpriteDirection(_player.GetDirection());
            UpdatePosition(_player.GetDirection() * _player.GetSpeed() , Time.fixedDeltaTime);
            _player.SetCurrentState(State.IDLE);
        }

        /* method that perform the jumping action */
        public void Jumping(){
            _player.SetisGrounded(false);
            _rb.AddForce(new Vector2(_player.GetDirection().x,_player.GetJumpForce()), ForceMode2D.Impulse);
            _player.SetCurrentState(State.IDLE);
        }

        /* method that perform the Dashing action */
        public void Dashing(){
            UpdateSpriteDirection(_player.GetDirection());
            UpdatePosition(_player.GetDirection()* _player.GetSpeed() * _player.GetDashMultiplier(), Time.fixedDeltaTime);
            _player.SetCurrentState(State.IDLE);
        }

        /* method that change the position of the gameObject (the character)
           using rigidbody instead of transform prevent the character from passing through walls during dashing */
         private void UpdatePosition(Vector2 movement, float delta)
        {
            if (movement.magnitude != 0)
                _rb.velocity = Vector3.zero;
            //_rb.velocity = movement * _speed * delta;
            //transform.position += new Vector3(movement.x, movement.y, 0) * delta;
            _rb.MovePosition(_rb.position + movement * delta);       
        }
        
        /* method that check if the prefab should be flipped */
        private void UpdateSpriteDirection(Vector3 movement)
        {
            if (movement.x > 0 && _player.GetIsFacingLeft() ||
                movement.x < 0 && !_player.GetIsFacingLeft())
                Flip();
        }

        /* method that flip the prefab of the character in the direction that is facing */
        private void Flip()
        {
            if (_player.GetIsFacingLeft())
            {
                _player.GetSprite().transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                _player.GetSprite().transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            _player.SetIsFacingLeft(!_player.GetIsFacingLeft());
        }


        #endregion
    }
}
