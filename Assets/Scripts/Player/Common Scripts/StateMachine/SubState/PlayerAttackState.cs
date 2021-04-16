using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

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
                    Debug.Log("[ATTACK STATE] entered (base human) ciaone");
                    //_player.state.BaseHumanAttack();
                    BaseHumanAttack();
                    _isAbilityDone = true;
                    Debug.Log("[ATTACK STATE] ability done (base human) ciaone");
                }
                else
                {
                    Debug.Log("[ATTACK STATE] entered (base werewolf)");
                    //_player.state.BaseWereWolfAttack();
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
            GameObject blt = BoltNetwork.Instantiate(_player.GetBullet(), _player.GetAttackPoint().position, _player.GetAttackPoint().rotation);
            blt.GetComponent<Bullet>().SetAttackPower(_player.GetPlayerData().powerBaseHuman);
        }

        public void BaseWereWolfAttack()
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_player.GetAttackPoint().position, _player.GetPlayerData().rangeBaseWerewolf, LayerMask.GetMask("Enemies", "Players"));
            Debug.Log("werewolf attack is owner ");
            foreach (Collider2D collision in hitEnemies)
            {
                Debug.Log(collision.gameObject.name + " hit");

                Enemy enemy;
                Player hitPlayer;
                if (LayerMask.LayerToName(collision.gameObject.layer) == "Enemies")
                {
                    enemy = collision.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.ApplyDamage(_player.GetPlayerData().powerBaseWerewolf);
                        Debug.Log(collision.gameObject.name + " health: " + enemy.GetHealth());
                    }
                }
                else if (LayerMask.LayerToName(collision.gameObject.layer) == "Players")
                {
                    hitPlayer = collision.GetComponent<Player>();
                    Debug.Log("player hit is owner: " + hitPlayer.entity.IsOwner);
                    Debug.Log("player attacker is owner: " + _player.entity.IsOwner);
                    if (hitPlayer != null)
                    {
                        if (hitPlayer.entity.IsOwner != _player.entity.IsOwner)
                        {
                            Debug.Log("hit other player health is owner: " + collision.gameObject.name);
                            hitPlayer.ApplyDamage(_player.GetPlayerData().powerBaseWerewolf);
                        }
                    }
                }
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
