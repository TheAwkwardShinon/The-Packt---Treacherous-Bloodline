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


        private bool _jumpInput;
        private bool _isGrounded;
        private bool _dashInput;
        private bool _attackInput;

        private bool _transformInput;

        protected bool _isStand = true;

        public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

           // Debug.Log("[GROUNDED STATE] is grounded? "+ _isGrounded);
            //Debug.Log("attack input state " + _attackInput);

            _xInput = _player._inputHandler._normInputX;
            _yInput = _player._inputHandler._normInputY;
            _jumpInput = _player._inputHandler._jumpInput;;
            _dashInput = _player._inputHandler._dashInput;
            _transformInput = _player._inputHandler._transformInput;
            _attackInput = _player._inputHandler._attackInputs.ContainsValue(true);

            _isTouchingCeiling = _player.CheckForCeiling();
            _isGrounded = _player.CheckIfGrounded();


            if(_isStand){
                if(_downed){
                     Debug.LogError("[GROUNDED STATE] -->  DOWN");
                    _stateMachine.ChangeState(_player._downState);
                }
                else if(_detransformationInput && !_player.GetIsHuman() ){
                    Debug.LogError("[GROUNDED STATE] -->  detransformation");
                    _player.SetIsHuman(true);
                    _detransformationInput = false;
                    _stateMachine.ChangeState(_player._detransformationState);
                }
                else if (_jumpInput && _player._jumpState.CanJump() && _isGrounded && !_isTouchingCeiling )
                {
                    Debug.LogError("[GROUNDED STATE] -->  jump");
                    _stateMachine.ChangeState(_player._jumpState);
                }else if (!_isGrounded && !_downed)
                {
                    Debug.LogError("[GROUNDED STATE] -->  inAir");
                    _stateMachine.ChangeState(_player._inAirState);
                }
                else if (_dashInput && _player._dashState.CheckIfCanDash())
                {
                    Debug.LogError("[GROUNDED STATE] -->  dash");
                    _stateMachine.ChangeState(_player._dashState);
                }
                else if (_attackInput && _player._attackState.CheckIfCanAttack())
                {
                    Debug.LogError("[GROUNDED STATE] -->  attack");
                    _stateMachine.ChangeState(_player._attackState);
                }
                else if(_transformInput && _player.GetIsHuman()){
                    Debug.LogError("[GROUNDED STATE] -->  transform");
                    _player.GetPlayerData()._startTransformationTime = Time.time;
                    _player.SetIsHuman(false);
                    _stateMachine.ChangeState(_player._transformState);
                }
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}
