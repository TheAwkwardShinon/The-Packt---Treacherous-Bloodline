using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bolt;

namespace ThePackt
{
    public class NetworkCallbacks : GlobalEventListener
    {
        public Utils.PrefabAssociation[] playerPrefabs;
        public Vector2 playerSpawnPos;
        public Utils.PrefabAssociation[] enemyPrefabs;
        public Vector2 enemySpawnPos;
        private Player _player;
        private string _playerToSpawnName;
        private List<string> _availableFactions;

        #region callbacks

        /*
        public override void SceneLoadLocalBegin(string scene, Bolt.IProtocolToken token)
        {
            Debug.Log("[SPAWNPLAYER] scene load begin");
            if (BoltNetwork.IsClient)
            {
                var evnt = RequestAvailableFactions.Create(BoltNetwork.Server);
                evnt.Send();

                Debug.Log("[SPAWNPLAYER] request available factions sent at: " + BoltNetwork.Server.ConnectionId);
            }
        }
        */

        public override void SceneLoadLocalDone(string scene, Bolt.IProtocolToken token)
        {
            if (BoltNetwork.IsServer)
            {
                //if this is the server set all characters as available and spawn the selected one

                _availableFactions = new List<string>();
                foreach (Utils.PrefabAssociation assoc in playerPrefabs)
                {
                    _availableFactions.Add(assoc.name);
                }

                Debug.Log("[SPAWNPLAYER] start available: " + GetAvailableFactionString());

                SpawnPlayer();
            }
            else
            {
                //if this is a client, request to the server the list of available characters

                var evnt = RequestAvailableFactions.Create(BoltNetwork.Server);
                evnt.Send();

                Debug.Log("[SPAWNPLAYER] request available factions sent at: " + BoltNetwork.Server.ConnectionId);
            }

            //only the server spawns enemies for everyone
            if (BoltNetwork.IsServer)
            {
                BoltNetwork.Instantiate(enemyPrefabs[0].prefab, enemySpawnPos, Quaternion.identity);
            }
        }

        public override void EntityAttached(BoltEntity entity)
        {
            Player plyr = entity.GetComponent<Player>();
            if (plyr != null)
            {
                //if the spawned entity is a player and this is the owner, store the player info in the _player variable
                if (entity.IsOwner)
                {
                    _player = entity.GetComponent<Player>();
                }

                //if the spawned entity is a player remove it from the available ones
                if (_availableFactions != null)
                {
                    _availableFactions.Remove(entity.tag);
                }

                Debug.Log("[SPAWNPLAYER] attached available: " + GetAvailableFactionString());
            }
        }

        public override void EntityDetached(BoltEntity entity)
        {
            Player plyr = entity.GetComponent<Player>();
            if (plyr != null)
            {
                //if the destroyed entity is a player add it to the available ones
                if(_availableFactions != null)
                {
                    _availableFactions.Add(entity.tag);
                }

                Debug.Log("[SPAWNPLAYER] detatched available: " + GetAvailableFactionString());
            }
        }

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

        public override void OnEvent(RequestAvailableFactions evnt)
        {
            if (BoltNetwork.IsServer)
            {
                //if received by the server, send the available factions to the sender in an event

                Debug.Log("[NETWORKLOG] server recieved available factions request");

                string availableString = "";
                foreach (string f in _availableFactions)
                {
                    availableString += "," + f;
                }

                var newEvnt = RequestAvailableFactions.Create(evnt.RaisedBy);
                newEvnt.AvailableFactions = availableString;
                newEvnt.Send();
            }
            else
            {
                //if received by the client, extract the available factions

                Debug.Log("[NETWORKLOG] client recieved available factions");

                _availableFactions = new List<string>();
                foreach (string f in evnt.AvailableFactions.Split(','))
                {
                    _availableFactions.Add(f);
                }

                Debug.Log("[SPAWNPLAYER] start available: " + GetAvailableFactionString());

                SpawnPlayer();
            }
        }
        #endregion

        #region methods

        /*
        private bool IsCharacterAvailable(string faction)
        {
            return GameObject.FindWithTag(faction) == null;
        }
        */

        //spawns a random player from the available ones
        private bool SpawnRandomPlayer()
        {
            bool spawned = false;
            foreach (Utils.PrefabAssociation assoc in playerPrefabs)
            {
                if (_availableFactions.Contains(assoc.name))
                {
                    BoltNetwork.Instantiate(assoc.prefab, playerSpawnPos, Quaternion.identity);
                    spawned = true;
                    break;
                }
            }

            return spawned;
        }

        private void SpawnPlayer()
        {
            //ceuin as default for now, but with the UI menù -> spawn prefab based on the choice, if it is already present
            //select one free random
            if (BoltNetwork.IsServer)
            {
                _playerToSpawnName = Constants.CEUIN;
            }
            else
            {
                _playerToSpawnName = Constants.CEUIN;
            }

            //instantiate the selected player in the given position
            bool spawned = false;
            foreach (Utils.PrefabAssociation assoc in playerPrefabs)
            {
                if (assoc.name == _playerToSpawnName && _availableFactions.Contains(assoc.name))
                {
                    BoltNetwork.Instantiate(assoc.prefab, playerSpawnPos, Quaternion.identity);
                    spawned = true;
                    break;
                }
            }

            //if the player was not instantiated (because the chosen faction was already taken), spawn a random function among the available ones
            if (!spawned)
            {
                //if the player was yet not instantiated it means all factions were already taken
                if (!SpawnRandomPlayer())
                {
                    Debug.Log("All factions are already picked");
                }
            }
        }
        #endregion

        #region getters
        //only for debug purpuses
        private string GetAvailableFactionString()
        {
            string s = "";
            if (_availableFactions != null)
            {
                foreach (string f in _availableFactions)
                {
                    s += ", " + f;
                }
            }

            return s;
        }
        #endregion
    }
}