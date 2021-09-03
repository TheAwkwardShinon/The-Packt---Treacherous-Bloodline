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

        [SerializeField] private Vector2 _adjacentRoomsRange; //vector indicating vertical and horizontal distances from adjacents room
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

        ///<summary>
        ///spawns _fountainPrefab at each spawn point in _fountainsSpawnPoints
        ///</summary>
        private void SpawnFountains()
        {
            foreach (Vector2 sp in _fountainsSpawnPoints)
            {
                BoltNetwork.Instantiate(_fountainPrefab, sp, _fountainPrefab.transform.rotation);
            }
        }

        ///<summary>
        ///spawns _magicFloorPrefab at each spawn point in _magicFloorSpawnPoints and _magicWallPrefab at each spawn 
        ///point in _magicWallsSpawnPoints
        ///</summary>
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

        ///<summary>
        ///destroy all spawned magic obstacles
        ///</summary>
        public void DestroyMagicObstacles()
        {
            foreach (var obs in _magicObstacles)
            {
                BoltNetwork.Destroy(obs);
            }
        }

        ///<summary>
        ///spawns the rooms in _roomPrefabs at each spawn point in _roomsSpawnPoints so that there is a homogeneous distribution
        ///of the different types of rooms
        ///</summary>
        private void SpawnRooms()
        {
            int nPoints = _roomsSpawnPoints.Count;
            int nRooms = _roomPrefabs.Count;

            //firstly spawn the main room at a random spawn point
            int randomIndex = Random.Range(0, nPoints);
            Vector2 mainRoomPos = _roomsSpawnPoints[randomIndex];
            BoltNetwork.Instantiate(_mainRoomPrefab, mainRoomPos, _mainRoomPrefab.transform.rotation);
            _roomsSpawnPoints.RemoveAt(randomIndex);
            nPoints--;
            Debug.Log("[MAPGEN] main room " + mainRoomPos);

            //find index of farthest spawn point from main room
            float maxDist = 0f;
            int farthestPointIndex = 0; 
            for(int i = 0; i < nPoints; i++)
            {
                float dist = Vector2.Distance(_roomsSpawnPoints[i], mainRoomPos);
                if (dist > maxDist){
                    farthestPointIndex = i;
                    maxDist = dist;
                }
            }
            Debug.Log("[MAPGEN] farthest point " + _roomsSpawnPoints[farthestPointIndex]);

            //find max level of difficulty
            int maxLevel = DetermineInvertedDifficultyLevel(mainRoomPos, farthestPointIndex);
            Debug.Log("[MAPGEN] max level " + maxLevel);

            //initialize list of counters for the occurrencies of the room types in the neighborhood
            GameObject toSpawn;
            for (int i = 0; i < nPoints; i++)
            {
                int[] counts = new int[nRooms];
                Debug.Log("[MAPGEN] room " + _roomsSpawnPoints[i]);

                //determine real level of difficulty based on the distance from the main room
                //(neighbours = highest level, neighbours of neighbours = highest level - 1...)
                int level = DetermineInvertedDifficultyLevel(mainRoomPos, i);
                level = maxLevel + 1 - level;
                Debug.Log("[MAPGEN] level " + level);
                
                //get adjacent rooms
                Collider2D[] cols = Physics2D.OverlapBoxAll(_roomsSpawnPoints[i], 2f * _adjacentRoomsRange * _adjacentRoomsRangeMultiplier, 
                    0f, LayerMask.GetMask("Room"));

                if(cols.Length > 0)
                {
                    //increment counters based on the types of the adjacent rooms
                    foreach (Collider2D col in cols)
                    {
                        Quest q = col.gameObject.GetComponent<Quest>();
                        if (q)
                            counts[q.GetQuestType()]++;
                    }

                    //find the occurencies least present room types in the neighborhood
                    int min = int.MaxValue;
                    for (int j = 0; j < nRooms; j++)
                        if (counts[j] < min)
                            min = counts[j];

                    //find least present room types in the neighborhood 
                    List<int> fewerRooms = new List<int>();
                    for (int j = 0; j < nRooms; j++)
                        if (counts[j] == min)
                            fewerRooms.Add(j);

                    /*
                    var s = "fewer rooms: ";
                    for (int j = 0; j < fewerRooms.Count; j++)
                    {
                        s += fewerRooms[j];
                    }
                    Debug.Log("[MAPGEN] " + s);
                    */

                    //pick a random room type from the least present ones (there may be more than one)
                    randomIndex = Random.Range(0, fewerRooms.Count);

                    //pick the room prefab to spawn
                    toSpawn = null;
                    foreach (Utils.PrefabIntegerAssociation assoc in _roomPrefabs)
                    {
                        if (assoc.num == fewerRooms[randomIndex])
                        {
                            toSpawn = assoc.prefab;
                            break;
                        }
                    }
                }
                else
                {
                    //if a room has no adjacent rooms pick a completely random type
                    randomIndex = Random.Range(0, nRooms);
                    toSpawn = _roomPrefabs[randomIndex].prefab;
                }

                //spawn the picked type asitting also the difficulty level and the max difficulty level
                BoltEntity spawnedRoom = BoltNetwork.Instantiate(toSpawn, _roomsSpawnPoints[i], toSpawn.transform.rotation);
                spawnedRoom.GetComponent<Quest>().SetDifficultyLevel(level);
                spawnedRoom.GetComponent<Quest>().SetMaxDifficultyLevel(maxLevel);
            }
        }

        ///<summary>
        ///returns inverted max level of difficulty, which depends on how many rooms can be placed between the main room and 
        ///the spawn point with index i
        ///</summary>
        public int DetermineInvertedDifficultyLevel(Vector2 mainRoomPos, int i)
        {
            //can be found by continuously increasing of 2f * _adjacentRoomsRange the size of a rectangle centered int the main room until
            //the spawn point with index i is contained in it
            int maxLevel = 0;
            Rect r = new Rect(mainRoomPos, 2f * _adjacentRoomsRange);
            r.center = mainRoomPos;
            bool found = false;
            while (!found)
            {
                maxLevel++;
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

            return maxLevel;
        }
        #endregion
    }
}