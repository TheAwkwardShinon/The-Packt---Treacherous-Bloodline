using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{

    public class PlayerGroundedState : PlayerState
    {
        protected int _xInput;
        protected int _yInput;

        protected bool _isTouchingCeiling;

        private bool _jumpInput;
        private bool _isGrounded;
        private bool _dashInput;

        public PlayerGroundedState(Werewolf player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
        }

        public override void Checks()
        {
            base.Checks();

            _isGrounded = _player.GetIsGrounded();
           
        }

        public override void Enter()
        {
            base.Enter();

           // _player._jumpState.ResetAmountOfJumpsLeft();
            //_player._dashState.ResetCanDash();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
/*
            _xInput = _player._inputHandler._normInputX;
            _yInput = _player._inputHandler._normInputY;
            _jumpInput = _player._inputHandler._jumpInput;;
            _dashInput = _player._inputHandler._dashInput;

            if (_player._inputHandler.AttackInputs[(int)CombatInputs.primary] ) //more attacks and special abilities
            {
                _stateMachine.ChangeState(_player._attackState);
            }
            
            else if (_jumpInput && _player._jumpState.CanJump())
            {
                _stateMachine.ChangeState(_player._jumpState);
            }else if (!_isGrounded)
            {
                _player._inAirState.StartCoyoteTime();
                _stateMachine.ChangeState(_player._inAirState);
            }
            else if (_dashInput && _player._dashState.CheckIfCanDash())
            {
                _stateMachine.ChangeState(_player._dashState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
        */
        }
    }
}
