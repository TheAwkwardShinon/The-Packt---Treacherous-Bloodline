using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class NetworkCallbacks : GlobalEventListener
{
    public GameObject playerPrefab;
    public Vector2 spawnPos;

    public override void SceneLoadLocalDone(string scene, Bolt.IProtocolToken token)
    {
        // when the scene is loaded instantiate the player in the given position
        BoltNetwork.Instantiate(playerPrefab, spawnPos, Quaternion.identity);
    }
}
