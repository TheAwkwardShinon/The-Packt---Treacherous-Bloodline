using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class HiddenCanvas : MonoBehaviour
    {
        [SerializeField] private GameObject menu;
        [SerializeField] private GameObject firstPanel;

        [SerializeField] private tabButton _firstTab;

        public GameObject GetMenu(){
            return menu;
        }

        public GameObject GetFirstPanel(){
            return firstPanel;
        }

        public tabButton GetFirstTab(){
            return _firstTab;
        }

    }
}
