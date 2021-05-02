using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ThePackt{
    public class CharacterShowcaseManager : MonoBehaviour
    {

        //booleano true, se bottone cliccato/slezionato. Se è selezionato bloccare navigazione e attivare comando con frecce per selezionare character. Se premo cerchio deseleziono
        //e riattivo a false booleano

        public EventSystem eventSystem;
        private bool _isSelected;

        private bool _clicked = false;

        public Button button;

        private Navigation _standardNav;

        #region tobeFilled
        [SerializeField] private Image _characterSprite;
        [SerializeField] private List<Image> _abilitiesImage;
        [SerializeField] private Text _lore;
        [SerializeField] private Text _personalAbilityName;
        [SerializeField] private Text _className;
        [SerializeField] private List<Text> _abilitiesDescription; //TODO change this to a single text (tooltip)
        [SerializeField] private Image _logo;
        [SerializeField] private List<CharacterShowCaseData> _characters;

        private int _currentIndex = 0;

        #endregion

        public void getCharLeft(){
            _currentIndex = _currentIndex - 1 < 0 ? _characters.Count - 1 : _currentIndex - 1;
            CharacterSelection();
        }

        public void getCharRight(){
            _currentIndex = _currentIndex + 1 == _characters.Count ? 0 : _currentIndex + 1;
            CharacterSelection();
        }

        /* todo on button click */
        public void click(){
            if(!_clicked){
                Navigation nav = new Navigation();
                nav.mode = Navigation.Mode.None;
                button.navigation = nav;
                _clicked = true;
           }
            else{
                _clicked = false;
                button.navigation = _standardNav;
            }
        }

        private void CharacterSelection(){
            _personalAbilityName.text = _characters[_currentIndex].personalAbilityName;
            _abilitiesImage[0].sprite = _characters[_currentIndex].personalAbility;
            _className.text = _characters[_currentIndex].ClassName;
            _characterSprite.sprite = _characters[_currentIndex].classData.characterSprite;
            _logo.sprite = _characters[_currentIndex].classData.clanLogo;
            /*to add description and other things */
            for(int i=1; i<=_characters[_currentIndex].classData._abilitisSprite.Count;i++){ 
                _abilitiesImage[i].sprite = _characters[_currentIndex].classData._abilitisSprite[i-1];
            }/* aggiungere varie descrizioni */
        }

        public void CharacterSelectionRight(){
            _currentIndex = _currentIndex + 1 == _characters.Count ? 0 : _currentIndex + 1;
             CharacterSelection();
        } 

        public void CharacterSelectionLeft(){
            _currentIndex = _currentIndex - 1 < 0 ? _characters.Count - 1 : _currentIndex - 1;
            CharacterSelection();
        }

        private void Start(){
            CharacterSelection();
            _standardNav = button.navigation;
        }

        private void Update()
        {

            if(!eventSystem.currentSelectedGameObject.Equals(button.gameObject)){
                button.navigation = _standardNav;
                _clicked = false;
            }
                
        }

        public int GetIndex(){
            return _currentIndex;
        }

        public  List<CharacterShowCaseData> GetcharacterList(){
            return _characters;
        }

        public bool GetClicked(){
            return _clicked;
        }
    }
}
