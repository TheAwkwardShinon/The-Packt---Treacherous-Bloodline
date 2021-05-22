using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

namespace ThePackt
{
    public class Bullet : Bolt.EntityBehaviour<ICustomBulletState>
    {
        #region variables
        [SerializeField] protected float _speed;
        [SerializeField] protected float _range;
        [SerializeField] protected float _attackPower;
        private Rigidbody2D _rb;
        private Vector2 _startPos;
        protected Player _owner { get; private set; }
        #endregion

        #region methods
        // executed when the player prefab is instatiated (quite as Start())
        public override void Attached()
        {
            state.SetTransforms(state.Transform, transform);
        }

        // Start is called before the first frame update
        void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _rb.velocity = transform.right * _speed;
            _startPos = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (Vector2.Distance(transform.position, _startPos) >= _range)
            {
                BoltNetwork.Destroy(gameObject);
            }
        }

        // react to the hit of an enemy applying damage to that enemy
        protected void EnemyHitReaction(Collider2D collision)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                EnemyAttackHitEvent evnt;

                // if we are on the server, directly apply the damage to the enemy
                // otherwise we sent an event to the server
                if (BoltNetwork.IsServer)
                {
                    Debug.Log("[NETWORKLOG] server hit enemy");
                    enemy.ApplyDamage(_attackPower, _owner);
                }
                else
                {
                    Debug.Log("[NETWORKLOG] from client to server");
                    evnt = EnemyAttackHitEvent.Create(BoltNetwork.Server);
                    evnt.HitNetworkId = enemy.entity.NetworkId;
                    evnt.AttackerNetworkId = _owner.entity.NetworkId;
                    evnt.Damage = _attackPower;
                    evnt.Send();
                }
            }
        }

        // react to the hit of an objective applying damage to that enemy
        protected void ObjectiveHitReaction(Collider2D collision)
        {
            Objective obj = collision.GetComponent<Objective>();
            if (obj != null)
            {
                ObjectiveHitEvent evnt;

                // if we are on the server, directly apply the damage to the objective
                // otherwise we sent an event to the server
                if (BoltNetwork.IsServer)
                {
                    Debug.Log("[NETWORKLOG] server hit objective");
                    obj.ApplyDamage(_attackPower);
                }
                else
                {
                    Debug.Log("[NETWORKLOG] from client to server");
                    evnt = ObjectiveHitEvent.Create(BoltNetwork.Server);
                    evnt.HitNetworkId = obj.entity.NetworkId;
                    evnt.Damage = _attackPower;
                    evnt.Send();
                }
            }
        }

        public void Die()
        {
            BoltNetwork.Destroy(gameObject);
        }

        #endregion

        #region getter

        public float GetSpeed()
        {
            return _speed;
        }

        public float GetAttackPower()
        {
            return _attackPower;
        }

        #endregion

        #region setter

        public void SetSpeed(float value)
        {
            _speed = value;
        }

        public void SetAttackPower(float value)
        {
            _attackPower = value;
        }

        public void SetOwner(Player plyr)
        {
            _owner = plyr;
        }

        #endregion
    }

}
