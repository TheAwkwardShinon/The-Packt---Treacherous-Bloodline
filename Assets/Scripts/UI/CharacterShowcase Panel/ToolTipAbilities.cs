using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ThePackt{
    public class ToolTipAbilities : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        //private List<CharacterShowCaseData> _characters;
        [SerializeField] private CharacterShowcaseManager _manager;
        [SerializeField] int my_index;

        [SerializeField] private GameObject _tooltip;

        [SerializeField] private Text _cost;
        [SerializeField] private Text _description;
        [SerializeField] private Text _name;
        [SerializeField] private Image _abilityIcon;
        

        public void OnDeselect(BaseEventData eventData)
        {
            _tooltip.SetActive(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            _tooltip.transform.position = new Vector2(this.transform.position.x - 450f,this.transform.position.y);
            _cost.text = _manager.GetcharacterList()[_manager.GetIndex()].classData.abilitiesCost[my_index].ToString();
            _description.text = _manager.GetcharacterList()[_manager.GetIndex()].classData.abilitiesDescription[my_index];
            _name.text = _manager.GetcharacterList()[_manager.GetIndex()].classData.abilitiesName[my_index];
            _abilityIcon.sprite = _manager.GetcharacterList()[_manager.GetIndex()].classData._abilitisSprite[my_index];

            _tooltip.SetActive(true);
        }

        public void Start(){
           // _characters = _manager.GetComponent<CharacterShowcaseManager>().GetcharacterList();
        }
    }
}
