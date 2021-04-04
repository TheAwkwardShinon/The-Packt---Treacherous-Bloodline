using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ThePackt.Werewolf;

namespace ThePackt{
    public class InputHandler : MonoBehaviour
    {
        #region variables
       private Werewolf _player;

       private MovementController _moveControl;
       private CombatSystem _combat;
       private bool _allowInput = true; //true until we implement aniamtor
       private Vector2 _movement;


        #endregion


        #region methods

        private void Start()
        {
            _player = GetComponent<Werewolf>();
            _combat = GetComponent<CombatSystem>();
            _moveControl = GetComponent<MovementController>();
        }

        /* in the update we shoul check if the palyer is performing inputs */
        private void Update()
        {
            
            if (_allowInput) //true until we implement aniamtor
            {
                HandleMovementInput(Time.deltaTime);

                HandleDashInput(Time.deltaTime);

                HandleJumpInput(Time.deltaTime);

                HandleCrouchInput(Time.deltaTime);

                HandleAttackInput(Time.deltaTime);

                HandleTransformationInput(Time.deltaTime);
            }
        }

        /* method that check if is possible to move, changes state, activates the aniamtion trigger and then call the handler in order to perform teh action */
         private void HandleMovementInput(float delta)
        {
            if(_player.GetIsGrounded() && _player.GetCurrentState().Equals(State.IDLE)){
                Debug.Log(Time.time+" isgrounded = true");
                _movement.x = Input.GetAxisRaw("Horizontal");
                _movement.y = 0;
                _movement.Normalize();
                _player.SetDirection(_movement);
                _player.SetCurrentState(State.MOVE);
                _moveControl.Moving();
            }
            //TODO animator trigger
        }


        /* method that check if is possible to dash, changes state, activates the aniamtion trigger and then call the handler in order to perform teh action */

        private void HandleDashInput(float delta){
            if(_player.GetCurrentState().Equals(State.IDLE)||_player.GetCurrentState().Equals(State.MOVE)){
                if (Input.GetKey(KeyCode.Space)){ //TODO adding stamina??? not a good idea to dash-spamming.
                    _player.SetCurrentState(State.DASH);
                    _moveControl.Dashing();
                    //TODO set trigger like : _animatorHandler.ActivateTargetTrigger("Dash");
                    //if stamina then spend it here.
                }
            }
        }

        /* method that check if is possible to jump, changes state, activates the aniamtion trigger and then call the handler in order to perform teh action */

        private void HandleJumpInput(float delta){
            if(_player.GetCurrentState().Equals(State.IDLE) &&_player.GetIsGrounded()||_player.GetCurrentState().Equals(State.MOVE) && _player.GetIsGrounded()){
                if(Input.GetKeyDown(KeyCode.W) && _player.GetIsGrounded()){ 
                    _player.SetCurrentState(State.JUMP);
                    _moveControl.Jumping();
                    //TODO set animator trigger
                } 
            }
            //TODO SPECIAL ABILITY DOUBLE JUMP
        }

        /* method that check if is possible to move, changes state, activates the aniamtion trigger. This method checks if you release the input in order to change state */

        private void HandleCrouchInput(float delta)
        {
            if(_player.GetCurrentState().Equals(State.IDLE) && _player.GetIsGrounded()||_player.GetCurrentState().Equals(State.MOVE) || _player.GetCurrentState().Equals(State.CROUCH)){
                if (Input.GetKey(KeyCode.S))
                {
                    _player.SetCurrentState(State.CROUCH);
                    Debug.Log("["+Time.time+"]"+ " is crouched");
                    //TODO set animator trigger
                }
                if (Input.GetKeyUp(KeyCode.S))
                {
                    _player.SetCurrentState(State.IDLE);
                     Debug.Log("["+Time.time+"]"+ " not crouched anymore");
                    //TODO set animator trigger
                }
            }
        }

        /* method that check if is possible to attack, changes state, activates the aniamtion trigger and then call the handler in order to perform teh action */

        private void HandleAttackInput(float delta)
        {
            if(_player.GetCurrentState().Equals(State.IDLE)||_player.GetCurrentState().Equals(State.MOVE) || _player.GetCurrentState().Equals(State.CROUCH)){
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (_player.GetIsHuman())
                    {
                        Debug.Log("["+Time.time+"]"+"pressed human attacking");
                        _player.SetIsUsingHumanBaseAttack(true);
                        //TODO set animator trigger
                    }
                    else
                    {
                        Debug.Log("["+Time.time+"]"+"pressed werewolf attacking");
                        _player.SetIsUsingBaseWereWolfAttack(true);
                        //TODO set animator trigger
                    }
                }
            }
        }

        /* method that check if is possible to transform, changes state, activates the aniamtion trigger and then call the handler in order to perform teh action */

        private void HandleTransformationInput(float delta)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (_player.GetIsHuman())
                {
                    Debug.Log("["+Time.time+"]"+"transforming to werewolf");

                    _player.SetIsTransformingToWereWolf(true);
                    //TODO set animator trigger
                    _player.SetIsHuman(false);
                }
                else
                {
                    Debug.Log("transforming to human");

                    _player.SetIsTransformingToHuman(true);
                    //TODO set animator trigger
                    _player.SetIsHuman(true);
                }
            }
        }
        #endregion
    }
}
