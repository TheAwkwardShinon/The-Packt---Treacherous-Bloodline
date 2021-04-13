using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class PlayerAttackState : PlayerAbilityState
    {
        #region variables
        public bool CanAttack { get; private set; }

        #endregion

        #region methods

        public PlayerAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            if (_player._inputHandler._attackInputs[Constants.BASE])
            {
                _player._inputHandler.UseBaseAttackInput();

                if (_player.GetIsHuman())
                {
                    Debug.Log("[ATTACK STATE] entered (base human)");
                    BaseHumanAttack();
                    _isAbilityDone = true;
                    Debug.Log("[ATTACK STATE] ability done (base human)");
                }
                else
                {
                    Debug.Log("[ATTACK STATE] entered (base werewolf)");
                    BaseWereWolfAttack();
                    _isAbilityDone = true;
                    Debug.Log("[ATTACK STATE] ability done (base werewolf)");
                }
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public void BaseHumanAttack() 
        {
            GameObject blt = GameObject.Instantiate(_player.GetBullet(), _player.GetAttackPoint().position, _player.GetAttackPoint().rotation);
            blt.GetComponent<Bullet>().SetAttackPower(_player.GetPlayerData().powerBaseHuman);
        }

        public void BaseWereWolfAttack()
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_player.GetAttackPoint().position, _player.GetPlayerData().rangeBaseWerewolf, LayerMask.GetMask("Enemies"));

            foreach (Collider2D enemy in hitEnemies)
            {
                Debug.Log(enemy.gameObject.name + " hit");
                enemy.gameObject.GetComponent<Enemy>().ApplyDamage(_player.GetPlayerData().powerBaseWerewolf);
                Debug.Log(enemy.gameObject.name + " health: " + enemy.gameObject.GetComponent<Enemy>().GetHealth());
            }
        }

        public bool CheckIfCanAttack()
        {
            //da impostare in base ai vari cooldown
            return true;
        }

        public void ResetCanAttack() => CanAttack = true;

        #endregion
    }
}
