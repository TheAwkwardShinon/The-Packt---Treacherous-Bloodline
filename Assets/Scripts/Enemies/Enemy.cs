using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class Enemy : MonoBehaviour
    {
        #region variables
        [SerializeField] protected float _health;
        #endregion

        #region methods

        // Start is called before the first frame update
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }

        public void ApplyDamage(float damage)
        {
            _health -= damage;

            if (_health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Bullet bullet;
            if (LayerMask.LayerToName(collision.gameObject.layer) == "Bullets")
            {
                bullet = collision.GetComponent<Bullet>();
                if (bullet != null)
                { 
                    ApplyDamage(bullet.GetAttackPower());
                    bullet.Die();
                }
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
        #endregion
    }
}
