using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Bolt;

namespace ThePackt
{
    public class NetworkCallbacks : GlobalEventListener
    {
        public GameObject playerPrefab;
        public Vector2 playerSpawnPos;
        public GameObject enemyPrefab;
        public Vector2 enemySpawnPos;
        private Player _player;


        public override void SceneLoadLocalDone(string scene, Bolt.IProtocolToken token)
        {
            // when the scene is loaded instantiate the player in the given position
            BoltNetwork.Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);

            if (BoltNetwork.IsServer)
            {
                BoltNetwork.Instantiate(enemyPrefab, enemySpawnPos, Quaternion.identity);
            }
        }

        public override void EntityAttached(BoltEntity entity)
        {
            Player plyr = entity.GetComponent<Player>();
            if (plyr != null && entity.IsOwner)
            {
                _player = entity.GetComponent<Player>();
            }
        }

        public override void OnEvent(PlayerAttackHitEvent evnt)
        {
            Debug.Log("[HEALTH] attack hit with damage: " + evnt.Damage);

            if (BoltNetwork.IsServer)
            {
                Debug.Log("[NETWORKLOG] server received hit event");
                if (evnt.RaisedBy.ConnectionId == Utils.GetServerID())
                {
                    Debug.Log("[NETWORKLOG] server was hit, applying damage");
                    _player.ApplyDamage(evnt.Damage);
                }
                else
                {
                    Debug.Log("[NETWORKLOG] server redirect to: " + evnt.RaisedBy.ConnectionId);

                    var newEvnt = PlayerAttackHitEvent.Create(evnt.RaisedBy);
                    newEvnt.Damage = evnt.Damage;
                    evnt.Send();
                }
            }
            else
            {
                Debug.Log("[NETWORKLOG] client was hit, applying damage");
                _player.ApplyDamage(evnt.Damage);
            }
        }

        public override void OnEvent(EnemyAttackHitEvent evnt)
        {
            Debug.Log("[HEALTH] enemy attack hit with damage: " + evnt.Damage);

            if (BoltNetwork.IsServer)
            {
                Debug.Log("[NETWORKLOG] server received enemy hit event");

                BoltEntity entity = BoltNetwork.FindEntity(evnt.HitNetworkId);
                Enemy enemy = entity.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.ApplyDamage(evnt.Damage);
                }
            }
        }

        public override void OnEvent(DisconnectEvent evnt)
        {
            if (BoltNetwork.IsServer)
            {
                Debug.Log("[NETWORKLOG] server received disconnect event");

                evnt.RaisedBy.Disconnect();
            }
        }
    }
}