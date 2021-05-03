using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bolt;
using UdpKit;

namespace ThePackt
{
    public class LobbyNetworkCallbacks : NetworkCallbacks
    {
        [SerializeField] private CharacterSelectionData _selectedData;
        public Utils.PrefabAssociation[] playerPrefabs;
        public Vector2 playerSpawnPos;
        private Player _player;
        private string _playerToSpawnName;
        private List<string> _availableFactions;
        private int _playerNumber;
        public int _minPlayerNumber;
        public float gameStartSeconds;

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
            _playerNumber = 0;

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
        }

        public override void EntityAttached(BoltEntity entity)
        {
            Player plyr = entity.GetComponent<Player>();
            if (plyr != null)
            {
                //if the spawned entity is a player and this is the owner, store the player info in the _player variable
                if (entity.IsOwner)
                {
                    _player = plyr;
                    _selectedData.SetPlayerScript(_player);
                }

                //if the spawned entity is a player remove it from the available ones
                if (_availableFactions != null)
                {
                    _availableFactions.Remove(entity.tag);
                }

                Debug.Log("[SPAWNPLAYER] attached available: " + GetAvailableFactionString());

                if (BoltNetwork.IsServer)
                {
                    _playerNumber++;
                    if (_playerNumber == _minPlayerNumber)
                    {
                        StartCoroutine("StartGame");
                    }

                    Debug.Log("TIMER server: " + BoltNetwork.ServerTime);
                    Debug.Log("TIMER client: " + BoltNetwork.Time);
                }
            }
        }

        IEnumerator StartGame()
        {
            yield return new WaitForSeconds(gameStartSeconds);

            //enable black screen here

            BoltNetwork.LoadScene(Constants.MAP);
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

                if (BoltNetwork.IsServer)
                {
                    _playerNumber--;
                }
            }
        }

        public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
        {
            Debug.Log("[CONNECTIONLOG] connect request");
       
            BoltNetwork.Accept(endpoint);
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
                    _selectedData.SetNickname("Player-" + UnityEngine.Random.Range(1, 9999));
                    _selectedData.SetCharacterSelected(assoc.name);
                    BoltNetwork.Instantiate(assoc.prefab, playerSpawnPos, Quaternion.identity);
                    spawned = true;
                    break;
                }
            }

            return spawned;
        }

        private void SpawnPlayer()
        {
            _playerToSpawnName = _selectedData.GetCharacterSelected();
            Debug.Log("[SPAWNPLAYER] choice: " + _playerToSpawnName);
            
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