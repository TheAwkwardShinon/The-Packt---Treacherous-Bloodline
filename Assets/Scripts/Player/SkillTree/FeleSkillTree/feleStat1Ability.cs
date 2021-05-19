using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{

    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data/fele/stat1")]
    public class feleStat1Ability : AbilityData
    {
        public override void GainAbility(Player player)
        {
            base.GainAbility(player);
            player.GetPlayerData().movementVelocityMultiplier += 0.15f * player.GetPlayerData().movementVelocity ; //multiply the by 15%
        }
    }
}
