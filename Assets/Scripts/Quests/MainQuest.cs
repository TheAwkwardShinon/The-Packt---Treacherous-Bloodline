using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class MainQuest : Bolt.EntityBehaviour<IQuestState>
    {
        [SerializeField] private GameObject _objectivePrefab;
        [SerializeField] private Vector2[] _objectivePositions;
        [SerializeField] private float _returnToMenuSeconds;
        private List<BoltEntity> _objectives;
        private List<BoltEntity> _notImpostors;
        private BoltEntity _impostor;
        private string _state;

        private Player _localPlayer;
        protected CharacterSelectionData _selectedData;

        private static MainQuest _instance;

        public static MainQuest Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MainQuest>();
                }

                return _instance;
            }
        }

        #region methods

        public override void Attached()
        {
            _selectedData = CharacterSelectionData.Instance;
            _localPlayer = _selectedData.GetPlayerScript();

            if (entity.IsOwner)
            {
                _notImpostors = new List<BoltEntity>();
                _objectives = new List<BoltEntity>();
                state.State = Constants.READY;

                SpawnObjective(_objectivePositions[1]);

                /*
                foreach(Vector2 pos in _objectivePositions)
                {
                    SpawnObjective(pos);
                }
                */
            }

            state.AddCallback("State", StateCallback);
        }
        public override void SimulateOwner()
        {
            if (_state == Constants.STARTED)
            {
                if (_objectives.Count == 0)
                { 
                    state.State = Constants.COMPLETED;
                }
            }
        }

        protected void SpawnObjective(Vector2 pos)
        {
            if (BoltNetwork.IsServer)
            {
                BoltEntity _spawnedObjective = BoltNetwork.Instantiate(_objectivePrefab, pos, Quaternion.identity);
                _objectives.Add(_spawnedObjective);
            }
        }

        protected bool WereNotImpostorsKilled()
        {
            if (_notImpostors.Count == 0)
            {
                return true;
            }

            return false;
        }

        private void StateCallback()
        {
            _state = state.State;

            Debug.Log("[MAIN] " + _state);

            if (BoltNetwork.IsServer)
            {
                if(_state == Constants.STARTED)
                {
                    Debug.Log("[MAIN] main quest started");

                    //TODO ui of impostor/not impostor, open gates...
                }
            }

            if (_state == Constants.COMPLETED)
            {
                Debug.Log("[MAIN] all objectives detroyed. victory for non impostors");

                //TODO victory ui if _localPlayer is not impostor or defeat ui otherwise

                StartCoroutine("ReturnToMenu");
            }

            if (_state == Constants.FAILED)
            {
                //TODO victory ui if _localPlayer is not impostor or defeat ui otherwise

                StartCoroutine("ReturnToMenu");
            }
        }

        IEnumerator ReturnToMenu()
        {
            yield return new WaitForSeconds(_returnToMenuSeconds);

            //enable black screen here

            BoltNetwork.Shutdown();
        }

        public void RemoveObjective(BoltEntity objective)
        {
            _objectives.Remove(objective);
        }

        public void RemovePlayer(BoltEntity plyr)
        {
            if (!(_impostor == plyr) && _notImpostors.Contains(plyr))
            {
                Debug.Log("[MAIN] removed player " + plyr.NetworkId);

                _notImpostors.Remove(plyr);

                string s = "";
                foreach (BoltEntity e in _notImpostors)
                {
                    s += e.NetworkId + ", ";
                }
                Debug.Log("[MAIN] not impostors: " + s);

                if (_state == Constants.STARTED && _notImpostors.Count == 0)
                {
                    Debug.Log("[MAIN] all other players killed. victory for impostor");

                    state.State = Constants.FAILED;
                }
            }
        }

        public void AddPlayer(BoltEntity plyr)
        {
            if (!(_impostor == plyr) && !_notImpostors.Contains(plyr))
            {
                Debug.Log("[MAIN] added player " + plyr.NetworkId);

                _notImpostors.Add(plyr);

                string s = "";
                foreach (BoltEntity e in _notImpostors)
                {
                    s += e.NetworkId + ", ";
                }
                Debug.Log("[MAIN] not impostors: " + s);
            }
        }

        public void ChooseImpostor()
        {
            int randomIndex = Random.Range(0, _notImpostors.Count - 1);
            _impostor = _notImpostors[randomIndex];
            _notImpostors.Remove(_impostor);

            if (_impostor.IsOwner)
            {
                Debug.Log("[MAIN] server is impostor");

                _localPlayer.SetIsImpostor(true);
            }
            else
            {
                Debug.Log("[MAIN] impostor is client " + _impostor.NetworkId);

                ImpostorEvent evnt = ImpostorEvent.Create(_impostor.Source);
                evnt.ImpostorNetworkID = _impostor.NetworkId;
                evnt.Send();
            }
        }
        #endregion

        #region getters
        public string GetQuestState()
        {
            return _state;
        }
        #endregion

        #region setters
        public void SetQuestState(string value)
        {
            if (entity.IsOwner)
            {
                state.State = value;
            }
        }

        public void SetPlayers(List<BoltEntity> plyrs)
        {
            _notImpostors = plyrs;

            string s = "";
            foreach (BoltEntity e in _notImpostors)
            {
                s += e.NetworkId + ", ";
            }
            Debug.Log("[MAIN] all players: " + s);

            ChooseImpostor();
        }

        public void TimeElapsed()
        {
            Debug.Log("[MAIN] victory for impostor");

            state.State = Constants.FAILED;
        }
        #endregion

    }
}
