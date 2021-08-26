using System;
using UnityEngine;

namespace ThePackt
{
    public class Utils
    {
        #region methods


        #endregion

        #region inner classes
        [Serializable]
        public class CustomVector3
        {
            public bool isNull;
            public Vector3 vector;
        }

        [Serializable]
        public class PrefabAssociation
        {
            public string name;
            public GameObject prefab;
        }

        [Serializable]
        public class PrefabIntegerAssociation
        {
            public int num;
            public GameObject prefab;
        }

        [Serializable]
        public class VectorAssociation
        {
            public string name;
            public Vector3 position;
        }

        [Serializable]
        public class DamageAssociation
        {
            public string name;
            public float damage;

            public DamageAssociation(string name, float damage)
            {
                this.name = name;
                this.damage = damage;
            }
        }
        #endregion
    }
}