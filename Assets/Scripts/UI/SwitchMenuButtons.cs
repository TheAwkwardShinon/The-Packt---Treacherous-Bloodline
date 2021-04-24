using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ThePackt{
    public class SwitchMenuButtons : MonoBehaviour
    {
         private PlayerInput _inputSystem;
         public List<Sprite> gamepadSprite;
         public List<Sprite> keyboardSprite;
         public List<Image> iconsMenu;

        private bool _keyboardScheme = true;
        private bool _gamepadScheme = false;

        private void ChangeIconScheme(){
            int i = 0;
            if(_keyboardScheme){
                foreach(Image img in iconsMenu){
                    img.sprite = keyboardSprite[i];
                    i++;
                }
            }
            else{
                foreach(Image img in iconsMenu){
                    img.sprite = gamepadSprite[i];
                    i++;
                }
            }
        }

        private void Start()
        {
            _inputSystem = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            if(_inputSystem.currentControlScheme.Equals("Gamepad") && _keyboardScheme){
                _keyboardScheme = false;
                _gamepadScheme = true;
                ChangeIconScheme();
            }
            else if(_inputSystem.currentControlScheme.Equals("Keyboard") && _gamepadScheme){
                _gamepadScheme = false;
                _keyboardScheme = true;
                ChangeIconScheme();
            }

        }
    }
}
