using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerCrouchMoveState : PlayerGroundedState
    {
        public PlayerCrouchMoveState(Werewolf player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _player.SetColliderHeight(_player.GetPlayerData().crouchColliderHeight);
        }

        public override void Exit()
        {
            base.Exit();
            _player.SetColliderHeight(_player.GetPlayerData().standColliderHeight);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!_isExitingState)
            {
                _player.SetVelocityX(_player.GetPlayerData().crouchMovementVelocity * _player._facingDirection);
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
    }
}
