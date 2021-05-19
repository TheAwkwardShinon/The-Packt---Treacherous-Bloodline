using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    
    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data/fele/passive tier 3")]
    public class felePassive3 : AbilityData
    {
        public override void GainAbility(Player player)
        {
            base.GainAbility(player);
            player.GetPlayerData().healAfterHit += 0.01f * player.GetPlayerData().maxLifePoints; //cura dell'1% della vita totale per ogni colpo andato a segno.
        }
    }
}