﻿using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Bolt;

namespace ThePackt
{
    public class QuestCallbacks : GlobalEventListener
    {
        #region variables
        private CharacterSelectionData _selectedData;
        private Player _player;
        #endregion

        #region methods

        private void Awake()
        {
            _selectedData = CharacterSelectionData.Instance;
        }

        #region callbacks

        public override void SceneLoadLocalDone(string scene, IProtocolToken token)
        {
            _player = _selectedData.GetPlayerScript();
        }

        public override void OnEvent(QuestAcceptedEvent evnt)
        {
            Debug.Log("[QUEST] accepted event received");

            if (BoltNetwork.IsServer)
            {
                Debug.Log("[QUEST] server received accepted event");

                //find and start the quest
                BoltEntity questEntity = BoltNetwork.FindEntity(evnt.QuestNetworkID);
                Quest quest = questEntity.GetComponent<Quest>();

                if(quest.GetQuestState() == Constants.READY)
                {
                    quest.SetQuestState(Constants.STARTED);

                    //TODO special effect

                    /*
                    //get the players in the room
                    List<BoltEntity> players = quest.GetPlayersInRoom();

                    QuestAcceptedEvent newEvnt;
                    foreach (BoltEntity e in players)
                    {
                        if (e.IsOwner)
                        {
                            Debug.Log("[QUEST] server joins quest");
                            _player.JoinQuest(quest);
                        }
                        else
                        {
                            Debug.Log("[QUEST] client " + e.NetworkId + " partecipate. Sending event");
                            newEvnt = QuestAcceptedEvent.Create(e.Source);
                            newEvnt.Quest = questEntity;
                            newEvnt.PartecipantNetworkID = e.NetworkId;
                            newEvnt.Send();
                        }
                    }
                    */
                }
            }

            /*
            else
            {
                Debug.Log("[QUEST] client recieved accepted event");
                if (_player.entity.NetworkId.Equals(evnt.PartecipantNetworkID))
                {
                    Debug.Log("[QUEST] client joins quest " + evnt.Quest.NetworkId);
                    _player.JoinQuest(evnt.Quest.GetComponent<Quest>());
                }
            }
            */
        }

        public override void OnEvent(QuestAbandonedEvent evnt)
        {
            Debug.Log("[QUEST] abandoned event received");

            if (BoltNetwork.IsServer)
            {
                Debug.Log("[QUEST] server received abandoned event. abandoning player: " + evnt.AbandoningPlayerNetworkID);

                BoltEntity questEntity = BoltNetwork.FindEntity(evnt.QuestNetworkID);
                Quest quest = questEntity.GetComponent<Quest>();

                BoltEntity abandoningPlayer = BoltNetwork.FindEntity(evnt.AbandoningPlayerNetworkID);

                if(quest.GetQuestState() == Constants.STARTED)
                {
                    quest.RemovePlayer(abandoningPlayer);

                    if (quest.GetPlayers().Count == 0)
                    {
                        Debug.Log("[QUEST] no more partecipating players. quest " + quest._title + " failed");

                        //timer and visual effect that indicates that the quest is failing???

                        quest.SetQuestState(Constants.FAILED);
                    }
                }
            }
        }

        public override void OnEvent(QuestJoinedEvent evnt)
        {
            Debug.Log("[QUEST] joined event received");

            if (BoltNetwork.IsServer)
            {
                Debug.Log("[QUEST] server received joined event. joining player: " + evnt.JoiningPlayerNetworkID);

                BoltEntity questEntity = BoltNetwork.FindEntity(evnt.QuestNetworkID);
                Quest quest = questEntity.GetComponent<Quest>();

                BoltEntity joiningPlayer = BoltNetwork.FindEntity(evnt.JoiningPlayerNetworkID);

                if (quest.GetQuestState() == Constants.STARTED)
                {
                    quest.AddPlayer(joiningPlayer);
                }
            }
        }

        #endregion

        #endregion

        #region setter

        #endregion

    }
}
