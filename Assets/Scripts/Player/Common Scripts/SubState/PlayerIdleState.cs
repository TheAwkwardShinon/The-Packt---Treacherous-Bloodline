using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(Werewolf player, PlayerStateMachine stateMachine,PlayerData data, string animation) : base(player, stateMachine, data,animation)
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
        }
    }
}
