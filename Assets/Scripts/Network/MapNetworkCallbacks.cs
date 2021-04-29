using System.Collections;
using UdpKit;
using UnityEngine;
using System;
using Bolt;

namespace ThePackt
{
    public class MapNetworkCallbacks : GlobalEventListener
    {
        public Utils.PrefabAssociation[] enemyPrefabs;
        public Vector2 enemySpawnPos;
        public Utils.VectorAssociation[] playersSpawnPositions;
        [SerializeField] private CharacterSelectionData _selectedData;
        private Player _player;

        #region callbacks
        
        /*
        public override void SceneLoadLocalBegin(string scene, IProtocolToken token)
        {
            Debug.Log("begin aaaaa");

            //put here black screen while loading
            if (BoltNetwork.IsServer)
            {
                foreach (BoltEntity ent in BoltNetwork.SceneObjects)
                {
                    ent.transform.position = new Vector3(-7f, 0f, 0f);
                }
            }
        }
        */

        public override void SceneLoadLocalDone(string scene, IProtocolToken token)
        {
            foreach (BoltEntity ent in BoltNetwork.Entities)
            {
                if (ent.IsOwner)
                {
                    _player = _selectedData.GetPlayerScript();

                    foreach (Utils.VectorAssociation assoc in playersSpawnPositions)
                    {
                        if (assoc.name == _selectedData.GetCharacterSelected())
                        {
                            ent.gameObject.transform.position = assoc.position;
                        }
                    }
                }
            }

            //only the server spawns enemies for everyone
            if (BoltNetwork.IsServer)
            {
                BoltNetwork.Instantiate(enemyPrefabs[0].prefab, enemySpawnPos, Quaternion.identity);
            }

            //disable black screen here
        }

        public override void EntityAttached(BoltEntity entity)
        {
        }

        public override void EntityDetached(BoltEntity entity)
        {
        }

        /*
        public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
        {
            Debug.Log("DISCONNECTREASON: " + disconnectReason);

            _selectedData.Reset();
        }

        public override void Disconnected(BoltConnection connection)
        {
            Debug.Log("DISCONNECT: " + connection.ConnectionId);

            _selectedData.Reset();
        }
        */

        public override void OnEvent(PlayerAttackHitEvent evnt)
        {
            Debug.Log("[HEALTH] attack hit with damage: " + evnt.Damage);

            if (BoltNetwork.IsServer)
            {
                Debug.Log("[NETWORKLOG] server received hit event");

                //if received by the server
                //if the server itself was hit apply damage to its player
                if (evnt.RaisedBy.ConnectionId == Utils.GetServerID())
                {
                    Debug.Log("[NETWORKLOG] server was hit, applying damage");
                    _player.ApplyDamage(evnt.Damage);
                }
                else
                {
                    Debug.Log("[NETWORKLOG] server redirect to: " + evnt.RaisedBy.ConnectionId);

                    //otherwise redirect the event to the client that was hit

                    var newEvnt = PlayerAttackHitEvent.Create(evnt.RaisedBy);
                    newEvnt.Damage = evnt.Damage;
                    evnt.Send();
                }
            }
            else
            {
                //if received by the client, apply damage to the player of which the client is owner

                Debug.Log("[NETWORKLOG] client was hit, applying damage");
                _player.ApplyDamage(evnt.Damage);
            }
        }

        public override void OnEvent(EnemyAttackHitEvent evnt)
        {
            Debug.Log("[HEALTH] enemy attack hit with damage: " + evnt.Damage);

            //if received by the server, apply damage to the enemy with the network id stored in the event
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
            //if received by the server disconnect the sender
            if (BoltNetwork.IsServer)
            {
                Debug.Log("[NETWORKLOG] server received disconnect event");

                evnt.RaisedBy.Disconnect();
            }
        }

        #endregion

        #region methods

        #endregion

        #region getters
     
        #endregion
    }
}