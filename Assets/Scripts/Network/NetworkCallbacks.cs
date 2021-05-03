using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using UnityEngine.SceneManagement;

namespace ThePackt
{
    public class NetworkCallbacks : GlobalEventListener
    {
        public override void Disconnected(BoltConnection connection)
        {
            if (BoltNetwork.IsClient)
            {
                SceneManager.LoadScene(Constants.MENU);
            }
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
    }
}
