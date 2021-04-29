using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        
    }
}
