using System;
using UnityEngine;

namespace ThePackt
{
    public class Utils
    {
        #region methods

        public static BoltConnection GetServerConnection()
        {
            if (BoltNetwork.IsClient)
            {
                //my Id
                return BoltNetwork.Server;
            }
            else
            {
                //Id of first client
                foreach (BoltConnection client in BoltNetwork.Clients)
                {
                    return client;
                }
            }

            return null;
        }

        public static bool IsServerConnection(int id)
        {
            foreach (BoltConnection client in BoltNetwork.Clients)
            {
                Debug.Log("[NETWORKLOG] client id " + client.ConnectionId);
                if (client.ConnectionId == id)
                {
                    Debug.Log("[NETWORKLOG] not server ");
                    return false;
                }
            }

            return true;
        }

        public static BoltConnection FindConnection(int id)
        {
            foreach (BoltConnection client in BoltNetwork.Clients)
            {
                if (client.ConnectionId == id)
                {
                    return client;
                }
            }

            return null;
        }
        #endregion

        #region inner classes
        [Serializable]
        public class PrefabAssociation
        {
            public string name;
            public GameObject prefab;
        }

        [Serializable]
        public class VectorAssociation
        {
            public string name;
            public Vector3 position;
        }
        #endregion
    }
}