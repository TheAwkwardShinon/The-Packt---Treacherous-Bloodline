using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThePackt {
    public class UICallbacks : MonoBehaviour
    {
        public BoltEntity timer;

        public void Disconnect()
        {
            /*
            if (BoltNetwork.IsServer)
            {
                foreach(BoltEntity e in BoltNetwork.Entities)
                {
                    if(e.GetComponent<TimerManager>() != null)
                    {
                        timer = e;
                        timer.GetComponent<TimerManager>().addTime(30);
                    }
                }
            }*/

            
            if (BoltNetwork.IsServer)
            {
                Debug.Log("[NETWORKLOG] server disconnecting");

                BoltNetwork.Shutdown();
            }
            else
            {
                Debug.Log("[NETWORKLOG] client disconnecting");

                var evnt = DisconnectEvent.Create(BoltNetwork.Server);
                evnt.Send();
            }
            
        }
    }
}
