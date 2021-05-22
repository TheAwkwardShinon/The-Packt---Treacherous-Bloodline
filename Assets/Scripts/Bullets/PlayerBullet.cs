using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class PlayerBullet : Bullet { 
        
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
                    isLocalPlayer = PlayerHitReaction(collision);
                    isLocalBullet = entity.IsOwner;
                }

                // Does not destroy bullets on impact with other bullets or the local player
                if (!(LayerMask.LayerToName(collision.gameObject.layer) == "Bullets") && isLocalPlayer != isLocalBullet && !(LayerMask.LayerToName(collision.gameObject.layer) == "Room"))
                {
                    BoltNetwork.Destroy(gameObject);
                }
            }
        }

        // react to the hit of a player applying damage to that player. returns if the player is the owner
        private bool PlayerHitReaction(Collider2D collision)
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
                    evnt.Damage = _attackPower;
                    evnt.Send();
                }
            }

            return isLocalPlayer;
        }
    }
}
