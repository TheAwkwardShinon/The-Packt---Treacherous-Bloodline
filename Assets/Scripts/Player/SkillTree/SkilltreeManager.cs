using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThePackt{

    //this class load teh correct skill tree based on player selection
    public class SkilltreeManager : MonoBehaviour
    {

        #region variables

        protected CharacterSelectionData _selectedData;
        [SerializeField] private Image _characterSprite;
        [SerializeField] private List<Image> _abilitiesImage;
        [SerializeField] private Text _personalAbilityName;
        [SerializeField] private Text _className;
        [SerializeField] private List<Text> _abilitiesDescription; //TODO change this to a single text (tooltip)
        [SerializeField] private Image _logo;
        [SerializeField] private List<CharacterShowCaseData> _characters;

        private int _index;
        #endregion

        #region methods

        private void Awake(){
            _selectedData = CharacterSelectionData.Instance;
             _index = _selectedData.GetCharacterIndex();
        }
        private void Start(){
            
          
            _personalAbilityName.text = _characters[_index].personalAbilityName;
            _abilitiesImage[0].sprite = _characters[_index].personalAbility;
            _className.text = _characters[_index].ClassName;
            _characterSprite.sprite = _characters[_index].classData.characterSprite;
            _logo.sprite = _characters[_index].classData.clanLogo;
            for(int i=1; i<=_characters[_index].classData._abilitisSprite.Count;i++){ 
                _abilitiesImage[i].sprite = _characters[_index].classData._abilitisSprite[i-1];
            }
        }

        public CharacterShowCaseData getChardata(){
            return _characters[_index];
        }

        #endregion
    }
}