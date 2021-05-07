using System.Collections;
using UdpKit;
using UnityEngine;
using System;
using Bolt;

namespace ThePackt
{
    public class MapNetworkCallbacks : NetworkCallbacks
    {
        public Utils.PrefabAssociation[] enemyPrefabs;
        public Vector2 enemySpawnPos;
        public Utils.VectorAssociation[] playersSpawnPositions;
        [SerializeField] private CharacterSelectionData _selectedData;
        [SerializeField] private GameObject _timeManagerPrefab;
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
                if (ent.IsOwner && ent.gameObject.GetComponent<Player>() != null)
                {
                    _player = _selectedData.GetPlayerScript();

                    foreach (Utils.VectorAssociation assoc in playersSpawnPositions)
                    {
                        if (assoc.name == _selectedData.GetCharacterSelected())
                        {
                            ent.gameObject.GetComponent<Rigidbody2D>().MovePosition(assoc.position);
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

            if (BoltNetwork.IsServer)
            {
                BoltEntity timeManager = BoltNetwork.Instantiate(_timeManagerPrefab, Vector3.zero, Quaternion.identity);
                timeManager.GetComponent<TimerManager>().SetStartTime(BoltNetwork.ServerTime);
            }
        }

        public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
        {
            Debug.Log("[CONNECTIONLOG] connect request");

            BoltNetwork.Refuse(endpoint);
        }

        public override void OnEvent(PlayerAttackHitEvent evnt)
        {
            Debug.Log("[HEALTH] attack hit with damage: " + evnt.Damage);

            if (BoltNetwork.IsServer)
            {
                Debug.Log("[NETWORKLOG] server received hit event");

                //if received by the server
                //if the server itself was hit apply damage to its player
                Debug.Log("[NETWORKLOG] " + evnt.HitNetworkId + " was hit");
                BoltEntity hitEntity = BoltNetwork.FindEntity(evnt.HitNetworkId);
                if(hitEntity != null)
                {
                    if (hitEntity.IsOwner)
                    {
                        Debug.Log("[NETWORKLOG] server was hit, applying damage");
                        _player.ApplyDamage(evnt.Damage);
                    }
                    else
                    {
                        //otherwise redirect the event to the client that was hit

                        Debug.Log("[NETWORKLOG] server redirect to connection: " + hitEntity.Source.ConnectionId);

                        var newEvnt = PlayerAttackHitEvent.Create(hitEntity.Source);
                        newEvnt.HitNetworkId = evnt.HitNetworkId;
                        newEvnt.Damage = evnt.Damage;
                        newEvnt.Send();
                    }
                }
            }
            else
            {
                //if received by the client, apply damage to the player of which the client is owner
                Debug.Log("[NETWORKLOG] client was hit. my: " + _player.entity.NetworkId + "  other: " + evnt.HitNetworkId);

                if (_player.entity.NetworkId.Equals(evnt.HitNetworkId))
                {
                    Debug.Log("[NETWORKLOG] client was hit, applying damage");
                    _player.ApplyDamage(evnt.Damage);
                }
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

        #endregion

        #region methods

        #endregion

        #region getters
     
        #endregion
    }
}