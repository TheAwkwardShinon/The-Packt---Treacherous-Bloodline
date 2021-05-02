using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


namespace ThePackt{
    public class UI_inputManager : MonoBehaviour
    {
    [SerializeField] private GameObject _warningTooltip;
    [SerializeField] private GameObject _infoTooltip;
    [SerializeField] private Button _charShowcaseButton;

    [SerializeField] private TabGroup _tabgroup;

    [SerializeField] private GameObject _loreTab;

    [SerializeField] private GameObject _playTab;

    [SerializeField] private CharacterShowcaseManager _showcaseManager;

    [SerializeField] private CharacterSelectionData _playerInformationData;

    [SerializeField] private LogoOnCharSelection _logoSelectionHandler;

    //[SerializeField] private Button 


        public void OnChangeCharacterRight(InputAction.CallbackContext context){
                if (context.started)
                {
                    if(_tabgroup.GetCurrentTab().gameObject.Equals(_loreTab)){
                        if(_showcaseManager.GetClicked()){
                            _showcaseManager.CharacterSelectionRight();
                        }
                    }
                }
                else if (context.canceled)
                {
                }
            }

            public void OnChangeCharacterLeft(InputAction.CallbackContext context){
                if (context.started)
                {
                    if(_tabgroup.GetCurrentTab().gameObject.Equals(_loreTab)){
                        if(_showcaseManager.GetClicked()){
                            _showcaseManager.CharacterSelectionLeft();
                        }
                    }
                }
                else if (context.canceled)
                {
                }
            }

            public void OnDeselect(InputAction.CallbackContext context){
                if (context.started)
                {
                   
                    if(_tabgroup.GetCurrentTab().gameObject.Equals(_loreTab)){
                        if(_showcaseManager.GetClicked()){
                           _charShowcaseButton.onClick.Invoke();
                        }
                    }
                    else if(_tabgroup.GetCurrentTab().gameObject.Equals(_playTab)){
                        _logoSelectionHandler.Reset();
                        _playerInformationData.Reset();
                    }
                }
                else if (context.canceled)
                {
                }
            }

            public void OnChangeTabInputLeft(InputAction.CallbackContext context)
            {
                if (context.started)
                {
                    _tabgroup.ChangeTabLeft();    
                }
                else if(context.canceled){

                }
            }

        public void OnChangeTabInputRight(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _tabgroup.ChangeTabRight();
            }
            else if(context.canceled){

            }
        }

    
    }
}
