using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThePackt {
    public class UICallbacks : MonoBehaviour
    {
        public GameObject p;
        public void Disconnect()
        {
            //BoltNetwork.Instantiate(p, Vector3.zero, Quaternion.identity);

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
        }
    }
}
