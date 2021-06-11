using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{

    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data/hpUp")]
    public class HpStatUP : AbilityData
    {
        public override void GainAbility(Player player)
        {
            base.GainAbility(player);
            player.GetPlayerData().maxLifePoints += 0.15f * player.GetPlayerData().maxLifePoints; //multiply hp the by 15%
        }
    }
}

