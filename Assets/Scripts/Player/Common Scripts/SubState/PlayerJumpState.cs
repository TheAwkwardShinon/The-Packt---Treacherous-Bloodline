using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace ThePackt{
    public class PlayerJumpState : PlayerAbilityState
    {
        private int amountOfJumpsLeft;

        public PlayerJumpState(Werewolf player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
            amountOfJumpsLeft = playerData.amountOfJumps;
        }

        public override void Enter()
        {
            base.Enter();
            _player._inputHandler.UseJumpInput();
            _player.SetVelocityY(_player.GetPlayerData().jumpVelocity);
            _isAbilityDone = true;
            amountOfJumpsLeft--;
            _player._inAirState.SetIsJumping();
        }

        public bool CanJump()
        {
            if (amountOfJumpsLeft > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ResetAmountOfJumpsLeft() => amountOfJumpsLeft = _player.GetPlayerData().amountOfJumps;

        public void DecreaseAmountOfJumpsLeft() => amountOfJumpsLeft--;
    }
}
