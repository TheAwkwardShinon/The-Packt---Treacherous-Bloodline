using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    
    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data/fele/passive tier 2")]
    public class felePassive2 : AbilityData
    {
        public override void GainAbility(Player player)
        {
            base.GainAbility(player);
            player.GetPlayerData().movementVelocity += 0.05f *  player.GetPlayerData().movementVelocity;
            player.GetPlayerData().movementMultiplierWhenFullLife += 0.05f *  player.GetPlayerData().movementVelocity; 

        }
    }
}

