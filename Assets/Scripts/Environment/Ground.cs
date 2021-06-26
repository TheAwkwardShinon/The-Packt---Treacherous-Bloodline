using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class Ground : MonoBehaviour
    {
        private void OnCollisionStay2D(Collision2D collision)
        {
            Debug.Log("[GLITCH] stay");
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 0.01f)); //
        }
    }
}

