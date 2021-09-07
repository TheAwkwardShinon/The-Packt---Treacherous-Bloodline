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
        private string _sessionId = null;
        private int _timeDuration = 360;
        private int _playersNumber = 6;
        private bool _damageEnabled = true;
        private bool _fogEnabled = true;

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

        public string GetSessionId()
        {
            return _sessionId;
        }

        public int GetTimeDuration()
        {
            return _timeDuration;
        }

        public bool GetDamageEnabled()
        {
            return _damageEnabled;
        }

        public int GetPlayersNumber()
        {
            return _playersNumber;
        }

        public bool GetFogEnabled()
        {
            return _fogEnabled;
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

        public void SetSessionId(string value)
        {
            _sessionId = value;
        }

        public void SwitchFogEnabled()
        {
            _fogEnabled = !_fogEnabled;
        }

        public void SwitchDamageEnabled()
        {
            _damageEnabled = !_damageEnabled;
        }

        public void SetPlayersNumber(int value)
        {
            _playersNumber = value;
        }

        public void SetTimeDuration(int value)
        {
            _timeDuration = value;
        }

        public void Reset(){
            _characterSelected = "none";
            _playerNickname = "none";
            _playerScript = null;
            _disconnectReason = null;
            _sessionId = null;
        }
    }
}
