using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class PlayerBullet : Bullet {

        protected Player _owner { get; private set; }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (entity.IsOwner)
            {
                bool isLocalPlayer = false;
                bool isLocalBullet = true;

                if (LayerMask.LayerToName(collision.gameObject.layer) == "Enemies")
                {
                    EnemyHitReaction(collision);
                }
                else if (LayerMask.LayerToName(collision.gameObject.layer) == "Objectives")
                {
                    ObjectiveHitReaction(collision);
                }
                else if (LayerMask.LayerToName(collision.gameObject.layer) == "Players")
                {
                    Debug.LogError("[BULLET] colpito un giocatore, on trigegr enter");
                    isLocalPlayer = PlayerHitReaction(collision);
                    isLocalBullet = entity.IsOwner;
                }

                // Does not destroy bullets on impact with other bullets or the local player
                if (!(LayerMask.LayerToName(collision.gameObject.layer) == "Bullets") && isLocalPlayer != isLocalBullet && !(LayerMask.LayerToName(collision.gameObject.layer) == "Room"))
                {
                    Debug.LogError("[BULLET] destroyed");
                    BoltNetwork.Destroy(gameObject);
                }
            }
        }

        // react to the hit of a player applying damage to that player. returns if the player is the owner
        protected virtual bool PlayerHitReaction(Collider2D collision)
        {
            Player player;
            bool isLocalPlayer = false;
            bool isLocalBullet = true;

            player = collision.GetComponent<Player>();
            if (player != null)
            {
                isLocalPlayer = player.entity.IsOwner;
                isLocalBullet = entity.IsOwner;
                Debug.Log("player is owner: " + isLocalPlayer);
                Debug.Log("bullet is owner: " + isLocalBullet);

                //If the player and the bullet have different owners a damage must be applied.
                //When two player fight and neither of them is the owner, both the bullet and hit player will be false,
                //so the bullet will not be destroyed or the damage applied -> which is wrong.
                //Anyway there is a machine in which the owner is the hit player and that machine
                //will identify that bullet and player have different owners and the damage will be applied,
                //health will be synchronized and the bullet destroyed for every player (thanks to BoltNetwork.Destroy)
                if (isLocalPlayer != isLocalBullet && isLocalBullet)
                {
                    Debug.Log("[HEALTH] hit other player: " + collision.gameObject.name);

                    PlayerAttackHitEvent evnt;

                    // if we are on the server, send the hit event to the connection of the player that was hit
                    // otherwise we sent it to the server with the connection id of the player that was hit
                    if (BoltNetwork.IsServer)
                    {
                        Debug.Log("[NETWORKLOG] server hit " + player.entity.NetworkId);
                        Debug.Log("[NETWORKLOG] from server to connection: " + player.entity.Source.ConnectionId);
                        evnt = PlayerAttackHitEvent.Create(player.entity.Source);
                    }
                    else
                    {
                        Debug.Log("[NETWORKLOG] from client to server. must redirect to the connection of: " + player.entity.NetworkId);
                        evnt = PlayerAttackHitEvent.Create(BoltNetwork.Server);
                    }

                    evnt.HitNetworkId = player.entity.NetworkId;
                    // if the player has a dmg reduction debuff thank substract the dmg reduction value before applaying damage
                   if(_owner.GetPlayerData().isDmgReductionDebuffActive)
                        evnt.Damage = (_attackPower +( _attackPower * _owner.GetPlayerData().damageMultiplier)) - _owner.GetPlayerData().dmgReduction;
                   else evnt.Damage = _attackPower +(_attackPower * _owner.GetPlayerData().damageMultiplier);
                   evnt.Send();
                }
            }

            return isLocalPlayer;
        }

        // react to the hit of an enemy applying damage to that enemy
        protected virtual void EnemyHitReaction(Collider2D collision)
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
                    // if the player has a dmg reduction debuff thank substract the dmg reduction value before applaying damage
                    if (_owner.GetPlayerData().isDmgReductionDebuffActive)
                        enemy.ApplyDamage((_attackPower + (_attackPower * _owner.GetPlayerData().damageMultiplier)) - _owner.GetPlayerData().dmgReduction, _owner);
                    else enemy.ApplyDamage((_attackPower + (_attackPower * _owner.GetPlayerData().damageMultiplier)), _owner);
                }
                else
                {
                    Debug.Log("[NETWORKLOG] from client to server");
                    evnt = EnemyAttackHitEvent.Create(BoltNetwork.Server);
                    evnt.HitNetworkId = enemy.entity.NetworkId;
                    evnt.AttackerNetworkId = _owner.entity.NetworkId;
                    // if the player has a dmg reduction debuff thank substract the dmg reduction value before applaying damag

                    if (_owner.GetPlayerData().isDmgReductionDebuffActive)
                        evnt.Damage = (_attackPower + (_attackPower * _owner.GetPlayerData().damageMultiplier)) - _owner.GetPlayerData().dmgReduction;
                    else evnt.Damage = _attackPower + (_attackPower * _owner.GetPlayerData().damageMultiplier);

                    evnt.Send();
                }
            }
        }

        // react to the hit of an objective applying damage to that enemy
        protected virtual void ObjectiveHitReaction(Collider2D collision)
        {
            Objective obj = collision.GetComponent<Objective>();
            if (obj != null)
            {
                ObjectiveHitEvent evnt;

                // if we are on the server, directly apply the damage to the objective
                // otherwise we sent an event to the server
                if (BoltNetwork.IsServer)
                {
                    Debug.Log("[NETWORKLOG] server hit objective");
                    if (_owner.GetPlayerData().isDmgReductionDebuffActive)
                        obj.ApplyDamage((_attackPower + (_attackPower * _owner.GetPlayerData().damageMultiplier)) - _owner.GetPlayerData().dmgReduction);
                    else obj.ApplyDamage((_attackPower + (_attackPower * _owner.GetPlayerData().damageMultiplier)));
                }
                else
                {
                    Debug.Log("[NETWORKLOG] from client to server");
                    evnt = ObjectiveHitEvent.Create(BoltNetwork.Server);
                    evnt.HitNetworkId = obj.entity.NetworkId;

                    if (_owner.GetPlayerData().isDmgReductionDebuffActive)
                        evnt.Damage = (_attackPower + (_attackPower * _owner.GetPlayerData().damageMultiplier)) - _owner.GetPlayerData().dmgReduction;
                    else evnt.Damage = _attackPower + (_attackPower * _owner.GetPlayerData().damageMultiplier);

                    evnt.Send();
                }
            }
        }

        public void SetOwner(Player plyr)
        {
            _owner = plyr;
        }
    }
}
