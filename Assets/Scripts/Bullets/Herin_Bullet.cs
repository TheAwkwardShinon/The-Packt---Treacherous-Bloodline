using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class Herin_Bullet : Bullet
    {
        [SerializeField] private float _slowValue;
        [SerializeField] private float _timeOfSlow;

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
        }

        protected override void Start()
        {
            base.Start();
        }


    }
}
