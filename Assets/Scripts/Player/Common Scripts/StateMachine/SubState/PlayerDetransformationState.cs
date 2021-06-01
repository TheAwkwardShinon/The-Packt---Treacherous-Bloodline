using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }


        public override void LogicUpdate()
        {
            base.LogicUpdate();
           

            if(Time.time > _startTime + _detransformationTime){
                _player.GetWolfObject().SetActive(false);
                _player.GetHumanObject().SetActive(true);
                _isAbilityDone = true;
            }

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

    }
}
