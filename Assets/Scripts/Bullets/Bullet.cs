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

        protected Player _player;
        private Rigidbody2D _rb;
        private Vector2 _startPos;
        #endregion

        #region methods
        // executed when the player prefab is instatiated (quite as Start())

        private void Awake(){
            _player = GetComponent<Player>();
        }
        public override void Attached()
        {
            state.SetTransforms(state.Transform, transform);
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _rb.velocity = transform.right * _speed;
            _startPos = transform.position;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (Vector2.Distance(transform.position, _startPos) >= _range)
            {
                Destroy(gameObject);
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

        #endregion
    }

}
