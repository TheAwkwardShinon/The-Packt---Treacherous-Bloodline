using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace ThePackt{

    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/ability Data")]

    public class AbilityData : ScriptableObject
    {
    #region variables
    public ClanShowCaseData data;
    public int index;
    public string abilityName;
    public string description;
    public  string className = "Fele";
    public Sprite icon;
    public int abilityCost;
    public List<AbilityData> _unlockableAbilities;

    #endregion

    #region methods

    private void start(){
        abilityName = data.abilitiesName[index];
        description = data.abilitiesDescription[index];
        icon = data._abilitisSprite[index];
        abilityCost = data.abilitiesCost[index];
    }

    public virtual void GainAbility(Player player){
        
    }

    /* method that returns true if the node isn't a leaf */
    public bool HasChildren(){
        return _unlockableAbilities.Count != 0;
    }

    #endregion

    }
}
