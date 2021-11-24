using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerMoveState : PlayerGroundedState
    {
        public PlayerMoveState(Player player, PlayerStateMachine stateMachine,PlayerData data, string animation,string wolfAnimBool) : base(player, stateMachine,data,animation,wolfAnimBool)
        {
        }

        public override void Checks()
        {
            base.Checks();
        }

        public override void Enter()
        {
            base.Enter();
        }

       
        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            _player.CheckIfShouldFlip(_xInput);
            if(_player.state.slowDebuff)
                _player.SetVelocityX(_player.GetPlayerData().velocityWhenSlowed * _xInput);
            else _player.SetVelocityX(_player.GetPlayerData().movementVelocity * _xInput);

            if (!_isExitingState)
            {
                if (_xInput == 0)
                {
                    _stateMachine.ChangeState(_player._idleState);
                }
                else if (_yInput == -1)
                {
                    SetColliderSizeEvent evnt;
                    evnt = SetColliderSizeEvent.Create(Bolt.GlobalTargets.Everyone);

                    if (_player.GetIsHuman())
                    {
                        evnt.Offset = new Vector2(-1.39756346f, -8.94320488f);
                        evnt.Size = new Vector2(11.9792986f, 29.9855232f);

                    }
                    else
                    {
                        evnt.Offset = new Vector2(-2.93918324f, -8.61205387f);
                        evnt.Size = new Vector2(28.6108532f, 30.6478252f);
                    }
                    evnt.TargetPlayerNetworkID = _player.entity.NetworkId;
                    evnt.Send();

                    _stateMachine.ChangeState(_player._crouchMoveState);
                }
            }        
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

       
       
    }
}
