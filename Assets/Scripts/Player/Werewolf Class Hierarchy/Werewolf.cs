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
        public enum State {IDLE, MOVE, JUMP, ATTACK, CROUCH, DASH, TRANSFORMING};
        //use it in order to make the code cleaner
        #region variables  
        protected Rigidbody2D _rb; // put '_' before every protected or private variable
        protected Animator _anim;
        [SerializeField] private GameObject _sprite;
        [SerializeField] protected float _speed; //use serializefield if you want to initialize the variable from unity's inspector.

        [SerializeField] protected float _dashMultiplier;
        [SerializeField] protected float _jumpForce;
        [SerializeField] protected float _powerBaseWerewolfAttack;
        [SerializeField] protected float _powerBaseHumanAttack;
        [SerializeField] protected float _rangeBaseWerewolfAttack;
        [SerializeField] protected Transform _attackPoint;
        [SerializeField] protected GameObject _bullet;
        private Vector2 _direction;
        private bool _isFacingLeft = true;
        private bool _isGrounded = true;
        private bool _isHuman = true;

        private State _currentState = State.IDLE;

        
        /*

        private bool _isIdle = true;
        private bool _isDashing = false;
        private bool _isMoving = false;
        private bool _isJumping = false;
        private bool _isCrouching = false;
        */
        private bool _isUsingBaseWereWolfAttack = false;
        private bool _isUsingBaseHumanAttack = false;
        private bool _isUsingSpecialAttack = false;
        private bool _isUsingItem = false;
      

        private bool _isTransformingToWereWolf = false;
        private bool _isTransformingToHuman = false;
        /**/


        #endregion


        #region methods


        #region characterController


        private void BaseHumanAttack()  //method name always uppercase
        {
            Debug.Log("human attacking");
            
            GameObject blt = Instantiate(_bullet, _attackPoint.position, _attackPoint.rotation);
            blt.GetComponent<Bullet>().SetAttackPower(_powerBaseHumanAttack);
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
        /*
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
        }*/

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
/*
        public bool GetIsIdle(){
            return _isIdle;
        }*/

        public bool GetIsGrounded(){
            return _isGrounded;
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

        public bool GetIsFacingLeft(){
            return _isFacingLeft;
        }

        public Vector2 GetDirection(){
            return _direction;
        }

        public GameObject GetSprite(){
            return _sprite;
        }

        public State GetCurrentState(){
            return _currentState;
        }

        #endregion 

        #region setter
        /*
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
*/
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
/*
        public void SetIsIdle(bool value){
            _isIdle = value;
        }*/

        public void SetDirection(Vector2 direction){
            _direction = direction;
        }

        public void SetIsFacingLeft(bool value){
            _isFacingLeft = value;
        }

        public void SetisGrounded(bool value){
            _isGrounded = value;
        }

        public void SetCurrentState(State s){
            _currentState = s;
        }
        #endregion
    }
}
