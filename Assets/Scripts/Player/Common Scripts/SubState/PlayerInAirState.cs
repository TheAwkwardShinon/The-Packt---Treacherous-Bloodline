using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerInAirState : PlayerState
    {
        //Input
        private int _xInput;
        private bool _jumpInput;
        private bool _jumpInputStop;
        private bool _grabInput;
        private bool _dashInput;

        //Checks
        private bool _isGrounded;
        private bool _isTouchingWall;
        private bool _isTouchingWallBack;
        private bool _oldIsTouchingWall;
        private bool _oldIsTouchingWallBack;
        private bool _isTouchingLedge;

        private bool _coyoteTime;
        private bool _wallJumpCoyoteTime;
        private bool _isJumping;

        private float _startWallJumpCoyoteTime;

        public PlayerInAirState(Werewolf player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
        }

        public override void Checks()
        {
            base.Checks();

            _oldIsTouchingWall = _isTouchingWall;
            _oldIsTouchingWallBack = _isTouchingWallBack;

            _isGrounded = _player.GetIsGrounded();
            _isTouchingWall = _player.CheckIfTouchingWall();
            
            _isTouchingLedge = _player.CheckIfTouchingLedge();


        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();
            _oldIsTouchingWall = false;
            _isTouchingWall = false;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            _xInput = _player._inputHandler._normInputX;
            _jumpInput = _player._inputHandler._jumpInput;
            _jumpInputStop = _player._inputHandler._jumpInputStop;
            _dashInput = _player._inputHandler._dashInput;

            CheckJumpMultiplier();
            /*
            if (_player.InputHandler.AttackInputs[(int)CombatInputs.primary])
            {
                stateMachine.ChangeState(player.PrimaryAttackState);
            }
            */
            if (_isGrounded && _player._currentVelocity.y < 0.01f)
            {            
                _stateMachine.ChangeState(_player._landState);
            }
            /*
            else if(_isTouchingWall && !_isTouchingLedge && !_isGrounded)
            {
                stateMachine.ChangeState(_player._slideState);
            }*/
            else if(_jumpInput && _player._jumpState.CanJump())
            {
                _stateMachine.ChangeState(_player._jumpState); //if more jump can be done
            }
            else if(_isTouchingWall && _xInput == _player._facingDirection && _player._currentVelocity.y <= 0)
            {
                _stateMachine.ChangeState(_player._wallSlideState);
            }
            else if(_dashInput && _player._dashState.CheckIfCanDash())
            {
                _stateMachine.ChangeState(_player._dashState);
            }
            else
            {
                _player.CheckIfShouldFlip(_xInput);
                _player.SetVelocityX(_player.GetPlayerData().movementVelocity * _xInput);

                _player._anim.SetFloat("yVelocity", _player._currentVelocity.y);
                _player._anim.SetFloat("xVelocity", Mathf.Abs(_player._currentVelocity.x));
            }

        }

        private void CheckJumpMultiplier()
        {
            if (_isJumping)
            {
                if (_jumpInputStop)
                {
                    _player.SetVelocityY(_player._currentVelocity.y * _player.GetPlayerData().variableJumpHeightMultiplier);
                    _isJumping = false;
                }
                else if (_player._currentVelocity.y <= 0f)
                {
                    _isJumping = false;
                }

            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public void SetIsJumping() => _isJumping = true;
    }
}