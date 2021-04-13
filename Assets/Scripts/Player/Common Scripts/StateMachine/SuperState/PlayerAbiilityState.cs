using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerAbilityState : PlayerState
    {
        protected bool _isAbilityDone;

        private bool _isGrounded;

        public PlayerAbilityState(Werewolf player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
        }

        public override void Checks()
        {
            base.Checks();

            _isGrounded = _player.CheckIfGrounded();
        }

        public override void Enter()
        {
            base.Enter();
            
            _isAbilityDone = false;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (_isAbilityDone)
            {
                if (_isGrounded && _player._currentVelocity.y < 0.01f)
                {
                    _stateMachine.ChangeState(_player._idleState);
                }
                else
                {
                    _stateMachine.ChangeState(_player._inAirState);
                }
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}