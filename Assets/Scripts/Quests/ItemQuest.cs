using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class ItemQuest : Quest
    {
        protected CharacterSelectionData _selectedData;
        [SerializeField] private GameObject[] _itemPrefabs;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private float _fogOfWarDiameter;
        private List<BoltEntity> _spawnedItems;

        #region methods
        private void Awake()
        {
            _selectedData = CharacterSelectionData.Instance;
            _spawnedItems = new List<BoltEntity>();

            _completeCondition = WereItemsCollected;

            _startAction = SpawnItems;

            _failAction = DespawnItems;

            _localPlayer = _selectedData.GetPlayerScript();
        }

        private void Update()
        {
            if (_state == Constants.STARTED && _localPlayerPartecipates)
            {
                _localPlayer.SetFogOfWarDiameter(_fogOfWarDiameter);
            }
        }

        protected void SpawnItems()
        {
            if (BoltNetwork.IsServer)
            {
                foreach (var sp in _spawnPoints)
                {
                    int randomIndex = Random.Range(0, _itemPrefabs.Length - 1);
                    BoltEntity spawnedItem = BoltNetwork.Instantiate(_itemPrefabs[randomIndex], sp.position, sp.rotation);
                    _spawnedItems.Add(spawnedItem);
                }
            }
        }

        protected bool WereItemsCollected()
        {
            foreach (var item in _spawnedItems)
            {
                if (item.IsAttached)
                {
                    return false;
                }
            }

            return true;
        }

        protected void DespawnItems()
        {
            foreach (var item in _spawnedItems)
            {
                if (item.IsAttached)
                {
                    BoltNetwork.Destroy(item);
                }
            }
        }
        #endregion
    }
}
