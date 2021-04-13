using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class NetworkCallbacks : GlobalEventListener
{
    public GameObject playerPrefab;

    public override void SceneLoadLocalDone(string scene)
    {
        // when the scene is loaded instantiate the player in the given position
        Vector2 spawnPos = new Vector2(0, 0);
        BoltNetwork.Instantiate(playerPrefab, spawnPos, Quaternion.identity);
    }
}
