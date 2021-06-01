using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class PlayerSpecialAttackState : PlayerAbilityState
    {

        private float _lastAttackTime;
        private bool firstAttack = true;
        private string[] nextAttack = {"naturia","naturia"};


        public PlayerSpecialAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName,string wolfAnimBool) : base(player, stateMachine, playerData, animBoolName,wolfAnimBool)
        {
        }

       
        public override void Enter()
        {
            base.Enter();
            nextAttack[0] = _player.GetNextBullet();
            _player._inputHandler.UseSpecialAttackInput();

            if (firstAttack)
            {
                firstAttack = false;
            }
            
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

        }

        public override void AnimationFinishTrigger()
        {
             if (!_player.GetIsHuman()){
                SpecialAttack(_player.gameObject.tag);
                _lastAttackTime = Time.time;
             }
                
            _isAbilityDone = true;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public void SpecialAttack(string clan){
            GameObject blt;
            if(clan.Equals("naturia") && nextAttack[0].Equals("naturia")){
                clan = nextAttack[0];
                nextAttack[0] = nextAttack[1];
                nextAttack[1] = "naturia";
            }else if(clan.Equals("naturia") && !nextAttack[0].Equals("naturia")){
                clan = nextAttack[0];
                _player.NextBullet(nextAttack[1]);
            }
            switch(clan){
                case "ayatana": blt = BoltNetwork.Instantiate(_player.GetAyatanaBullet(), _player.GetSpecialAttackPoint().position, _player.GetSpecialAttackPoint().rotation);
                                blt.GetComponent<Bullet>().SetOwner(_player);
                            break;
                case "ceuin": blt = BoltNetwork.Instantiate(_player.GetCeuinBullet(), _player.GetSpecialAttackPoint().position, _player.GetSpecialAttackPoint().rotation);
                              blt.GetComponent<Bullet>().SetOwner(_player);
                            break;
                case "fele": blt = BoltNetwork.Instantiate(_player.GetFeleBullet(), _player.GetSpecialAttackPoint().position, _player.GetSpecialAttackPoint().rotation);
                             blt.GetComponent<Bullet>().SetOwner(_player);
                            break;
                case "herin": blt = BoltNetwork.Instantiate(_player.GetHerinBullet(), _player.GetSpecialAttackPoint().position, _player.GetSpecialAttackPoint().rotation);
                              blt.GetComponent<Bullet>().SetOwner(_player);
                            break;
                case "moonsighters": blt = BoltNetwork.Instantiate(_player.GetMoonsighterBullet(), _player.GetSpecialAttackPoint().position, _player.GetSpecialAttackPoint().rotation);
                                     blt.GetComponent<Bullet>().SetOwner(_player);
                            break;
                case "naturia": blt = BoltNetwork.Instantiate(_player.GetNaturiaBullet(), _player.GetSpecialAttackPoint().position, _player.GetSpecialAttackPoint().rotation);
                                blt.GetComponent<Bullet>().SetOwner(_player);
                             break;
            }
            
        }

         public bool CheckIfCanAttack()
         {
            return firstAttack || Time.time >= _lastAttackTime + _player.GetPlayerData().specialAttackCooldown;
         }        
    }
}