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
        public const string MAP = "MapScene";
        public const string MENU = "MenuScene";
        #endregion 

        #region quests
        public const int COMPLETED = 3;
        public const int FAILED = 2;
        public const int STARTED = 1;
        public const int READY = 0;
        #endregion 

        #region quest type
        public const int BULLET = 3;
        public const int ENEMY = 2;
        public const int ITEM = 1;
        public const int CHARGE = 0;
        #endregion 

        #region altar
        public const int NOTCHARGING = 0;
        public const int CHARGING = 1;
        #endregion

        #region enemy sound state
        public const int DEATH = 3;
        public const int ATTACK = 2;
        public const int WALK = 1;
        public const int HURT = 0;
        #endregion 

        public static readonly int[][] ADJACENCYMATRIXENEMYQUEST = { new int [] { -1, 1, 1, 1, 7, 7, 7, 7, 7, 7 },
                                                           new int [] { 0, -1, 2, 4, 4, 4, 7, 7, 7, 7 },
                                                           new int [] { 1, 1, -1, 1, 1, 1, 1, 1, 1, 1 },
                                                           new int [] { 0, 4, 4, -1, 4, 4, 4, 4, 4, 4 },
                                                           new int [] { 1, 1, 1, 3, -1, 5, 6, 6, 6, 6 },
                                                           new int [] { 4, 4, 4, 4, 4, -1, 4, 4, 4, 4 },
                                                           new int [] { 7, 7, 7, 4, 4, 4, -1, 7, 7, 7 },
                                                           new int [] { 0, 1, 1, 1, 6, 6, 6, -1, 8, 8 },
                                                           new int [] { 7, 7, 7, 7, 7, 7, 7, 7, -1, 9 },
                                                           new int [] { 8, 8, 8, 8, 8, 8, 8, 8, 8, -1 },
        };

        public static readonly int[][] ADJACENCYMATRIXMAINQUEST = { new int [] { -1, 1, 2, 1, 2, 1 },
                                                               new int [] { 0, -1, 2, 3, 3, 3 },
                                                               new int [] { 0, 1, -1, 3, 3, 3 },
                                                               new int [] { 2, 1, 2, -1, 4, 5 },
                                                               new int [] { 3, 3, 3, 3, -1, 3 },
                                                               new int [] { 3, 3, 3, 3, 3, -1 },
        };
    }
}