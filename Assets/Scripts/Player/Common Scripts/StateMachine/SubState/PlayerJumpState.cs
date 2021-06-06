using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace ThePackt{
    public class PlayerJumpState : PlayerAbilityState
    {
        private int amountOfJumpsLeft;

        public PlayerJumpState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName,string wolfAnimBool) : base(player, stateMachine, playerData, animBoolName,wolfAnimBool)
        {
            amountOfJumpsLeft = playerData.amountOfJumps;
        }

        public override void Enter()
        {
            base.Enter();
            _player._inputHandler.UseJumpInput();
            _player.SetVelocityY(_player.GetPlayerData().jumpVelocity);

            if (_player.GetIsHuman())
            {
                _player.PlayJumpSFX();
            }
            else
            {
                _player.PlayWolfJumpSFX();
            }
            
            Debug.LogWarning("[JUMP STATE] entered + number of jump left before perform this: "+amountOfJumpsLeft);
            amountOfJumpsLeft--;
            Debug.LogWarning("[JUMP STATE] numebr of jump left after this jump = "+ amountOfJumpsLeft);
            _player._inAirState.SetIsJumping();
            _isAbilityDone = true;
        }

        public override void Exit()
        {
            base.Exit();
            
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public bool CanJump()
        {
            Debug.LogWarning("[CAN JUMP] number of jump left: "+amountOfJumpsLeft);
            if (amountOfJumpsLeft > 0 && !_player.GetPlayerData().cantJump && !_player._crouchIdleState.isCrouched())
            {
                Debug.LogWarning("[CAN JUMP] returning true");
                return true;
            }
            else
            {
                Debug.LogWarning("[CAN JUMP] returning false");
                return false;
            }
        }

        public void ResetAmountOfJumpsLeft() => amountOfJumpsLeft = _player.GetPlayerData().amountOfJumps;

        public void DecreaseAmountOfJumpsLeft() => amountOfJumpsLeft--;
    }
}
