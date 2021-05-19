using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class FeleSpecialAttackState : PlayerAbilityState
    {

        private float _lastWerewolfAttackTime;
        private bool firstAttack = true;


        public FeleSpecialAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
        }

       
        public override void Enter()
        {
            base.Enter();
             if (_player.GetIsHuman()){
                SpecialAttack();
                _lastWerewolfAttackTime = Time.time;
             }
                
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

        public void SpecialAttack(){

        }


        
    }
}
