using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{

    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data/BackUp")]
    public class BackUp : AbilityData
    {
        public override void GainAbility(Player player)
        {
            base.GainAbility(player);
            player.GetPlayerData().numOfReviveAction = 1;
        }
    }
}