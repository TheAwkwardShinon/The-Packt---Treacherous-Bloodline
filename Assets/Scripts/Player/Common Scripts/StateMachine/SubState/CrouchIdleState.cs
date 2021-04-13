using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{

    public class PlayerCrouchIdleState : PlayerGroundedState
    {

        #region variables
        private float _heightValue;
        #endregion

        #region methods

        public PlayerCrouchIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {

        }

        /* when the player enter in this state we set velocity to zero and change the collider height */
        public override void Enter()
        {
            base.Enter();
            _heightValue = _player.GetPlayerData().ceilingHeight;
            _player.GetPlayerData().ceilingHeight = _heightValue *2 +0.05f;
            _player.SetVelocityZero();
            _player.SetColliderHeight(_player.GetPlayerData().crouchColliderHeight);

            
        }

        /* reset the collider height */
        public override void Exit()
        {
            base.Exit();
            _player.SetColliderHeight(_player.GetPlayerData().standColliderHeight);
            _player.GetPlayerData().ceilingHeight = _heightValue;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            _isTouchingCeiling = _player.CheckForCeiling();

            if (!_isExitingState)
            {
                if(_xInput != 0)
                {
                    _stateMachine.ChangeState(_player._crouchMoveState);
                }
                else if(_yInput != -1 && !_isTouchingCeiling)
                {
                    Debug.Log("yinput = "+_yInput);
                    _stateMachine.ChangeState(_player._idleState);
                }
            } 
        }

        #endregion
    }
}