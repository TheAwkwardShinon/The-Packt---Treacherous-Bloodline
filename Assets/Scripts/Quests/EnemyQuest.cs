using System.Collections;
using UnityEngine;

namespace ThePackt
{
    public class EnemyQuest : Quest
    {
        [SerializeField] protected CharacterSelectionData _selectedData;

        #region methods
        private void Start()
        {
            _completeCondition = IsEnemyDead;

            _localPlayer = _selectedData.GetPlayerScript();
        }

        protected bool IsEnemyDead()
        {
            return true;
        }
        #endregion
    }
}