using UnityEditor;
using UnityEngine;

namespace ThePackt
{
    public class Constants
    {
        #region attacks
        public const string BASE = "base";
        #endregion

        #region factions
        public const string CEUIN = "ceuin";
        public const string MOONSIGHTERS = "moonsighters";
        public const string HERIN = "herin";
        public const string NATURIA = "naturia";
        public const string FELE = "fele";
        public const string AYATANA = "ayatana";
        #endregion 

        #region scenes
        public const string LOBBY = "LobbyScene";
        public const string MAP = "OldMapScene";
        public const string MENU = "MenuScene";
        #endregion 

        #region quests
        public const int COMPLETED = 3;
        public const int FAILED = 2;
        public const int STARTED = 1;
        public const int READY = 0;
        #endregion 

        #region enemy sound state
        public const int DEATH = 3;
        public const int ATTACK = 2;
        public const int WALK = 1;
        public const int HURT = 0;
        #endregion 
    }
}