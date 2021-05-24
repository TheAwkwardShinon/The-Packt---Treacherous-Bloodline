using System.Collections;
using UnityEngine;

namespace ThePackt
{
    public class BulletQuest : Quest
    {
        protected CharacterSelectionData _selectedData;
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private float _duration;
        [SerializeField] private float _fireRate;
        [SerializeField] private float _maxDegreeOfFire;
        private float _lastFireTime;

        #region methods
        private void Awake()
        {
            Debug.Log("[QUEST] " + _title);

            _selectedData = CharacterSelectionData.Instance;

            _startAction = null;

            _failAction = null;

            _completeCondition = IsTimeEnded;

            _inProgressAction = SpawnBullets;

            _localPlayer = _selectedData.GetPlayerScript();
        }

        protected void SpawnBullets()
        {
            if (BoltNetwork.IsServer && Time.time >= _lastFireTime + _fireRate)
            {
                foreach(Transform sp in _spawnPoints)
                {
                    float angle = UnityEngine.Random.Range(-_maxDegreeOfFire, _maxDegreeOfFire);
                    BoltNetwork.Instantiate(_bulletPrefab, sp.position, sp.rotation * Quaternion.Euler(0,0,angle));

                    _lastFireTime = Time.time;
                }
            }
        }

        protected bool IsTimeEnded()
        {
            if(Time.time >= _startTime + _duration)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}