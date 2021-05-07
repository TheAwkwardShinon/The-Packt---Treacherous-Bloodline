using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

namespace ThePackt
{
    public delegate bool CompleteCondition();

    public class Quest : Bolt.EntityBehaviour<IQuestState>
    {
        #region variables
        [SerializeField] public string _title { get; private set; }
        [SerializeField] public string _description { get; private set; }
        [SerializeField] public float _expReward { get; private set; }
        [SerializeField] public float _timeReward { get; private set; }

        protected string _state;
        protected bool _localPlayerPartecipates;

        protected List<BoltEntity> _players;
        protected Player _localPlayer;

        protected CompleteCondition _completeCondition;
        #endregion

        #region methods
        // Start is called before the first frame update
        public override void Attached()
        {
            if (BoltNetwork.IsServer)
            {
                _players = new List<BoltEntity>();
            }

            _state = Constants.READY;

            state.AddCallback("State", StateCallback);
        }

        // Update is called once per frame
        public override void SimulateOwner()
        {
            if (_state == Constants.STARTED)
            {
                if (CheckIfCompleted())
                {
                    state.State = Constants.COMPLETED;
                }
            }
        }

        public void Accept()
        {
            if (_state == Constants.READY)
            {
                //send quest start notification to the server passing the bolt network id of the quest

                Debug.Log("[QUEST] accepted event from client to server for quest " + entity.NetworkId);
                QuestAcceptedEvent evnt = QuestAcceptedEvent.Create(BoltNetwork.Server);
                evnt.QuestNetworkID = entity.NetworkId;
                evnt.Send();
            }
        }

        public void Abandon(BoltEntity playerEntity)
        {
            if (BoltNetwork.IsServer)
            {
                Debug.Log("[QUEST] server abandoned the quest " + _title);

                _players.Remove(playerEntity);

                if (_players.Count == 0)
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
        }

        public List<BoltEntity> GetPlayersInRoom()
        {
            //room dimensions and center needed
            //check in a square with the right dimensions what is found and keep the gameobjects with a player component

            _players = new List<BoltEntity>();
            return _players;
        }

        public void RemovePlayer(BoltEntity plyr)
        {
            _players.Remove(plyr);
        }

        public bool CheckIfCompleted()
        {
            return _completeCondition();
        }

        private void StateCallback()
        {
            Debug.Log("[QUEST] started");
            _state = state.State;

            if(_state == Constants.COMPLETED && _localPlayerPartecipates)
            {
                _localPlayer.ObtainRewards(_expReward, _timeReward);
            }
        }

        public void LocalPartecipate(Player plyr)
        {
            _localPlayer = plyr;
            _localPlayerPartecipates = true;
        }
        #endregion

        #region getters
        public List<BoltEntity> GetPlayers()
        {
            return _players;
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
            _players = plyrs;
        }
        #endregion
    }
}
