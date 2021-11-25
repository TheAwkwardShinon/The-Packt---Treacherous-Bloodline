using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using UnityEngine.SceneManagement;
using UdpKit;

namespace ThePackt
{
    public class NetworkCallbacks : GlobalEventListener
    {
        protected CharacterSelectionData _selectedData;

        private void Awake()
        {
            _selectedData = CharacterSelectionData.Instance;
        }

        public override void OnEvent(SetColliderSizeEvent evnt)
        {
            Debug.Log("[SIZE] setting collider size");

            BoltEntity entity = BoltNetwork.FindEntity(evnt.TargetPlayerNetworkID);
            Player player = entity.GetComponent<Player>();
            player.GetComponent<BoxCollider2D>().size = evnt.Size;
            player.GetComponent<BoxCollider2D>().offset = evnt.Offset;

        }

        public override void OnEvent(DisconnectEvent evnt)
        {
            //if received by the server disconnect the sender
            if (BoltNetwork.IsServer)
            {
                Debug.Log("[NETWORKLOG] server received disconnect event");

                evnt.RaisedBy.Disconnect();
            }
        }

        public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
        {
            Debug.Log("[NETWORKLOG] Shutdown begin");

            VoiceManager voiceManager = VoiceManager.Instance;

            if(voiceManager != null)
            {
                voiceManager.Logout();
            }

            registerDoneCallback(() =>
            {
                // Will show disconnect reason.
                Debug.LogFormat("[NETWORKLOG] Shutdown Done with Reason: {0}", disconnectReason);

                // Show the current connectivity of the peer
                ConnectivityCheck();

                _selectedData.SetDisconnectReason(disconnectReason.ToString());

                if(SceneManager.GetActiveScene().name != Constants.MENU)
                {
                    SceneManager.LoadScene(Constants.MENU);
                }
            });
        }

        protected void ConnectivityCheck()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("[NETWORKLOG] NotReachable");
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                Debug.Log("[NETWORKLOG] ReachableViaCarrierDataNetwork");
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                Debug.Log("[NETWORKLOG] ReachableViaLocalAreaNetwork");
            }
        }

        public override void BoltStartFailed(UdpConnectionDisconnectReason disconnectReason)
        {
            Debug.LogFormat("[NETWORKLOG] Start Failed with Reason: {0}", disconnectReason);

            //TODO advice the player to check his connection
        }
    }
}
