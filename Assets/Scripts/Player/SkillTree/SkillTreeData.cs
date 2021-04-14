using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{

    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/skillTree Data/tree Data")]

    public class SkillTreeData : ScriptableObject
    {
        #region variables
        public string className;

        private List<AbilityData> _unlockedAbility;
        public AbilityData _rootAbility; //we handle the path information node by node, basically is the skilltree's navigator
        private  List<AbilityData> _unlockableAbility; //all the skill unlockable

        #endregion

        #region methods

        /* very important at the beginning to put the root of the tree on the unlocable list */
        public void Awake(){
            _unlockableAbility.Add(_rootAbility);
        }

        /* methods that add all the children of an already bought ability */
        private void AddUnlockableAbility(List<AbilityData> ability){
            _unlockableAbility.AddRange(ability);
        }

        /* with this method the player gain an ability if he could buy it, then the method manage the skillTree progression */
        public void BuyAbility(Player player,string name){
            foreach(AbilityData a in _unlockableAbility){
                if(a.name.Equals(name)){
                    if(player.GetPlayerData().points < a.abilityCost)
                        return;
                    else{
                        player.GetPlayerData().points -= a.abilityCost;
                        _unlockedAbility.Add(a);
                        _unlockableAbility.Remove(a);
                        if(a.HasChildren())
                            AddUnlockableAbility(a._unlockableAbilities);
                        a.GainAbility(player);
                    }
                }
            }
        }

        /* method that just reset the tree */
        public void ResetTree(){
            _unlockableAbility.Clear();
            _unlockedAbility.Clear();
        }

        #endregion
    }
}
