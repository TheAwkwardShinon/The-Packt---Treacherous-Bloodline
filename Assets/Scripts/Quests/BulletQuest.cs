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
            foreach(Transform sp in _spawnPoints)
            {
                sp.GetComponent<BulletSpawnPoint>().Shoot(_bulletPrefab);
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

        public void PlayShotSound(int cannonId)
        {
            foreach (Transform sp in _spawnPoints)
            {
                if (sp.GetComponent<BulletSpawnPoint>().GetId() == cannonId)
                {
                    sp.GetComponent<AudioSource>().Play();
                    break;
                }
            }
        }

        #endregion
    }
}