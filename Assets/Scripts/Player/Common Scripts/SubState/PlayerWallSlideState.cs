using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ThePackt{
    public class PlayerWallSlideState : PlayerState
    {
        public PlayerWallSlideState(Werewolf player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!_isExitingState)
            {
                _player.SetVelocityY(_player.GetPlayerData().wallSlideVelocity);
                if(_player.GetIsGrounded())
                    _stateMachine.ChangeState(_player._idleState);
                
            }
        
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}