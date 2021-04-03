using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class InputHandler : MonoBehaviour
    {
       private Werewolf _player;
       private bool _allowInput = true; //true until we implement aniamtor
       private Vector2 _movement;





        private void Start()
        {
            _player = GetComponent<Werewolf>();
        }


        private void Update()
        {
            
            if (_allowInput) //true until we implement aniamtor
            {
                HandleMovementInput(Time.deltaTime);

                HandleDashInput(Time.deltaTime);

                HandleJumpInput(Time.deltaTime);

               //TODO  HandleAttacks(Time.deltaTime);

            }
        }

         private void HandleMovementInput(float delta)
        {
            _movement.x = Input.GetAxisRaw("Horizontal");
            _movement.y = 0;
            _movement.Normalize();
            _player.SetDirection(_movement);
            _player.SetIsMoving(true);
            //TODO animator trigger
        }

        private void HandleDashInput(float delta){

            if (Input.GetKey(KeyCode.Space)){ //TODO adding stamina??? not a good idea to dash-spamming.
                _player.SetIsDashing(true);
                //TODO set trigger like : _animatorHandler.ActivateTargetTrigger("Dash");
                //if stamina then spend it here.
            }
        }

        private void HandleJumpInput(float delta){
            // Debug.Log("is pressing w?");
            if(Input.GetKeyDown(KeyCode.W) && _player.GetIsGrounded()){ //Stamina??
                // Debug.Log("yes!");
                _player.SetIsJumping(true);
                //if stamina then spend it here
                //TODO set animator trigger
            } 
            // else Debug.Log("no");
            
        }


    }
}
