using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ThePackt
{
    public class MainQuest : Bolt.EntityBehaviour<IQuestState>
    {
        [SerializeField] private GameObject _objectivePrefab;
        [SerializeField] private Transform[] _objectivePositions;
        [SerializeField] private Transform _upLeftPosition;
        [SerializeField] private float _returnToMenuSeconds;
        private List<BoltEntity> _objectives;
        private List<BoltEntity> _notImpostors;
        private BoltEntity _impostor;
        private int _state;

        private Player _localPlayer;
        protected CharacterSelectionData _selectedData;

        private static MainQuest _instance;

        private List<BoltEntity> _playersInRoom;

        [SerializeField] private AudioClip _startSound;
        [SerializeField] private AudioClip _victorySound;
        [SerializeField] private AudioClip _defeatSound;

        private HiddenCanvas _hiddenCanvas;
        private GameObject _objectiveMessage;
        private Text _objectiveText;

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

        public void Update(){
            if(SceneManager.GetActiveScene().name.Equals("MapScene") && _hiddenCanvas == null){
                _hiddenCanvas = GameObject.Find("Canvas").GetComponent<HiddenCanvas>();
                _objectiveMessage = _hiddenCanvas.getObjectiveMessage();
                _objectiveText = _objectiveMessage.GetComponentInChildren<Text>();
            }
        }

        private void Awake(){
            _notImpostors = new List<BoltEntity>();
            _objectives = new List<BoltEntity>();
            _playersInRoom = new List<BoltEntity>();
            
        }

        public override void Attached()
        {
            _selectedData = CharacterSelectionData.Instance;
            _localPlayer = _selectedData.GetPlayerScript();
            if(SceneManager.GetActiveScene().Equals("MapScene")){
                _hiddenCanvas = GameObject.Find("Canvas").GetComponent<HiddenCanvas>();
                _objectiveMessage = _hiddenCanvas.getObjectiveMessage();
                _objectiveText = _objectiveMessage.GetComponentInChildren<Text>();
            }

            if (entity.IsOwner)
            {
                
                state.State = Constants.READY;

                foreach(Transform pos in _objectivePositions)
                {
                    SpawnObjective(pos);
                }
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

        protected void SpawnObjective(Transform pos)
        {
            if (BoltNetwork.IsServer)
            {
                var token = new ObjectiveDataToken();
                if (pos == _upLeftPosition)
                {
                    token.SetIsUpLeft(true);
                }
                else
                {
                    token.SetIsUpLeft(false);
                }

                BoltEntity _spawnedObjective = BoltNetwork.Instantiate(_objectivePrefab, token, pos.position, pos.rotation);
                _objectives.Add(_spawnedObjective);

                _spawnedObjective.GetComponent<Objective>()._rotation = pos.rotation;
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

            if(_state == Constants.STARTED)
            {
                Debug.Log("[MAIN] main quest started");

                if(_localPlayer.isImpostor()){
                    _objectiveText.text = " YOU ARE THE BAD WOLF";
                    _objectiveText.color = Color.red;
                    _objectiveMessage.SetActive(true);
                }
                else{
                    _objectiveText.text = " YOU ARE THE GOOD WOLF";
                    _objectiveText.color = Color.blue;
                    _objectiveMessage.SetActive(true);
                }


                AudioSource.PlayClipAtPoint(_startSound, Camera.main.transform.position);
            }

            if (_state == Constants.COMPLETED)
            {
                Debug.Log("[MAIN] all objectives detroyed. victory for non impostors");

                 if(_localPlayer.isImpostor()){
                    _objectiveText.text = " YOU HAVE BEEN DEFEATED";
                    _objectiveText.color = Color.red;
                    _objectiveMessage.SetActive(true);
                }
                else{
                    _objectiveText.text = " YOUR TEAM WON";
                    _objectiveText.color = Color.yellow;
                    _objectiveMessage.SetActive(true);
                }

                if (_localPlayer.isImpostor())
                {
                    AudioSource.PlayClipAtPoint(_defeatSound, Camera.main.transform.position);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(_victorySound, Camera.main.transform.position);
                }

                /*
                VoiceManager voiceManager = VoiceManager.Instance;
                if (voiceManager != null)
                {
                    voiceManager.Logout();
                }
                */

                StartCoroutine("ReturnToMenu");
            }

            if (_state == Constants.FAILED)
            {
                
                if(_localPlayer.isImpostor()){
                    _objectiveText.text = " YOU WON";
                    _objectiveText.color = Color.yellow;
                    _objectiveMessage.SetActive(true);
                }
                else{
                    _objectiveText.text = " YOUR TEAM HAVE BEEN DEFEATED";
                    _objectiveText.color = Color.red;
                    _objectiveMessage.SetActive(true);
                }
                Debug.Log("[MAIN] failed impostor" + _localPlayer.isImpostor());
                if (_localPlayer.isImpostor())
                {
                    AudioSource.PlayClipAtPoint(_victorySound, Camera.main.transform.position);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(_defeatSound, Camera.main.transform.position);
                }

                /*
                VoiceManager voiceManager = VoiceManager.Instance;
                if (voiceManager != null)
                {
                    voiceManager.Logout();
                }
                */

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
            
            int randomIndex = Random.Range(0, _notImpostors.Count);
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

        #region others
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Player enteredPlayer = collision.GetComponent<Player>();
            if (enteredPlayer != null)
            {
                if(enteredPlayer.GetPlayerData().astralconjuntion)
                    enteredPlayer.GetPlayerData().damageMultiplier +=  enteredPlayer.GetPlayerData().portalRoomDamageMultiplier;
                if (!_playersInRoom.Contains(enteredPlayer.entity))
                {
                    Debug.Log("[MAIN] player entered");

                    _playersInRoom.Add(enteredPlayer.entity);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Player exitingPlayer = collision.GetComponent<Player>();
            if (exitingPlayer != null)
            {
                if(exitingPlayer.GetPlayerData().astralconjuntion)
                    exitingPlayer.GetPlayerData().damageMultiplier -=  exitingPlayer.GetPlayerData().portalRoomDamageMultiplier;
                if (_playersInRoom.Contains(exitingPlayer.entity))
                {
                    Debug.Log("[QUEST] player leaved");

                    _playersInRoom.Remove(exitingPlayer.entity);
                }
            }
        }


        public bool CheckIfPlayerIsInRoom(BoltEntity plyr)
        {
            if (_playersInRoom.Contains(plyr))
            {
                return true;
            }

            return false;
        }
        #endregion

        #region getters
        public int GetQuestState()
        {
            return _state;
        }
        #endregion

        #region setters
        public void SetQuestState(int value)
        {
            if (entity.IsOwner)
            {
                state.State = value;
            }
        }

        public void SetPlayers(List<BoltEntity> plyrs)
        {
            _localPlayer = _selectedData.GetPlayerScript();
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
