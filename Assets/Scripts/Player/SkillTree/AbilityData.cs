using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace ThePackt{

    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data")]

    public class AbilityData : ScriptableObject
    {
    #region variables

    public List<AbilityData> _unlockableAbilities;
    public string abilityName;
    public string animationName;

    public int colorIndex;

    #endregion

    #region methods

    public virtual void GainAbility(Player player){
        Debug.LogError("[BUY ABILITY] gaining ability...");
    }

    /* method that returns true if the node isn't a leaf */
    public bool HasChildren(){
        return _unlockableAbilities.Count != 0;
    }

    #endregion

    }
}
