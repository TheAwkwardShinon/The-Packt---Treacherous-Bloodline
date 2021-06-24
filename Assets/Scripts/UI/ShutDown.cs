using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class ShutDown : MonoBehaviour
    {
        public void ShutDownGame(){
            Application.Quit();
        }
    }
}