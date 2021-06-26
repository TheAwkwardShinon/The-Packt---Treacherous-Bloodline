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
        private bool _specialAttack;

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

        

        public PlayerInAirState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName,string wolfAnimBool) : base(player, stateMachine, playerData, animBoolName,wolfAnimBool)
        {
        }

        public override void Checks()
        {
            base.Checks();
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
            _specialAttack = _player._inputHandler._specialAttackInput;
            _isGrounded = _player.CheckIfGrounded();


           

            CheckJumpMultiplier();

           
            if(_player._isDowned){
                Debug.LogWarning("[IN AIR STATE] -->  DOWN");
                 _player.GetPlayerData().downedStartTime = Time.time;
                _stateMachine.ChangeState(_player._downState);
            }
            else if(_detransformationInput && !_player.GetIsHuman()){
                Debug.LogWarning("[IN AIR STATE] ----> detransform");
                _player.SetIsHuman(true);
                _detransformationInput = false;
                _stateMachine.ChangeState(_player._detransformationState);
            }
            else if (_isGrounded && _player._currentVelocity.y < 0.01f)
            {        
                Debug.LogError("[IN AIR STATE] ----> land");
               // _player._jumpState.ResetAmountOfJumpsLeft();
                _stateMachine.ChangeState(_player._landState);
            }
            else if(_jumpInput && _player._jumpState.CanJump())
            {
                Debug.LogError("[IN AIR STATE] ----> jump");
                _stateMachine.ChangeState(_player._jumpState); //if more jump can be done
            }
            else if(_isTouchingWall && _xInput == _player._facingDirection && _player._currentVelocity.y <= 0)
            {
                Debug.LogError("[IN AIR STATE] ----> wallslide");
                _stateMachine.ChangeState(_player._wallSlideState);
            }
            else if(_dashInput && _player._dashState.CheckIfCanDash())
            {
                Debug.LogError("[IN AIR STATE] ----> dash");
                _stateMachine.ChangeState(_player._dashState);
            }
            else if (_attackInput && _player._attackState.CheckIfCanAttack())
            {
                Debug.LogError("[IN AIR STATE] ----> attack");
                _stateMachine.ChangeState(_player._attackState);
            }
            else if(_transformationInput && _player.GetIsHuman()){
                Debug.LogWarning("[IN AIR STATE] ----> transformation");
                _player.GetPlayerData()._startTransformationTime = Time.time;
                _player.SetIsHuman(false);
                _stateMachine.ChangeState(_player._transformState);
            }
            else if(_specialAttack && !_player.GetIsHuman() && _player._specialAttack.CheckIfCanAttack()){
                     Debug.LogWarning("[IN AIR STATE] -->  special");
                     _stateMachine.ChangeState(_player._specialAttack);
            }
            else
            {
                //Debug.Log("[PLAYER IS IN AIR] no input from palyer, just falling, waiting to land");
                _player.CheckIfShouldFlip(_xInput);
                _player.SetVelocityX(_player.GetPlayerData().movementVelocity * _xInput);

                _player._anim.SetFloat("yVelocity", _player._currentVelocity.y);
                _player._anim.SetFloat("xVelocity", Mathf.Abs(_player._currentVelocity.x));
                _player.state.yVelocity = _player._currentVelocity.y;
                _player.state.xVelocity = Mathf.Abs(_player._currentVelocity.x);
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