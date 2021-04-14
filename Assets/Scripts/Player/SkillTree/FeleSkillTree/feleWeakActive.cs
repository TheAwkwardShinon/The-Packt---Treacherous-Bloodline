using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    
    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data/fele/weak active")]
    public class feleWeakActive : AbilityData
    {
 
        public override void GainAbility(Player player)
        {
            base.GainAbility(player);
            player.SetWeakActive(true); //enable weak Active
        }
    }
}
