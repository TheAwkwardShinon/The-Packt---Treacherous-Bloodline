using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region variables 
    [SerializeField] private List<Vector2> _spawnPoints;
    [SerializeField] private List<GameObject> _roomPrefabs;
    [SerializeField] private GameObject _mainRoomPrefab;
    [SerializeField] private GameObject _testRoomPrefab;
    [SerializeField] private GameObject _mainTestRoomPrefab;
    #endregion

    #region methods 
    // Start is called before the first frame update
    void Start()
    {
        if (BoltNetwork.IsServer)
        {
            int nPoints = _spawnPoints.Count;
            int nRooms = _roomPrefabs.Count;

            int randomIndex = Random.Range(0, nPoints);
            BoltNetwork.Instantiate(_mainRoomPrefab, _spawnPoints[randomIndex], _mainRoomPrefab.transform.rotation);
            _spawnPoints.RemoveAt(randomIndex);
            nPoints--;

            GameObject toSpawn;
            for (int i = 0; i < nPoints; i++)
            {
                randomIndex = Random.Range(0, nRooms);
                toSpawn = _roomPrefabs[randomIndex];

                BoltNetwork.Instantiate(toSpawn, _spawnPoints[i], toSpawn.transform.rotation);
            }

            //BoltNetwork.Instantiate(_testRoomPrefab, _testRoomPrefab.transform.position, _testRoomPrefab.transform.rotation);
            //BoltNetwork.Instantiate(_mainTestRoomPrefab, _mainTestRoomPrefab.transform.position, _mainTestRoomPrefab.transform.rotation);
        }

    }
    #endregion

    
}
