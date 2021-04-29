using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    [CreateAssetMenu(fileName = "newCharacterData", menuName = "Data/Character Data/Selection Data")]
    public class CharacterSelectionData : ScriptableObject
    {
        private string _characterSelected = "none";
        private string _playerNickname = "none";
        private Player _playerScript = null;

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

        public Player GetPlayerScript()
        {
            return _playerScript;
        }

        public void SetPlayerScript(Player value)
        {
            _playerScript = value;
        }


        public void Reset(){
            _characterSelected = "none";
            _playerNickname = "none";
            _playerScript = null;
        }
    }
}
