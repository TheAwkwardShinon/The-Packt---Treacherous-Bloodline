using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class CharSelectionHandler : MonoBehaviour
    {
        [SerializeField] private  CharacterSelectionData _charSelectdata;
        

        public void CharacterSetUp(int _index){
            _charSelectdata.SetNickname("Player-"+Random.Range(1,9999));
            switch(_index){
                case 0: _charSelectdata.SetCharacterSelected("Moonsighters");
                        break;
                case 1: _charSelectdata.SetCharacterSelected("Fele");
                        break;
                case 2: _charSelectdata.SetCharacterSelected("Ayatana");
                        break;
                case 3: _charSelectdata.SetCharacterSelected("Naturia");
                        break;
                case 4: _charSelectdata.SetCharacterSelected("Herin");
                        break;
                case 5: _charSelectdata.SetCharacterSelected("Ceuin");
                        break;
            }
        }
    }
}
