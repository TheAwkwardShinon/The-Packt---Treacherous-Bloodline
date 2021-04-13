using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class Player : MonoBehaviour
    {
        #region variables

        #region core
        
        public newInputHandler _inputHandler { get; private set; }

        [Header("Core Data Fields")]
        [SerializeField] protected PlayerData _playerData;
        [SerializeField] private Transform _wallCheck;
        [SerializeField] private Transform _ledgeCheck;
        [SerializeField] private Transform _ceilingCheck;
        [SerializeField] protected Transform _attackPoint;
        [SerializeField] protected GameObject _bullet;

        #endregion


        #region velocity
        public Vector2 _currentVelocity { get; private set; }
        private Vector2 _workspace;
        public int _facingDirection { get; private set; }    

        #endregion

        #region flags
        private bool _isHuman = true;

        #endregion

        #region components
        public  Animator _anim {get; private set;} 
        protected Rigidbody2D _rb;
        protected BoxCollider2D _col;

        #endregion

        #region state variables
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

        public PlayerAttackState _attackState {get; private set;}

        #endregion

        #endregion



        #region methods

        #region core methods

        /* initialize every "common" possible state in the fsm */
        private void Awake(){
            _stateMachine = new PlayerStateMachine();
            _idleState = new PlayerIdleState(this,_stateMachine,_playerData,"Idle");
            _moveState = new PlayerMoveState(this,_stateMachine,_playerData,"Move");
            _jumpState = new PlayerJumpState(this, _stateMachine, _playerData, "InAir");
            _inAirState = new PlayerInAirState(this, _stateMachine, _playerData, "InAir");
            _crouchIdleState = new PlayerCrouchIdleState(this, _stateMachine, _playerData, "CrouchIdle");
            _crouchMoveState = new PlayerCrouchMoveState(this, _stateMachine, _playerData, "CrouchMove");
            _landState = new PlayerLandState(this, _stateMachine, _playerData, "Land");
            _wallSlideState = new PlayerWallSlideState(this, _stateMachine, _playerData, "WallSlide");
            _dashState = new PlayerDashState(this, _stateMachine, _playerData, "Dash");
            _attackState = new PlayerAttackState(this,_stateMachine, _playerData, "attack");
        }


        /* get core components and initialize the fsm */
        private void Start(){
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _col = gameObject.GetComponent<BoxCollider2D>();
            _anim = GetComponent<Animator>();
            _inputHandler = GetComponent<newInputHandler>();
            _facingDirection = -1;
            _stateMachine.Initialize(_idleState);
        }

        /* at every frame set the current velocity and update the current state */
        private void Update()
        {
            _currentVelocity = _rb.velocity;
            _stateMachine._currentState.LogicUpdate();
        }

        #endregion

        #region velocity modificator


        /* A velocity in Unity is units per second. 
        The units are often thought of as metres but could be millimetres or light years.
        Unity velocity also has the speed in X, Y, and Z defining the direction.*/

        /* set the velocity to zero */
        public void SetVelocityZero()
        {
            _rb.velocity = Vector2.zero;
            _currentVelocity = Vector2.zero;
        }

        /* set the velocity considering the speed, the direction (left or right) and the angle on the x axis */
        public void SetVelocity(float velocity, Vector2 angle, int direction)
        {
            angle.Normalize();
            _workspace.Set(angle.x * velocity * direction, angle.y * velocity);
            _rb.velocity = _workspace;
            _currentVelocity = _workspace;
        }

        /* set the velocity considering the speed and the direction (left or right) */
        public void SetVelocity(float velocity, Vector2 direction)
        {
            _workspace = direction * velocity;
            _rb.velocity = _workspace;
            _currentVelocity = _workspace;
        }

        /* set the velocity only in the x axis */
        public void SetVelocityX(float velocity)
        {
            _workspace.Set(velocity, _currentVelocity.y);
            _rb.velocity = _workspace;
            _currentVelocity = _workspace;
        }

        /* set the velocity only in the y axis */
        public void SetVelocityY(float velocity)
        {
            _workspace.Set(_currentVelocity.x, velocity);
            _rb.velocity = _workspace;
            _currentVelocity = _workspace;
        }

        #endregion

        #region check methods

        /* method that returns true if the player is touching a ceiling with his head, false instead */
        public bool CheckForCeiling()
        {
            return Physics2D.OverlapCircle(_ceilingCheck.position, _playerData.groundCheckRadius, _playerData.whatIsGround);
        }

        /* method that returns true if the player is grounded, false instead */
         public bool CheckIfGrounded()
        {   
            _workspace = _col.bounds.center + Vector3.down * _col.bounds.size.y * 0.5f;
            return Physics2D.OverlapBox(_workspace, new Vector3(_col.bounds.size.x - 0.01f, 0.1f, 0f), 0f, _playerData.whatIsGround);
        }

        /* method that returns true if the player is touching a wall (in the direction that he's facing), false instead */
        public bool CheckIfTouchingWall()
        {
            return Physics2D.Raycast(_wallCheck.position, Vector2.right * _facingDirection, _playerData.wallCheckDistance, _playerData.whatIsGround);
        }

        /* method that returns true if the player is very close to a ledge (and he is heading to it), false instead */
        public bool CheckIfTouchingLedge()
        {
            return Physics2D.Raycast(_ledgeCheck.position, Vector2.right * _facingDirection, _playerData.wallCheckDistance, _playerData.whatIsGround);
        }

        /* method that flip the player if he's moving in the opposite direction that he's facing */
         public void CheckIfShouldFlip(int xInput)
        {
            if(xInput != 0 && xInput != _facingDirection)
            {
                Flip();
            }
        }

        /* method that flips the player */
        private void Flip()
        {
            _facingDirection *= -1;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }


        #endregion


        #region triggers

        private void AnimationTrigger() => _stateMachine._currentState.AnimationTrigger();

        private void AnimtionFinishTrigger() => _stateMachine._currentState.AnimationFinishTrigger();

        #endregion


        #region getter
        public Transform GetAttackPoint()
        {
            return _attackPoint;
        }
        public GameObject GetBullet()
        {
            return _bullet;
        }

        public PlayerData GetPlayerData(){
            return _playerData;
        }
        public bool GetIsHuman()
        {
            return _isHuman;
        }

        #endregion

        #region setter

        /* method that sets the height of the collider, useful in the crouch states */
        public void SetColliderHeight(float height)
        {
            Vector2 center = _col.offset;
            _workspace.Set(_col.size.x, height);

            center.y += (height - _col.size.y) / 2;

            _col.size = _workspace;
            _col.offset = center;
        }

        public void SetIsHuman(bool value)
        {
            _isHuman = value;
        }
        public void SetRigidBodyDrag(float drag){
            _rb.drag = drag;

        }

        #endregion




        #endregion


    }
}
