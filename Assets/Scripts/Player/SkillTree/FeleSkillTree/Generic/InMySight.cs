using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{

    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data/InMySight")]
    public class InMySight : AbilityData
    {
        public override void GainAbility(Player player)
        {
            base.GainAbility(player);
            player.GetPlayerData().standardCircleSize += 3f;
            player.SetFogOfWarDiameter( player.GetPlayerData().standardCircleSize);
        }
    }
}
