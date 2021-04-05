using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//code design pattern sample: tell me if u like it or not.

namespace ThePackt{  //to be used in every class

    /* 
     * Models the all the playable characters
     */
    public class Werewolf : MonoBehaviour
    {
        public enum State {IDLE, MOVE, JUMP, ATTACK, CROUCH, CROUCH_MOVE, DASH, TRANSFORM}; //our state checker (to be updated with other states).
        //use it in order to make the code cleaner
        #region variables  
        protected Rigidbody2D _rb;
        protected Collider2D _col;
        protected Animator _anim;
        [SerializeField] private GameObject _sprite;
        [SerializeField] protected float _speed; 
        [SerializeField] protected float _dashMultiplier;
        [SerializeField] protected float _jumpForce;
        [SerializeField] protected float _extraHeight; // must be adjusted if the jump force is reduced, else there could be a double jump issue
        [SerializeField] protected float _powerBaseWerewolfAttack;
        [SerializeField] protected float _powerBaseHumanAttack;
        [SerializeField] protected float _rangeBaseWerewolfAttack;
        [SerializeField] protected Transform _attackPoint;
        [SerializeField] protected GameObject _bullet;
        // [SerializeField] protected GameObject prova;
        private Vector2 _direction;
        private bool _isFacingLeft = true;
        private bool _isGrounded = true;
        private bool _isOnEnemy = true;
        private bool _isHuman = true;

        private State _currentState = State.IDLE;

        private bool _isUsingSpecialAttack = false;
        private bool _isUsingItem = false;
        /**/


        #endregion


        #region methods 

        // Start is called before the first frame update
        private void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _col = gameObject.GetComponent<CapsuleCollider2D>();
        }


        private void Update()
        {
            CheckUnderFeet();
        }

        public void CheckUnderFeet()
        {

            LayerMask lm = LayerMask.GetMask("Ground", "Enemies");

            Vector2 boxCenter = _col.bounds.center + Vector3.down * _col.bounds.size.y * 0.5f;
            // prova.transform.position = boxCenter;
            Collider2D hit = Physics2D.OverlapBox(boxCenter, new Vector3(_col.bounds.size.x - 0.01f, _extraHeight, 0f), 0f, lm);

            _isGrounded = false;
            _isOnEnemy = false;
            if (hit != null)
            {
                if (LayerMask.LayerToName(hit.gameObject.layer) == "Ground")
                {
                    _isGrounded = true;
                }
                else if (LayerMask.LayerToName(hit.gameObject.layer) == "Enemies")
                {
                    _isOnEnemy = true;
                }
            }

            /* raycast implementation
            RaycastHit2D rayhit = Physics2D.BoxCast(_col.bounds.center, new Vector3(_col.bounds.size.x - 0.1f, _col.bounds.size.y, 0f), 0f, Vector2.down, _extraHeight, lm);
            if (rayhit.collider != null)
                Debug.Log(rayhit.collider.gameObject.name);

            _isGrounded = (rayhit.collider != null);
            */
            /* velocity implementation
            if (Mathf.Abs(_rb.velocity.y) == 0)
            {
                _isGrounded = true;
            }
            else _isGrounded = false;
            */
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(_attackPoint.position, _rangeBaseWerewolfAttack);
            Gizmos.color = Color.red;

            _col = gameObject.GetComponent<CapsuleCollider2D>();
            Vector2 temp = _col.bounds.center + Vector3.down * _col.bounds.size.y * 0.5f;
            Gizmos.DrawCube(temp, new Vector3(_col.bounds.size.x - 0.01f, _extraHeight, 0f));
        }

        #endregion




        #region getter

        public bool GetIsHuman()
        {
            return _isHuman;
        }

        public bool GetIsFacingLeft()
        {
            return _isFacingLeft;
        }

        public bool GetIsUsingSpecialAttack(){
            return _isUsingSpecialAttack;
        }

        public bool GetIsUsingItem(){
            return _isUsingItem;
        }

        public bool GetIsGrounded(){
            return _isGrounded;
        }

        public bool GetIsOnEnemy()
        {
            return _isOnEnemy;
        }

        public float GetSpeed(){
            return _speed;
        }

        public float GetJumpForce(){
            return _jumpForce;
        }

        public float GetDashMultiplier(){
            return _dashMultiplier;
        }

        public Vector2 GetDirection(){
            return _direction;
        }

        public GameObject GetBullet()
        {
            return _bullet;
        }

        public float GetRangeBaseWerewolfAttack()
        {
            return _rangeBaseWerewolfAttack;
        }

        public float GetPowerBaseWerewolfAttack()
        {
            return _powerBaseWerewolfAttack;
        }

        public float GetPowerBaseHumanAttack()
        {
            return _powerBaseHumanAttack;
        }

        public GameObject GetSprite(){
            return _sprite;
        }

        public State GetCurrentState(){
            return _currentState;
        }
        public Transform GetAttackPoint()
        {
            return _attackPoint;
        }

        #endregion 

        #region setter

        public void SetIsHuman(bool value)
        {
            _isHuman = value;
        }

        public void SetIsUsingSpecialAttack(bool value){
             _isUsingSpecialAttack = value;
        }

        public void SetIsUsingItem(bool value){
             _isUsingItem = value;
        }

        public void SetDirection(Vector2 direction){
            _direction = direction;
        }

        public void SetIsFacingLeft(bool value)
        {
            _isFacingLeft = value;
        }

        public void SetCurrentState(State s){
            _currentState = s;
        }
        #endregion
    }
}
