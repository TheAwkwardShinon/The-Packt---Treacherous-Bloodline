using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class ClanFele : Player
    {
        #region variables
        private bool weakActive = false;
        private bool mediumActive = false;
        private bool attackModifier = false;
        #endregion

        #region methods
        public  override void Start()
        {
            
        }

        public override void SimulateOwner()
        {
            
        }

        #region setter
        public void setWeakActive(bool value){
            weakActive = value;
        }
        #endregion

        #endregion
    }
}

