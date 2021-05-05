using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ThePackt{
    public class VerticalTabGroup : MonoBehaviour
    {
        #region variables
        [SerializeField] private List<tabButton> tabButtons;
        [SerializeField] private List<GameObject> objectsToSwap;
        private tabButton _selectedTab;
        [SerializeField] private EventSystem _eventSystem;
        private int index = 0;

        #endregion

        #region methods


        public void OnTabSelected(tabButton button){
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
            if((index = tabButtons.IndexOf(_eventSystem.currentSelectedGameObject.GetComponent<tabButton>()))!= -1)
                OnTabSelected(_eventSystem.currentSelectedGameObject.GetComponent<tabButton>());
        }

        #endregion


    }
}
