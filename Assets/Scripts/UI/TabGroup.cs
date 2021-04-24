using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ThePackt{
    public class TabGroup : MonoBehaviour
    {
        public List<tabButton> tabButtons;
        private tabButton _selectedTab;

        public List<GameObject> objectsToSwap;
        private Color _tabActive = Color.red;
        private Color _tabIdle = new Color(255,255,255,255);
        private Color _tabHover = Color.red;

        private PlayerInput inputSystem;

        public void Subscribe(tabButton button){
            if(tabButtons == null)
                tabButtons = new List<tabButton>();
            tabButtons.Add(button);
        }

        public void OnTabSelected(tabButton button){
            _selectedTab = button;
            ResetTabs();
            button.background.color = _tabActive;
            int index = button.transform.GetSiblingIndex();
            for(int i=0;i< objectsToSwap.Count;i++){
                if(i == index){
                    
                    objectsToSwap[i].SetActive(true);
                }
                else{
                    objectsToSwap[i].SetActive(false);
                }
            }
        }

        public void OnTabExit(tabButton button){
            ResetTabs();
        }

        public void OnTabEnter(tabButton button){
            ResetTabs();
            if(_selectedTab == null || button != _selectedTab)
                button.background.color = _tabHover;
        }

        public void ResetTabs(){
            foreach(tabButton button in tabButtons){
                if(_selectedTab != null && button == _selectedTab) continue;
                button.background.color = _tabIdle;
            }
        }

        public void OnChangeTabInputLeft(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if(_selectedTab.transform.GetSiblingIndex() > 0){
                    OnTabSelected(tabButtons[tabButtons.IndexOf(_selectedTab)-1]);
                }
                else{
                    OnTabSelected(tabButtons[tabButtons.Count-1]);
                }
            }
            else if(context.canceled){

            }
        }

        public void OnChangeTabInputRight(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if(_selectedTab.transform.GetSiblingIndex() < tabButtons.Count - 1){
                    OnTabSelected(tabButtons[tabButtons.IndexOf(_selectedTab)+1]);
                }
                else{
                    OnTabSelected(tabButtons[0]);
                }
            }
            else if(context.canceled){

            }
        }
        private void Start(){
            inputSystem = GetComponent<PlayerInput>();
        }

        private void Update(){
            if(_selectedTab == null && tabButtons.Count > 0)
                OnTabSelected(tabButtons[0]);
            if(inputSystem.currentControlScheme.Equals("Gamepad"))
                Debug.Log("gamepad detected");
            else if(inputSystem.currentControlScheme.Equals("Keyboard"))
                Debug.Log("keyboard detected");
        }

 

    }
}
