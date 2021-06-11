using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    
    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data/CuttingEdge")]
    public class CuttingEdge : AbilityData
    {
        public override void GainAbility(Player player)
        {
            base.GainAbility(player);
            player.GetPlayerData().dashCooldown -= 0.2f;
            player.GetPlayerData().dashTime += 0.4f;
        }
    }
}
