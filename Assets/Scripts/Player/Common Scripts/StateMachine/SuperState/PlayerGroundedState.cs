using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{

    public class PlayerGroundedState : PlayerState
    {

        
        protected int _xInput;
        protected int _yInput;

        protected bool _isTouchingCeiling;
        private bool _isTouchingLedge;
        private bool _isTouchingWall;

        private bool _isNearDownedPlayer;


        protected bool _jumpInput;
        protected bool _isGrounded;
        private bool _dashInput;
        private bool _attackInput;

        private bool _transformInput;

        private bool _interactInput;

        private bool _specialAttack;

        protected bool _isStand = true;

        public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName,string wolfAnimBool) : base(player, stateMachine, playerData, animBoolName,wolfAnimBool)
        {
        }

        public override void Checks()
        {
            base.Checks();

            _isGrounded = _player.CheckIfGrounded();
            _isTouchingWall = _player.CheckIfTouchingWall();
            _isTouchingLedge = _player.CheckIfTouchingLedge();
            _isTouchingCeiling = _player.CheckForCeiling();
           
        }

        public override void Enter()
        {
            base.Enter();
            _player._dashState.ResetCanDash(); //todo usless line
            _player._jumpState.ResetAmountOfJumpsLeft();
        }

        public override void Exit()
        {
            base.Exit();
            
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            _xInput = _player._inputHandler._normInputX;
            _yInput = _player._inputHandler._normInputY;
            _jumpInput = _player._inputHandler._jumpInput;;
            _dashInput = _player._inputHandler._dashInput;
            _transformInput = _player._inputHandler._transformInput;
            _attackInput = _player._inputHandler._attackInputs.ContainsValue(true);
            _interactInput = _player._inputHandler._interactInput;
            _specialAttack = _player._inputHandler._specialAttackInput;
            _player._interactState.CheckIfCanInteract();

            _isTouchingCeiling = _player.CheckForCeiling();
            _isGrounded = _player.CheckIfGrounded();


            if(_isStand){
                if(_player._isDowned){
                     Debug.LogWarning("[GROUNDED STATE] -->  DOWN");
                     _player.GetPlayerData().downedStartTime = Time.time;
                    _stateMachine.ChangeState(_player._downState);
                }
                else if(_detransformationInput && !_player.GetIsHuman() ){
                    Debug.LogWarning("[GROUNDED STATE] -->  detransformation");
                    _player.SetIsHuman(true);
                    _detransformationInput = false;
                    _stateMachine.ChangeState(_player._detransformationState);
                }
                //else if (_jumpInput && _player._jumpState.CanJump() && _isGrounded && !_isTouchingCeiling && !_isTouchingWall||
                     //_isTouchingWall && _jumpInput && _player._jumpState.CanJump() && _isGrounded && !_isTouchingCeiling && _xInput == 0)
                else if(_jumpInput && _player._jumpState.CanJump())
                {
                    Debug.LogWarning("[GROUNDED STATE] -->  jump");
                    _stateMachine.ChangeState(_player._jumpState);
                }else if (!_isGrounded )
                {
                    Debug.LogWarning("[GROUNDED STATE] -->  inAir");
                    _stateMachine.ChangeState(_player._inAirState);
                }
                else if (_dashInput && _player._dashState.CheckIfCanDash()) //&& !_isTouchingWall)
                {
                    Debug.LogWarning("[GROUNDED STATE] -->  dash");
                    _stateMachine.ChangeState(_player._dashState);
                }
                else if (_attackInput && _player._attackState.CheckIfCanAttack())
                {
                    Debug.LogWarning("[GROUNDED STATE] -->  attack");
                    _stateMachine.ChangeState(_player._attackState);
                }
                else if(_transformInput && _player.GetIsHuman()){
                    Debug.LogWarning("[GROUNDED STATE] -->  transform");
                    _player.GetPlayerData()._startTransformationTime = Time.time;
                    _player.SetIsHuman(false);
                    _stateMachine.ChangeState(_player._transformState);
                }
                else if(_interactInput && _player._interactState.CheckIfCanInteract()){
                    Debug.LogWarning("[GROUNDED STATE] -->  interact");
                    _stateMachine.ChangeState(_player._interactState);
                }
                else if(_specialAttack && !_player.GetIsHuman() && _player._specialAttack.CheckIfCanAttack()){
                     Debug.LogWarning("[GROUNDED STATE] -->  special");
                     _stateMachine.ChangeState(_player._specialAttack);
                }
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}
