using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

namespace ThePackt{
    public class Player : Bolt.EntityBehaviour<ICustomPlayerState>
    {
        #region variables

        #region core
        
        public newInputHandler _inputHandler { get; private set; }
        private PlayerInput _playerInput;
        private PlayerData _playerData;

        [Header("Core Data Fields")]
        [SerializeField] protected PlayerData _playerBaseData;
        [SerializeField] private Transform _wallCheck;
        [SerializeField] private Transform _ledgeCheck;
        [SerializeField] private Transform _ceilingCheck;
        [SerializeField] protected Transform _attackPoint;
        [SerializeField] protected GameObject _bullet;

        #endregion

        #region UI
        private Slider healthSlider;
        [SerializeField] protected Canvas canvas;
        [SerializeField] protected Text nicknameText;
        [SerializeField] protected GameObject healthBar;
        [SerializeField] protected Image healthImage;
        [SerializeField] protected Gradient healthGradient;
        protected CharacterSelectionData _selectedData;
        #endregion

        #region velocity
        public Vector2 _currentVelocity { get; private set; }
        private Vector2 _workspace;
        public int _facingDirection { get; private set; }    

        #endregion

        #region flags
        protected bool _isHuman = true;
        protected bool weakActive = false;
        protected bool mediumActive = false;
        protected bool attackModifier = false;
        protected bool hasActiveQuest = false;

        protected bool passive1 = false;
        protected bool passive2 = false;

        protected bool passive3 = false;

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

        public PlayerTransformationState _transformState {get; private set;}
        public PlayerDetransformationState _detransformationState {get; private set;}

        public PlayerDownState _downState {get; private set;}
        public PlayerDownMoveState _downMoveState {get; private set;}

        public PlayerInteractState _interactState {get; private set;}


        #endregion

        #region other
        protected Quest _activeQuest;
        protected float _spendableExp = 0;
        protected float _spendableTime = 0;
        #endregion

        #endregion



        #region methods

        #region core methods

        ///<summary>
        ///initialize every "common" possible state in the fsm
        ///</summary>
        public override void Initialized()
        {
            _selectedData = CharacterSelectionData.Instance;
            _playerData = Instantiate(_playerBaseData);
            _stateMachine = new PlayerStateMachine();
            _idleState = new PlayerIdleState(this, _stateMachine, _playerData, "Idle");
            _moveState = new PlayerMoveState(this, _stateMachine, _playerData, "Move");
            _jumpState = new PlayerJumpState(this, _stateMachine, _playerData, "InAir");
            _inAirState = new PlayerInAirState(this, _stateMachine, _playerData, "InAir");
            _crouchIdleState = new PlayerCrouchIdleState(this, _stateMachine, _playerData, "CrouchIdle");
            _crouchMoveState = new PlayerCrouchMoveState(this, _stateMachine, _playerData, "CrouchMove");
            _landState = new PlayerLandState(this, _stateMachine, _playerData, "Land");
            _wallSlideState = new PlayerWallSlideState(this, _stateMachine, _playerData, "WallSlide");
            _dashState = new PlayerDashState(this, _stateMachine, _playerData, "Dash");
            _attackState = new PlayerAttackState(this, _stateMachine, _playerData, "attack");
            _transformState = new PlayerTransformationState(this, _stateMachine, _playerData, "transformation");
            _detransformationState = new PlayerDetransformationState(this, _stateMachine, _playerData, "transformation");
            _downMoveState = new PlayerDownMoveState(this, _stateMachine, _playerData, "DownMove");
            _downState = new PlayerDownState(this, _stateMachine, _playerData, "Down");
            _interactState = new PlayerInteractState(this, _stateMachine, _playerData, "Interact");
        }

        ///<summary>
        /// executed when the player prefab is instatiated (quite as Start())
        /// </summary>
        public override void Attached()
        {
            if(entity.IsOwner)
               Debug.Log("[NETWORKLOG] my network id: " + entity.NetworkId);
            else
                Debug.Log("[NETWORKLOG] other network id: " + entity.NetworkId);
            //get core components and initialize the fsm
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _col = gameObject.GetComponent<BoxCollider2D>();
            _playerInput = GetComponent<PlayerInput>();
            _anim = GetComponent<Animator>();
            _facingDirection = -1;
            _stateMachine.Initialize(_idleState);

            //disable input handling if the player is not the owner
            if (!entity.IsOwner)
            {
                _playerInput.enabled = false;
            }
            else
            {
                _inputHandler = GetComponent<newInputHandler>();
                Camera.main.GetComponent<CameraFollow>().SetFollowTransform(transform);
            }

            _playerData.currentLifePoints = _playerData.maxLifePoints;
            if (entity.IsOwner)
            {
                state.Health = _playerData.currentLifePoints;
                state.Nickname = _selectedData.GetNickname();
            }

            state.AddCallback("Health", HealthCallback);
            state.AddCallback("Nickname", NicknameCallback);

            healthSlider = healthBar.GetComponent<Slider>();
            healthImage.color = healthGradient.Evaluate(1f);
            healthSlider.maxValue = _playerData.maxLifePoints;

            // synchronize the bolt player state transform with the player gameobject transform
            state.SetTransforms(state.Transform, transform);

            state.SetAnimator(_anim);
        }

        ///<summary>
        ///executed at every frame as Update(), but called only on the owner's computer
        ///</summary>
        public override void SimulateOwner()
        {

            // at every frame set the current velocity and update the current state
            _currentVelocity = _rb.velocity;
            if (_inputHandler != null)
            {
                _stateMachine._currentState.LogicUpdate();
            }

            healthSlider.value = _playerData.currentLifePoints;
            healthImage.color = healthGradient.Evaluate(healthSlider.normalizedValue);
        }

        private void Update()
        {
            if (!entity.IsOwner)
            {
                canvas.transform.rotation = Quaternion.identity;
            }
        }

        #endregion

        #region velocity modificator

            
        /* A velocity in Unity is units per second. 
        The units are often thought of as metres but could be millimetres or light years.
        Unity velocity also has the speed in X, Y, and Z defining the direction.*/

        ///<summary>
        ///set the velocity to zero
        ///</summary>
        public void SetVelocityZero()
        {
            _rb.velocity = Vector2.zero;
            _currentVelocity = Vector2.zero;
        }

        ///<summary>
        ///set the velocity considering the speed, the direction (left or right) and the angle on the x axis
        ///</summary>
        public void SetVelocity(float velocity, Vector2 angle, int direction)
        {
            angle.Normalize();
            _workspace.Set(angle.x * velocity * direction, angle.y * velocity);
            _rb.velocity = _workspace;
            _currentVelocity = _workspace;
        }

        ///<summary>
        ///set the velocity considering the speed and the direction (left or right)
        ///</summary>
        public void SetVelocity(float velocity, Vector2 direction)
        {
            _workspace = direction * velocity;
            _rb.velocity = _workspace;
            _currentVelocity = _workspace;
        }

        ///<summary>
        ///set the velocity only in the x axis
        ///</summary>
        public void SetVelocityX(float velocity)
        {
            _workspace.Set(velocity, _currentVelocity.y);
            _rb.velocity = _workspace;
            _currentVelocity = _workspace;
        }

        ///<summary>
        ///set the velocity only in the y axis
        ///</summary>
        public void SetVelocityY(float velocity)
        {
            _workspace.Set(_currentVelocity.x, velocity);
            _rb.velocity = _workspace;
            _currentVelocity = _workspace;
        }

        #endregion

        #region stats modification

        ///<summary>
        /// subtracts the player's health component by "damage" input field
        ///</summary>
        public void ApplyDamage(float damage)
        {
            //Debug.Log("[HEALTH] apply damage: " + entity.IsOwner);
            if (entity.IsOwner)
            {
                state.Health -= damage;
            }
        }

        ///<summary>
        /// add experience and spendable time to the player
        ///</summary>
        public void ObtainRewards(float exp, float time)
        {
            Debug.Log("[QUEST] rewards obtained. Exp: " + exp + ", Time: " + time);
            _spendableTime += time;
            _spendableExp += exp;

            _activeQuest = null;
            hasActiveQuest = false;
        }

        ///TODO change in deathState.

        ///<summary>
        /// changes the state to death state
        ///</summary>
        private void Die()
        {
            BoltNetwork.Destroy(gameObject);
            AbandonQuest();
        }
        #endregion

        #region callbacks

        ///<summary>
        /// Callback called when state.Health is modified -> we update the local health and do checks on it
        ///</summary>
        private void HealthCallback()
        {
            _playerData.currentLifePoints = state.Health;
            //Debug.Log("[HEALTH] callback. Owner: " + entity.IsOwner + " New currentLifePoints: " + _playerData.currentLifePoints);
            //Debug.Log("[HEALTH] callback. Slider of " + healthSlider.gameObject.transform.parent.gameObject.transform.parent.gameObject.name);

            healthSlider.value = _playerData.currentLifePoints;
            healthImage.color = healthGradient.Evaluate(healthSlider.normalizedValue);

            if (entity.IsOwner && _playerData.currentLifePoints <= 0)
            {
                if(_stateMachine._currentState.isDowned())
                    Die();
                else _stateMachine._currentState.SetDowned(true);
            }
        }

        ///<summary>
        /// callback that set  the nickname component
        ///</summary>
        private void NicknameCallback()
        {
            nicknameText.text = state.Nickname;
            //Debug.Log("[NICKNAME] callback: " + nicknameText.text);
        }
        #endregion

        #region check methods

        ///<summary>
        ///method that returns true if the player is touching a ceiling with his head, false instead
        ///</summary>
        public bool CheckForCeiling()
        {
            _workspace = _col.bounds.center + Vector3.up * _col.bounds.size.y * 0.5f;
            return Physics2D.OverlapBox(_workspace, new Vector3(_col.bounds.size.x - 0.01f, _playerData.ceilingHeight, 0f), 0f, _playerData.whatIsCeiling);
        }

        ///<summary>
        ///method that returns true if the player is grounded, false instead
        ///<summary>
         public bool CheckIfGrounded()
        {   
            _workspace = _col.bounds.center + Vector3.down * _col.bounds.size.y * 0.5f;
            return Physics2D.OverlapBox(_workspace, new Vector3(_col.bounds.size.x - 0.01f, 0.1f, 0f), 0f, _playerData.whatIsGround);
        }

        ///<summary>
        /// method that returns true if the player is on other players, false instead
        ///</summary>
        public bool CheckIfGroundOnOtherPlayer(){
            _workspace = _col.bounds.center + Vector3.down * _col.bounds.size.y * 0.5f;
            Collider2D col = Physics2D.OverlapBox(_workspace, new Vector3(_col.bounds.size.x - 0.01f, 0.1f, 0f), 0f, 
                _playerData.WhatIsPlayer);
            if(col.gameObject != gameObject)
                return true;
            else return false;
        }

        ///<summary>
        /// method that returns true if the player is on an enemy, false instead
        ///</summary>
        public bool CheckIfGroundedOnEnemy(){
             _workspace = _col.bounds.center + Vector3.down * _col.bounds.size.y * 0.5f;
              return Physics2D.OverlapBox(_workspace, new Vector3(_col.bounds.size.x - 0.01f, 0.1f, 0f), 0f, 
                _playerData.WhatIsEnemy);
        }

        ///<summary>
        ///method that returns true if the player is touching a wall (in the direction that he's facing), false instead
        ///</summary>
        public bool CheckIfTouchingWall()
        {
            return Physics2D.Raycast(_wallCheck.position, Vector2.right * _facingDirection, _playerData.wallCheckDistance, _playerData.whatIsWall);
        }

        ///<summary>
        ///method that returns true if the player is very close to a ledge (and he is heading to it), false instead
        ///</summary>
        public bool CheckIfTouchingLedge()
        {
            return Physics2D.Raycast(_ledgeCheck.position, Vector2.right * _facingDirection, _playerData.wallCheckDistance, _playerData.whatIsLedge);
        }

        ///<summary>
        ///method that flip the player if he's moving in the opposite direction that he's facing 
        ///</summary>
         public void CheckIfShouldFlip(int xInput)
        {
            if(xInput != 0 && xInput != _facingDirection)
            {
                Flip();
            }
        }

        #endregion

        #region others

        ///<summary>
        /// if player has not an active quest, the methods trigger the quest to be initialized and ready
        ///</summary>
        private void AcceptQuest(Quest quest)
        {
            if (!hasActiveQuest)
            {
                Debug.Log("[QUEST] player accepted the quest " + quest._title);
                quest.Accept();
            }
        }

        ///<summary>
        ///TODO add summary
        ///</summary>
        public void JoinQuest(Quest quest)
        {
            if (!hasActiveQuest && entity.IsOwner)
            {
                _activeQuest = quest;
                hasActiveQuest = true;
                quest.LocalPartecipate();

                Debug.Log("[QUEST] player joined the quest " + quest._title);

                //TODO show quest UI
            }
        }

        ///<summary>
        ///TODO add summary
        ///</summary>
        public void AbandonQuest()
        {
            if (hasActiveQuest)
            {
                _activeQuest = null;
                hasActiveQuest = false;

                Debug.Log("[QUEST] player abandoned the quest " + _activeQuest._title);

                //TODO remove quest UI
            }
        }
  
        ///<summary>
        ///method that flips the player
        ///</summary>
        private void Flip()
        {
            _facingDirection *= -1;
            transform.Rotate(0.0f, 180.0f, 0.0f);
            canvas.transform.rotation = Quaternion.identity;
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

        ///<summary>
        ///method that return the basic player data scriptable object, in which are contained a lot of base data
        ///<summary>
        public PlayerData GetPlayerData(){
            return _playerData;
        }

        ///<summary>
        ///method that returns true if the player is in a human state, false instead
        ///<summary>
        public bool GetIsHuman()
        {
            return _isHuman;
        }

        public uint getConnectionID()
        {
            return entity.Source.ConnectionId;
        }

        #endregion

        #region setter

        ///<summary>
        ///method that sets the height of the collider, useful in the crouch states
        ///</summary>
        public void SetColliderHeight(float height)
        {
            Vector2 center = _col.offset;
            _workspace.Set(_col.size.x, height);

            center.y += (height - _col.size.y) / 2;

            _col.size = _workspace;
            _col.offset = center;
        }

        ///<summary>
        ///method that sets the width of the collider, useful in the down states
        ///</summary>
        public void SetColliderWidth(float width)
        {
            Vector2 center = _col.offset;
            _workspace.Set(_col.size.y, width);

            center.x += (width - _col.size.x) / 2;

            _col.size = _workspace;
            _col.offset = center;
        }
        
        ///<summary>
        ///set the variable isHuman to a value, this method should be used in transformation and detransformation states
        ///</summary>
        public void SetIsHuman(bool value)
        {
            _isHuman = value;
        }
        public void SetWeakActive(bool value){
            weakActive = value;
        }

        #endregion

        #endregion
    }
}
