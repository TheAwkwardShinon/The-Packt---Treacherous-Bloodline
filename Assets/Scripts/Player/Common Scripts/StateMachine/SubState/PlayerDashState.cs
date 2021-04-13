using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerDashState : PlayerAbilityState
    {
        public bool CanDash { get; private set; }
        private bool _isHolding;
        private bool _dashInputStop;

        private float _lastDashTime;

        private Vector2 _dashDirection;
        private Vector2 _dashDirectionInput;
        private Vector2 _lastAIPos;

        public PlayerDashState(Werewolf player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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
/*
            if (!_isExitingState)
            {
                Debug.Log("[DASH STATE] is not exiting from state, just setting directions and velocity");
                _player._anim.SetFloat("yVelocity", _player._currentVelocity.y);
                _player._anim.SetFloat("xVelocity", Mathf.Abs(_player._currentVelocity.x));
                _dashDirection = Vector2.right * _player._facingDirection;
                _dashDirection.Normalize();
                _player.CheckIfShouldFlip(Mathf.RoundToInt(_dashDirection.x));
                _player.SetRigidBodyDrag(_player.GetPlayerData().drag);
                _player.SetVelocity(_player.GetPlayerData().dashVelocity, _dashDirection);                    

            }
            else
            {
                Debug.Log("[DASH STATE] is exiting from state, set velocity and check if ability is done");
                _player.SetVelocity(_player.GetPlayerData().dashVelocity, _dashDirection);
                if (Time.time >= _startTime + _player.GetPlayerData().dashTime)
                {
                    Debug.Log("[DASH STATE] ability done");
                    _player.SetRigidBodyDrag(0f);
                    _isAbilityDone = true;
                    _lastDashTime = Time.time;
                }
            }*/
            Debug.Log("[DASH STATE] performing dash");
            _player._anim.SetFloat("yVelocity", _player._currentVelocity.y);
            _player._anim.SetFloat("xVelocity", Mathf.Abs(_player._currentVelocity.x));
            _dashDirection = Vector2.right * _player._facingDirection;
            _dashDirection.Normalize();
            _player.CheckIfShouldFlip(Mathf.RoundToInt(_dashDirection.x));
            _player.SetRigidBodyDrag(_player.GetPlayerData().drag);
            _player.SetVelocity(_player.GetPlayerData().dashVelocity, _dashDirection);
            if (Time.time >= _startTime + _player.GetPlayerData().dashTime)
            {
                Debug.Log("[DASH STATE] ability done");
                _player.SetRigidBodyDrag(0f);
                _isAbilityDone = true;
                _lastDashTime = Time.time;
            }
           
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }


        public bool CheckIfCanDash()
        {
            return CanDash && Time.time >= _lastDashTime + _player.GetPlayerData().dashCooldown;
        }

        public void ResetCanDash() => CanDash = true;

    }
}