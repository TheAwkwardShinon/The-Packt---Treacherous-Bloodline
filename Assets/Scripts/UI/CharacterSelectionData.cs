using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    [CreateAssetMenu(fileName = "newCharacterData", menuName = "Data/Character Data/Selection Data")]
    public class CharacterSelectionData : ScriptableObject
    {
        private string _characterSelected;
        private string _playerNickname;

        public string GetCharacterSelected(){
            return _characterSelected;
        }

        public string GetNickname(){
            return _playerNickname;
        }

        public void SetNickname(string name){
            _playerNickname = name;
        }

        public void SetCharacterSelected(string character){
            _characterSelected = character;
        }
    }
}
