using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ThePackt{
    public class VerticalTabGroup : MonoBehaviour
    {
        #region variables
        [SerializeField] private List<GameObject> objectsToSwap;
        public List<tabVerticalButton> tabButtons;
        private tabVerticalButton _selectedTab;
        
        [SerializeField] private EventSystem _eventSystem;

        #endregion

        #region methods


        public void Subscribe(tabVerticalButton button){
            if(tabButtons == null)
                tabButtons = new List<tabVerticalButton>();
            tabButtons.Add(button);
        }

        public void OnTabSelected(tabVerticalButton button){
            _selectedTab = button;

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

        private void Update(){
            if(_selectedTab == null && tabButtons.Count > 0){
                OnTabSelected(tabButtons[0]);
            }
           
        }
        #endregion

    }
}
