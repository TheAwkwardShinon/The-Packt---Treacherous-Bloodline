using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerCrouchIdleState : PlayerGroundedState
    {
        public PlayerCrouchIdleState(Werewolf player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
        
            _player.SetVelocityZero();
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
                if(_xInput != 0)
                {
                    _stateMachine.ChangeState(_player._crouchMoveState);
                }
                else if(_yInput != -1 && !_isTouchingCeiling)
                {
                    _stateMachine.ChangeState(_player._idleState);
                }
            } 
        }
    }
}