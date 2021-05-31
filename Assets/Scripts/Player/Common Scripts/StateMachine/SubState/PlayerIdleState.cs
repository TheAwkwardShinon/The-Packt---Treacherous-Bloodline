using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(Player player, PlayerStateMachine stateMachine,PlayerData data, string animation,string wolfAnimBool) : base(player, stateMachine, data,animation,wolfAnimBool)
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

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            //Debug.LogWarning("[IDLE STATE] WAIT TO CHANGE STATE.....");

            if (_xInput != 0)
            {
                _stateMachine.ChangeState(_player._moveState);
            }
            else if (_yInput == -1)
            {
                _stateMachine.ChangeState(_player._crouchIdleState);
            }   

        }
    }
}
