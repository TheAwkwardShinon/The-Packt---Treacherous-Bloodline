using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    [Serializable]
    public class Platform
    {
        public int num;
        public Transform left;
        public Transform right;

        public Vector2 GetPlatformCenter()
        {
            return new Vector2((left.position.x + right.position.x) / 2, left.position.y);
        }
    }
}
