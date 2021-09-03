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
        private Vector3 _direction;

        public override void Attached()
        {
            base.Attached();

            ChangeDirection();
        }

        public override void SimulateOwner()
        {
            base.SimulateOwner();

            //at each frame set the direction and attack
            SetDirection();
            Attack();

            if (!_stunned)
            {
                //then check if there is a wall in front
                CheckInFront();

                //finally move towards the facing direction
                if (_slowed)
                    _rb.velocity = new Vector2(_slowedSpeed * -transform.right.x, _slowedSpeed * -transform.right.y);
                else
                    _rb.velocity = new Vector2(_movementSpeed * -transform.right.x, _movementSpeed * -transform.right.y);
            }
            else
                _rb.velocity = Vector2.zero;
        }

        public override void Update()
        {
            base.Update();

            canvas.transform.rotation = transform.rotation;
        }

        /*
        private void OnDrawGizmos()
		{
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_col.bounds.center, -transform.right * _avoidRange);

            Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(_col.bounds.center, _attackRange);
		}
        */

        ///<summary>
        ///check if there is an obstacle in front, if so change direction
        ///</summary>
        private void CheckInFront()
        {
            RaycastHit2D[] hits = new RaycastHit2D[1];
            ContactFilter2D filter = new ContactFilter2D();
            
            filter.SetLayerMask(LayerMask.GetMask("Ground", "Wall", "EnemyInvisibleWall", "Enemies", "EnemyInvisibleGround"));

            int numHits = _col.Cast(-transform.right, filter, hits, _avoidRange, true);

            if (numHits > 0)
            {
                Debug.Log("[SKULL] change");
                ChangeDirection();
            }
        }

        ///<summary>
        ///change the facing direction
        ///</summary>
        private void ChangeDirection()
        {
            float randomZ = Random.Range(_minRotation, _maxRotation);
            Debug.Log("[SKULL] change " + randomZ);

            Quaternion angle = Quaternion.Euler(new Vector3(0, 0, randomZ));

            _direction = angle * transform.right;

            SetDirection();
        }

        ///<summary>
        ///set the facing direction to _direction
        ///</summary>
        private void SetDirection()
        {
            transform.right = _direction;
        }

        ///<summary>
        ///deal damage to all the players in _attackRange
        ///</summary>
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