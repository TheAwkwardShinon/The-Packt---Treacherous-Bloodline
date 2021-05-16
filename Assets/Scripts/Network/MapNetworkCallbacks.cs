using System.Collections.Generic;
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
            List<BoltEntity> players = new List<BoltEntity>();
            foreach (BoltEntity ent in BoltNetwork.Entities)
            {
                if(ent.gameObject.GetComponent<Player>() != null)
                {
                    players.Add(ent);

                    if (ent.IsOwner)
                    {
                        _player = _selectedData.GetPlayerScript();
                        _player.ActivateFogCircle();

                        foreach (Utils.VectorAssociation assoc in playersSpawnPositions)
                        {
                            if (assoc.name == _selectedData.GetCharacterSelected())
                            {
                                ent.gameObject.transform.position = assoc.position;
                                Camera.main.GetComponent<CameraFollow>().SetFollowTransform(ent.gameObject.transform);
                            }
                        }
                    }
                }
            }

            MainQuest.Instance.SetPlayers(players);

            //only the server spawns enemies for everyone
            if (BoltNetwork.IsServer)
            {
                //BoltNetwork.Instantiate(enemyPrefabs[0].prefab, enemySpawnPos, Quaternion.identity);
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

        public override void OnEvent(ObjectiveHitEvent evnt)
        {
            Debug.Log("[OBJECTIVE] objective hit with damage: " + evnt.Damage);

            //if received by the server, apply damage to the objective with the network id stored in the event
            if (BoltNetwork.IsServer)
            {
                Debug.Log("[NETWORKLOG] server received objective hit event");

                BoltEntity entity = BoltNetwork.FindEntity(evnt.HitNetworkId);
                Objective objective = entity.GetComponent<Objective>();
                if (objective != null)
                {
                    objective.ApplyDamage(evnt.Damage);
                }
            }
        }

        public override void OnEvent(ImpostorEvent evnt)
        {
            Debug.Log("[MAIN] received impostor event");

            if (_player.entity.NetworkId.Equals(evnt.ImpostorNetworkID))
            {
                Debug.Log("[MAIN] i'm the fucking impostor");

                _player.SetIsImpostor(true);
            }
        }

        public override void OnEvent(StartHealingEvent evnt)
        {
            if (BoltNetwork.IsServer)
            {
                if (_player.entity.NetworkId.Equals(evnt.TargetPlayerNetworkID))
                {
                    _player.SetIsBeingHealed(true);
                }
                else {
                    BoltEntity entity = BoltNetwork.FindEntity(evnt.TargetPlayerNetworkID);
                    var newEvnt = StartHealingEvent.Create(entity.Source);
                    newEvnt.TargetPlayerNetworkID = evnt.TargetPlayerNetworkID;
                    newEvnt.Send();
                }
            }
            else{
                if (_player.entity.NetworkId.Equals(evnt.TargetPlayerNetworkID))
                {
                    _player.SetIsBeingHealed(true);
                }
            }
        }

         public override void OnEvent(HealEvent evnt)
        {
             if (BoltNetwork.IsServer)
            {
                BoltEntity entity = BoltNetwork.FindEntity(evnt.TargetPlayerNetworkID);

                 if(entity != null)
                {
                    if (entity.IsOwner)
                    {
                        Debug.Log("[NETWORKLOG] server was hit, applying damage");
                        _player.Heal();
                    }
                    else
                    {
                        var newEvnt = HealEvent.Create(entity.Source);
                        newEvnt.TargetPlayerNetworkID = evnt.TargetPlayerNetworkID;
                        newEvnt.Send();
                    }
                }
            }else{
                if (_player.entity.NetworkId.Equals(evnt.TargetPlayerNetworkID))
                {
                    _player.Heal();
                     Debug.Log("[INTERACTION] HEAL CALLBACK CORRECTLY CALLED");
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


/* public override void OnEvent(HealEvent evnt)
        {
             if (BoltNetwork.IsServer)
            {
                if (_player.entity.NetworkId.Equals(evnt.TargetPlayerNetworkID))
                {
                    Debug.Log("[INTERACTION] HEAL CALLBACK CORRECTLY CALLED");
                    _player.Heal();
                }
                else {
                    BoltEntity entity = BoltNetwork.FindEntity(evnt.TargetPlayerNetworkID);
                    var newEvnt = HealEvent.Create(entity.Source);
                    newEvnt.TargetPlayerNetworkID = evnt.TargetPlayerNetworkID;
                    newEvnt.Send();
                }
            }
            else{
                if (_player.entity.NetworkId.Equals(evnt.TargetPlayerNetworkID))
                {
                    _player.Heal();
                     Debug.Log("[INTERACTION] HEAL CALLBACK CORRECTLY CALLED");
                }
            }
        }
*/