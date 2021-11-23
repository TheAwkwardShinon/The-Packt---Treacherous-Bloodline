using Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{

    public class PlayerCrouchIdleState : PlayerGroundedState
    {

        #region variables
        private float _heightValue;
        private bool _isCrouched = false;
        #endregion

        #region methods

        public PlayerCrouchIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName,string wolfAnimBool) : base(player, stateMachine, playerData, animBoolName,wolfAnimBool)
        {

        }

        /* when the player enter in this state we set velocity to zero and change the collider height */
        public override void Enter()
        {
            base.Enter();
            _isCrouched = true;
            _player.SetVelocityZero();
               
        }

        /* reset the collider height */
        public override void Exit()
        {
            base.Exit();
            _player.GetPlayerData().ceilingHeight = _heightValue;
        }

         public override void AnimationFinishTrigger()
        {
            SetColliderSizeEvent evnt;
            evnt = SetColliderSizeEvent.Create(GlobalTargets.Everyone);
            evnt.TargetPlayerNetworkID = _player.entity.NetworkId;

            if (_player.GetIsHuman()){
                
                evnt.Offset = new Vector2(-1.39756346f, -8.94320488f);
                evnt.Size = new Vector2(11.9792986f, 29.9855232f);

                Debug.Log("[SIZEEE] crouch");

                _player.GetComponent<BoxCollider2D>().offset = new Vector2(-1.39756346f, -8.94320488f);
                _player.GetComponent<BoxCollider2D>().size = new Vector2(11.9792986f, 29.9855232f);
            }else{

                evnt.Offset = new Vector2(-2.93918324f, -8.61205387f);
                evnt.Size = new Vector2(28.6108532f, 30.6478252f);

                Debug.Log("[SIZEEE] crouch");

                _player.GetComponent<BoxCollider2D>().offset = new Vector2(-2.93918324f, -8.61205387f);
                _player.GetComponent<BoxCollider2D>().size = new Vector2(28.6108532f, 30.6478252f);
            }

            evnt.Send();

        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            _isTouchingCeiling = _player.CheckForCeiling();

            if (!_isExitingState)
            {
                if(_xInput != 0)
                {
                    _stateMachine.ChangeState(_player._crouchMoveState);
                }
                else if(_yInput !=-1 && !_isTouchingCeiling)
                {   
                    SetCrouch(false);

                    SetColliderSizeEvent evnt;
                    evnt = SetColliderSizeEvent.Create(GlobalTargets.Everyone);
                    evnt.TargetPlayerNetworkID = _player.entity.NetworkId;

                    if (_player.GetIsHuman()){

                        evnt.Offset = new Vector2(-0.7352595f, -5.962845f);
                        evnt.Size = new Vector2(8.667796f, 35.94624f);

                        Debug.Log("[SIZEEE] crouch o");

                       // _player.GetComponent<BoxCollider2D>().offset = new Vector2(-0.7352595f,-5.962845f);
                        // _player.GetComponent<BoxCollider2D>().size = new Vector2(8.667796f,35.94624f);
                    }
                    else{

                        evnt.Offset = new Vector2(-1.780157f, -5.962845f);
                        evnt.Size = new Vector2(24.9682f, 35.94624f);

                        Debug.Log("[SIZEEE] crouch o");

                        _player.GetComponent<BoxCollider2D>().offset = new Vector2(-1.780157f,-5.962845f);
                        _player.GetComponent<BoxCollider2D>().size = new Vector2(24.9682f,35.94624f);
                    }
                    evnt.Send();

                    _stateMachine.ChangeState(_player._idleState);
                }
            } 
        }

        public bool isCrouched(){
            return _isCrouched;
        }

        public void SetCrouch(bool value){
            _isCrouched = value;
        }

        #endregion
    }
}