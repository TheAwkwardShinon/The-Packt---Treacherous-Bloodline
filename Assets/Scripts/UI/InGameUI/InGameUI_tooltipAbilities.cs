using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ThePackt{
    public class InGameUI_tooltipAbilities : MonoBehaviour, ISelectHandler, IDeselectHandler
    {   

        #region variables
        [SerializeField] private SkilltreeManager _manager;
        [SerializeField] int my_index;

        [SerializeField] private GameObject _tooltip;

        [SerializeField] private Text _cost;
        [SerializeField] private Text _description;
        [SerializeField] private Text _name;
        [SerializeField] private Image _abilityIcon;

        private CharacterShowCaseData _myCharacter;

        #endregion

        #region methods

        private void Start(){
            _myCharacter = _manager.getChardata();
        }
        public void OnDeselect(BaseEventData eventData)
        {
            _tooltip.SetActive(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if(_myCharacter != null){
                _tooltip.transform.position = new Vector2(this.transform.position.x + 400f,this.transform.position.y);
                _cost.text = _myCharacter.classData.abilitiesCost[my_index].ToString();
                _description.text =_myCharacter.classData.abilitiesDescription[my_index];
                _name.text = _myCharacter.classData.abilitiesName[my_index];
                _abilityIcon.sprite = _myCharacter.classData._abilitisSprite[my_index];

                _tooltip.SetActive(true);
            }
        }

        public int GetCost(){
            return _myCharacter.classData.abilitiesCost[my_index];
        }

        public string GetAbilityName(){
            return _myCharacter.classData.abilitiesName[my_index];
        }

        #endregion
    }
}
