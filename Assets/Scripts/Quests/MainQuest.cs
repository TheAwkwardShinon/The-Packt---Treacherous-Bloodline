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
        [SerializeField] private Platform[] _platforms;
        [SerializeField] private Transform _upLeftPosition;
        [SerializeField] private float _returnToMenuSeconds;
        private List<BoltEntity> _objectives;
        private List<BoltEntity> _notImpostors;
        private List<BoltEntity> _playersInRoom;
        private BoltEntity _impostor;
        private int _state;
        private Player _localPlayer;
        protected CharacterSelectionData _selectedData;

        [Header("Sounds")]
        [SerializeField] private AudioClip _startSound;
        [SerializeField] private AudioClip _victorySound;
        [SerializeField] private AudioClip _defeatSound;

        #region ui
        private HiddenCanvas _hiddenCanvas;
        private GameObject _objectiveMessage;
        private Text _objectiveText;
        #endregion

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
        //executed on every machine
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

            //when the entity is attached the server spawns the objectives
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

        //executed only for the owner
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

        /*
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (var platform in _platforms)
            {
                Gizmos.DrawWireSphere(platform.left.position, 0.1f);
                Gizmos.DrawWireSphere(platform.right.position, 0.1f);
            }
        }
        */

        ///<summary>
        ///spawns an objective at position pos
        ///</summary>
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

        ///<summary>
        ///callback called when the quest state changes
        ///</summary>
        private void StateCallback()
        {
            _state = state.State;

            Debug.Log("[MAIN] " + _state);

            if(_state == Constants.STARTED)
            {
                Debug.Log("[MAIN] main quest started");

                //if it started show the role to the local player
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
            else if (_state == Constants.COMPLETED)
            {
                Debug.Log("[MAIN] all objectives detroyed. victory for non impostors");

                //if it was a success show victory to non impostors and defeat to the impostor
                if (_localPlayer.isImpostor()){
                    _objectiveText.text = " YOU HAVE BEEN DEFEATED";
                    _objectiveText.color = Color.red;
                    _objectiveMessage.SetActive(true);

                    AudioSource.PlayClipAtPoint(_defeatSound, Camera.main.transform.position);
                }
                else 
                {
                    _objectiveText.text = " YOUR TEAM WON";
                    _objectiveText.color = Color.yellow;
                    _objectiveMessage.SetActive(true);

                    AudioSource.PlayClipAtPoint(_victorySound, Camera.main.transform.position);
                }

                //then return to menu
                StartCoroutine("ReturnToMenu");
            }
            else if (_state == Constants.FAILED)
            {
                Debug.Log("[MAIN] failed impostor" + _localPlayer.isImpostor());

                //if it was a failure show victory to the impostor and defeat to the non impostors
                if (_localPlayer.isImpostor()){
                    _objectiveText.text = " YOU WON";
                    _objectiveText.color = Color.yellow;
                    _objectiveMessage.SetActive(true);

                    AudioSource.PlayClipAtPoint(_victorySound, Camera.main.transform.position);
                }
                else
                {
                    _objectiveText.text = " YOUR TEAM HAVE BEEN DEFEATED";
                    _objectiveText.color = Color.red;
                    _objectiveMessage.SetActive(true);

                    AudioSource.PlayClipAtPoint(_defeatSound, Camera.main.transform.position);
                }

                //then return to menu
                StartCoroutine("ReturnToMenu");
            }
        }

        ///<summary>
        ///waits _returnToMenuSeconds befor returning to the menu screen
        ///</summary>
        IEnumerator ReturnToMenu()
        {
            yield return new WaitForSeconds(_returnToMenuSeconds);

            //enable black screen here

            BoltNetwork.Shutdown();
        }

        //triggered when something enters the room
        private void OnTriggerEnter2D(Collider2D collision)
        {
            //if the entered collider is a player
            Player enteredPlayer = collision.GetComponent<Player>();
            if (enteredPlayer)
            {
                if(enteredPlayer.GetPlayerData().astralconjuntion)
                    enteredPlayer.GetPlayerData().damageMultiplier +=  enteredPlayer.GetPlayerData().portalRoomDamageMultiplier;

                //add the player to the players in room
                if (!_playersInRoom.Contains(enteredPlayer.entity))
                {
                    Debug.Log("[MAIN] player entered");
                    _playersInRoom.Add(enteredPlayer.entity);
                }
            }
        }

        //triggered when something exits the room
        private void OnTriggerExit2D(Collider2D collision)
        {
            //if the entered collider is a player
            Player exitingPlayer = collision.GetComponent<Player>();
            if (exitingPlayer)
            {
                if(exitingPlayer.GetPlayerData().astralconjuntion)
                    exitingPlayer.GetPlayerData().damageMultiplier -=  exitingPlayer.GetPlayerData().portalRoomDamageMultiplier;

                //remove the player from the players in room
                if (_playersInRoom.Contains(exitingPlayer.entity))
                {
                    Debug.Log("[MAIN] player leaved");

                    _playersInRoom.Remove(exitingPlayer.entity);
                }
            }
        }

        ///<summary>
        ///returns true if plyr is in the room, false otherwise 
        ///</summary>
        public bool CheckIfPlayerIsInRoom(BoltEntity plyr)
        {
            if (_playersInRoom.Contains(plyr))
            {
                return true;
            }

            return false;
        }

        ///<summary>
        ///select the impostor randomly and communicate the choice to the impostor
        ///</summary>
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

        ///<summary>
        ///sets the players and choose an impostor among them
        ///</summary>
        public void SetPlayers(List<BoltEntity> plyrs)
        {
            _localPlayer = _selectedData.GetPlayerScript();
            _notImpostors = plyrs;

            /*
            string s = "";
            foreach (BoltEntity e in _notImpostors)
            {
                s += e.NetworkId + ", ";
            }
            Debug.Log("[MAIN] all players: " + s);
            */

            ChooseImpostor();
        }

        ///<summary>
        ///removes objective from _objectives 
        ///</summary>
        public void RemoveObjective(BoltEntity objective)
        {
            _objectives.Remove(objective);
        }

        ///<summary>
        ///removes plyr from _notImpostors if plyr is not the impostor
        ///</summary>
        public void RemovePlayer(BoltEntity plyr)
        {
            if (!(_impostor == plyr) && _notImpostors.Contains(plyr))
            {
                Debug.Log("[MAIN] removed player " + plyr.NetworkId);

                _notImpostors.Remove(plyr);

                //if no non impostor remains the main quest fails and the impostor wins
                if (_state == Constants.STARTED && _notImpostors.Count == 0)
                {
                    Debug.Log("[MAIN] all other players killed. victory for impostor");

                    state.State = Constants.FAILED;
                }
            }
        }

        ///<summary>
        ///adds plyr to _notImpostors if plyr is not the impostor
        ///</summary>
        public void AddPlayer(BoltEntity plyr)
        {
            if (!(_impostor == plyr) && !_notImpostors.Contains(plyr))
            {
                Debug.Log("[MAIN] added player " + plyr.NetworkId);

                _notImpostors.Add(plyr);
            }
        }
        #endregion

        #region getters
        public int GetQuestState()
        {
            return _state;
        }

        public Platform[] GetPlatforms()
        {
            return _platforms;
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
        #endregion

    }
}
