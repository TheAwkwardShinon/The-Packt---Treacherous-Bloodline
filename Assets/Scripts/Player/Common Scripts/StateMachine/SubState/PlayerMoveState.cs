using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerMoveState : PlayerGroundedState
    {
        public PlayerMoveState(Player player, PlayerStateMachine stateMachine,PlayerData data, string animation) : base(player, stateMachine,data,animation)
        {
        }

        public override void Checks()
        {
            base.Checks();
        }

        public override void Enter()
        {
            base.Enter();
        }

       
        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            _player.CheckIfShouldFlip(_xInput);
            if(_player.state.slowDebuff)
                _player.SetVelocityX(_player.GetPlayerData().velocityWhenSlowed * _xInput);
            else _player.SetVelocityX(_player.GetPlayerData().movementVelocity * _xInput);

            if (!_isExitingState)
            {
                if (_xInput == 0)
                {
                    _stateMachine.ChangeState(_player._idleState);
                }
                else if (_yInput == -1)
                {
                    _stateMachine.ChangeState(_player._crouchMoveState);
                }
            }        
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

       
       
    }
}
