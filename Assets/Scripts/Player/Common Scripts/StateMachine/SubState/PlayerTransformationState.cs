using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

namespace ThePackt{
    public class PlayerTransformationState : PlayerAbilityState
    {
        private float _transformationTime = 1.5f;
        public PlayerTransformationState(Player player, PlayerStateMachine stateMachine, PlayerData data, string animationName,string wolfAnimBool) : base(player, stateMachine, data, animationName,wolfAnimBool)
        {
        }

        public override void Checks()
        {
            base.Checks();
        }

        public override void Enter()
        {
            base.Enter();
          
            _player._inputHandler.UseTransformInput();

        }

        public override void Exit()
        {
            base.Exit();
            
        }

        public override void AnimationFinishTrigger()
        {
            if(_player.entity.IsOwner){

                if (_player.entity.IsOwner) {
                TransformationEvent evnt;
                evnt = TransformationEvent.Create(GlobalTargets.Everyone);
                evnt.TargetPlayerNetworkID = _player.entity.NetworkId;
                evnt.Send();


                SetColliderSizeEvent sizeEvent;
                sizeEvent = SetColliderSizeEvent.Create(GlobalTargets.Everyone);
                sizeEvent.TargetPlayerNetworkID = _player.entity.NetworkId;
                sizeEvent.Offset =  new Vector2(-1.780157f, -5.962845f);
                sizeEvent.Size = new Vector2(24.9682f, 35.94624f);
                sizeEvent.Send();
            }
            }

            
        }

      

        public override void LogicUpdate()
        {
            base.LogicUpdate();
           

            if(Time.time > _startTime + _transformationTime){
                
                _isAbilityDone = true;  
            }


        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

    }
}
