using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    
    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data/fele/TasteLikeIron")]
    public class TasteLikeIron : AbilityData
    {
        public override void GainAbility(Player player)
        {
            base.GainAbility(player);
            //player.GetPlayerData().tasteLikeIron = true;
        }
    }
}
