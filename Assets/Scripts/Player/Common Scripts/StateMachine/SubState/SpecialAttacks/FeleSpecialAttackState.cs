using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class FeleSpecialAttackState : PlayerAbilityState
    {

        private float _lastAttackTime;
        private bool firstAttack = true;


        public FeleSpecialAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
        }

       
        public override void Enter()
        {
            base.Enter();
             if (_player.GetIsHuman()){
                SpecialAttack(_player.gameObject.tag);
                _lastAttackTime = Time.time;
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

        public void SpecialAttack(string clan){
            GameObject blt;
            switch(clan){
                case "ayatana": break;
                case "ceuin": break;
                case "fele": blt = BoltNetwork.Instantiate(_player.GetFeleBullet(), _player.GetAttackPoint().position, _player.GetAttackPoint().rotation);
                            break;
                case "herin": break;
                case "moonsighter": break;
                case "naturia": break;
            }
        }

         public bool CheckIfCanAttack()
         {
            return firstAttack || Time.time >= _lastAttackTime + _player.GetPlayerData().specialAttackCooldown;
         }        
    }
}
