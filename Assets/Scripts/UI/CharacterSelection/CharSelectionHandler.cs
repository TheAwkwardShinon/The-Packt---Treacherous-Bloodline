using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class CharSelectionHandler : MonoBehaviour
    {
        private CharacterSelectionData _charSelectdata;

        void OnEnable()
        {
            _charSelectdata = CharacterSelectionData.Instance;
            _charSelectdata.Reset();
        }

        public void CharacterSetUp(int _index){
            _charSelectdata.SetNickname("Player-"+Random.Range(1,9999));
            switch(_index){
                case 0: _charSelectdata.SetCharacterSelected(Constants.MOONSIGHTERS); //obviously in the final version we should remake this piece of code
                        _charSelectdata.SetCharacterIndex(14); //index for researcher
                        break;
                case 1: _charSelectdata.SetCharacterSelected(Constants.FELE);
                        _charSelectdata.SetCharacterIndex(15); //index for soldier
                        break;
                case 2: _charSelectdata.SetCharacterSelected(Constants.AYATANA);
                        _charSelectdata.SetCharacterIndex(17); //index for writer
                        break;
                case 3: _charSelectdata.SetCharacterSelected(Constants.NATURIA);
                        _charSelectdata.SetCharacterIndex(9); //index for herbalist
                        break;
                case 4: _charSelectdata.SetCharacterSelected(Constants.HERIN);
                        _charSelectdata.SetCharacterIndex(8); //index for anchor
                        break;
                case 5: _charSelectdata.SetCharacterSelected(Constants.CEUIN);
                        _charSelectdata.SetCharacterIndex(10); //index for lawyer
                        break;
            }
        }
    }
}
