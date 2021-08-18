using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class EnemyQuest : Quest
    {
        protected CharacterSelectionData _selectedData;
        [SerializeField] private GameObject[] _enemyPrefabs;
        [SerializeField] private Transform[] _spawnPoints;
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
        #endregion
    }
}