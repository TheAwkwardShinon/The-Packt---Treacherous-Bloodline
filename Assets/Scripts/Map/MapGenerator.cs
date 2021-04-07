using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region variables 
    [SerializeField] private List<Vector2> _spawnPoints;
    [SerializeField] private List<GameObject> _roomPrefabs;
    #endregion

    #region methods 
    // Start is called before the first frame update
    void Start()
    {
        int nPoints = _spawnPoints.Count;
        int nRooms = _roomPrefabs.Count;

        int randomIndex = 0;
        GameObject toSpawn;
        for (int i = 0; i < nPoints; i++)
        {
            randomIndex = Random.Range(0, nRooms - 1);
            toSpawn = _roomPrefabs[randomIndex];
            _roomPrefabs.RemoveAt(randomIndex);
            nRooms--;

            // Debug.Log(randomIndex);

            Instantiate(toSpawn, _spawnPoints[i], toSpawn.transform.rotation);
        }
    }
    #endregion

    
}
