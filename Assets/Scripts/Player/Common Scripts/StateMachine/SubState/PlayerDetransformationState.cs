using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;


namespace ThePackt{
    public class PlayerDetransformationState : PlayerAbilityState
    {
        private float _detransformationTime = 1.5f;
        public PlayerDetransformationState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName,string wolfAnimBool) : base(player, stateMachine, playerData, animBoolName,wolfAnimBool)
        {
        }

        public override void Checks()
        {
            base.Checks();
        }

        public override void Enter()
        {
            base.Enter();
            
            Debug.LogWarning("DETRANSFORMATION : ABILITY START");
            
            
        }

        public override void Exit()
        {
            base.Exit();
            
            Debug.LogWarning("DETRANSFORMATION : ABILITY DONE");

        }
         public override void AnimationFinishTrigger()
        {
            if(_player.entity.IsOwner){


                DetransformationEvent evnt;
                evnt = DetransformationEvent.Create(GlobalTargets.Everyone);
                evnt.TargetPlayerNetworkID = _player.entity.NetworkId;
                evnt.Send();

                SetColliderSizeEvent sizeEvent;
                sizeEvent = SetColliderSizeEvent.Create(GlobalTargets.Everyone);
                sizeEvent.TargetPlayerNetworkID = _player.entity.NetworkId;
                sizeEvent.Offset =  new Vector2(-0.7352595f, -5.962845f);
                sizeEvent.Size =  new Vector2(8.667796f, 35.94624f);
                sizeEvent.Send();

            }

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }


        public override void LogicUpdate()
        {
            base.LogicUpdate();
           

            if(Time.time > _startTime + _detransformationTime){
               
                _isAbilityDone = true;
            }

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

    }
}
