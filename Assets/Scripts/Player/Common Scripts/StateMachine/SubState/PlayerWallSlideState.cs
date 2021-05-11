using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ThePackt{
    public class PlayerWallSlideState : PlayerState
    {
        public PlayerWallSlideState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();


                _player.SetVelocityY(_player.GetPlayerData().wallSlideVelocity);
                if(_player.CheckIfGrounded() || _player.CheckIfGroundedOnEnemy() || _player.CheckIfGroundOnOtherPlayer() || !_player.CheckIfTouchingWall()) 
                    _stateMachine.ChangeState(_player._idleState);
        
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}