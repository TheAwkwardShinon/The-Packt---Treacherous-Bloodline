using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
  public virtual void gainAbility(Player player){
    
  }

  #endregion

}
