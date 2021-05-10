using System.Collections;
using UnityEngine;

namespace ThePackt
{
    public class EnemyQuest : Quest
    {
        protected CharacterSelectionData _selectedData;
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private Transform _spawnPoint;

        #region methods
        private void Awake()
        {
            _selectedData = CharacterSelectionData.Instance;

            _completeCondition = IsEnemyDead;

            _startAction = SpawnEnemy;

            _localPlayer = _selectedData.GetPlayerScript();

            _startAction();
        }

        protected void SpawnEnemy()
        {
            if (BoltNetwork.IsServer)
            {
                BoltNetwork.Instantiate(_enemyPrefab, _spawnPoint.position, _spawnPoint.rotation);
            }
        }

        protected bool IsEnemyDead()
        {
            return true;
        }
        #endregion
    }
}