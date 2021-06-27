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
        protected Player _player;
        [SerializeField] private Image _characterSprite;
        [SerializeField] private List<Image> _abilitiesImage;
        [SerializeField] private Text _personalAbilityName;
        [SerializeField] private Text _className;
        [SerializeField] private List<Text> _abilitiesDescription; //TODO change this to a single text (tooltip)
        [SerializeField] private Image _logo;
        [SerializeField] private List<CharacterShowCaseData> _characters;

        [SerializeField] private Text _impostorText;
        [SerializeField] private Text _spendableExp;


        private int _index;
        #endregion

        #region methods

        private void Awake(){
            _selectedData = CharacterSelectionData.Instance;
             _index = _selectedData.GetCharacterIndex();
        }
        private void Start(){
            
            _player =  GameObject.FindWithTag(_selectedData.GetCharacterSelected()).GetComponent<Player>();
            _personalAbilityName.text = _characters[_index].personalAbilityName;
            _abilitiesImage[0].sprite = _characters[_index].personalAbility;
            _className.text = _characters[_index].ClassName;
            _characterSprite.sprite = _characters[_index].classData.characterSprite;
            _logo.sprite = _characters[_index].classData.clanLogo;
            _spendableExp.text = _player.GetSpendableExp().ToString();
            if(_player.isImpostor())
                _impostorText.gameObject.SetActive(true);
            for(int i=1; i<=_characters[_index].classData._abilitisSprite.Count;i++){ 
                _abilitiesImage[i].sprite = _characters[_index].classData._abilitisSprite[i-1];
            }
        }

        private void Update()
        {
            _spendableExp.text = _player.GetSpendableExp().ToString();
        }

        public CharacterShowCaseData getChardata(){
            return _characters[_index];
        }

        #endregion
    }
}