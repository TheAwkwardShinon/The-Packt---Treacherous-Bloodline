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

            _isHolding = true;
            _dashDirection = Vector2.right * _player._facingDirection;

            Time.timeScale = _player.GetPlayerData().holdTimeScale;
            _startTime = Time.unscaledTime;

            _player._dashDirectionIndicator.gameObject.SetActive(true);

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

            if (!_isExitingState)
            {

                _player._anim.SetFloat("yVelocity", _player._currentVelocity.y);
                _player._anim.SetFloat("xVelocity", Mathf.Abs(_player._currentVelocity.x));


                if (_isHolding)
                {
                    _dashDirectionInput = _player._inputHandler._dashDirectionInput;
                    _dashInputStop = _player._inputHandler._dashInputStop;

                    if(_dashDirectionInput != Vector2.zero)
                    {
                        _dashDirection = _dashDirectionInput;
                        _dashDirection.Normalize();
                    }

                    float angle = Vector2.SignedAngle(Vector2.right, _dashDirection);
                    _player._dashDirectionIndicator.rotation = Quaternion.Euler(0f, 0f, angle - 45f);

                    if(_dashInputStop || Time.unscaledTime >= _startTime + _player.GetPlayerData().maxHoldTime)
                    {
                        _isHolding = false;
                        Time.timeScale = 1f;
                        _startTime = Time.time;
                        _player.CheckIfShouldFlip(Mathf.RoundToInt(_dashDirection.x));
                        _player.SetRigidBodyDrag(_player.GetPlayerData().drag);
                        _player.SetVelocity(_player.GetPlayerData().dashVelocity, _dashDirection);
                        _player._dashDirectionIndicator.gameObject.SetActive(false);
                        //PlaceAfterImage();
                    }
                }
                else
                {
                    _player.SetVelocity(_player.GetPlayerData().dashVelocity, _dashDirection);
                   // CheckIfShouldPlaceAfterImage();

                    if (Time.time >= _startTime + _player.GetPlayerData().dashTime)
                    {
                        _player.SetRigidBodyDrag(0f);
                        _isAbilityDone = true;
                        _lastDashTime = Time.time;
                    }
                }
            }
        }
/*
        private void CheckIfShouldPlaceAfterImage()
        {
            if(Vector2.Distance(player.transform.position, lastAIPos) >= playerData.distBetweenAfterImages)
            {
                PlaceAfterImage();
            }
        }

        private void PlaceAfterImage()
        {
            _playerAfterImagePool.Instance.GetFromPool();
            _lastAIPos = player.transform.position;
        }*/

        public bool CheckIfCanDash()
        {
            return CanDash && Time.time >= _lastDashTime + _player.GetPlayerData().dashCooldown;
        }

        public void ResetCanDash() => CanDash = true;

    }
}