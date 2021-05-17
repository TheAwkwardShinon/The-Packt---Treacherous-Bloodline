using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace ThePackt
{
    public class Enemy : Bolt.EntityBehaviour<ICustomEnemyState>
    {
        #region variables
        [SerializeField] protected float _health;
        [SerializeField] protected Canvas canvas;
        [SerializeField] protected GameObject healthBar;
        [SerializeField] protected Image healthImage;
        [SerializeField] protected Gradient healthGradient;
        private Slider healthSlider;
        #endregion

        #region methods

        public override void Attached()
        {
            // synchronize the bolt player state transform with the player gameobject transform
            state.SetTransforms(state.Transform, transform);

            if (BoltNetwork.IsServer)
            {
                state.Health = _health;
            }

            state.AddCallback("Health", HealthCallback);

            healthSlider = healthBar.GetComponent<Slider>();
            healthImage.color = healthGradient.Evaluate(1f);
            healthSlider.maxValue = _health;
        }

        private void Update()
        {
            canvas.transform.rotation = Quaternion.identity;
        }

        public void ApplyDamage(float damage)
        {
            Debug.Log("[ENEMY] apply damage: " + entity.IsOwner);

            if (BoltNetwork.IsServer)
            {
                state.Health -= damage;
            }
        }

        private void Die()
        {
            BoltNetwork.Destroy(gameObject);
        }

        //called when state.Health is modified -> we update the local health and do checks on it
        private void HealthCallback()
        {
            _health = state.Health;
            Debug.Log("[ENEMY] health callback. New _health: " + _health);

            healthSlider.value = _health;
            healthImage.color = healthGradient.Evaluate(healthSlider.normalizedValue);

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
