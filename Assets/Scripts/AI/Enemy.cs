using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace ThePackt
{
    public class Enemy : Bolt.EntityBehaviour<ICustomEnemyState>
    {
        #region variables
        [Header("General")]
        [SerializeField] protected float _health;
        [SerializeField] protected float _healthLevelIncrement; //increment of health for each level of the quest
        [SerializeField] protected Canvas canvas;
        [SerializeField] protected GameObject healthBar;
        [SerializeField] protected Image healthImage;
        [SerializeField] protected Gradient healthGradient;
        [SerializeField] protected float _attackRange;
        [SerializeField] protected float _attackPower;
        [SerializeField] protected float _attackRate;
        [SerializeField] protected float _movementSpeed;
        [SerializeField] protected float _slowedSpeed;

        protected int _facingDirection;
        protected Rigidbody2D _rb;
        protected Collider2D _col;
        protected Slider healthSlider;
        protected FSM _fsm;

        #region target selection
        protected BoltEntity _lastAttacker;
        protected float _lastTargetHitTime;
        protected Dictionary<BoltEntity,float> _damageMap;
        protected Dictionary<BoltEntity, float> _unreachableTargets;
        #endregion

        #region pathfinding
        protected Quest _room;
        protected Platform[] _platforms;
        #endregion

        #region ability effects
        protected float _lastStunTime;
        protected float _stunTime;
        protected bool _stunned;

        protected float _lastSlowTime;
        protected float _slowTime;
        protected bool _slowed;

        protected float _lastDamageReductionTime;
        protected float _damageReductionTime;
        protected bool _damageReduced;
        protected float _damageReductionValue;
        #endregion

        #region sfx
        [Header("SFX")]
        [SerializeField] protected GameObject _walkSfx;
        [SerializeField] protected GameObject _attackSfx;
        [SerializeField] protected GameObject _hurtSfx;
        #endregion

        #endregion

        #region methods

        protected virtual void Awake()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _col = gameObject.GetComponent<Collider2D>();
        }

        public override void Attached()
        {
            //synchronize the bolt enemy state transform with the enemy's gameobject transform
            state.SetTransforms(state.Transform, transform);

            //the server adjusts the initial health based on the difficulty level of the quest
            if (BoltNetwork.IsServer)
            {
                var token = (LevelDataToken) entity.AttachToken;

                if (token != null)
                {
                    _health += _healthLevelIncrement * (token._level - 1);
                    state.Health = _health;
                }
                else
                {
                    state.Health = _health;
                }

                _damageMap = new Dictionary<BoltEntity, float>();
                _unreachableTargets = new Dictionary<BoltEntity, float>();
            }

            state.AddCallback("Health", HealthCallback);

            healthSlider = healthBar.GetComponent<Slider>();
            healthImage.color = healthGradient.Evaluate(1f);
            healthSlider.maxValue = _health;
        }

        public virtual void Update()
        {
            if (entity.IsOwner)
            {
                //change _facingDirection based on the actual facing direction
                _facingDirection = 1;
                if (IsFacingLeft())
                {
                    _facingDirection = -1;
                }

                //debuff timeout timers
                if (_stunned && Time.time >= _lastStunTime + _stunTime)
                {
                    _stunned = false;
                }

                if (_slowed && Time.time >= _lastSlowTime + _slowTime)
                {
                    _slowed = false;
                }

                if (_damageReduced && Time.time >= _lastDamageReductionTime + _damageReductionTime)
                {
                    _damageReduced = false;
                }
            }

            canvas.transform.rotation = Quaternion.identity;
        }

        ///<summary>
        ///apply the specified damage to the health and increase the data of the total damage dealt by the specified attacker
        ///</summary>
        public void ApplyDamage(float damage, Player attacker)
        {
            Debug.Log("[ENEMY] apply damage: " + entity.IsOwner + ". From attacker: " + attacker.tag);

            if (BoltNetwork.IsServer)
            {
                //play hurt sound
                var evnt = PlayEnemySoundEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.ReliableOrdered);
                evnt.EntityID = entity.NetworkId;
                evnt.Sound = Constants.HURT;
                evnt.Send();

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

        ///<summary>
        ///destroys the attached gameobject
        ///</summary>
        private void Die()
        {
            //play death sound
            var evnt = PlayEnemySoundEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.ReliableOrdered);
            evnt.EntityID = entity.NetworkId;
            evnt.Sound = Constants.DEATH;
            evnt.Position = transform.position;
            evnt.Send();

            BoltNetwork.Destroy(gameObject);
        }

        ///<summary>
        ///called when health is modified
        ///</summary>
        private void HealthCallback()
        {
            _health = state.Health;
            Debug.Log("[ENEMY] health callback. New _health: " + _health);

            healthSlider.value = _health;
            healthImage.color = healthGradient.Evaluate(healthSlider.normalizedValue);

            //if health is now lower then zero i'm dead
            if (entity.IsOwner && _health <= 0)
            {
                Debug.Log("[ENEMY] dead");
                Die();
            }
        }

        ///<summary>
        ///deals damage to the specified hitPlayer
        ///</summary>
        protected void DealDamage(BoltEntity hitPlayer)
        {
            Debug.Log("[BASEENEMY] hit player " + hitPlayer.NetworkId);

            //if the damage is debuffed reduce it and clamp it if below minimum
            float damage = 0;
            if (_damageReduced)
                damage = _attackPower - _damageReductionValue;
            else damage = _attackPower;

            if(damage <= 0)
                damage = 0.1f;

            if (hitPlayer.IsOwner)
            {
                hitPlayer.GetComponent<Player>().ApplyDamage(damage);
            }
            else
            {
                var evnt = PlayerAttackHitEvent.Create(hitPlayer.Source);
                evnt.HitNetworkId = hitPlayer.NetworkId;
                evnt.Damage = damage;
                evnt.Send();
            }
        }

        ///<summary>
        ///returns true if the enemy is facing left, false otherwise
        ///</summary>
        protected bool IsFacingLeft()
        {
            if (transform.right.x > 0)
            {
                return true;
            }

            return false;
        }

        #region ability effects
        ///<summary>
        ///inflicts the slowed status to the enemy
        ///</summary>
        public void ApplySlow(float time)
        {
            _slowTime = time;
            _lastSlowTime = Time.time;
            _slowed = true;
        }

        ///<summary>
        ///inflicts the damage reduced status to the enemy
        ///</summary>
        public void ApplyDamageReduction(float time, float reductionValue)
        {
            _damageReductionTime = time;
            _lastDamageReductionTime = Time.time;
            _damageReduced = true;

            _damageReductionValue = reductionValue;
        }

        ///<summary>
        ///applies knockback to the enemy
        ///</summary>
        public void ApplyKnockback(Vector3 direction, float power)
        {
            _rb.AddForce(direction * power);
        }

        ///<summary>
        ///inflicts the stunned status to the enemy
        ///</summary>
        public void Stun(float time)
        {
            Debug.Log("[STUN]");
            _stunTime = time;
            _lastStunTime = Time.time;
            _stunned = true;
        }
        #endregion

        #region sound effects
        public void PlayWalkSFX()
        {
            _walkSfx.GetComponent<AudioSource>().Play();
        }

        public void PlayAttackSFX()
        {
            _attackSfx.GetComponent<AudioSource>().Play();
        }

        public void PlayHurtSFX()
        {
            _hurtSfx.GetComponent<AudioSource>().Play();
        }
        #endregion

        #endregion

        #region getter
        public float GetHealth()
        {
            return _health;
        }
        #endregion

        #region setter
        public void SetRoom(Quest value, Platform[] platforms)
        {
            _room = value;

            _platforms = platforms;
        }

        public void SetRoom(MainQuest value)
        {
            _platforms = value.GetPlatforms();
        }
        
        #endregion
    }
}