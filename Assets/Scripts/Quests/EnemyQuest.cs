using System.Collections;
using UnityEngine;

namespace ThePackt
{
    public class EnemyQuest : Quest
    {
        #region methods
        private void Start()
        {
            _completeCondition = IsEnemyDead;
        }

        protected bool IsEnemyDead()
        {
            return true;
        }
        #endregion
    }
}