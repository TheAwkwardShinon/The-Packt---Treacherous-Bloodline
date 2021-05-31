using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ThePackt{
    public class PlayerWallSlideState : PlayerTouchingWallState
    {
        public PlayerWallSlideState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName,string wolfAnimBool) : base(player, stateMachine, playerData, animBoolName,wolfAnimBool)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            _player.SetVelocityY(_player.GetPlayerData().wallSlideVelocity);
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}