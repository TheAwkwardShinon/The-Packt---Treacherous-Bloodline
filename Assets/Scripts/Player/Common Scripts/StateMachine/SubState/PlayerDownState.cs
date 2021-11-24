using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class PlayerDownState : PlayerGroundedState
    {
        public PlayerDownState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName,string wolfAnimBool) : base(player, stateMachine, playerData, animBoolName,wolfAnimBool)
        {
        }

       
        public override void Checks()
        {
            base.Checks();
        }

        public override void Enter()
        {
            base.Enter();

            _isStand = false;
            if( _player.GetPlayerData().numOfReviveAction > 0){
                 _player.GetPlayerData().numOfReviveAction = 0f;
                 _player.Heal();
            }

          
            Debug.LogWarning("[DOWNED STATE] ENTER");

        }

        public override void Exit()
        {
            base.Exit();
            Debug.LogWarning("[DOWNED STATE] EXIT ---> stand = "+_isStand);
        }

        public override void AnimationFinishTrigger()
        {
            SetColliderSizeEvent evnt;
            evnt = SetColliderSizeEvent.Create(Bolt.GlobalTargets.Everyone);

            if (_player.GetIsHuman())
            {
                evnt.Offset = new Vector2(0.920493364f, -12.7514372f);
                evnt.Size = new Vector2(13.9662056f, 22.3690586f);
            }
            else
            {
                evnt.Offset = new Vector2(-1.231987f, -11.0956793f);
                evnt.Size = new Vector2(26.8810806f, 26.3428669f);
            }
            evnt.TargetPlayerNetworkID = _player.entity.NetworkId;
            evnt.Send();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if(Time.time >= _player.GetPlayerData().downedStartTime + _player.GetPlayerData().bleedOutTime){
                _player.Die();
                return;
            }

            if(_player.state.Health >= _player.GetPlayerData().maxLifePoints * 0.3f){
                 Debug.LogWarning("[DOWNED STATE] ---> IDLE");
                _isStand = true;
                _player.state.isDowned = false;

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
                evnt.TargetPlayerNetworkID = _player.entity.NetworkId;
                evnt.Send();

                _stateMachine.ChangeState(_player._idleState);
            }else if(_xInput != 0){
                Debug.LogWarning("[DOWNED STATE] ---> DOWNED MOVE STATE");
                _stateMachine.ChangeState(_player._downMoveState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
