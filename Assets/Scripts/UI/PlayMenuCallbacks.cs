using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using Bolt;
using Bolt.Matchmaking;
using UdpKit;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace ThePackt{
    public class PlayMenuCallbacks : GlobalEventListener
    {
        #region variables
        [SerializeField] private CharacterSelectionData _selectedData;
        [SerializeField] private GameObject _tooltip;

        [SerializeField] private EventSystem _eventSystem;
        private string _class;
        private string _nickname;
        [SerializeField] private string lobby;
        #endregion

        #region methods

        // called from host button
        public void StartServer()
        {
            if(_selectedData.GetCharacterSelected().Equals("none")){
                _tooltip.SetActive(true);
                ChangeSelectedElement(_tooltip.GetComponentInChildren<Button>().gameObject);
                return;
            }
            else{
                _class = _selectedData.GetCharacterSelected();
                _nickname = _selectedData.GetNickname();
                //_selectedData.Reset();
            }
            BoltConfig config = BoltRuntimeSettings.instance.GetConfigCopy();
            config.serverConnectionLimit = 5;

            BoltLauncher.StartServer();
        }

        public override void BoltStartDone()
        {
            
            var id = Guid.NewGuid().ToString().Split('-')[0];
            var matchName = string.Format("{0} - {1}", id, lobby);

            BoltMatchmaking.CreateSession(sessionID: matchName, sceneToLoad: lobby);

            /*
           int sessions = BoltNetwork.SessionList.Count;
           Debug.Log("NUMBER OF SESSIONS: " + sessions);
           if (sessions <= 3)
           {
               BoltMatchmaking.CreateSession(sessionID: matchName, sceneToLoad: map);
           }
           */
        }

        // called from client button
        public void StartClient()
        {
            Debug.Log("selected character = "+ _selectedData.GetCharacterSelected());
            if(_selectedData.GetCharacterSelected().Equals("none")){
                _tooltip.SetActive(true);
                ChangeSelectedElement(_tooltip.GetComponentInChildren<Button>().gameObject);
                return;
            }
            else{
                _class = _selectedData.GetCharacterSelected();
                _nickname = _selectedData.GetNickname();
                //_selectedData.Reset();
            }
            BoltLauncher.StartClient();
        }

        // called from shutdown button
        public void Shutdown()
        {
            BoltLauncher.Shutdown();
        }

        public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
        {
            // look through all photon sessions and join one using bolt matchmaking
            foreach (var session in sessionList)
            {
                UdpSession photonSession = session.Value as UdpSession;

                if(photonSession.Source == UdpSessionSource.Photon)
                {
                    BoltMatchmaking.JoinSession(photonSession);
                }
            }
        }


        public void ChangeSelectedElement(GameObject go){
            _eventSystem.SetSelectedGameObject(go);
        }
        #endregion
    }
}
