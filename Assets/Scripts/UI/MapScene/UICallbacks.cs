using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThePackt {
    public class UICallbacks : MonoBehaviour
    {
        [SerializeField] GameObject _networkManager;

        public void Disconnect()
        {
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

        public void Respawn()
        {
            CharacterSelectionData selectedData = CharacterSelectionData.Instance;

            foreach (Utils.VectorAssociation assoc in _networkManager.GetComponent<MapNetworkCallbacks>().GetPlayerSpawnPositions())
            {
                if (assoc.name == selectedData.GetCharacterSelected())
                {
                    selectedData.GetPlayerScript().gameObject.transform.position = assoc.position;
                }
            }
        }
    }
}
