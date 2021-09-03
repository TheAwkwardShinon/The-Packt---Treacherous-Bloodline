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
        [SerializeField] private Platform[] _platforms;
        private List<BoltEntity> _spawnedEnemies;

        #region methods
        private void Awake()
        {
            _selectedData = CharacterSelectionData.Instance;
            _spawnedEnemies = new List<BoltEntity>();

            _completeCondition = AreEnemiesDead;

            _startAction = SpawnEnemies;

            _failAction = DespawnEnemies;

            _localPlayer = _selectedData.GetPlayerScript();

            _type = Constants.ENEMY;
        }

        public override void Attached()
        {
            base.Attached();

            //iterate on platform objects of the room and project the extremes down to the platform walkable ground 
            foreach (var platform in _platforms)
            {
                var hit = Physics2D.Raycast(platform.left.position, new Vector2(0f, -1f), 1f, LayerMask.GetMask("Ground", "EnemyInvisibleGround"));
                platform.left.position = hit.point;
                platform.right.position = new Vector2(platform.right.position.x, hit.point.y);
            }
        }

        /*
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (var platform in _platforms)
            {
                Gizmos.DrawWireSphere(platform.left.position, 0.1f);
                Gizmos.DrawWireSphere(platform.right.position, 0.1f);
            }
        }
        */

        ///<summary>
        ///spawn a random enemy chosen among the _enemyPrefabs at each spawn point in _spawnPoints. The spawned enemies will have 
        ///different strength parameters based on the difficulty level of this quest
        ///</summary>
        protected void SpawnEnemies()
        {
            if (BoltNetwork.IsServer)
            {
                foreach(var sp in _spawnPoints)
                {
                    int randomIndex = Random.Range(0, _enemyPrefabs.Length);
                    var token = new LevelDataToken();

                    //prepare the token with the info on the difficuty level
                    token.SetLevel(_difficultyLevel);

                    BoltEntity spawnedEnemy = BoltNetwork.Instantiate(_enemyPrefabs[randomIndex], token, sp.position, sp.rotation);

                    //set the belonging room of the enemy to this, also the platforms will be passed to the enemy
                    spawnedEnemy.GetComponent<Enemy>().SetRoom(this, _platforms);
                    _spawnedEnemies.Add(spawnedEnemy);
                }
            }
        }

        ///<summary>
        ///checks if all the spawned enemies are dead
        ///</summary>
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

        ///<summary>
        ///despawn all spawned enemies
        ///</summary>
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

        ///<summary>
        ///sets the difficulty level of this room
        ///</summary>
        public override void SetDifficultyLevel(int level)
        {
            base.SetDifficultyLevel(level);

            Debug.Log("[MAPGEN] enemy. level: " + level);
        }
        #endregion
    }
}