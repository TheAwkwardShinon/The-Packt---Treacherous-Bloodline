using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region variables 
    [SerializeField] private List<Vector2> _roomsSpawnPoints;
    [SerializeField] private List<GameObject> _roomPrefabs;
    [SerializeField] private GameObject _mainRoomPrefab;
    [SerializeField] private List<Vector2> _magicWallsSpawnPoints;
    [SerializeField] private List<Vector2> _magicFloorSpawnPoints;
    [SerializeField] private GameObject _magicWallPrefab;
    [SerializeField] private GameObject _magicFloorPrefab;
    #endregion

    #region methods 
    // Start is called before the first frame update
    void Start()
    {
        if (BoltNetwork.IsServer)
        {
            SpawnMagicObstacles();

            SpawnRooms();
        }

    }

    private void SpawnMagicObstacles()
    {
        foreach(Vector2 sp in _magicFloorSpawnPoints)
        {
            BoltNetwork.Instantiate(_magicFloorPrefab, sp, _magicFloorPrefab.transform.rotation);
        }

        foreach (Vector2 sp in _magicWallsSpawnPoints)
        {
            BoltNetwork.Instantiate(_magicWallPrefab, sp, _magicWallPrefab.transform.rotation);
        }
    }

    private void SpawnRooms()
    {
        int nPoints = _roomsSpawnPoints.Count;
        int nRooms = _roomPrefabs.Count;

        int randomIndex = Random.Range(0, nPoints);
        BoltNetwork.Instantiate(_mainRoomPrefab, _roomsSpawnPoints[randomIndex], _mainRoomPrefab.transform.rotation);
        _roomsSpawnPoints.RemoveAt(randomIndex);
        nPoints--;

        GameObject toSpawn;
        for (int i = 0; i < nPoints; i++)
        {
            randomIndex = Random.Range(0, nRooms);
            toSpawn = _roomPrefabs[randomIndex];

            BoltNetwork.Instantiate(toSpawn, _roomsSpawnPoints[i], toSpawn.transform.rotation);
        }
    }
    #endregion

    
}
