using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class ChargeQuest : Quest
    {
        protected CharacterSelectionData _selectedData;
        [SerializeField] private GameObject[] _altarPrefabs;
        [SerializeField] private Transform[] _spawnPoints;
        private List<BoltEntity> _spawnedAltars;

        #region methods
        private void Awake()
        {
            _selectedData = CharacterSelectionData.Instance;
            _spawnedAltars = new List<BoltEntity>();

            _completeCondition = WereAltarsActivated;

            _startAction = SpawnAltars;

            _failAction = DespawnAltars;

            _localPlayer = _selectedData.GetPlayerScript();
        }

        protected override void Update()
        {
            base.Update();

            /*
            if (_state == Constants.STARTED && _localPlayerPartecipates)
            {
                _localPlayer.SetFogOfWarDiameter(_fogOfWarDiameter);
            }
            */
        }

        protected void SpawnAltars()
        {
            if (BoltNetwork.IsServer)
            {
                foreach (var sp in _spawnPoints)
                {
                    int randomIndex = Random.Range(0, _altarPrefabs.Length - 1);
                    BoltEntity spawnedAltar = BoltNetwork.Instantiate(_altarPrefabs[randomIndex], sp.position, sp.rotation);
                    _spawnedAltars.Add(spawnedAltar);
                }
            }
        }

        protected bool WereAltarsActivated()
        {
            foreach (var item in _spawnedAltars)
            {
                if (!item.GetComponent<Altar>().IsActivated())
                {
                    return false;
                }
            }

            DespawnAltars();

            return true;
        }

        protected void DespawnAltars()
        {
            foreach (var item in _spawnedAltars)
            {
                if (item.IsAttached)
                {
                    BoltNetwork.Destroy(item);
                }
            }

            _spawnedAltars = new List<BoltEntity>();
        }
        #endregion
    }
}