using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class SkullEnemy : Enemy
    {
        [SerializeField] private float _minRotation;
        [SerializeField] private float _maxRotation;
        private Vector2 _currentVelocity;
        private Rigidbody2D _rb;
        private Collider2D _col;
        private Vector2 _direction;

        public override void Attached()
        {
            base.Attached();

            _rb = gameObject.GetComponent<Rigidbody2D>();
            _col = gameObject.GetComponent<Collider2D>();

            ChangeDirection();
        }

        public override void SimulateOwner()
        {
            base.SimulateOwner();

            _currentVelocity = _rb.velocity;
            _rb.velocity = new Vector2(_movementSpeed, _currentVelocity.y);
        }

        private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(_col.bounds.center, _attackRange);
		}

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("[SKULL] collision");
       
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Wall") 
                || collision.gameObject.layer == LayerMask.NameToLayer("Objectives") || collision.gameObject.layer == LayerMask.NameToLayer("Ceiling")
                || collision.gameObject.layer == LayerMask.NameToLayer("EnemyInvisibleWall"))
            {
                ChangeDirection();
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Players"))
            {
                Player player = collision.gameObject.GetComponent<Player>();

                DealDamage(player.entity);
            }
        }

        private void ChangeDirection()
        {
            float randomZ = UnityEngine.Random.Range(_minRotation, _maxRotation);
            Debug.Log("[SKULL] change " + randomZ);
            transform.Rotate(new Vector3(0, 0, randomZ));
        }

    }
}
