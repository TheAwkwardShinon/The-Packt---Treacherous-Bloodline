using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerCrouchMoveState : PlayerGroundedState
    {
        #region variables
        private float _heightValue;
        #endregion

        #region methods
        public PlayerCrouchMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName,string wolfAnimBool) : base(player, stateMachine, playerData, animBoolName,wolfAnimBool)
        {
        }


        /* when the player enter in this state we change the collider height */
        public override void Enter()
        {
            base.Enter();
           // _heightValue = _player.GetPlayerData().ceilingHeight;
            //_player.GetPlayerData().ceilingHeight = _heightValue *2 +0.05f;
           // _player.SetColliderHeight(_player.GetPlayerData().crouchColliderHeight);
        }

        /* reset the collider height */
        public override void Exit()
        {
            base.Exit();
            //_player.SetColliderHeight(_player.GetPlayerData().standColliderHeight);
            //_player.GetPlayerData().ceilingHeight = _heightValue;

        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            _isTouchingCeiling = _player.CheckForCeiling();
            if (!_isExitingState)
            {
                if(_player.GetPlayerData().isSlowed)
                    _player.SetVelocityX(_player.GetPlayerData().velocityWhenSlowed * _player._facingDirection);
                else _player.SetVelocityX(_player.GetPlayerData().crouchMovementVelocity * _player._facingDirection);
                _player.CheckIfShouldFlip(_xInput);

                if(_xInput == 0)
                {
                    _stateMachine.ChangeState(_player._crouchIdleState);
                }
                else if(_yInput != -1 && !_isTouchingCeiling)
                {
                    _stateMachine.ChangeState(_player._moveState);
                }

            }

        }

        #endregion
    }
}
