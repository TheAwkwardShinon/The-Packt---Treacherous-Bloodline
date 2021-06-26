using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    
    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data/TasteLikeIron")]
    public class TastesLikeIron : AbilityData
    {
        public override void GainAbility(Player player)
        {
            base.GainAbility(player);
            player.GetPlayerData().tasteLikeIron = true;
        }
    }
}
