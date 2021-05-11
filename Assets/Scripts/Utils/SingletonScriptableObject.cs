using System.Linq;
using UnityEngine;

namespace ThePackt{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        static T _instance = null;
    
        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    //_instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();

                    T[] assets = Resources.LoadAll<T>("");
                    _instance = assets[0];
                }
                    
                return _instance;
            }
        }
    }
}