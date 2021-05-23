using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class Fele_Bullet : PlayerBullet
    {


        protected override void EnemyHitReaction(Collider2D collision)
        {
            base.EnemyHitReaction(collision);
        }

        protected override bool PlayerHitReaction(Collider2D collision)
        {
            return base.PlayerHitReaction(collision);
        }

        protected override void Update()
        {
            base.Update();
            //Debug.LogError("[PROJECTILE] MOVING AT : "+_speed+" SPEED");
        }

        protected override void Start()
        {
            base.Start();
        }


    }
}
