using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerLandState : PlayerGroundedState
    {
        public PlayerLandState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
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
            Debug.Log("[LAND STATE] wait to understand what state will be, idle or move?");
            
                if (_xInput != 0)
                {
                    Debug.Log("[LAND STATE] changing to move state...");
                    _stateMachine.ChangeState(_player._moveState);
                }
                else
                {
                    Debug.Log("[LAND STATE] changing state to idle state...");
                    _stateMachine.ChangeState(_player._idleState);
                }
                  
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}