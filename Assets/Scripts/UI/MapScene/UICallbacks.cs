using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThePackt {
    public class UICallbacks : MonoBehaviour
    {
        public GameObject timer;

        public void Disconnect()
        {
            Debug.Log("caia 1" + BoltNetwork.IsServer);
            if (BoltNetwork.IsServer)
            {
                Debug.Log("caia 2" + timer.GetComponent<TimerManager>() == null);
                timer.GetComponent<TimerManager>().subTime(30);
            }

            /*
            if (BoltNetwork.IsServer)
            {
                Debug.Log("[NETWORKLOG] server disconnecting");

                BoltNetwork.Shutdown();

                SceneManager.LoadScene("MenuScene");
            }
            else
            {
                Debug.Log("[NETWORKLOG] client disconnecting");

                var evnt = DisconnectEvent.Create(Utils.GetServerConnection());
                evnt.Send();

                SceneManager.LoadScene("MenuScene");
            }
            */
        }
    }
}
