using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class MapGenerator : MonoBehaviour
    {
        #region variables 
        [SerializeField] private List<Vector2> _roomsSpawnPoints;
        [SerializeField] private List<Utils.PrefabIntegerAssociation> _roomPrefabs;
        [SerializeField] private GameObject _mainRoomPrefab;
        [SerializeField] private List<Vector2> _magicWallsSpawnPoints;
        [SerializeField] private List<Vector2> _magicFloorSpawnPoints;
        [SerializeField] private GameObject _magicWallPrefab;
        [SerializeField] private GameObject _magicFloorPrefab;
        [SerializeField] private GameObject _fountainPrefab;
        [SerializeField] private List<Vector2> _fountainsSpawnPoints;

        [SerializeField] private Vector2 _adjacentRoomsRange; //vector indicating vertical and horizontal distances from adjacents
        [SerializeField] private int _adjacentRoomsRangeMultiplier; //integer multiplier to increase considered neighboorhood


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
            foreach (Vector2 sp in _magicFloorSpawnPoints)
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
            Vector2 mainRoomPos = _roomsSpawnPoints[randomIndex];
            BoltNetwork.Instantiate(_mainRoomPrefab, mainRoomPos, _mainRoomPrefab.transform.rotation);
            _roomsSpawnPoints.RemoveAt(randomIndex);
            nPoints--;

            Debug.Log("[MAPGEN] main room " + mainRoomPos);

            //find farest position from main room
            float maxDist = 0f;
            int farestPointIndex = 0; 
            for(int i = 0; i < nPoints; i++)
            {
                float dist = Vector2.Distance(_roomsSpawnPoints[i], mainRoomPos);
                if (dist > maxDist){
                    farestPointIndex = i;
                    maxDist = dist;
                }
            }
            Debug.Log("[MAPGEN] farest point " + _roomsSpawnPoints[farestPointIndex]);

            //find max level of difficulty
            int maxLevel = 0;
            Rect r = new Rect(mainRoomPos, 2f * _adjacentRoomsRange);
            r.center = mainRoomPos;
            bool found = false;
            while (!found)
            {
                maxLevel++;
                if (r.Contains(_roomsSpawnPoints[farestPointIndex]))
                {
                    found = true;
                }
                else
                {
                    r.size += 2f * _adjacentRoomsRange;
                    r.center = mainRoomPos;
                }
            }
            Debug.Log("[MAPGEN] max level " + maxLevel);

            //initialize list of counters for the types of the adjacent rooms
            GameObject toSpawn;
            List<int> counts = new List<int>();
            for (int i = 0; i < nRooms; i++)
            {
                counts.Add(0);
            }

            for (int i = 0; i < nPoints; i++)
            {
                Debug.Log("[MAPGEN] room " + _roomsSpawnPoints[i]);

                //determine level of difficulty based on the distance from the main room (neighbours = highest level, neighbours of neighbours = highest level - 1...)
                r = new Rect(mainRoomPos, 2f * _adjacentRoomsRange);
                r.center = mainRoomPos;
                found = false;
                int level = 0;
                while (!found)
                {
                    level++;
                    if (r.Contains(_roomsSpawnPoints[i]))
                    {
                        found = true;
                    }
                    else
                    {
                        r.size += 2f * _adjacentRoomsRange;
                        r.center = mainRoomPos;
                    }
                }
                Debug.Log("[MAPGEN] inverted level " + level);
                level = maxLevel + 1 - level;

                Debug.Log("[MAPGEN] level " + level);
                
                //get adjacent rooms
                Collider2D[] cols = Physics2D.OverlapBoxAll(_roomsSpawnPoints[i], 2f * _adjacentRoomsRange * _adjacentRoomsRangeMultiplier, 
                    0f, LayerMask.GetMask("Room"));
                Debug.Log("[MAPGEN] cols " + cols.Length);

                if(cols.Length > 0)
                {
                    //increment counters based on the types of the adjacent rooms
                    foreach (Collider2D col in cols)
                    {
                        Quest q = col.gameObject.GetComponent<Quest>();

                        if (q)
                        {
                            counts[q.GetQuestType()]++;
                        }
                    }

                    //find the min
                    int min = 100;
                    for (int j = 0; j < nRooms; j++)
                    {
                        if (counts[j] < min)
                            min = counts[j];
                    }
                    //Debug.Log("[MAPGEN] min " + min);

                    //find room types whose number among the adjacent is min 
                    List<int> fewerRooms = new List<int>();
                    for (int j = 0; j < nRooms; j++)
                    {
                        if (counts[j] == min)
                            fewerRooms.Add(j);

                        //Debug.Log("[MAPGEN] " + j + " - " + counts[j]);
                    }

                    var s = "fewer rooms: ";
                    for (int j = 0; j < fewerRooms.Count; j++)
                    {
                        s += fewerRooms[j];
                    }
                    //Debug.Log("[MAPGEN] " + s);

                    //pick a room type from the least present ones (if there is more than one)
                    randomIndex = Random.Range(0, fewerRooms.Count);

                    toSpawn = null;
                    foreach (Utils.PrefabIntegerAssociation assoc in _roomPrefabs)
                    {
                        if (assoc.num == fewerRooms[randomIndex])
                        {
                            toSpawn = assoc.prefab;
                            break;
                        }
                    }

                    for (int j = 0; j < nRooms; j++)
                    {
                        counts[j] = 0;
                    }
                }
                else
                {
                    //if a room has no adjacent rooms pick a completely random type
                    randomIndex = Random.Range(0, nRooms);
                    toSpawn = _roomPrefabs[randomIndex].prefab;
                }

                //spawn the picked type
                BoltEntity spawnedRoom = BoltNetwork.Instantiate(toSpawn, _roomsSpawnPoints[i], toSpawn.transform.rotation);
                spawnedRoom.GetComponent<Quest>().SetDifficultyLevel(level);
                spawnedRoom.GetComponent<Quest>().SetMaxDifficultyLevel(maxLevel);
            }
        }

        public void DestroyMagicObstacles()
        {
            foreach (var obs in _magicObstacles)
            {
                BoltNetwork.Destroy(obs);
            }
        }
        #endregion


    }
}

