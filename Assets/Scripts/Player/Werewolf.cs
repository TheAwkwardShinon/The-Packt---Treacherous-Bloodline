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
        //use it in order to make the code cleaner
        #region variables  
        protected Rigidbody2D _rb; // put '_' before every protected or private variable
        protected Animator _anim;
        [SerializeField] private GameObject _sprite;
        [SerializeField] protected float _speed; //use serializefield if you want to initialize the variable from unity's inspector.

        [SerializeField] protected float _dashMultiplier;
        [SerializeField] protected float _jumpForce;
        [SerializeField] protected float _powerBaseWerewolfAttack;
        [SerializeField] protected float _rangeBaseWerewolfAttack;
        [SerializeField] protected Transform _attackPoint;
        private Vector2 _direction;
        private bool _isFacingLeft = true;
        private bool _isGrounded = true;
        private bool _isDashing = false;
        private bool _isMoving = false;
        private bool _isJumping = false;
        private bool _isCrouching = false;
        private bool _isUsingBaseWereWolfAttack = false;
        private bool _isUsingBaseHumanAttack = false;
        private bool _isUsingSpecialAttack = false;
        private bool _isUsingItem = false;
        private bool _isHuman = true;
        private bool _isTransformingToWereWolf = false;
        private bool _isTransformingToHuman = false;



        #endregion


        #region methods


        #region characterController

        /* method description using '/*' */
        /* 
         * Implements the standard jump
         */
        private void Jump()  //method name always uppercase
        {
            Debug.Log("jumping");
            _isGrounded = false;
            _rb.AddForce(transform.up * _jumpForce, ForceMode2D.Impulse);
        }

        private void BaseHumanAttack()  //method name always uppercase
        {
            Debug.Log("human attacking");
            //TODO
        }

        private void BaseWereWolfAttack()  //method name always uppercase
        {
            Debug.Log("werewolf attacking");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_attackPoint.position, _rangeBaseWerewolfAttack, 1 << LayerMask.NameToLayer("Enemies"));
            
            foreach(Collider2D enemy in hitEnemies)
            {
                Debug.Log(enemy.gameObject.name + " hit");
                enemy.gameObject.GetComponent<Enemy>().ApplyDamage(_powerBaseWerewolfAttack);
                Debug.Log(enemy.gameObject.name + " health: " + enemy.gameObject.GetComponent<Enemy>().GetHealth());
            }
        }

        private void UpdatePosition(Vector2 movement, float delta)
        {
            if (movement.magnitude != 0)
                _rb.velocity = Vector3.zero;
            //_rb.velocity = movement * _speed * delta;
            transform.position += new Vector3(movement.x, movement.y, 0) * Time.deltaTime * _speed;
            //_rb.MovePosition(_rb.position + movement * delta);       
        }

        private void UpdateSpriteDirection(Vector3 movement)
        {
            if (movement.x > 0 && _isFacingLeft ||
                movement.x < 0 && !_isFacingLeft)
                Flip();
        }

        private void Flip()
        {
            if (_isFacingLeft)
            {
                _sprite.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                _sprite.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            _isFacingLeft = !_isFacingLeft;
        }

        #endregion 

        // Start is called before the first frame update
        private void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            //to be done later :  _anim = gameObject.GetComponent<Animator>();
        }


        private void FixedUpdate()
        {
            if (Mathf.Abs(_rb.velocity.y) == 0)
            {
                _isGrounded = true;
            }

            if (_isDashing)
            {
                UpdateSpriteDirection(_direction);
                UpdatePosition(_direction * _speed * _dashMultiplier, Time.fixedDeltaTime);
                _isDashing = false;
            }
            else if (_isJumping){
                //UpdateSpriteDirection(_direction);
                Jump();
                _isJumping = false;
            }
            else if(_isMoving)
            {
                UpdateSpriteDirection(_direction);
                UpdatePosition(_direction * _speed, Time.fixedDeltaTime);
                _isMoving = false;
            }
            
            if (_isUsingBaseHumanAttack)
            {
                BaseHumanAttack();
                _isUsingBaseHumanAttack = false;
            }
            
            if (_isUsingBaseWereWolfAttack)
            {
                BaseWereWolfAttack();
                _isUsingBaseWereWolfAttack = false;
            }

        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(_attackPoint.position, _rangeBaseWerewolfAttack);
        }

        #endregion




        #region getter
        public bool GetIsDashing(){
            return _isDashing;
        }

        public bool GetIsJumping(){
            return _isJumping;
        }

        public bool GetIsMoving(){
            return _isMoving;
        }

        public bool GetIsCrouching(){
            return _isCrouching;
        }

        public bool GetIsHuman()
        {
            return _isHuman;
        }

        public bool GetIsUsingBaseWereWolfAttack(){
            return _isUsingBaseWereWolfAttack;
        }

        public bool GetIsUsingHumanBaseAttack(){
            return _isUsingBaseHumanAttack;
        }

        public bool GetIsTransformingToWereWolf()
        {
            return _isTransformingToWereWolf;
        }

        public bool GetIsTransformingToHuman()
        {
            return _isTransformingToHuman;
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

        #endregion 

        #region setter
        public void SetIsDashing(bool value){
            _isDashing = value;
        }

        public void SetIsJumping(bool value){
             _isJumping = value;
        }

        public void SetIsMoving(bool value){
            _isMoving = value;
        }

        public void SetIsCrouching(bool value){
             _isCrouching = value;
        }

        public void SetIsHuman(bool value)
        {
            _isHuman = value;
        }

        public void SetIsUsingBaseWereWolfAttack(bool value){
            _isUsingBaseWereWolfAttack = value;
        }

        public void SetIsUsingHumanBaseAttack(bool value){
             _isUsingBaseHumanAttack = value;
        }

        public void SetIsTransformingToWereWolf(bool value)
        {
            _isTransformingToWereWolf = value;
        }

        public void SetIsTransformingToHuman(bool value)
        {
            _isTransformingToHuman = value;
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
        #endregion
    }
}
