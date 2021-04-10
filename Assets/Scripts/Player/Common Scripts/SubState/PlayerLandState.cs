using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerLandState : PlayerGroundedState
    {
        public PlayerLandState(Werewolf player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            Debug.Log("stuck here??");
            
                if (_xInput != 0)
                {
                    _stateMachine.ChangeState(_player._moveState);
                }
                else
                {
                    _stateMachine.ChangeState(_player._idleState);
                }
                  
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}