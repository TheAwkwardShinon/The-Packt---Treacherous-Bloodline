using System.Collections;
using UnityEngine;

namespace ThePackt
{
    public class Cooldown
    {
        #region variables  

        string _actionName;
        float _remainingTime;

        #endregion


        #region methods

        public Cooldown(string actionName, float remainingTime)
        {
            _actionName = actionName;
            _remainingTime = remainingTime;
        }

        public bool DecrementCooldown(float deltaTime)
        {
            _remainingTime = Mathf.Max(_remainingTime - deltaTime, 0f);

            // Debug.Log(_remainingTime == 0f);

            return _remainingTime == 0f;
        }
        #endregion

        #region getter

        public string GetActionName()
        {
            return _actionName;
        }

        public float GetRemainingTime()
        {
            return _remainingTime;
        }
        #endregion

        #region setter

        public void SetRemainingTime(float value)
        {
            _remainingTime = value;
        }
        #endregion
    }
}