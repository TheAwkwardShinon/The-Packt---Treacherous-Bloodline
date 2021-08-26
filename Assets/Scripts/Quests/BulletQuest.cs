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
        [SerializeField] private float _durationLevelIncrement;

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

            _type = Constants.BULLET;
        }

        /*
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position, 2f * new Vector3(11f, 6f, 0f));
        }
        */

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

        public override void SetDifficultyLevel(int level)
        {
            base.SetDifficultyLevel(level);

            _duration += _durationLevelIncrement * (level - 1);

            _description += " for " + _duration + " seconds!";

            Debug.Log("[MAPGEN] bullet. level: " + level + "   duration: " + _duration);
        }

        #endregion
    }
}