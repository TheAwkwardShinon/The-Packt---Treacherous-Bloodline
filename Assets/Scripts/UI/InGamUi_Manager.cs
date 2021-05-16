using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ThePackt{
    public class InGamUi_Manager : MonoBehaviour
    {
        
        #region variables
        private TabGroup _tabgroup;
        private EventSystem _eventsystem;

        private GameObject _menuInGameUI;

        #endregion



        #region methods

      private void Start(){
            _tabgroup = GameObject.Find("Canvas").GetComponent<HiddenCanvas>().GetTabGroup();
            _eventsystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            _menuInGameUI = GameObject.Find("Canvas").GetComponent<HiddenCanvas>().GetMenu();

        }

        private void Update(){
            if(_tabgroup == null){
               _tabgroup = GameObject.Find("Canvas").GetComponent<HiddenCanvas>().GetTabGroup();
               _eventsystem = GameObject.Find("EventSystem").GetComponent<EventSystem>(); 
               _menuInGameUI = GameObject.Find("Canvas").GetComponent<HiddenCanvas>().GetMenu();
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
        

         public void OnDisableInGameUI(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                GetComponent<PlayerInput>().SwitchCurrentActionMap("Gameplay");
                _menuInGameUI.SetActive(false);
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


        

        #endregion
    }
}
