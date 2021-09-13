using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using UnityEngine.UI;

namespace ThePackt
{
    public delegate bool CompleteCondition();
    public delegate void StartAction();
    public delegate void InProgressAction();
    public delegate void FailAction();

    public class Quest : Bolt.EntityBehaviour<IQuestState>
    {
        #region variables
        [Header("Sprites")]
        [SerializeField] protected Sprite _readySprite;
        [SerializeField] protected Sprite _activeSprite;
        [SerializeField] protected Sprite _notReadySprite;

        [Header("Details")]
        public string _title;
        [TextArea(0,50)]public string _description;
        public float _expReward;
        public float _timeReward;
        public float _cooldown;
        protected int _type;

        protected int _state;

        #region difficulty level
        [SerializeField] protected int _difficultyLevel; //shown in inspector only for debug purposes
        [SerializeField] protected int _maximumReachableLevel;
        protected int _numberOfCompletements;
        [SerializeField] protected int _completementsForLevel; //number of completements needed to make the difficulty level increase
        #endregion

        #region partecipation
        protected float _enteringTime;
        protected float _leavingTime;
        [SerializeField] protected float _autoJoinTime = 5.0f;
        [SerializeField] protected float _autoAbandonTime = 5.0f;
        protected bool _timerJoin;
        protected bool _timerAbandon;
        protected bool _localPlayerPartecipates;
        protected Player _localPlayer;
        protected List<BoltEntity> _playersInRoom;
        protected List<BoltEntity> _playersPartecipating;
        #endregion

        #region restart
        protected float _startTime;
        protected float _endTime;
        protected bool _timerEnd;
        #endregion

        #region delegates
        protected CompleteCondition _completeCondition;
        protected StartAction _startAction;
        protected StartAction _inProgressAction;
        protected FailAction _failAction;
        #endregion

        #region ui
        private HiddenCanvas _hiddenCanvas;
        private GameObject _objectiveMessage;
        private Text _objectiveText;
        #endregion
        #endregion


        #region methods
        public override void Attached()
        {
            _playersInRoom = new List<BoltEntity>();
            _playersPartecipating = new List<BoltEntity>();

            if (entity.IsOwner)
            {
                state.State = Constants.READY;
            }

            _timerJoin = false;
            _timerAbandon = false;
            _numberOfCompletements = 0;

            state.AddCallback("State", StateCallback);
        }

        //executed on every machine
        protected virtual void Update()
        {
            /*
            string s = "";
            foreach (BoltEntity e in _playersInRoom)
            {
                s += e.NetworkId + ", ";
            }
            Debug.Log("[QUEST] in room: " + s);
            */

            //make the local player join when joining timer timeouts
            if (_timerJoin && Time.time >= _enteringTime + _autoJoinTime)
            {
                _timerJoin = false;

                if (_localPlayer.entity.IsAttached)
                {
                    Join(_localPlayer.entity);
                }
            }

            //make the local player abandon when abandoning timer timeouts
            if (_timerAbandon && Time.time >= _leavingTime + _autoAbandonTime)
            {
                _timerAbandon = false;

                if (_localPlayer.entity.IsAttached)
                {
                    Abandon(_localPlayer.entity);
                }
            }

            if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("MapScene") && _hiddenCanvas == null){
                _hiddenCanvas = GameObject.Find("Canvas").GetComponent<HiddenCanvas>();
                _objectiveMessage = _hiddenCanvas.getObjectiveMessage();
                _objectiveText = _objectiveMessage.GetComponentInChildren<Text>();
            }
        }

        //executed only on the server
        public override void SimulateOwner()
        {
            if (_state == Constants.STARTED)
            {
                //if the quest is started check if it is completed, otherwise execute the _inProgressAction delegate
                if (_completeCondition())
                    state.State = Constants.COMPLETED;
                else if (_inProgressAction != null)
                    _inProgressAction();
            }
            else
            {
                //reactivate the quest when the cooldown timeouts
                if (_timerEnd && Time.time >= _endTime + _cooldown)
                {
                    Debug.Log("[QUEST] quest reactivated " + _title);
                    state.State = Constants.READY;
                    _timerEnd = false;
                }
            }
        }

        ///<summary>
        ///accept the quest by sending quest start notification to the server
        ///</summary>
        public void Accept()
        {
            if (_state == Constants.READY)
                if (BoltNetwork.IsServer)
                {
                    Debug.Log("[QUEST] server accepted quest");
                    SetQuestState(Constants.STARTED);
                }
                else{

                    Debug.Log("[QUEST] accepted event from client to server for quest " + entity.NetworkId);
                    QuestAcceptedEvent evnt = QuestAcceptedEvent.Create(BoltNetwork.Server);
                    evnt.QuestNetworkID = entity.NetworkId;
                    evnt.Send();
                }
        }

        ///<summary>
        ///abandon quest by sending quest abandoning notification to the server for the abandoning player
        ///</summary>
        public void Abandon(BoltEntity playerEntity)
        {
            if (BoltNetwork.IsServer)
            {
                Debug.Log("[QUEST] server abandoned the quest " + _title);
                RemovePlayer(playerEntity);

                //if no player partecipating remains the quest fails
                if (_playersPartecipating.Count == 0)
                {
                    Debug.Log("[QUEST] no more partecipating players. quest " + _title + " failed");
                    state.State = Constants.FAILED;
                }
            }
            else
            {
                Debug.Log("[QUEST] client abandoned the quest " + entity.NetworkId);

                QuestAbandonedEvent evnt = QuestAbandonedEvent.Create(BoltNetwork.Server);
                evnt.QuestNetworkID = entity.NetworkId;
                evnt.AbandoningPlayerNetworkID = playerEntity.NetworkId;
                evnt.Send();
            }

            _localPlayerPartecipates = false;
            _localPlayer.AbandonQuest();
        }

        ///<summary>
        ///join quest by sending quest joining notification to the server for the joining player
        ///</summary>
        public void Join(BoltEntity playerEntity)
        {
            if (BoltNetwork.IsServer)
            {
                Debug.Log("[QUEST] server joined the quest " + _title);
                AddPlayer(playerEntity);
            }
            else
            {
                Debug.Log("[QUEST] client joined the quest " + entity.NetworkId);

                QuestJoinedEvent evnt = QuestJoinedEvent.Create(BoltNetwork.Server);
                evnt.QuestNetworkID = entity.NetworkId;
                evnt.JoiningPlayerNetworkID = playerEntity.NetworkId;
                evnt.Send();
            }

            _localPlayerPartecipates = true;
            _localPlayer.JoinQuest(this);
        }

        //triggered when something enters the room
        private void OnTriggerEnter2D(Collider2D collision)
        {
            //if the entered collider is a player
            Player enteredPlayer = collision.GetComponent<Player>();
            if (enteredPlayer)
            {
                //add the player to the players in room
                if (!_playersInRoom.Contains(enteredPlayer.entity))
                {
                    Debug.Log("[QUEST] player entered");
                    _playersInRoom.Add(enteredPlayer.entity);
                }

                //if the quest is started and the entered player is the local player
                if (_localPlayer == enteredPlayer && state.State == Constants.STARTED)
                    if (!_localPlayerPartecipates)
                    {
                        Debug.Log("[QUEST] starting timer for joining local player");

                        //TODO visualize timer for joining the quest

                        //if the player is not partecipating start timer for the player to join
                        _enteringTime = Time.time;
                        _timerJoin = true;
                    }
                    else if (_timerAbandon)
                    {
                        Debug.Log("[QUEST] stopping timer for leaving local player");

                        //TODO hide timer for leaving the quest

                        //if the player is partecipating stop timer for the player to abandon
                        _timerAbandon = false;
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
                //remove the player from the players in room
                if (_playersInRoom.Contains(exitingPlayer.entity))
                {
                    Debug.Log("[QUEST] player leaved");
                    _playersInRoom.Remove(exitingPlayer.entity);
                }

                //if the quest is started and the entered player is the local player
                if (_localPlayer == exitingPlayer && state.State == Constants.STARTED)
                    if (!_localPlayerPartecipates)
                    {
                        if (_timerJoin)
                        {
                            Debug.Log("[QUEST] stopping timer for joining local player");

                            //TODO hide timer for joining the quest

                            //if the player is not partecipating stop the timer for the player to join
                            _timerJoin = false;
                        }
                    }
                    else 
                    {
                        Debug.Log("[QUEST] starting timer for leaving local player");

                        //TODO visualize timer for leaving the quest

                        //if the player is partecipating start timer for the player to abandon
                        _leavingTime = Time.time;
                        _timerAbandon = true;
                    }
            }
        }

        ///<summary>
        ///callback called when the quest state changes
        ///</summary>
        private void StateCallback()
        {
            Debug.Log("[QUEST] " + _state);
            _state = state.State;
            gameObject.GetComponent<AudioSource>().Play();

            if (_state == Constants.READY)
            {
                GetComponent<SpriteRenderer>().sprite = _readySprite;
            }
            else if (_state == Constants.STARTED)
            {
                GetComponent<SpriteRenderer>().sprite = _activeSprite;

                //if the quest started the server executes the _startAction (if there is one)
                if (BoltNetwork.IsServer)
                {
                    _startTime = Time.time;
                    if (_startAction != null)
                        _startAction();
                }

                //if the quest started and the local player is in room make him join the quest
                if (_playersInRoom.Contains(_localPlayer.entity))
                    _localPlayer.JoinQuest(this);
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = _notReadySprite;

                //if quest is failed or completed the server starts the reactivation cooldown
                if (BoltNetwork.IsServer)
                {
                    Debug.Log("[QUEST] cooldown started");
                    _endTime = Time.time;
                    _timerEnd = true;

                    //if it is failed the server executes the _failAction
                    if (_state == Constants.FAILED && _failAction != null)
                        _failAction();

                    //if it is completed the server checks if the difficulty level must be increased
                    if (_state == Constants.COMPLETED)
                    {
                        _numberOfCompletements++;
                        if ((_numberOfCompletements % _completementsForLevel == 0) && _difficultyLevel < _maximumReachableLevel)
                            IncrementDifficultyLevel();
                    }
                }

                //if quest is completed and the local player is partecipating give him the rewards
                if (_state == Constants.COMPLETED && _localPlayerPartecipates)
                {
                    _localPlayer.ObtainRewards(_expReward, _timeReward);
                    _localPlayerPartecipates = false;
                }

                //if quest is failed and the local player is partecipating show the fail panel and remove him from the quest
                if (_state == Constants.FAILED && _localPlayerPartecipates)
                {
                    _objectiveText.text = "QUEST FAILED";
                    _objectiveText.color = Color.red;
                    _objectiveMessage.SetActive(true);

                    _timerJoin = false;
                    _timerAbandon = false;
                    _localPlayerPartecipates = false;
                }
            }
        }

        ///<summary>
        ///removes player plyr from partecipating players
        ///</summary>
        public void RemovePlayer(BoltEntity plyr)
        {
            if (_playersPartecipating.Contains(plyr))
            {
                _playersPartecipating.Remove(plyr);
            }
        }

        ///<summary>
        ///adds player plyr to partecipating players
        ///</summary>
        public void AddPlayer(BoltEntity plyr)
        {
            if (!_playersPartecipating.Contains(plyr))
            {
                _playersPartecipating.Add(plyr);
            }
        }

        ///<summary>
        ///returns true the player plyr is in room, false otherwise
        ///</summary>
        public bool CheckIfPlayerIsInRoom(BoltEntity plyr)
        {
            return _playersInRoom.Contains(plyr) ? true : false;
        }
        #endregion

        #region getters
        ///<summary>
        ///returns partecipating players
        ///</summary>
        public List<BoltEntity> GetPlayers()
        {
            return _playersPartecipating;
        }

        public int GetQuestState()
        {
            return _state;
        }

        public int GetQuestType()
        {
            return _type;
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
            _playersPartecipating = plyrs;
        }

        public virtual void SetDifficultyLevel(int level)
        {
            _difficultyLevel = level;

            Debug.Log("[MAPGEN] level set : " + _difficultyLevel);
        }

        ///<summary>
        ///increment difficulty level of 1
        ///</summary>
        public virtual void IncrementDifficultyLevel()
        {
            Debug.Log("[MAPGEN] incremented level of " + _title);
            SetDifficultyLevel(_difficultyLevel + 1);
        }
        #endregion
    }
}
