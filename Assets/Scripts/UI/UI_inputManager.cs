using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    [SerializeField] private EventSystem _eventsystem;

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

        public void OnNavigateThroughUI(InputAction.CallbackContext context){
             if (context.started)
            {   
                try{
                    Vector2 _rawMovementInput = context.ReadValue<Vector2>();
                    int _normInputX = Mathf.RoundToInt(_rawMovementInput.x);
                    int _normInputY = Mathf.RoundToInt(_rawMovementInput.y);
                    Debug.Log("[ON NAVIGATION EVENT] INPUT X = "+_normInputX+" INPUT Y = "+_normInputY);
                    if(_normInputX > 0 && _normInputY == 0){
                        Debug.Log("[ON NAVIGATION EVENT] GOING RIGHT");
                        if(_eventsystem.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnRight.gameObject != null){
                            _eventsystem.SetSelectedGameObject(_eventsystem.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnRight.gameObject);
                            Debug.Log("[ON NAVIGATION EVENT] NEW ELEMENT SELECTED = "+_eventsystem.currentSelectedGameObject);
                        }
                    }
                    else if(_normInputX < 0 && _normInputY == 0){
                        Debug.Log("[ON NAVIGATION EVENT] GOING LEFT");
                        if(_eventsystem.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnLeft.gameObject != null){
                            _eventsystem.SetSelectedGameObject(_eventsystem.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnLeft.gameObject);
                            Debug.Log("[ON NAVIGATION EVENT] NEW ELEMENT SELECTED = "+_eventsystem.currentSelectedGameObject);
                        }
                    }
                    else if(_normInputX == 0 && _normInputY > 0){
                        Debug.Log("[ON NAVIGATION EVENT] GOING UP");
                        if(_eventsystem.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnUp.gameObject != null){
                            _eventsystem.SetSelectedGameObject(_eventsystem.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnUp.gameObject);
                            Debug.Log("[ON NAVIGATION EVENT] NEW ELEMENT SELECTED = "+_eventsystem.currentSelectedGameObject);
                        }
                    }
                    else if(_normInputX == 0 && _normInputY < 0){
                        Debug.Log("[ON NAVIGATION EVENT] GOING DOWN");
                        if(_eventsystem.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnDown.gameObject != null){
                            _eventsystem.SetSelectedGameObject(_eventsystem.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnDown.gameObject);
                            Debug.Log("[ON NAVIGATION EVENT] NEW ELEMENT SELECTED = "+_eventsystem.currentSelectedGameObject);
                        }
                    }
                    
                }catch(NullReferenceException e){
                    return;
                }
            }
            else if(context.canceled){

            }
        }

        
    }
}
