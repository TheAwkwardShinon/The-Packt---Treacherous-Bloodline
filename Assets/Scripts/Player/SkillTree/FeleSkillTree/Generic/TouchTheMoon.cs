using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    
    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data/TouchTheMoon")]
    public class TouchTheMoon : AbilityData
    {
        public override void GainAbility(Player player)
        {
            base.GainAbility(player);
            player.GetPlayerData().amountOfJumps = 2; //double jump ability
        }
    }
}
