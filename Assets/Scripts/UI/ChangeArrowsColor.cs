using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ThePackt{
    public class ChangeArrowsColor : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private EventSystem _eventSystem;

        private bool _selected = false;

        private void Update(){
            if(_eventSystem.currentSelectedGameObject.Equals(_button.gameObject) && !_selected){
                this.GetComponent<Image>().color = Color.red;
                _selected = true;
            }
            else if(!_eventSystem.currentSelectedGameObject.Equals(_button.gameObject) && _selected){
                _selected = false;
                this.GetComponent<Image>().color = Color.white;
            }
        }
    }
}
