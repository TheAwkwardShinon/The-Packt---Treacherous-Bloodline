using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ThePackt{
    public class LogoOnCharSelection : MonoBehaviour
    {
        [SerializeField] private List<Image> _images;

        public void LogoAppearOnClick(int index){
            for(int i = 0; i< _images.Count;i++){
                if(i==index){
                    _images[i].gameObject.SetActive(true);
                }
                else _images[i].gameObject.SetActive(false);
            }
        }

        public void Reset(){
            foreach(Image i in _images)
                i.gameObject.SetActive(false);
        }

        public bool isSomethingActive(){
            foreach(Image i in _images)
                if(i.IsActive())
                    return true;
            return false;
        }
        
        

        
    }
}
