using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class PlayerSpecialAttackState : PlayerAbilityState
    {

        private float _lastAttackTime;
        private bool firstAttack = true;


        public PlayerSpecialAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
        }

       
        public override void Enter()
        {
            base.Enter();

            _player._inputHandler.UseSpecialAttackInput();

            if (firstAttack)
            {
                firstAttack = false;
            }
             if (!_player.GetIsHuman()){
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
                case "ayatana": blt = BoltNetwork.Instantiate(_player.GetAyatanaBullet(), _player.GetAttackPoint().position, _player.GetAttackPoint().rotation);
                            break;
                case "ceuin": blt = BoltNetwork.Instantiate(_player.GetCeuinBullet(), _player.GetAttackPoint().position, _player.GetAttackPoint().rotation);
                            break;
                case "fele": blt = BoltNetwork.Instantiate(_player.GetFeleBullet(), _player.GetAttackPoint().position, _player.GetAttackPoint().rotation);
                            break;
                case "herin": blt = BoltNetwork.Instantiate(_player.GetHerinBullet(), _player.GetAttackPoint().position, _player.GetAttackPoint().rotation);
                            break;
                case "moonsighters": blt = BoltNetwork.Instantiate(_player.GetMoonsighterBullet(), _player.GetAttackPoint().position, _player.GetAttackPoint().rotation);
                            break;
                case "naturia": break;
            }
        }

         public bool CheckIfCanAttack()
         {
            return firstAttack || Time.time >= _lastAttackTime + _player.GetPlayerData().specialAttackCooldown;
         }        
    }
}