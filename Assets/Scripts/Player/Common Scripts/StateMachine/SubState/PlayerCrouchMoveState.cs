using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerCrouchMoveState : PlayerGroundedState
    {
        #region variables
        private float _heightValue;
        #endregion

        #region methods
        public PlayerCrouchMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName,string wolfAnimBool) : base(player, stateMachine, playerData, animBoolName,wolfAnimBool)
        {
        }


        /* when the player enter in this state we change the collider height */
        public override void Enter()
        {
            base.Enter();
            _player._crouchIdleState.SetCrouch(true);

        }

        /* reset the collider height */
        public override void Exit()
        {
            base.Exit();

        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            _isTouchingCeiling = _player.CheckForCeiling();
            _isGrounded = _player.CheckIfGrounded();
            if (!_isExitingState)
            {
                if(_player.GetPlayerData().isSlowed)
                    _player.SetVelocityX(_player.GetPlayerData().velocityWhenSlowed * _player._facingDirection);
                else _player.SetVelocityX(_player.GetPlayerData().crouchMovementVelocity * _player._facingDirection);
                _player.CheckIfShouldFlip(_xInput);

                if(!_isGrounded){
                     _player._crouchIdleState.SetCrouch(false);

                    SetColliderSizeEvent evnt;
                    evnt = SetColliderSizeEvent.Create(Bolt.GlobalTargets.Everyone);

                    if (_player.GetIsHuman())
                    {
                        evnt.Offset = new Vector2(-0.7352595f, -5.962845f);
                        evnt.Size = new Vector2(8.667796f, 35.94624f);

                    }
                    else
                    {
                        evnt.Offset = new Vector2(-1.780157f, -5.962845f);
                        evnt.Size = new Vector2(24.9682f, 35.94624f);
                    }

                    if (_player.GetComponent<BoxCollider2D>().size != new Vector2(evnt.Size.x, evnt.Size.y))
                    {
                        evnt.TargetPlayerNetworkID = _player.entity.NetworkId;
                        evnt.Send();
                    }

                    _stateMachine.ChangeState(_player._inAirState);
                }
                else if(_xInput == 0)
                {
                    _stateMachine.ChangeState(_player._crouchIdleState);
                }
                else if(_yInput != -1 && !_isTouchingCeiling)
                {
                    _player._crouchIdleState.SetCrouch(false);

                    SetColliderSizeEvent evnt;
                    evnt = SetColliderSizeEvent.Create(Bolt.GlobalTargets.Everyone);

                    if (_player.GetIsHuman())
                    {
                        evnt.Offset = new Vector2(-0.7352595f, -5.962845f);
                        evnt.Size = new Vector2(8.667796f, 35.94624f);

                    }
                    else
                    {
                        evnt.Offset = new Vector2(-1.780157f, -5.962845f);
                        evnt.Size = new Vector2(24.9682f, 35.94624f);
                    }

                    if (_player.GetComponent<BoxCollider2D>().size != new Vector2(evnt.Size.x, evnt.Size.y))
                    {
                        evnt.TargetPlayerNetworkID = _player.entity.NetworkId;
                        evnt.Send();
                    }

                    _stateMachine.ChangeState(_player._moveState);
                }

            }

        }

        #endregion
    }
}
