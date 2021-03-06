using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace ThePackt
{
    public class Objective : Bolt.EntityBehaviour<IObjectiveState>
    {
        [SerializeField] private float _integrity;
        [SerializeField] private GameObject[] _enemyPrefabs;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] protected Canvas canvas;
        [SerializeField] protected GameObject integrityBar;
        [SerializeField] protected Image integrityImage;
        [SerializeField] protected Gradient integrityGradient;
        private Slider integritySlider;
        private MainQuest _mainQuest;

        private bool _isEnemyAlive;
        private bool _isUpLeft;
        [SerializeField] private float _spawnCooldown;
        private float _spawnStartTime;
        private BoltEntity _spawnedEnemy;
        private int _randomIndex;

        public Quaternion _rotation;

        public override void SimulateOwner()
        {
            if ((_spawnedEnemy != null && !_spawnedEnemy.IsAttached) || _spawnedEnemy == null)
            {
                _isEnemyAlive = false;
            }

            if(Time.time >= _spawnStartTime + _spawnCooldown)
            {
                Debug.Log("[OBJECTIVE] timeout");

                if (!_isEnemyAlive)
                {
                    Debug.Log("[OBJECTIVE] enemy spawning");

                    SpawnEnemy();
                }
                else
                {
                    Debug.Log("[OBJECTIVE] restarting timer");

                    _spawnStartTime = Time.time;
                }
            }
        }

        public override void Attached()
        {
            if (BoltNetwork.IsServer)
            {
                var token = (ObjectiveDataToken) entity.AttachToken;

                _isUpLeft = token._isUpLeft;
                _spawnedEnemy = null;
                _spawnStartTime = Time.time;
                _isEnemyAlive = false;
                _mainQuest = MainQuest.Instance;
                state.Integrity = _integrity;
                SpawnEnemy();
            }

            integritySlider = integrityBar.GetComponent<Slider>();
            integrityImage.color = integrityGradient.Evaluate(1f);
            integritySlider.maxValue = _integrity;

            state.AddCallback("Integrity", IntegrityCallback);
        }

        private void Update()
        {
            canvas.transform.rotation = _rotation;
        }

        public void ApplyDamage(float damage)
        {
            Debug.Log("[OBJECTIVE] apply damage");

            if (BoltNetwork.IsServer)
            {
                state.Integrity -= damage;
            }
        }

        private void Die()
        {
            BoltNetwork.Destroy(gameObject);
        }

        protected void SpawnEnemy()
        {
            if (BoltNetwork.IsServer)
            {
                if (_isUpLeft)
                {
                    _randomIndex = Random.Range(1, _enemyPrefabs.Length);
                }
                else
                {
                    _randomIndex = Random.Range(0, _enemyPrefabs.Length);
                }
                
                _spawnedEnemy = BoltNetwork.Instantiate(_enemyPrefabs[_randomIndex], _spawnPoint.position, _spawnPoint.rotation);
                _spawnedEnemy.GetComponent<Enemy>().SetRoom(MainQuest.Instance);
                _isEnemyAlive = true;

                if(!_spawnedEnemy)
                {
                    _spawnStartTime = Time.time;
                }
            }
        }

        private void IntegrityCallback()
        {
            _integrity = state.Integrity;
            Debug.Log("[OBJECTIVE] integrity callback. New _integrity: " + _integrity);

            integritySlider.value = _integrity;
            integrityImage.color = integrityGradient.Evaluate(integritySlider.normalizedValue);

            if (_integrity <= 0)
            {
                Debug.Log("[OBJECTIVE] destroyed");
                Die();
            }
        }

        public override void Detached()
        {
            _mainQuest.RemoveObjective(entity);
        }
    }
}

