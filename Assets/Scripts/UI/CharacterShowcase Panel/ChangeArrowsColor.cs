using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ThePackt{
    public class ChangeArrowsColor : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Button _buttonArrow;
        [SerializeField] private EventSystem _eventSystem;

        private bool _selected = false;

        private bool _clicked = false;

        private void Update(){

            if(_eventSystem.currentSelectedGameObject.Equals(_button.gameObject))
                _selected = true;
            else _selected = false;

            if(_selected && !_clicked)
                this.GetComponent<Image>().color = Color.red;
             else if(_selected && _clicked ){
                this.GetComponent<Image>().color = Color.green;
            }else{
                if(_eventSystem.currentSelectedGameObject.Equals(_buttonArrow.gameObject))
                    this.GetComponent<Image>().color = Color.green;
                else this.GetComponent<Image>().color = Color.white;
                _clicked = false;
            }
        }

        public void SetClicked(){
           
            _clicked = _clicked == true ? false : true;
        }
    }
}
