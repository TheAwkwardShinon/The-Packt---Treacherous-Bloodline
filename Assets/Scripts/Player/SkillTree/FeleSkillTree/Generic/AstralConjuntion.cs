using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    
    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data/AstralConjuntion")]
    public class AstralConjuntion : AbilityData
    {
        public override void GainAbility(Player player)
        {
            base.GainAbility(player);
            player.GetPlayerData().astralconjuntion = true;
        }
    }
}
