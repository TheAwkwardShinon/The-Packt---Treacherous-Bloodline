using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class ActivationTrigger : MonoBehaviour
    {
        public void Deactivate(){
            gameObject.SetActive(false);
        }
    }
}
