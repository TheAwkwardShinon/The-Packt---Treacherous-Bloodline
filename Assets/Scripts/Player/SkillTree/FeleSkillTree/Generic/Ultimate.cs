using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    
    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data/Ultimate")]
    public class Ultimate : AbilityData
    {
        public override void GainAbility(Player player)
        {
            base.GainAbility(player);
            player.GetPlayerData().specialAttackCooldown -= 0.3f;
        }
    }
}
