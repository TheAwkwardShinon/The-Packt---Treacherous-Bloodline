using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using Bolt;
using Bolt.Matchmaking;
using UdpKit;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ThePackt{
    public class PlayMenuCallbacks : NetworkCallbacks
    {
        #region variables
        [SerializeField] private GameObject _tooltip;

        #region searching tooltip
        [SerializeField] private GameObject _searchingTooltip;
        [SerializeField] private Text _searchingTooltipErrorText;
        [SerializeField] private Text _searchingTooltipFindText;

        [SerializeField] private Button _searchingTooltipCancelButton;
        [SerializeField] private Button _searchingTooltipGotItTButton;
        private string _searchingTooltipErrorMessage;

        #endregion

        [SerializeField] private EventSystem _eventSystem;
        [SerializeField] private LogoOnCharSelection _logoHandler;
        [SerializeField] private int _maxConnectionTentatives;
        [SerializeField] private uint _sessionSearchTimeout; //in seconds
        private string _class;
        private string _nickname;
        private Map<Guid, UdpSession> _sessionList;
        private HashSet<Guid> _triedSessions;
        private int _connectionTentatives;
        private bool _firstConnection;
        private uint _timeFromLastSessionUpdate; //in seconds

        private bool _tryingToConnect;
        private bool _canceled;

        
        #endregion

        #region methods

        // called from host button
        public void StartServer()
        {
            if(_selectedData.GetCharacterSelected().Equals("none") || !_logoHandler.isSomethingActive()){
                _tooltip.SetActive(true);
                ChangeSelectedElement(_tooltip.GetComponentInChildren<Button>().gameObject);
                return;
            }
            else{
                _class = _selectedData.GetCharacterSelected();
                _nickname = _selectedData.GetNickname();
                //_selectedData.Reset();
            }

            BoltLauncher.StartServer();
        }

        public override void BoltStartDone()
        {
            
            var id = Guid.NewGuid().ToString().Split('-')[0];
            var matchName = string.Format("{0} - {1}", id, Constants.LOBBY);

            BoltMatchmaking.CreateSession(sessionID: matchName, sceneToLoad: Constants.LOBBY);

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
            if(_selectedData.GetCharacterSelected().Equals("none") || !_logoHandler.isSomethingActive()){
                _tooltip.SetActive(true);
                ChangeSelectedElement(_tooltip.GetComponentInChildren<Button>().gameObject);
                return;
            }
            else{
                _class = _selectedData.GetCharacterSelected();
                _nickname = _selectedData.GetNickname();
                
            }
            //Debug.Log("sessions : "+)
            _searchingTooltipFindText.text = "Looking For A Game...";
            _searchingTooltip.SetActive(true);
             ChangeSelectedElement(_searchingTooltipCancelButton.gameObject);

            _canceled = false;
            _tryingToConnect = false;
            _triedSessions = new HashSet<Guid>();
            _connectionTentatives = 0;
            _timeFromLastSessionUpdate = 0;
            _firstConnection = true;
            BoltLauncher.StartClient();
            StartCoroutine("WaitForTimeout");
        }

        // called from shutdown button
        public void Shutdown()
        {
            try{
                BoltLauncher.Shutdown();
            }catch(BoltException e){
                Debug.Log("tried to shutdown but: "+e);
                Debug.Log("-> i assume you clicked \"cancel\" before clicking \"join\" or \"host\" so... you want to deselect the character right?");
            }
            _logoHandler.Reset();
            _selectedData.Reset();

            _canceled = true;
        }

        public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
        {
            Debug.Log("[CONNECTIONLOG] session list updated: " + sessionList.Count);

            _timeFromLastSessionUpdate = 0;
            _sessionList = sessionList;

            // if this is the first connection try look through all photon sessions and join one using bolt matchmaking
            if(_firstConnection)
            {
                _tryingToConnect = true;
                SearchAndJoinSession();
                _firstConnection = false;
            }
            else if(!_tryingToConnect)
            {
                _tryingToConnect = true;
                SearchAndJoinSessionRetry();
            }
        }

        public override void ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
        {
            Debug.Log("[CONNECTIONLOG] connection refused");
            Debug.Log("[CONNECTIONLOG] retrying after connection refused");

            _connectionTentatives++;
            SearchAndJoinSessionRetry();
        }

        public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
        {
            Debug.Log("[CONNECTIONLOG] connection failed");
            Debug.Log("[CONNECTIONLOG] retrying after connection failed");

            _connectionTentatives++;
            SearchAndJoinSessionRetry();
        }

        private void SearchAndJoinSession()
        {
            if(_sessionList.Count > 0)
            {
                foreach (var session in _sessionList)
                {
                    UdpSession photonSession = session.Value as UdpSession;

                    if (photonSession.Source == UdpSessionSource.Photon && !_triedSessions.Contains(session.Key))
                    {
                        _searchingTooltipFindText.text = "Game Found...";
                        BoltMatchmaking.JoinSession(photonSession, null);
                        _triedSessions.Add(session.Key);
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("[CONNECTIONLOG] no sessions");
                _searchingTooltipErrorMessage ="There aren't sessions, please host a new one";
                _searchingTooltipFindText.gameObject.SetActive(false);
                _searchingTooltipCancelButton.gameObject.SetActive(false);
                _searchingTooltipGotItTButton.gameObject.SetActive(true);
                ChangeSelectedElement(_searchingTooltipGotItTButton.gameObject);

                _searchingTooltipErrorText.text = _searchingTooltipErrorMessage;
                _searchingTooltipErrorText.gameObject.SetActive(true);
                //TODO advice the user to create his own session
            }
        }

        private void SearchAndJoinSessionRetry()
        {
            if (_sessionList.Count > 0)
            {
                if (_connectionTentatives >= _maxConnectionTentatives)
                {
                    Debug.Log("[CONNECTIONLOG] max tentatives reached");
                    _searchingTooltipErrorMessage ="max tentatives reached, please host a new game";
                    _searchingTooltipFindText.gameObject.SetActive(false);
                    _searchingTooltipCancelButton.gameObject.SetActive(false);
                    _searchingTooltipGotItTButton.gameObject.SetActive(true);
                    ChangeSelectedElement(_searchingTooltipGotItTButton.gameObject);
                    _searchingTooltipErrorText.text = _searchingTooltipErrorMessage;
                    _searchingTooltipErrorText.gameObject.SetActive(true);
                    return;
                    //TODO advice the user to create his own session
                }

                //get the ids of all the sessions
                HashSet<Guid> toTrySessions = new HashSet<Guid>();
                foreach (var session in _sessionList)
                {
                    toTrySessions.Add(session.Key);
                }

                //find the sessions that have not already been tried
                toTrySessions.ExceptWith(_triedSessions);
                Debug.Log("[CONNECTIONLOG] tried: " + _triedSessions.Count);
                Debug.Log("[CONNECTIONLOG] to try: " + toTrySessions.Count);
                if (toTrySessions.Count == 0)
                {
                    Debug.Log("[CONNECTIONLOG] all sessions already tried");
                    _tryingToConnect = false;
                    _searchingTooltipErrorMessage ="all active sessions tried withut success, please host a new one";
                    _searchingTooltipFindText.gameObject.SetActive(false);
                    _searchingTooltipCancelButton.gameObject.SetActive(false);
                    _searchingTooltipGotItTButton.gameObject.SetActive(true);
                    ChangeSelectedElement(_searchingTooltipGotItTButton.gameObject);
                    _searchingTooltipErrorText.text = _searchingTooltipErrorMessage;
                    _searchingTooltipErrorText.gameObject.SetActive(true);
                    return;
               
                    //TODO advice the user to create his own session
                }
                else
                {
                    //if not all sessions have already tried, try one of the remaining ones
                    foreach (Guid sessionGuid in toTrySessions)
                    {
                        UdpSession photonSession = _sessionList.Find(sessionGuid) as UdpSession;
                      
                        if (photonSession.Source == UdpSessionSource.Photon)
                        {
                            BoltMatchmaking.JoinSession(photonSession, null);
                            _triedSessions.Add(sessionGuid);
                            break;
                        }
                    }
                }
            }
            else
            {
                Debug.Log("[CONNECTIONLOG] no sessions");
                _tryingToConnect = false;
                _searchingTooltipErrorMessage ="There aren't sessions, please host a new one";
                _searchingTooltipFindText.gameObject.SetActive(false);
                _searchingTooltipCancelButton.gameObject.SetActive(false);
                _searchingTooltipGotItTButton.gameObject.SetActive(true);
                ChangeSelectedElement(_searchingTooltipGotItTButton.gameObject);
                _searchingTooltipErrorText.text = _searchingTooltipErrorMessage;
                _searchingTooltipErrorText.gameObject.SetActive(true);
                return;
                //TODO advice the user to create his own session
            }
        }

        IEnumerator WaitForTimeout()
        {
            yield return new WaitForSeconds(1);
            _timeFromLastSessionUpdate++;

            if(_timeFromLastSessionUpdate == _sessionSearchTimeout)
            {
                Debug.Log("[CONNECTIONLOG] session search timeout");

                //TODO advice the user to create his own session

                _timeFromLastSessionUpdate = 0;

                if (_sessionList == null)
                {
                    Debug.Log("[CONNECTIONLOG] no sessions");
                    _searchingTooltipErrorMessage ="There aren't sessions, please host a new one";
                    _searchingTooltipFindText.gameObject.SetActive(false);
                    _searchingTooltipCancelButton.gameObject.SetActive(false);
                    _searchingTooltipGotItTButton.gameObject.SetActive(true);
                    ChangeSelectedElement(_searchingTooltipGotItTButton.gameObject);
                    _searchingTooltipErrorText.text = _searchingTooltipErrorMessage;
                    _searchingTooltipErrorText.gameObject.SetActive(true);
                }
                else if (_tryingToConnect)
                {
                    Debug.Log("[CONNECTIONLOG] still trying to connect");
                }
                else
                {
                    Debug.Log("[CONNECTIONLOG] all sessions already tried");
                    _searchingTooltipErrorMessage ="all active sessions tried withut success, please host a new one";
                    _searchingTooltipFindText.gameObject.SetActive(false);
                    _searchingTooltipCancelButton.gameObject.SetActive(false);
                    _searchingTooltipGotItTButton.gameObject.SetActive(true);
                    ChangeSelectedElement(_searchingTooltipGotItTButton.gameObject);
                    _searchingTooltipErrorText.text = _searchingTooltipErrorMessage;
                    _searchingTooltipErrorText.gameObject.SetActive(true);
                    /*
                    Debug.Log("[CONNECTIONLOG] retrying after timeout");
                    _tryingToConnect = true;
                    SearchAndJoinSessionRetry();
                    */
                }
            }

            if (!BoltNetwork.IsConnected && !_canceled)
            {
                StartCoroutine("WaitForTimeout");
            }
        }


        public void ChangeSelectedElement(GameObject go){
            _eventSystem.SetSelectedGameObject(go);
        }
        #endregion
    }
}
