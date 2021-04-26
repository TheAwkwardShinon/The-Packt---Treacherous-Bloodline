using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class Enemy : Bolt.EntityBehaviour<ICustomEnemyState>
    {
        #region variables
        [SerializeField] protected float _health;
        #endregion

        #region methods
        public override void Attached()
        {
            // synchronize the bolt player state transform with the player gameobject transform
            state.SetTransforms(state.Transform, transform);

            state.Health = _health;
            state.AddCallback("Health", HealthCallback);
        }

        public void ApplyDamage(float damage)
        {
            Debug.Log("[ENEMY] apply damage: " + entity.IsOwner);

            state.Health -= damage;
        }

        private void Die()
        {
            Destroy(gameObject);
        }

        //called when state.Health is modified -> we update the local health and do checks on it
        private void HealthCallback()
        {
            _health = state.Health;
            Debug.Log("[ENEMY] health callback. New _health: " + _health);

            if (_health <= 0)
            {
                Debug.Log("[ENEMY] dead");
                Die();
            }
        }

        #endregion

        #region getter
        public float GetHealth()
        {
            return _health;
        }
        #endregion

        #region setter
        public void SetHealth(float value)
        {
            _health = value;
        }
        public uint getConnectionID()
        {
            return entity.Source.ConnectionId;
        }
        #endregion
    }
}
