using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class EnemyBullet : Bullet
    {
        protected BaseEnemy _owner { get; private set; }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (entity.IsOwner)
            {
                if (LayerMask.LayerToName(collision.gameObject.layer) == "Players")
                {
                    PlayerHitReaction(collision);
                }

                // Does not destroy bullets on impact with other bullets or enemies
                if (!(LayerMask.LayerToName(collision.gameObject.layer) == "Bullets") && !(LayerMask.LayerToName(collision.gameObject.layer) == "Room") && !(LayerMask.LayerToName(collision.gameObject.layer) == "Enemies"))
                {
                    BoltNetwork.Destroy(gameObject);
                }
            }
        }

        // react to the hit of a player applying damage to that player
        protected void PlayerHitReaction(Collider2D collision)
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                if (player.entity.IsOwner)
                {
                    player.ApplyDamage(_attackPower);
                }
                else
                {
                    var evnt = PlayerAttackHitEvent.Create(player.entity.Source);
                    evnt.HitNetworkId = player.entity.NetworkId;
                    evnt.Damage = _attackPower;
                    evnt.Send();
                }

                if (_owner)
                {
                    _owner.RegisterTargetHit(player.entity);
                }
            }
        }

        public void SetOwner(BaseEnemy enemy)
        {
            _owner = enemy;
        }
    }
}