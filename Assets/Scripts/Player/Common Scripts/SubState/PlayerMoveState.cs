using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerMoveState : PlayerGroundedState
    {
        public PlayerMoveState(Werewolf player, PlayerStateMachine stateMachine,PlayerData data, string animation) : base(player, stateMachine,data,animation)
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
           // base.LogicUpdate;
            /*
            player.CheckIfShouldFlip(xInput);

            player.SetVelocityX(playerData.movementVelocity * xInput);

            if (!isExitingState)
            {
                if (_xInput == 0)
                {
                    _stateMachine.ChangeState(player.IdleState);
                }
                else if (_yInput == -1)
                {
                    _stateMachine.ChangeState(player.CrouchMoveState);
                }
            }     */   
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

       
       
    }
}
