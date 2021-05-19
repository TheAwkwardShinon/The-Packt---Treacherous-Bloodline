using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{

    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data/fele/stat3")]
    public class feleStat3Ability : AbilityData
    {
        public override void GainAbility(Player player)
        {
            base.GainAbility(player);
            player.GetPlayerData().damageMultiplier += 0.15f; //multiply the damage by 15%
            Debug.LogError("[BUY ABILITY] ability gained!!!!!");
        }
    }
}
