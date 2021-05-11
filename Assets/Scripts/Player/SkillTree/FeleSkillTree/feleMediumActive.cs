using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    
[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data/fele/medium active")]
    public class feleMediumActive : AbilityData
    {
        public override void GainAbility(Player player)
        {
            base.GainAbility(player);
            player.SetMediumActive(true); //enable medium Active
        }
    }
}
