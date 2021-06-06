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
    [SerializeField] private GameObject _fountainPrefab;
    [SerializeField] private List<Vector2> _fountainsSpawnPoints;

    private List<BoltEntity> _magicObstacles;

    private static MapGenerator _instance;

    public static MapGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MapGenerator>();
            }

            return _instance;
        }
    }
    #endregion

    #region methods 
    // Start is called before the first frame update
    void Start()
    {
        _magicObstacles = new List<BoltEntity>();

        if (BoltNetwork.IsServer)
        {
            SpawnMagicObstacles();

            SpawnFountains();

            SpawnRooms();
        }

    }

    private void SpawnFountains()
    {
        foreach (Vector2 sp in _fountainsSpawnPoints)
        {
            BoltNetwork.Instantiate(_fountainPrefab, sp, _fountainPrefab.transform.rotation);
        }
    }

    private void SpawnMagicObstacles()
    {
        foreach(Vector2 sp in _magicFloorSpawnPoints)
        {
            _magicObstacles.Add(BoltNetwork.Instantiate(_magicFloorPrefab, sp, _magicFloorPrefab.transform.rotation));
        }

        foreach (Vector2 sp in _magicWallsSpawnPoints)
        {
            _magicObstacles.Add(BoltNetwork.Instantiate(_magicWallPrefab, sp, _magicWallPrefab.transform.rotation));
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

    public void DestroyMagicObstacles()
    {
        foreach(var obs in _magicObstacles)
        {
            BoltNetwork.Destroy(obs);
        }
    }
    #endregion

    
}
