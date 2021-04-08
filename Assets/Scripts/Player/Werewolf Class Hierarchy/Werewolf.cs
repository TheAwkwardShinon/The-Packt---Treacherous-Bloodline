using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        #region core
        [SerializeField] protected float _speed; 
        [SerializeField] protected float _dashMultiplier;
        [SerializeField] protected float _jumpForce;
        [SerializeField] protected float _extraHeight; // must be adjusted if the jump force is reduced, else there could be a double jump issue
        [SerializeField] protected float _powerBaseWerewolfAttack;
        [SerializeField] protected float _powerBaseHumanAttack;
        [SerializeField] protected float _rangeBaseWerewolfAttack;
        [SerializeField] protected List<Cooldown.CooldownData> _cooldownTimes;
        [SerializeField] protected Transform _attackPoint;
        [SerializeField] protected GameObject _bullet;
        // [SerializeField] protected GameObject prova;
        protected Dictionary<string, float> _cooldownDict;
        private List<Cooldown> _cooldowns;
        private Vector2 _direction;
        public Vector2 _currentVelocity { get; private set; }
        public int _facingDirection { get; private set; }    
        private Vector2 _workspace;

        [SerializeField] private Transform _wallCheck;
        [SerializeField] private Transform _ledgeCheck;
        [SerializeField] private Transform _ceilingCheck;

        public newInputHandler _inputHandler { get; private set; }
        public Transform _dashDirectionIndicator { get; private set; }



        #endregion

        #region flags
        
        private bool _isGrounded = true;
        private bool _isOnEnemy = true;
        private bool _isHuman = true;
        private bool _isUsingSpecialAttack = false;
        private bool _isUsingItem = false;

        #endregion
        
        #region components
        public  Animator _anim {get; private set;} 

        protected Rigidbody2D _rb;
        protected BoxCollider2D _col;

        #endregion

        #region State Variables
        public PlayerStateMachine _stateMachine { get; private set; }
        public PlayerIdleState _idleState { get; private set; }
        public PlayerMoveState _moveState { get; private set; }

        public PlayerJumpState _jumpState { get; private set; }
        public PlayerInAirState _inAirState { get; private set; }

        public PlayerCrouchIdleState _crouchIdleState { get; private set; }
        public PlayerCrouchMoveState _crouchMoveState { get; private set; }
        public PlayerLandState _landState { get; private set; }
        public PlayerWallSlideState _wallSlideState { get; private set; }
        public PlayerDashState _dashState { get; private set; }
        /* to be implemented 

        
       
        public PlayerAttackState _attackState { get; private set; }
        public PlayerTransformState _transformState { get; private set; }
        public PlayerDetransformState _detransformState {get; private set;}
        */

        [SerializeField]
        private PlayerData _playerData;
        #endregion

        #region old (to be deleted)
        [SerializeField] private GameObject _sprite;
         private State _currentState = State.IDLE;
        public enum State {IDLE, MOVE, JUMP, ATTACK, CROUCH, CROUCH_MOVE, DASH, TRANSFORM}; //our state checker (to be updated with other states).

        private bool _isFacingLeft = true;

        #endregion

        #endregion

        // better to create a class containing these constants, expecially for cooldowns
        #region costants
        //action names
        private const string DASH = "dash";
        #endregion

        #region methods

        #region base methods 

        private void Awake(){
            _stateMachine = new PlayerStateMachine();
            _idleState = new PlayerIdleState(this,_stateMachine,_playerData,"idle");
            _moveState = new PlayerMoveState(this,_stateMachine,_playerData,"move");
            _jumpState = new PlayerJumpState(this, _stateMachine, _playerData, "jump");
            _inAirState = new PlayerInAirState(this, _stateMachine, _playerData, "jump");
            _crouchIdleState = new PlayerCrouchIdleState(this, _stateMachine, _playerData, "crouchIdle");
            _crouchMoveState = new PlayerCrouchMoveState(this, _stateMachine, _playerData, "crouchMove");
            _landState = new PlayerLandState(this, _stateMachine, _playerData, "land");
            _wallSlideState = new PlayerWallSlideState(this, _stateMachine, _playerData, "wallSlide");
            _dashState = new PlayerDashState(this, _stateMachine, _playerData, "dash");

           
            /* to be coded */

            /*
            
            
            
            _attackState = new PlayerAttackState(this, _stateMachine, _playerData, "attack");
            _transformState = new PlayerTransformState(this, _stateMachine, _playerData, "transformation");
            _detransformState = new PlayerDetransformState(this,_stateMachine,_playerData,"detransformation");
            */
        }

        private void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _col = gameObject.GetComponent<BoxCollider2D>();
            _anim = GetComponent<Animator>();
            _facingDirection = 1;
            _stateMachine.Initialize(_idleState);
            _cooldowns = new List<Cooldown>();
            _cooldownDict = new Dictionary<string, float>();

            foreach(Cooldown.CooldownData cd in _cooldownTimes){
                _cooldownDict.Add(cd.action, cd.time);
            }
        }


        private void Update()
        {
            CheckUnderFeet();
            _currentVelocity = _rb.velocity;
            _stateMachine._currentState.LogicUpdate();
        }

    
        #endregion


        #region velocity methods

        public void SetVelocityZero()
        {
            _rb.velocity = Vector2.zero;
            _currentVelocity = Vector2.zero;
        }
        public void SetVelocity(float velocity, Vector2 angle, int direction)
        {
            angle.Normalize();
            _workspace.Set(angle.x * velocity * direction, angle.y * velocity);
            _rb.velocity = _workspace;
            _currentVelocity = _workspace;
        }

        public void SetVelocity(float velocity, Vector2 direction)
        {
            _workspace = direction * velocity;
            _rb.velocity = _workspace;
            _currentVelocity = _workspace;
        }

        public void SetVelocityX(float velocity)
        {
            _workspace.Set(velocity, _currentVelocity.y);
            _rb.velocity = _workspace;
            _currentVelocity = _workspace;
        }

        public void SetVelocityY(float velocity)
        {
            _workspace.Set(_currentVelocity.x, velocity);
            _rb.velocity = _workspace;
            _currentVelocity = _workspace;
        }
        #endregion

        #region other methods

        public void CheckIfShouldFlip(int xInput)
        {
            if(xInput != 0 && xInput != _facingDirection)
            {
                Flip();
            }
        }


        private void Flip()
        {
            _facingDirection *= -1;
            transform.Rotate(0.0f, 180.0f, 0.0f);

            ProcessCooldowns();
        }

        public void ProcessCooldowns()
        {
            float deltaTime = Time.deltaTime;

            for (int i = _cooldowns.Count - 1; i >= 0; i--)
            {
                if (_cooldowns[i].DecrementCooldown(deltaTime))
                {
                    _cooldowns.RemoveAt(i);
                    // Debug.Log("removed cooldown");
                }
            }
        }

        public bool IsOnCooldown(string actionName)
        {
            foreach(Cooldown cd in _cooldowns)
            {
                if(cd.GetActionName() == actionName)
                {
                    return true;
                }
            }

            return false;
        }

        public void PutOnCooldown(string actionName)
        {
            // Debug.Log("put cooldown");
            _cooldowns.Add(new Cooldown(actionName, _cooldownDict[actionName]));
        }

        public void CheckUnderFeet()
        {

            LayerMask lm = LayerMask.GetMask("Ground", "Enemies");
            Vector2 boxCenter = _col.bounds.center + Vector3.down * _col.bounds.size.y * 0.5f;
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
        }


        public void SetColliderHeight(float height)
        {
            Vector2 center = _col.offset;
            _workspace.Set(_col.size.x, height);

            center.y += (height - _col.size.y) / 2;

            _col.size = _workspace;
            _col.offset = center;
        }

        public Vector2 DetermineCornerPosition()
        {
            RaycastHit2D xHit = Physics2D.Raycast(_wallCheck.position, Vector2.right * _facingDirection, _playerData.wallCheckDistance, _playerData.whatIsGround);
            float xDist = xHit.distance;
            _workspace.Set((xDist + 0.015f) * _facingDirection, 0f);
            RaycastHit2D yHit = Physics2D.Raycast(_ledgeCheck.position + (Vector3)(_workspace), Vector2.down, _ledgeCheck.position.y - _wallCheck.position.y + 0.015f, _playerData.whatIsGround);
            float yDist = yHit.distance;

            _workspace.Set(_wallCheck.position.x + (xDist * _facingDirection), _ledgeCheck.position.y - yDist);
            return _workspace;
        }

        #region checker
        public bool CheckForCeiling()
        {
            return Physics2D.OverlapCircle(_ceilingCheck.position, _playerData.groundCheckRadius, _playerData.whatIsGround);
        }


        public bool CheckIfTouchingWall()
        {
            return Physics2D.Raycast(_wallCheck.position, Vector2.right * _facingDirection, _playerData.wallCheckDistance, _playerData.whatIsGround);
        }

        public bool CheckIfTouchingLedge()
        {
            return Physics2D.Raycast(_ledgeCheck.position, Vector2.right * _facingDirection, _playerData.wallCheckDistance, _playerData.whatIsGround);
        }

        #endregion

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
        [MethodImpl(MethodImplOptions.Synchronized)]
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
        [MethodImpl(MethodImplOptions.Synchronized)]
        public State GetCurrentState(){
            return _currentState;
        }
        public Transform GetAttackPoint()
        {
            return _attackPoint;
        }

        public PlayerData GetPlayerData(){
            return _playerData;
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetDirection(Vector2 direction){
            _direction = direction;
        }

        public void SetIsFacingLeft(bool value)
        {
            _isFacingLeft = value;
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetCurrentState(State s){
            _currentState = s;
        }

        public void SetRigidBodyDrag(float drag){
            _rb.drag = drag;

        }
        #endregion
        
        #endregion
    }
}
