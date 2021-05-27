using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

namespace ThePackt
{
    public class Enemy : Bolt.EntityBehaviour<ICustomEnemyState>
    {
        #region variables
        [Header("General")]
        [SerializeField] protected float _health;
        [SerializeField] protected Canvas canvas;
        [SerializeField] protected GameObject healthBar;
        [SerializeField] protected Image healthImage;
        [SerializeField] protected Gradient healthGradient;
        [SerializeField] protected float _attackRange;
        [SerializeField] protected float _attackPower;
        [SerializeField] protected float _attackRate;
        [SerializeField] protected float _movementSpeed;
        protected int _facingDirection;
        protected Slider healthSlider;
        //protected Vector3 _canvasPos;
        protected FSM _fsm;

        protected BoltEntity _lastAttacker;
        protected Dictionary<BoltEntity,float> _damageMap;
        protected Dictionary<BoltEntity, float> _hitTimeMap;
        protected Quest _room;
        #endregion

        #region methods

        public override void Attached()
        {
            // synchronize the bolt player state transform with the player gameobject transform
            state.SetTransforms(state.Transform, transform);

            if (BoltNetwork.IsServer)
            {
                state.Health = _health;

                _damageMap = new Dictionary<BoltEntity, float>();
                _hitTimeMap = new Dictionary<BoltEntity, float>();
            }

            state.AddCallback("Health", HealthCallback);

            healthSlider = healthBar.GetComponent<Slider>();
            healthImage.color = healthGradient.Evaluate(1f);
            healthSlider.maxValue = _health;
            //_canvasPos = canvas.transform.localPosition;
        }

        public virtual void Update()
        {
            if (entity.IsOwner)
            {
                _facingDirection = 1;
                if (IsFacingLeft())
                {
                    _facingDirection = -1;
                }
            }

            canvas.transform.rotation = Quaternion.identity;
            //canvas.transform.localPosition = _canvasPos;
        }

        public void ApplyDamage(float damage, Player attacker)
        {
            Debug.Log("[ENEMY] apply damage: " + entity.IsOwner + ". From attacker: " + attacker.tag);

            if (BoltNetwork.IsServer)
            {
                state.Health -= damage;
                _lastAttacker = attacker.entity;

                if (_damageMap.ContainsKey(attacker.entity))
                {
                    _damageMap[attacker.entity] += damage;
                }
                else
                {
                    _damageMap.Add(attacker.entity, damage);
                }
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

        protected void DealDamage(BoltEntity hitPlayer)
        {
            Debug.Log("[BASEENEMY] hit player " + hitPlayer.NetworkId);

            if (hitPlayer.IsOwner)
            {
                hitPlayer.GetComponent<Player>().ApplyDamage(_attackPower);
            }
            else
            {
                var evnt = PlayerAttackHitEvent.Create(hitPlayer.Source);
                evnt.HitNetworkId = hitPlayer.NetworkId;
                evnt.Damage = _attackPower;
                evnt.Send();
            }
        }

        protected bool IsFacingLeft()
        {
            if (transform.right.x > 0)
            {
                return true;
            }

            return false;
        }

        protected void SetHitTime(BoltEntity hitEntity)
        {
            if (_hitTimeMap.ContainsKey(hitEntity))
            {
                _hitTimeMap[hitEntity] = Time.time;
            }
            else
            {
                _hitTimeMap.Add(hitEntity, Time.time);
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

        public void SetRoom(Quest value)
        {
            _room = value;
        }
        #endregion
    }
}
