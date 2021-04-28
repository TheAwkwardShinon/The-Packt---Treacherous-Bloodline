using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bolt;
using Bolt.Matchmaking;
using UdpKit;

public class Menu : GlobalEventListener
{
    [SerializeField] private string map;

    // called from host button
    public void StartServer()
    {
        BoltConfig config = BoltRuntimeSettings.instance.GetConfigCopy();
        config.serverConnectionLimit = 5;

        BoltLauncher.StartServer();
    }

    public override void BoltStartDone()
    {
        var id = Guid.NewGuid().ToString().Split('-')[0];
        var matchName = string.Format("{0} - {1}", id, map);

        BoltMatchmaking.CreateSession(sessionID: matchName, sceneToLoad: map);

        /*
        int sessions = BoltNetwork.SessionList.Count;
        Debug.Log("NUMBER OF SESSIONS: " + sessions);
        if (sessions <= 3)
        {
            BoltMatchmaking.CreateSession(sessionID: matchName, sceneToLoad: map);
        }
        */
    }

    // called from client button
    public void StartClient()
    {
        BoltLauncher.StartClient();
    }

    // called from shutsown button
    public void Shutdown()
    {
        BoltLauncher.Shutdown();
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        // look through all photon sessions and join one using bolt matchmaking
        foreach (var session in sessionList)
        {
            UdpSession photonSession = session.Value as UdpSession;

            if(photonSession.Source == UdpSessionSource.Photon)
            {
                BoltMatchmaking.JoinSession(photonSession);
            }
        }
    }
}
