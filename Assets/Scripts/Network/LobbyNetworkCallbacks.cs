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
        public Utils.PrefabAssociation[] playerPrefabs;
        public Utils.VectorAssociation[] playersSpawnPositions;
        private Player _player;
        private string _playerToSpawnName;
        private List<string> _availableFactions;
        private int _playerNumber;
        public int _minPlayerNumber;
        public float gameStartSeconds;

        [SerializeField] private Canvas _blackScreenCanvas;
        [SerializeField] private float _loadingScreenTime;

        #region callbacks

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

            StartCoroutine("LoadingScreen");
        }

        IEnumerator LoadingScreen()
        {
            yield return new WaitForSeconds(_loadingScreenTime);

            _player.SetEnabledInput(true);

            _blackScreenCanvas.gameObject.SetActive(false);
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
                    //dev options
                    if (_playerNumber == _selectedData.GetPlayersNumber())
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

            _player.SetEnabledInput(false);

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

        public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
        {
            base.BoltShutdownBegin(registerDoneCallback, disconnectReason);
            _blackScreenCanvas.gameObject.SetActive(true);
        }

        public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
        {
            Debug.Log("[CONNECTIONLOG] connect request");
       
            BoltNetwork.Accept(endpoint);
        }

        public override void OnEvent(SetColliderSizeEvent evnt)
        {
            BoltEntity entity = BoltNetwork.FindEntity(evnt.TargetPlayerNetworkID);
            Player player = entity.GetComponent<Player>();

            player.GetComponent<BoxCollider2D>().offset = evnt.Offset;
            player.GetComponent<BoxCollider2D>().size = evnt.Size;
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
            Vector3 spawnPos = Vector3.zero;
            foreach (Utils.PrefabAssociation assoc in playerPrefabs)
            {
                if (_availableFactions.Contains(assoc.name))
                {
                    foreach (Utils.VectorAssociation assov in playersSpawnPositions)
                    {
                        if (assov.name == assoc.name)
                        {
                            spawnPos = assov.position;
                            break;
                        }
                    }

                    _selectedData.SetNickname("Player-" + UnityEngine.Random.Range(1, 9999));
                    _selectedData.SetCharacterSelected(assoc.name);

                    switch (assoc.name)
                    {
                        case Constants.MOONSIGHTERS:
                            _selectedData.SetCharacterIndex(14); //index for researcher
                            break;
                        case Constants.FELE:
                            _selectedData.SetCharacterIndex(15); //index for soldier
                            break;
                        case Constants.AYATANA:
                            _selectedData.SetCharacterIndex(17); //index for writer
                            break;
                        case Constants.NATURIA:
                            _selectedData.SetCharacterIndex(9); //index for herbalist
                            break;
                        case Constants.HERIN:
                            _selectedData.SetCharacterIndex(8); //index for anchor
                            break;
                        case Constants.CEUIN:
                            _selectedData.SetCharacterIndex(10); //index for lawyer
                            break;
                    }

                    BoltNetwork.Instantiate(assoc.prefab, spawnPos, Quaternion.identity);
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
            Vector3 spawnPos = Vector3.zero;
            foreach (Utils.PrefabAssociation assoc in playerPrefabs)
            {
                if (assoc.name == _playerToSpawnName && _availableFactions.Contains(assoc.name))
                {
                    foreach (Utils.VectorAssociation assov in playersSpawnPositions)
                    {
                        if (assov.name == assoc.name)
                        {
                            spawnPos = assov.position;
                            break;
                        }
                    }

                    BoltNetwork.Instantiate(assoc.prefab, spawnPos, Quaternion.identity);
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