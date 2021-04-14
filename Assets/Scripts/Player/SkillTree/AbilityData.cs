using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace ThePackt{

    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data")]

    public class AbilityData : ScriptableObject
    {
    #region variables
    public string abilityName;
    [TextArea] public string description;

    public string className;

    public Sprite _icon;

    public int abilityCost;

    public List<AbilityData> _unlockableAbilities;

    #endregion

    #region methods
    public virtual void GainAbility(Player player){
        
    }

    /* method that returns true if the node isn't a leaf */
    public bool HasChildren(){
        return _unlockableAbilities.Count != 0;
    }

    #endregion

    }
}
