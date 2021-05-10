using System.Collections;
using UnityEngine;

namespace ThePackt
{
    public class EnemyQuest : Quest
    {
        protected CharacterSelectionData _selectedData;

        #region methods
        private void Awake()
        {
            _completeCondition = IsEnemyDead;

            _localPlayer = _selectedData.GetPlayerScript();

            _selectedData = CharacterSelectionData.Instance;
        }

        protected bool IsEnemyDead()
        {
            return true;
        }
        #endregion
    }
}