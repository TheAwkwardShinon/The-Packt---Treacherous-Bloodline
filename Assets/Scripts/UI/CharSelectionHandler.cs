using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class CharSelectionHandler : MonoBehaviour
    {
        [SerializeField] private  CharacterSelectionData _charSelectdata;

        void OnEnable()
        {
            _charSelectdata.Reset();
        }

        public void CharacterSetUp(int _index){
            _charSelectdata.SetNickname("Player-"+Random.Range(1,9999));
            switch(_index){
                case 0: _charSelectdata.SetCharacterSelected(Constants.MOONSIGHTERS);
                        break;
                case 1: _charSelectdata.SetCharacterSelected(Constants.FELE);
                        break;
                case 2: _charSelectdata.SetCharacterSelected(Constants.AYATANA);
                        break;
                case 3: _charSelectdata.SetCharacterSelected(Constants.NATURIA);
                        break;
                case 4: _charSelectdata.SetCharacterSelected(Constants.HERIN);
                        break;
                case 5: _charSelectdata.SetCharacterSelected(Constants.CEUIN);
                        break;
            }
        }
    }
}
