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
        private Vector2 _direction;
        private bool _isFacingLeft = true;
        private bool _isGrounded = true;
        private bool _isDashing = false;
        private bool _isMoving = false;
        private bool _isJumping = false;
        private bool _isCrouching = false;
        private bool _isUsingWereWolfBaseAttack = false;
        private bool _isUsingBaseHumanAttack = false;
        private bool _isUsingSpecialAttack = false;
        private bool _isUsingItem = false;

    
   
       

        #endregion


        #region methods


        #region characterController

        /* method description using '/*' */
        /* 
         * Implements the standard movement
         */
       
        private void Jump()  //method name always uppercase
        {
            Debug.Log("jumping");
            _isGrounded = false;
            _rb.AddForce(new Vector2(0,_jumpForce), ForceMode2D.Impulse);
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
            
        }


        private void UpdatePosition(Vector2 movement, float delta)
        { 
            if (movement.magnitude != 0)
                _rb.velocity = Vector3.zero; 
            //_rb.velocity = movement * _speed * delta;
            transform.position += new Vector3(movement.x,movement.y,0) * Time.deltaTime * _speed;
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
            if(_isFacingLeft){
                _sprite.transform.rotation = Quaternion.Euler(0,180,0);
            }
            else{
                _sprite.transform.rotation = Quaternion.Euler(0,0,0);     
            }
            _isFacingLeft = !_isFacingLeft;
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

        public bool GetIsUsingWereWolfBaseAttack(){
            return _isUsingWereWolfBaseAttack;
        }

        public bool GetUsingHumanBaseAttack(){
            return _isUsingBaseHumanAttack;
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

        public void SetIsUsingWereWolfBaseAttack(bool value){
             _isUsingWereWolfBaseAttack = value;
        }

        public void SetUsingHumanBaseAttack(bool value){
             _isUsingBaseHumanAttack = value;
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
