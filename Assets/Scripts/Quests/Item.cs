using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class Item : Bolt.EntityBehaviour<IItemState>
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.PlayPickUpSFX();

                BoltNetwork.Destroy(gameObject);
            }
        }
    }
}
