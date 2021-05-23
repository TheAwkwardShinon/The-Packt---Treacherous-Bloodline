using System.Collections;
using UnityEngine;

namespace ThePackt
{
    public class EnemyQuest : Quest
    {
        protected CharacterSelectionData _selectedData;
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private Transform _spawnPoint;
        private BoltEntity _spawnedEnemy;

        #region methods
        private void Awake()
        {
            _selectedData = CharacterSelectionData.Instance;

            _completeCondition = IsEnemyDead;

            _startAction = SpawnEnemy;

            _failAction = DespawnEnemy;

            _localPlayer = _selectedData.GetPlayerScript();
        }

        protected void SpawnEnemy()
        {
            if (BoltNetwork.IsServer)
            {
                _spawnedEnemy = BoltNetwork.Instantiate(_enemyPrefab, _spawnPoint.position, _spawnPoint.rotation);
                _spawnedEnemy.GetComponent<Enemy>().SetRoom(this);
            }
        }

        protected bool IsEnemyDead()
        {
            if(!_spawnedEnemy.IsAttached)
            {
                return true;
            }

            return false;
        }

        protected void DespawnEnemy()
        {
            if (_spawnedEnemy.IsAttached)
            {
                BoltNetwork.Destroy(_spawnedEnemy);
            }
        }
        #endregion
    }
}