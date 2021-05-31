using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerDashState : PlayerAbilityState
    {
        #region variables
        public bool CanDash { get; private set; }
        private bool _isHolding;
        private bool _dashInputStop;

        private float _lastDashTime;

        private Vector2 _dashDirection;
        private Vector2 _dashDirectionInput;
        private Vector2 _lastAIPos;

        #endregion 

        #region methods

        public PlayerDashState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName,string wolfAnimBool) : base(player, stateMachine, playerData, animBoolName,wolfAnimBool)
        {
        }

        
        public override void Enter()
        {
            base.Enter();

            CanDash = false;
            _player._inputHandler.UseDashInput();
            _dashDirection = Vector2.right * _player._facingDirection;


        }

        public override void Exit()
        {
            base.Exit();

            if(_player._currentVelocity.y > 0)
            {
                _player.SetVelocityY(_player._currentVelocity.y * _player.GetPlayerData().dashEndYMultiplier);
            }

        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
//
           // Debug.Log("[DASH STATE] performing dash");
            //_player._anim.SetFloat("yVelocity", _player._currentVelocity.y);
            //_player._anim.SetFloat("xVelocity", Mathf.Abs(_player._currentVelocity.x));
            _player.state.yVelocity = _player._currentVelocity.y;
            //_player.state.xVelocity = Mathf.Abs(_player._currentVelocity.x);
            _dashDirection = Vector2.right * _player._facingDirection;
            _dashDirection.Normalize();
            _player.CheckIfShouldFlip(Mathf.RoundToInt(_dashDirection.x));
            _player.SetVelocity(_player.GetPlayerData().dashVelocity, _dashDirection);
            if (Time.time >= _startTime + _player.GetPlayerData().dashTime)
            {
                _isAbilityDone = true;
                _player.SetVelocity(0f, _dashDirection);
                _lastDashTime = Time.time;
            }
           
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        /* method that check if the player can dash */
        public bool CheckIfCanDash()
        {
            return CanDash && Time.time >= _lastDashTime + _player.GetPlayerData().dashCooldown && !_player.GetPlayerData().cantDash;
        }
        
        /*set dash true */
        public void ResetCanDash() => CanDash = true;

        #endregion

    }
}