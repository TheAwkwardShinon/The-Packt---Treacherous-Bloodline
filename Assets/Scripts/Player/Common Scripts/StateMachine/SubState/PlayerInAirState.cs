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
        private bool _attackInput;
        private bool _grabInput;
        private bool _dashInput;

        private bool _transformationInput;

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

        public PlayerInAirState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
        }

        public override void Checks()
        {
            base.Checks();
            Debug.Log("[PLAYER IS IN AIR] IN CHECKS(), CHECKING IS GROUNDED");
            _isGrounded = _player.CheckIfGrounded();
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
            _isTouchingWall = false;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            _xInput = _player._inputHandler._normInputX;
            _jumpInput = _player._inputHandler._jumpInput;
            _jumpInputStop = _player._inputHandler._jumpInputStop;
            _attackInput = _player._inputHandler._attackInputs.ContainsValue(true);
            _dashInput = _player._inputHandler._dashInput;
            _transformationInput = _player._inputHandler._transformInput;
            _isGrounded = _player.CheckIfGrounded();
            Debug.Log("[PLAYER IS IN AIR] is grounded: "+_isGrounded);

            CheckJumpMultiplier();

            if (_isGrounded && _player._currentVelocity.y < 0.01f)
            {        
                Debug.Log("[PLAYER IS IN AIR] seems that is grounded: "+_isGrounded + " so changing state to Land state...");    
                _stateMachine.ChangeState(_player._landState);
            }
            else if(_jumpInput && _player._jumpState.CanJump())
            {
                Debug.Log("[PLAYER IS IN AIR] player can jump, entering in jump state... ");
                _stateMachine.ChangeState(_player._jumpState); //if more jump can be done
            }
            else if(_isTouchingWall && _xInput == _player._facingDirection && _player._currentVelocity.y <= 0)
            {
                _stateMachine.ChangeState(_player._wallSlideState);
            }
            else if(_dashInput && _player._dashState.CheckIfCanDash())
            {
                Debug.Log("[PLAYER IS IN AIR] player can dash and player is pressing pace, entering in dash state... ");
                _stateMachine.ChangeState(_player._dashState);
            }
            else if (_attackInput && _player._attackState.CheckIfCanAttack())
            {
                Debug.Log("[PLAYER IS IN AIR] player can attack and player is pressing left mouse, entering in attack state... ");
                _stateMachine.ChangeState(_player._attackState);
            }
            else if(_transformationInput && _player.GetIsHuman()){
                 Debug.Log("[PLAYER IS IN AIR] player is human and wants to transform ");
                 _stateMachine.ChangeState(_player._transformState);
            }
            else
            {
                Debug.Log("[PLAYER IS IN AIR] no input from palyer, just falling, waiting to land");
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
            Debug.Log("[PLAYER IS IN AIR] PHYSICS UPDATE --> CALLING CHECKS");
        }

        public void SetIsJumping() => _isJumping = true;
    }
}