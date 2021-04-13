using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bolt;
using Bolt.Matchmaking;
using UdpKit;

public class Menu : GlobalEventListener
{
    // called from host button
    public void StartServer()
    {
        BoltLauncher.StartServer();
    }

    public override void BoltStartDone()
    {
        base.BoltStartDone();

        // creates a session startin with the specified scene
        BoltMatchmaking.CreateSession(sessionID: "test", sceneToLoad: "NetworkTestScene");
    }

    // called from client button
    public void StartClient()
    {

    }

}
