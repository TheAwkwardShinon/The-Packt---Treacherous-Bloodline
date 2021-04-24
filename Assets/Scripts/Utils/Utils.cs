using System.Collections;
using UnityEngine;

namespace ThePackt
{
    public class Utils
    {
        public static uint GetServerID()
        {
            if (BoltNetwork.IsClient)
            {
                //my Id
                return BoltNetwork.Server.ConnectionId;
            }
            else
            {
                //Id of first client
                foreach (BoltConnection client in BoltNetwork.Clients)
                    return client.ConnectionId;
            }

            return 1000;
        }

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

        public static BoltConnection findConnection(int id)
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
    }
}