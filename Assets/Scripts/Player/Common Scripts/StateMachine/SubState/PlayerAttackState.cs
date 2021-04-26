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
            GameObject blt = BoltNetwork.Instantiate(_player.GetBullet(), _player.GetAttackPoint().position, _player.GetAttackPoint().rotation);
            blt.GetComponent<Bullet>().SetAttackPower(_player.GetPlayerData().powerBaseHuman);
        }

        public void BaseWereWolfAttack()
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_player.GetAttackPoint().position, _player.GetPlayerData().rangeBaseWerewolf, LayerMask.GetMask("Enemies", "Players"));

            foreach (Collider2D collision in hitEnemies)
            {
                Debug.Log(collision.gameObject.name + " hit");

                if (LayerMask.LayerToName(collision.gameObject.layer) == "Enemies")
                {
                    EnemyHitReaction(collision);
                }
                else if (LayerMask.LayerToName(collision.gameObject.layer) == "Players")
                {
                    PlayerHitReaction(collision);
                }
            }
        }

        // react to the hit of an enemy applying damage to that enemy
        private void EnemyHitReaction(Collider2D collision)
        {
            Enemy enemy;
            enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                EnemyAttackHitEvent evnt;

                // if we are on the server, directly apply the damage to the enemy
                // otherwise we sent an event to the server
                if (BoltNetwork.IsServer)
                {
                    Debug.Log("[NETWORKLOG] server hit enemy");
                    enemy.ApplyDamage(_player.GetPlayerData().powerBaseWerewolf);
                }
                else
                {
                    Debug.Log("[NETWORKLOG] from client to server");
                    evnt = EnemyAttackHitEvent.Create(BoltNetwork.Server);
                    evnt.HitNetworkId = enemy.entity.NetworkId;
                    evnt.Damage = _player.GetPlayerData().powerBaseWerewolf;
                    evnt.Send();
                }
            }
        }

        // react to the hit of a player applying damage to that player. returns if the player is the owner
        private void PlayerHitReaction(Collider2D collision)
        {
            Player hitPlayer = collision.GetComponent<Player>();
            Debug.Log("player hit is owner: " + hitPlayer.entity.IsOwner);
            Debug.Log("player attacker is owner: " + _player.entity.IsOwner);

            if (hitPlayer != null)
            {
                if (hitPlayer.entity.IsOwner != _player.entity.IsOwner)
                {
                    Debug.Log("[HEALTH] hit other player: " + collision.gameObject.name);

                    PlayerAttackHitEvent evnt;

                    // if we are on the server, send the hit event to the connection of the player that was hit
                    // otherwise we sent it to the server with the connection id of the player that was hit
                    if (BoltNetwork.IsServer)
                    {
                        Debug.Log("[NETWORKLOG] from server to connection: " + hitPlayer.getConnectionID());
                        evnt = PlayerAttackHitEvent.Create(hitPlayer.entity.Source);
                    }
                    else
                    {
                        Debug.Log("[NETWORKLOG] from client to server. must redirect to: " + hitPlayer.getConnectionID());
                        evnt = PlayerAttackHitEvent.Create(BoltNetwork.Server);
                    }

                    evnt.Damage = _player.GetPlayerData().powerBaseWerewolf;
                    evnt.Send();
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
