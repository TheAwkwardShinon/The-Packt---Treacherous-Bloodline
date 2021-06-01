using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Debug.LogWarning("TRANSFORMATION : ABILITY START");
          
            _player._inputHandler.UseTransformInput();

        }

        public override void Exit()
        {
            base.Exit();
            
            Debug.LogWarning("TRANSFORMATION : ABILITY DONE");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
           

            if(Time.time > _startTime + _transformationTime){
                _player.GetHumanObject().SetActive(false);
                _player.GetWolfObject().SetActive(true);
                  Debug.LogWarning("TRANSFORMATION set active done");
                _isAbilityDone = true;  
            }


        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

    }
}
