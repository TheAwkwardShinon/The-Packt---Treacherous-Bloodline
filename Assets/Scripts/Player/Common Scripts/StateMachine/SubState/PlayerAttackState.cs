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
            //blt.GetComponent<Bullet>().SetPlayerNetworkId(_player.entity.NetworkId);
        }

        public void BaseWereWolfAttack()
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_player.GetAttackPoint().position, _player.GetPlayerData().rangeBaseWerewolf, LayerMask.GetMask("Enemies", "Players"));

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
                        EnemyAttackHitEvent evnt;
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
                else if (LayerMask.LayerToName(collision.gameObject.layer) == "Players")
                {
                    hitPlayer = collision.GetComponent<Player>();
                    Debug.Log("player hit is owner: " + hitPlayer.entity.IsOwner);
                    Debug.Log("player attacker is owner: " + _player.entity.IsOwner);
                    if (hitPlayer != null)
                    {
                        if (hitPlayer.entity.IsOwner != _player.entity.IsOwner)
                        {
                            Debug.Log("[HEALTH] hit other player: " + collision.gameObject.name);

                            /*
                            Debug.Log("[HEALTH] other network id: " + hitPlayer.entity.NetworkId.GetHashCode());
                            Debug.Log("[HEALTH] my network id: " + _player.entity.NetworkId.GetHashCode());
                            */

                            PlayerAttackHitEvent evnt;
                            if (BoltNetwork.IsServer)
                            {
                                Debug.Log("[NETWORKLOG] from server to connection: " + hitPlayer.getConnectionID());
                                evnt = PlayerAttackHitEvent.Create(hitPlayer.entity.Source);
                            }
                            else
                            {
                                Debug.Log("[NETWORKLOG] from client to server. must redirect to: " + hitPlayer.getConnectionID());
                                evnt = PlayerAttackHitEvent.Create(BoltNetwork.Server);
                                evnt.HitConnectionID = (int) hitPlayer.getConnectionID();
                            }

                            //evnt.HitNetworkID = player.entity.NetworkId;
                            evnt.Damage = _player.GetPlayerData().powerBaseWerewolf;
                            evnt.Send();
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
