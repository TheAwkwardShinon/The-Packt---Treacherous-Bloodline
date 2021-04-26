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
            Debug.Log("[GROUNDED STATE] numero salti resettato a 1");
            _player._jumpState.ResetAmountOfJumpsLeft();
            _player._dashState.ResetCanDash(); //todo usless line
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            Debug.Log("[GROUNDED STATE] is grounded? "+ _isGrounded);
            Debug.Log("attack input state " + _attackInput);

            _xInput = _player._inputHandler._normInputX;
            _yInput = _player._inputHandler._normInputY;
            _jumpInput = _player._inputHandler._jumpInput;;
            _dashInput = _player._inputHandler._dashInput;
            _transformInput = _player._inputHandler._transformInput;
            _attackInput = _player._inputHandler._attackInputs.ContainsValue(true);

            _isTouchingCeiling = _player.CheckForCeiling();
            _isGrounded = _player.CheckIfGrounded();

            
            Debug.Log("aaaaa " + _attackInput + " " + _player._attackState.CheckIfCanAttack());
            Debug.Log("aaaaaa jump input " + _jumpInput + " " + _player._jumpState.CanJump() + " " + _isGrounded + " " + !_isTouchingCeiling);

            if (_jumpInput && _player._jumpState.CanJump() && _isGrounded && !_isTouchingCeiling)
            {
                Debug.Log("[GROUNDED STATE] player pushing to jump, player is grounded -> "+_isGrounded+" and can jump so passing to jump state...");
                _stateMachine.ChangeState(_player._jumpState);
            }else if (!_isGrounded)
            {
                Debug.Log("[GROUNDED STATE] dunno wht, but the player is no more on the ground. changing state to on air ... ");
                _stateMachine.ChangeState(_player._inAirState);
            }
            else if (_dashInput && _player._dashState.CheckIfCanDash())
            {
                Debug.Log("[GROUNDED STATE] palyer is pushing space so he is wanna dash. changing to dash state...");
                _stateMachine.ChangeState(_player._dashState);
            }
            else if (_attackInput && _player._attackState.CheckIfCanAttack())
            {
                Debug.Log("[GROUNDED STATE] player can attack and player is pressing left mouse, entering in attack state... ");
                _stateMachine.ChangeState(_player._attackState);
            }
            else if(_transformInput && _player.GetIsHuman()){
                 Debug.Log("[GROUNDED STTE] player is human and wants to transform ");
                 _stateMachine.ChangeState(_player._transformState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}
