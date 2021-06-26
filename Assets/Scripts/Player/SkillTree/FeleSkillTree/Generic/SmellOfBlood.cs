using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{

    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data/SmellOfBlood")]
    public class SmellOfBlood : AbilityData
    {
        public override void GainAbility(Player player)
        {
            base.GainAbility(player);
            player.GetPlayerData().bleedOutTime += 15f;
        }
    }
}