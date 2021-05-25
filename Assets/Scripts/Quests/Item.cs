using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class Item : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                BoltNetwork.Destroy(gameObject);
            }
        }
    }
}
