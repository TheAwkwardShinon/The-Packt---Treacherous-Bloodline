using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ThePackt{
    public class TabGroup : MonoBehaviour
    {
        public List<tabButton> tabButtons;
        private tabButton _selectedTab;

        public List<GameObject> objectsToSwap;
        private Color _tabActive = Color.red;
        private Color _tabIdle = new Color(255,255,255,255);
        private Color _tabHover = Color.red;

        [SerializeField] private GameObject _tooltip;

        private PlayerInput inputSystem;

        [SerializeField] private EventSystem _eventSystem;

        [SerializeField] private List<Button> _firstSelectedOnTab;

        [SerializeField] private AudioSource _switchtabAudio;

        public void Subscribe(tabButton button){
            if(tabButtons == null)
                tabButtons = new List<tabButton>();
            tabButtons.Add(button);
        }

        public void OnTabSelected(tabButton button){
            _selectedTab = button;
            _switchtabAudio.Play();
            ResetTabs();
            button.background.color = _tabActive;
            int index = button.transform.GetSiblingIndex();
            for(int i=0;i< objectsToSwap.Count;i++){
                if(i == index){
                    
                    objectsToSwap[i].SetActive(true);
                    _eventSystem.SetSelectedGameObject(_firstSelectedOnTab[i].gameObject);
                }
                else{
                    objectsToSwap[i].SetActive(false);
                }
            }
            _tooltip.SetActive(false);
          
            
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

        
        public void ChangeTabLeft(){
             if(_selectedTab.transform.GetSiblingIndex() > 0){
                    OnTabSelected(tabButtons[tabButtons.IndexOf(_selectedTab)-1]);
            }
            else{
                OnTabSelected(tabButtons[tabButtons.Count-1]);
            }
        }

        public void ChangeTabRight(){
            if(_selectedTab.transform.GetSiblingIndex() < tabButtons.Count - 1){
                    
                    OnTabSelected(tabButtons[tabButtons.IndexOf(_selectedTab)+1]);
            }
            else{
                OnTabSelected(tabButtons[0]);

            }
        }
        private void Start(){
            inputSystem = GetComponent<PlayerInput>();
        }

      
        private void Update(){
            if(_selectedTab == null && tabButtons.Count > 0){
                OnTabSelected(tabButtons[0]);
            }
            
           
        }

 
        #region getters

        public tabButton GetCurrentTab(){
            return _selectedTab;
        }

        #endregion

    }
}
