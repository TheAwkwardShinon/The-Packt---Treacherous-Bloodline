using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

namespace ThePackt
{
    public delegate bool CompleteCondition();
    public delegate void StartAction();
    public delegate void InProgressAction();
    public delegate void FailAction();

    public class Quest : Bolt.EntityBehaviour<IQuestState>
    {
        #region variables
        [SerializeField] protected Sprite _readySprite;
        [SerializeField] protected Sprite _activeSprite;
        [SerializeField] protected Sprite _notReadySprite;

        public string _title;
        [TextArea(0,50)]public string _description;
        public float _expReward;
        public float _timeReward;
        public float _cooldown;
        
        protected int _state;

        protected float _enteringTime;
        protected float _leavingTime;
        protected float _autoJoinTime;
        protected float _autoAbandonTime;
        protected bool _timerJoin;
        protected bool _timerAbandon;

        protected float _startTime;
        protected float _endTime;
        protected bool _timerEnd;

        protected bool _localPlayerPartecipates;
        protected Player _localPlayer;

        protected List<BoltEntity> _playersInRoom;
        protected List<BoltEntity> _playersPartecipating;

        protected CompleteCondition _completeCondition;
        protected StartAction _startAction;
        protected StartAction _inProgressAction;
        protected FailAction _failAction;
        #endregion


        #region methods
        // Start is called before the first frame update
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

            _autoJoinTime = 5.0f;
            _autoAbandonTime = 5.0f;

            state.AddCallback("State", StateCallback);
        }

        protected virtual void Update()
        {
            string s = "";
            foreach (BoltEntity e in _playersInRoom)
            {
                s += e.NetworkId + ", ";
            }
            //Debug.Log("[QUESTA] in room: " + s);

            if (_timerJoin && Time.time >= _enteringTime + _autoJoinTime)
            {
                _timerJoin = false;

                Join(_localPlayer.entity);
            }
           
            if (_timerAbandon && Time.time >= _leavingTime + _autoAbandonTime)
            {
                _timerAbandon = false;

                Abandon(_localPlayer.entity);
            }
        }

        // Update is called once per frame
        public override void SimulateOwner()
        {
            if (_state == Constants.STARTED)
            {
                if (CheckIfCompleted())
                {
                    state.State = Constants.COMPLETED;

                    //TODO special effect
                }
                else if (_inProgressAction != null)
                {
                    _inProgressAction();
                }
            }
            else
            {
                //Debug.Log("[QUESTTIMER] reactivation. timerend: " + _timerEnd + "; Time: " + Time.time + "; cooldown: " + _cooldown);
                if (_timerEnd && Time.time >= _endTime + _cooldown)
                {
                    Debug.Log("[QUEST] quest reactivated " + _title);

                    //TODO special effect

                    state.State = Constants.READY;
                    _timerEnd = false;
                }
            }
        }
        
        public void Accept()
        {
            if (_state == Constants.READY)
            {
                //send quest start notification to the server passing the bolt network id of the quest

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
            else
            {
                Debug.Log("[QUEST] not ready");
            }
        }

        public void Abandon(BoltEntity playerEntity)
        {
            if (BoltNetwork.IsServer)
            {
                Debug.Log("[QUEST] server abandoned the quest " + _title);

                RemovePlayer(playerEntity);

                string s = "";
                foreach (BoltEntity e in _playersPartecipating)
                {
                    s += e.NetworkId + ", ";
                }
                Debug.Log("[QUEST] partecipating: " + s);

                if (_playersPartecipating.Count == 0)
                {
                    Debug.Log("[QUEST] no more partecipating players. quest " + _title + " failed");

                    //timer and visual effect that indicates that the quest is failing???

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

        public void Join(BoltEntity playerEntity)
        {
            if (BoltNetwork.IsServer)
            {
                Debug.Log("[QUEST] server joined the quest " + _title);

                AddPlayer(playerEntity);

                string s = "";
                foreach (BoltEntity e in _playersPartecipating)
                {
                    s += e.NetworkId + ", ";
                }
                Debug.Log("[QUEST] partecipating: " + s);
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
     
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Player enteredPlayer = collision.GetComponent<Player>();
            if (enteredPlayer != null)
            {
                if (!_playersInRoom.Contains(enteredPlayer.entity))
                {
                    Debug.Log("[QUEST] player entered");

                    _playersInRoom.Add(enteredPlayer.entity);
                }

                if (_localPlayer == enteredPlayer && state.State == Constants.STARTED)
                {
                    if (!_localPlayerPartecipates)
                    {
                        Debug.Log("[QUEST] starting timer for joining local player");

                        //TODO visualize timer for joining the quest

                        _enteringTime = Time.time;
                        _timerJoin = true;
                    }
                    else
                    {
                        if (_timerAbandon)
                        {
                            Debug.Log("[QUEST] stopping timer for leaving local player");

                            //TODO hide timer for leaving the quest

                            _timerAbandon = false;
                        }
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Player exitingPlayer = collision.GetComponent<Player>();
            if (exitingPlayer != null)
            {
                if (_playersInRoom.Contains(exitingPlayer.entity))
                {
                    Debug.Log("[QUEST] player leaved");

                    _playersInRoom.Remove(exitingPlayer.entity);
                }

                if (_localPlayer == exitingPlayer && state.State == Constants.STARTED)
                {
                    if (!_localPlayerPartecipates)
                    {
                        if (_timerJoin)
                        {
                            Debug.Log("[QUEST] stopping timer for joining local player");

                            //TODO hide timer for joining the quest

                            _timerJoin = false;
                        }
                    }
                    else 
                    {
                        Debug.Log("[QUEST] starting timer for leaving local player");

                        //TODO visualize timer for leaving the quest

                        _leavingTime = Time.time;
                        _timerAbandon = true;
                    }
                }
            }
        }

        public void RemovePlayer(BoltEntity plyr)
        {
            if (_playersPartecipating.Contains(plyr))
            {
                _playersPartecipating.Remove(plyr);
            }
        }

        public void AddPlayer(BoltEntity plyr)
        {
            if (!_playersPartecipating.Contains(plyr))
            {
                _playersPartecipating.Add(plyr);
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

        public bool CheckIfCompleted()
        {
            return _completeCondition();
        }

        private void StateCallback()
        {
            _state = state.State;

            Debug.Log("[QUEST] " + _state);

            if (_state == Constants.READY)
            {
                GetComponent<SpriteRenderer>().sprite = _readySprite;
            }
            else if(_state == Constants.STARTED)
            {
                GetComponent<SpriteRenderer>().sprite = _activeSprite;

                if (BoltNetwork.IsServer)
                {
                    _startTime = Time.time;

                    if (_startAction != null)
                    {
                        _startAction();
                    }
                }
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = _notReadySprite;

                if (BoltNetwork.IsServer)
                {
                    Debug.Log("[QUEST] cooldown started");

                    _endTime = Time.time;
                    _timerEnd = true;

                    if (_state == Constants.FAILED && _failAction != null)
                    {
                        _failAction();
                    }
                }
            }

            if (_state == Constants.STARTED && _playersInRoom.Contains(_localPlayer.entity))
            {
                _localPlayer.JoinQuest(this);
            }

            if (_state == Constants.COMPLETED && _localPlayerPartecipates)
            {
                _localPlayer.ObtainRewards(_expReward, _timeReward);
                _localPlayerPartecipates = false;
            }

            if (_state == Constants.FAILED && _localPlayerPartecipates)
            {
                //TODO display fail message
                
                _timerJoin = false;
                _timerAbandon = false;
                _localPlayerPartecipates = false;
            }
        }

        public void LocalPartecipate()
        {
            Join(_localPlayer.entity);
        }
        #endregion

        #region getters
        public List<BoltEntity> GetPlayers()
        {
            return _playersPartecipating;
        }

        public List<BoltEntity> GetPlayersInRoom()
        {
            return _playersInRoom;
        }

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
            _playersPartecipating = plyrs;
        }
        #endregion
    }
}
