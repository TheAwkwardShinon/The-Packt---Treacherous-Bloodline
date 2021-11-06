//using agora_gaming_rtc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThePackt
{
    public class AgoraVoiceManager : MonoBehaviour
    {
        public string agoraAppID;
        public string channelName;
        //private IRtcEngine _mRtcEngine = null;

        /*
        // Start is called before the first frame update
        void Start()
        {

            Debug.Log("[VOICE] engine creating");
            _mRtcEngine = IRtcEngine.GetEngine(agoraAppID);
            _mRtcEngine.SetLogFile("agora_log.txt");

            _mRtcEngine.EnableSoundPositionIndication(true);

            _mRtcEngine.OnJoinChannelSuccess += (string channelName, uint uid, int elapsed) => {
                string joinSuccessMessage = string.Format("[VOICE] joinChannel callback uid: {0}, channel: {1}, version: {2}", uid, channelName, IRtcEngine.GetSdkVersion());
                Debug.Log(joinSuccessMessage);
            };

            _mRtcEngine.OnLeaveChannel += (RtcStats stats) => {
                string leaveChannelMessage = string.Format("[VOICE] onLeaveChannel callback duration {0}, tx: {1}, rx: {2}, tx kbps: {3}, rx kbps: {4}", stats.duration, stats.txBytes, stats.rxBytes, stats.txKBitRate, stats.rxKBitRate);
                Debug.Log(leaveChannelMessage);
            };

            _mRtcEngine.OnUserJoined += (uint uid, int elapsed) => {
                string userJoinedMessage = string.Format("[VOICE] onUserJoined callback uid {0} {1}", uid, elapsed);
                Debug.Log(userJoinedMessage);
            };

            _mRtcEngine.JoinChannel(channelName, "", 0);
            _mRtcEngine.LeaveChannel();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnApplicationQuit()
        {
            Debug.Log("[VOICE] OnApplicationQuit");
            if (_mRtcEngine != null)
            {
                Debug.Log("[VOICE] destroying");
                IRtcEngine.Destroy();
            }
        }
        */
    }
}

