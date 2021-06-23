using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using VivoxUnity;

namespace ThePackt
{
    public class VoiceManager : MonoBehaviour
    {
        #region variables
        private static VoiceManager _instance;

        [SerializeField] private string _issuer = "federi9918-th71-dev";
        [SerializeField] private string _domain = "mt1s.vivox.com";
        [SerializeField] private string _tokenKey = "cozy037";
        [SerializeField] private Uri _server = new Uri("https://mt1s.www.vivox.com/api2");
        [SerializeField] private int _audibleDistance = 4; // not audible outside this distance
        [SerializeField] private int _conversationalDistance = 1; // starts fading outside this distance
        [SerializeField] private int _fadeIntensity = 1; // normal = 1

        private CharacterSelectionData _selectedData;
        private Player _localPlayer;

        private Client _client;
        private TimeSpan _timeSpan;
        private string _channelName;
        private bool _loggedIn;
        private bool _joinChannelStarted;
        private bool _channelJoined;

        private ILoginSession _loginSession;
        private IChannelSession _channelSession;

        #endregion

        #region methods

        #region core
        public static VoiceManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = GameObject.FindGameObjectWithTag("voicemanager");
                    if (obj)
                    {
                        _instance = obj.GetComponent<VoiceManager>();
                    }
                    else
                    {
                        _instance = null;
                    }
                }

                Debug.Log("[VOICE] is null " + _instance == null);
                return _instance;
            }
        }

        private void Awake()
        {
            _loggedIn = false;
            _joinChannelStarted = false;
            _channelJoined = false;

            DontDestroyOnLoad(gameObject);

            _selectedData = CharacterSelectionData.Instance;

            _timeSpan = TimeSpan.FromSeconds(90);

            //TODO set channel name to the name of session (passed to client using protocol token)
            _channelName = _selectedData.GetSessionId();

            _client = new Client();
            _client.Uninitialize();
            _client.Initialize();

            Login();
        }

        // Update is called once per frame
        void Update()
        {
            if (_loggedIn && !_joinChannelStarted)
            {
                _joinChannelStarted = true;

                JoinChannel();
            }

            if(_localPlayer == null)
            {
                _localPlayer = _selectedData.GetPlayerScript();
            }

            if (_channelJoined && _localPlayer != null)
            {
                //Debug.Log("[VOICE] update position");
                Transform speaker = _localPlayer.transform;
                Transform listener = _localPlayer.transform;
                _channelSession.Set3DPosition(speaker.position, listener.position, listener.forward, listener.up);
            }
        }

        private void OnApplicationQuit()
        {
            _client.Uninitialize();
        }

        #endregion

        #region login

        private void Login()
        {
            AccountId acc = new AccountId(_issuer, _selectedData.GetNickname(), _domain);
            _loginSession = _client.GetLoginSession(acc);

            BindLoginCallbackListeners(true, _loginSession);
            _loginSession.BeginLogin(_server, _loginSession.GetLoginToken(_tokenKey, _timeSpan), ar =>
            {
                try
                {
                    _loginSession.EndLogin(ar);
                }
                catch (Exception e)
                {
                    BindLoginCallbackListeners(false, _loginSession);
                    Debug.Log("[Voice] " + e.Message);
                }
                // run more code here 
            });
        }

        private void BindLoginCallbackListeners(bool bind, ILoginSession loginSesh)
        {
            if (bind)
            {
                loginSesh.PropertyChanged += LoginStatus;
            }
            else
            {
                loginSesh.PropertyChanged -= LoginStatus;
            }

        }

        private void LoginStatus(object sender, PropertyChangedEventArgs loginArgs)
        {
            var source = (ILoginSession)sender;

            switch (source.State)
            {
                case LoginState.LoggingIn:
                    Debug.Log("[VOICE] Logging In");
                    break;

                case LoginState.LoggedIn:
                    Debug.Log("[VOICE] Logged In " + _loginSession.LoginSessionId.Name);
                    _loggedIn = true;
                    break;
            }
        }

        #endregion

        #region logout

        public void Logout()
        {
            Debug.Log("[VOICE] Logging Out");

            _loginSession.Logout();
            BindLoginCallbackListeners(false, _loginSession);

            GameObject.Destroy(this);
        }

        #endregion

        #region channel

        private void JoinChannel()
        {
            Channel3DProperties properties = new Channel3DProperties(_audibleDistance, _conversationalDistance, _fadeIntensity, AudioFadeModel.LinearByDistance);
            
            ChannelId channelId = new ChannelId(_issuer, _channelName, _domain, ChannelType.Positional, properties);
            _channelSession = _loginSession.GetChannelSession(channelId);

            BindChannelCallbackListeners(true, _channelSession);
            _channelSession.BeginConnect(true, false, true, _channelSession.GetConnectToken(_tokenKey, _timeSpan), ar =>
            {
                try
                {
                    _channelSession.EndConnect(ar);
                }
                catch(Exception e)
                {
                    Debug.Log("[Voice] " + e.Message);
                }
            });
        }

        public void LeaveChannel()
        {
            _channelSession.Disconnect();
            _loginSession.DeleteChannelSession(new ChannelId(_issuer, _channelName, _domain));
        }

        public void BindChannelCallbackListeners(bool bind, IChannelSession channelSesh)
        {
            if (bind)
            {
                channelSesh.PropertyChanged += OnChannelStatusChanged;
            }
            else
            {
                channelSesh.PropertyChanged -= OnChannelStatusChanged;
            }
        }

        public void OnChannelStatusChanged(object sender, PropertyChangedEventArgs channelArgs)
        {
            IChannelSession source = (IChannelSession)sender;

            switch (source.ChannelState)
            {
                case ConnectionState.Connecting:
                    Debug.Log("[VOICE] Channel Connecting");
                    break;
                case ConnectionState.Connected:
                    Debug.Log("[VOICE] Connected " + source.Channel.Name);
                    _channelJoined = true;
                    break;
                case ConnectionState.Disconnecting:
                    Debug.Log("[VOICE] Disconnecting " + source.Channel.Name);
                    break;
                case ConnectionState.Disconnected:
                    Debug.Log("[VOICE] Disconnected " + source.Channel.Name);
                    break;
            }
        }

        public void Die()
        {
            Debug.Log("[VOICE] die");
            Destroy(gameObject);
        }
        #endregion

        #region positional

        

        #endregion

        #endregion
    }
}

