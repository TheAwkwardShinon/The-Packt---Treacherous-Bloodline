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

        #region tobeFilled
        [SerializeField] private Image _characterSprite;
        [SerializeField] private List<Image> _abilitiesImage;
        [SerializeField] private Text _lore;
        [SerializeField] private List<Text> _abilitiesDescription;

        #endregion

        
        private void Start()
        {
            button = GetComponent<Button>();

        }

        /* todo on button click */
        public void click(){
            if(!_isSelected) return;
            if(eventSystem.enabled == false){
                eventSystem.enabled = true;
                _clicked = false;
            }
            else{
                _clicked = true;
                eventSystem.enabled = false;
            }
        }


        public void OnChangeCharacterRight(InputAction.CallbackContext context){
            if (context.started)
            {
           
            }
            else if (context.canceled)
            {
            }
        }

        public void OnChangeCharacterLeft(InputAction.CallbackContext context){
             if (context.started)
            {
           
            }
            else if (context.canceled)
            {
            }
        }


        private void Update()
        {
            if(eventSystem.currentSelectedGameObject.Equals(button))
                _isSelected = true;
            else _isSelected = false;
        }
    }
}
