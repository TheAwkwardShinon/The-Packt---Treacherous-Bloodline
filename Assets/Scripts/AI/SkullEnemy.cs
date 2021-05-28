using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class SkullEnemy : Enemy
    {
        [Header("Specific")]
        [SerializeField] private float _minRotation;
        [SerializeField] private float _maxRotation;
        [SerializeField] private float _avoidRange;
        private Vector2 _currentVelocity;
        private Rigidbody2D _rb;
        private Collider2D _col;
        private Vector3 _direction;

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

            SetDirection();

            Attack();

            if (!_stunned)
            {
                CheckInFront();

                _currentVelocity = _rb.velocity;
                _rb.velocity = new Vector2(_movementSpeed * -transform.right.x, _movementSpeed * -transform.right.y);
            }
            else
            {
                _rb.velocity = Vector2.zero;
            }
        }

        public override void Update()
        {
            canvas.transform.rotation = transform.rotation;
        }

        private void OnDrawGizmos()
		{
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_col.bounds.center, -transform.right * _avoidRange);

            Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(_col.bounds.center, _attackRange);
		}

        /*
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
        */

        private void CheckInFront()
        {
            RaycastHit2D[] hits = new RaycastHit2D[1];
            ContactFilter2D filter = new ContactFilter2D();
            
            filter.SetLayerMask(LayerMask.GetMask("Ground", "Wall", "EnemyInvisibleWall", "Enemies", "EnemyInvisibleFloor"));

            int numHits = _col.Cast(-transform.right, filter, hits, _avoidRange, true);

            if (numHits > 0)
            {
                Debug.Log("[SKULL] change");
                ChangeDirection();
            }
        }

        private void ChangeDirection()
        {
            float randomZ = UnityEngine.Random.Range(_minRotation, _maxRotation);
            Debug.Log("[SKULL] change " + randomZ);

            Quaternion angle = Quaternion.Euler(new Vector3(0, 0, randomZ));

            _direction = angle * transform.right;

            SetDirection();

            //transform.Rotate(new Vector3(0, 0, randomZ));
        }

        private void SetDirection()
        {
            transform.right = _direction;

            /*
            if (transform.up.y < 0)
            {
                transform.up = -transform.up;
            }
            */
        }

        private void Attack()
        {
            Collider2D[] playersInRange = Physics2D.OverlapCircleAll(_col.bounds.center, _attackRange, LayerMask.GetMask("Players"));

            foreach (var col in playersInRange)
            {
                BoltEntity hitEntity = col.GetComponent<Player>().entity;
                DealDamage(hitEntity);
            }
        }

    }
}
