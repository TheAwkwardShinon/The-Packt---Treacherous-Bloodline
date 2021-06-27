using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThePackt{

    
    public class SkillTreeData : MonoBehaviour
    {
        #region variables
        public string className;

        private List<AbilityData> _unlockedAbility;
        private AbilityData _rootAbility; //we handle the path information node by node, basically is the skilltree's navigator
        private  List<AbilityData> _unlockableAbility; //all the skill unlockable
        public SkilltreeManager _manager;
        private Player _player;

        [SerializeField] private Animator _anim;
        [SerializeField] private AudioSource _audio;
        [SerializeField] private AudioSource _audioDeny;
        [SerializeField] private ChangeBranchColor _changeBranch;

        [SerializeField] private Text _spendableExp;




        #endregion

        #region methods

        /* very important at the beginning to put the root of the tree on the unlocable list */
        public void Start(){
            _rootAbility = _manager.getChardata().classData._rootAbility;
            _unlockableAbility = new List<AbilityData>();
            _unlockedAbility = new List<AbilityData>();
            _unlockableAbility.Add(_rootAbility);
            Debug.LogError("[BUY ABILITY] unlockable " + _unlockableAbility[0].abilityName);
            string temp = _manager.getChardata().ClassName.Split('-')[0].Trim().ToLower();
            _player = GameObject.FindWithTag(temp).GetComponent<Player>();
           
        }

        /* methods that add all the children of an already bought ability */
        private void AddUnlockableAbility(List<AbilityData> ability){
            _unlockableAbility.AddRange(ability);
        }

        /* with this method the player gain an ability if he could buy it, then the method manage the skillTree progression */
        public void BuyAbility(string name, int cost){
            Debug.LogError("[BUY ABILITY] trying to buy: "+name + " at cost: "+cost);
            foreach(AbilityData a in _unlockableAbility){
                
                if (a.abilityName.Equals(name)){
                    Debug.LogError("[BUY ABILITY] available " + name + ", cost: " + cost + " spendable: " + _player.GetSpendableExp());
                    if (_player.GetSpendableExp() < cost){
                        _audioDeny.Play();
                        Debug.LogError("[BUY ABILITY] can't afford this ability due to its cost");
                        return;
                    }
                    else{
                        if(a.animationName != null){
                            _anim.SetTrigger(a.animationName);
                        }
                        else{
                            //Debug.LogError("trigger name wrong");
                        }
                
                        //Debug.LogError("[BUY ABILITY] you got sufficent money to buy it");
                        _player.SetSpendableExp(_player.GetSpendableExp() - cost);
                        if(a.HasChildren())
                            AddUnlockableAbility(a._unlockableAbilities);
                        a.GainAbility(_player);
                         _unlockedAbility.Add(a);
                        _unlockableAbility.Remove(a);
                        _spendableExp.text = _player.GetSpendableExp().ToString();
                        _audio.Play();
                        return;
                        
                    }
                }
            }
            Debug.LogError("[BUY ABILITY] not unlockable");
            _audioDeny.Play();
            
        }

        /* method that just reset the tree */
        public void ResetTree(){
            _unlockableAbility.Clear();
            _unlockedAbility.Clear();
        }

        #endregion
    }
}
