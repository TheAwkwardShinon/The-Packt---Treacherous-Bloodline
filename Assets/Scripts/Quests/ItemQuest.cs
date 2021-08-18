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
        [SerializeField] private int _numberOfItems;
        [SerializeField] private int _numberLevelIncrement;
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

            _type = Constants.ITEM;
        }

        protected override void Update()
        {
            base.Update();

            if (_state == Constants.STARTED && _localPlayerPartecipates)
            {
                _localPlayer.SetFogOfWarDiameter(_fogOfWarDiameter);
            }
        }

        protected void SpawnItems()
        {
            List<Transform> tmp = new List<Transform>();
            foreach(Transform sp in _spawnPoints)
            {
                tmp.Add(sp);
            }

            if (BoltNetwork.IsServer)
            {
                for (int i = 0; i < _numberOfItems && tmp.Count > 0; i++) 
                {
                    //int randomIndex = Random.Range(0, _itemPrefabs.Length - 1);
                    int randomIndex = Random.Range(0, tmp.Count);
                    Transform sp = tmp[randomIndex];

                    tmp.RemoveAt(randomIndex);

                    BoltEntity spawnedItem = BoltNetwork.Instantiate(_itemPrefabs[0], sp.position, sp.rotation);
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

            _spawnedItems = new List<BoltEntity>();

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

            _spawnedItems = new List<BoltEntity>();
        }

        public override void SetDifficultyLevel(int level)
        {
            base.SetDifficultyLevel(level);

            _numberOfItems += _numberLevelIncrement * (level - 1);

            Debug.Log("[MAPGEN] item. level: " + level);
        }
        #endregion
    }
}
