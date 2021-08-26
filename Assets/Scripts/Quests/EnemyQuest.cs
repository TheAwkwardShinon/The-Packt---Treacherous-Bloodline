using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ThePackt
{
    public class EnemyQuest : Quest
    {
        protected CharacterSelectionData _selectedData;
        [SerializeField] private GameObject[] _enemyPrefabs;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private Tilemap _waypointsTilemap;
        [SerializeField] private Tilemap _groundTilemap;
        [SerializeField] private Tilemap _wallTilemap;
        private List<Vector3> _waypoints;
        private List<BoltEntity> _spawnedEnemies;

        #region methods
        private void Awake()
        {
            _selectedData = CharacterSelectionData.Instance;
            _spawnedEnemies = new List<BoltEntity>();

            _waypoints = new List<Vector3>();

            _completeCondition = AreEnemiesDead;

            _startAction = SpawnEnemies;

            _failAction = DespawnEnemies;

            _localPlayer = _selectedData.GetPlayerScript();

            _type = Constants.ENEMY;
        }

        public override void Attached()
        {
            base.Attached();

            foreach (var position in _waypointsTilemap.cellBounds.allPositionsWithin)
            {
                if (_waypointsTilemap.HasTile(position))
                {
                    var hit = Physics2D.Raycast(_waypointsTilemap.GetCellCenterWorld(position), new Vector2(0f, -1f), 3f, LayerMask.GetMask("Ground", "EnemyInvisibleGround"));
                    _waypoints.Add(hit.point);
                    Debug.Log("[PATH] waypoint: " + hit.point);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (var w in _waypoints)
            {
                Gizmos.DrawWireSphere(w, 0.1f);
            }
        }

        protected void SpawnEnemies()
        {
            if (BoltNetwork.IsServer)
            {
                foreach(var sp in _spawnPoints)
                {
                    int randomIndex = Random.Range(0, _enemyPrefabs.Length);
                    var token = new LevelDataToken();

                    token.SetLevel(_difficultyLevel);

                    BoltEntity spawnedEnemy = BoltNetwork.Instantiate(_enemyPrefabs[randomIndex], token, sp.position, sp.rotation);
                    spawnedEnemy.GetComponent<Enemy>().SetRoom(this);
                    _spawnedEnemies.Add(spawnedEnemy);
                }
            }
        }

        protected bool AreEnemiesDead()
        {
            foreach (var enemy in _spawnedEnemies)
            {
                if (enemy.IsAttached)
                {
                    return false;
                }
            }

            return true;
        }

        protected void DespawnEnemies()
        {
            foreach(var enemy in _spawnedEnemies)
            {
                if (enemy.IsAttached)
                {
                    BoltNetwork.Destroy(enemy);
                }
            }
        }

        public override void SetDifficultyLevel(int level)
        {
            base.SetDifficultyLevel(level);

            Debug.Log("[MAPGEN] enemy. level: " + level);
        }

        #region getters
        public Tilemap GetWaypointsTilemap()
        {
            return _waypointsTilemap;
        }

        public Tilemap GetGroundTilemap()
        {
            return _groundTilemap;
        }

        public Tilemap GetWallTilemap()
        {
            return _wallTilemap;
        }

        public List<Vector3> GetWaypoints()
        {
            return _waypoints;
        }
        #endregion
        #endregion
    }
}