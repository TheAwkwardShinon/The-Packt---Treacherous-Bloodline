using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{

    public class PlayerCrouchIdleState : PlayerGroundedState
    {

        #region variables
        private float _heightValue;
        #endregion

        #region methods

        public PlayerCrouchIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName,string wolfAnimBool) : base(player, stateMachine, playerData, animBoolName,wolfAnimBool)
        {

        }

        /* when the player enter in this state we set velocity to zero and change the collider height */
        public override void Enter()
        {
            base.Enter();
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
            if(_player.GetIsHuman()){
                _player.GetComponent<BoxCollider2D>().offset = new Vector2(-4.783879f,-18.35041f);
                _player.GetComponent<BoxCollider2D>().size = new Vector2(38.76051f,10.20428f);
            }else{
                _player.GetComponent<BoxCollider2D>().offset = new Vector2(-5.583345f,-9.130901f);
                _player.GetComponent<BoxCollider2D>().size = new Vector2(36.60429f,29.61012f);
            }

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
                else if(_yInput != -1 && !_isTouchingCeiling)
                {   
                    if(_player.GetIsHuman()){
                         _player.GetComponent<BoxCollider2D>().offset = new Vector2(-0.7352595f,-5.962845f);
                         _player.GetComponent<BoxCollider2D>().size = new Vector2(8.667796f,35.94624f);
                    }
                    else{
                        _player.GetComponent<BoxCollider2D>().offset = new Vector2(-1.780157f,-5.962845f);
                        _player.GetComponent<BoxCollider2D>().size = new Vector2(24.9682f,35.94624f);
                    }
                    _stateMachine.ChangeState(_player._idleState);
                }
            } 
        }

        #endregion
    }
}