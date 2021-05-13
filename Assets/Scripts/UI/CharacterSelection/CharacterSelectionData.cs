using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    [CreateAssetMenu(fileName = "newCharacterData", menuName = "Data/Character Data/Selection Data")]
    public class CharacterSelectionData : SingletonScriptableObject<CharacterSelectionData>
    {
        private string _characterSelected = "none";
        private string _playerNickname = "none";
        private Player _playerScript = null;
        private string _disconnectReason = null;

        private int _characterIndex = 0;

        public string GetCharacterSelected(){
            return _characterSelected;
        }

        public string GetNickname(){
            return _playerNickname;
        }

        public string GetDisconnectReason()
        {
            return _disconnectReason;
        }

        public Player GetPlayerScript()
        {
            return _playerScript;
        }

        public int GetCharacterIndex(){
            return _characterIndex;
        }

        public void SetNickname(string name){
            _playerNickname = name;
        }

        public void SetCharacterIndex(int index){
            _characterIndex = index;
        }

        public void SetCharacterSelected(string character){
            _characterSelected = character;
        }

        public void SetPlayerScript(Player value)
        {
            _playerScript = value;
        }

        public void SetDisconnectReason(string value)
        {
            _disconnectReason = value;
        }


        public void Reset(){
            _characterSelected = "none";
            _playerNickname = "none";
            _characterIndex = 0;
            _playerScript = null;
            _disconnectReason = null;

        }
    }
}
