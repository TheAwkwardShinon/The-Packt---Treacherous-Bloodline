using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using Bolt;
using UnityEngine.SceneManagement;

namespace ThePackt{
    public class Player : Bolt.EntityBehaviour<ICustomPlayerState>
    {
        #region variables

        #region core
       
        public newInputHandler _inputHandler { get; private set; }
        private PlayerInput _playerInput;
        private PlayerData _playerData;
        protected CharacterSelectionData _selectedData;

        [Header("Core Data Fields")]
        [SerializeField] protected PlayerData _playerBaseData;
        [SerializeField] private Transform _wallCheck;
        [SerializeField] private Transform _wallCheckWolf;

        [SerializeField] private Transform _ledgeCheck;
        [SerializeField] private Transform _ceilingCheck;
        [SerializeField] protected Transform _attackPoint;
        [SerializeField] protected GameObject _bullet;
        [SerializeField] protected GameObject  _feleBullet;
        [SerializeField] protected GameObject  _herinBullet;
        [SerializeField] protected GameObject  _ceuinBullet;
        [SerializeField] protected GameObject  _ayatanaBullet;
        [SerializeField] protected GameObject  _naturiaBullet;
        [SerializeField] protected GameObject  _moonsightersBullet;

        [SerializeField] protected Transform _specialAttackPoint;


        [SerializeField] protected GameObject _fogCircle;

        #endregion

        #region UI
        private Slider healthSlider;

        [Header("UI")]
        [SerializeField] protected Canvas canvas;
        [SerializeField] protected Text nicknameText;
        [SerializeField] protected GameObject healthBar;
        [SerializeField] protected Image healthImage;
        [SerializeField] protected Gradient healthGradient;
        [SerializeField] public GameObject _interactTooltip;
        [SerializeField]protected GameObject _humanObject;
        [SerializeField]protected GameObject _wolfObject;
        #endregion

        #region velocity
        public Vector2 _currentVelocity { get; private set; }
        private Vector2 _workspace;
        public int _facingDirection { get; private set; }    

        #endregion

        #region flags
        protected bool _isHuman = true;
        protected bool _isImpostor = false;

        public bool _isGrounded { get; private set; }
        public  bool _isDowned {get; private set;} = false;
        public  bool _isBeingHealed {get; private set;} = false;

        protected bool hasActiveQuest = false;

        protected QuestUIHandler _questHandler;



        #endregion

        #region components
        public  Animator _anim {get; private set;} 
        protected Rigidbody2D _rb;
        protected Collider2D _col;//this is only used for checking the ground

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

        public PlayerSpecialAttackState _specialAttack {get; private set;}


        #endregion

        #region other
        protected Quest _activeQuest;
        protected float _spendableExp = 0;
        protected float _spendableTime = 0;

        protected string _nextBullet = "naturia";
        #endregion


        #region vfx
        [Header("VFX")]
        [SerializeField] private GameObject _transformationVfx;
        [SerializeField] private GameObject _shootVfx;
        [SerializeField] private GameObject _hitVfx;
        #endregion

        #region sfx
        [Header("SFX")]
        [SerializeField] private GameObject _gunshotSfx;
        [SerializeField] private GameObject _walkSfx;
        [SerializeField] private GameObject _jumpSfx;
        [SerializeField] private GameObject _wolfJumpSfx;
        [SerializeField] private GameObject _transformationSfx;
        [SerializeField] private GameObject _attackSfx;
        [SerializeField] private GameObject _dashSfx;
        [SerializeField] private GameObject _humanHurtSfx;
        [SerializeField] private GameObject _wolfHurtSfx;
        [SerializeField] private GameObject _drinkSfx;
        [SerializeField] private GameObject _interactSfx;
        [SerializeField] private GameObject _pickUpSfx;
        [SerializeField] private GameObject _humanDeathSfx;
        [SerializeField] private GameObject _wolfDeathSfx;
        [SerializeField] private GameObject _joinSfx;
        [SerializeField] private GameObject _abandonSfx;
        [SerializeField] private GameObject _completeSfx;
        #endregion


        #region quest UI
        private Text _questTitleText;
        private Text _questDescriptionText;
        private Text _questReward;
        private Text _questAction;
        private GameObject _questPanel;
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
            _idleState = new PlayerIdleState(this, _stateMachine, _playerData, "IdleHuman","IdleWolf");
            _moveState = new PlayerMoveState(this, _stateMachine, _playerData, "MoveHuman","MoveWolf");
            _jumpState = new PlayerJumpState(this, _stateMachine, _playerData, "InAirHuman","InAirWolf");
            _inAirState = new PlayerInAirState(this, _stateMachine, _playerData, "InAirHuman","InAirWolf");
            _crouchIdleState = new PlayerCrouchIdleState(this, _stateMachine, _playerData, "CrouchHuman","CrouchWolf");
            _crouchMoveState = new PlayerCrouchMoveState(this, _stateMachine, _playerData, "CrouchMoveHuman","CrouchMoveWolf");
            _landState = new PlayerLandState(this, _stateMachine, _playerData, "LandHuman","LandWolf");
            _wallSlideState = new PlayerWallSlideState(this, _stateMachine, _playerData, "WallSlideHuman","WallSlideWolf");
            _dashState = new PlayerDashState(this, _stateMachine, _playerData, "DashHuman","DashWolf");
            _attackState = new PlayerAttackState(this, _stateMachine, _playerData, "AttackHuman","AttackWolf");
            _transformState = new PlayerTransformationState(this, _stateMachine, _playerData, "Transformation","Transformation");
            _detransformationState = new PlayerDetransformationState(this, _stateMachine, _playerData, "Detransformation","Detransformation");
            _downMoveState = new PlayerDownMoveState(this, _stateMachine, _playerData, "DownedMoveHuman","DownedMoveWolf");
            _downState = new PlayerDownState(this, _stateMachine, _playerData, "DownedHuman","DownedWolf");
            _interactState = new PlayerInteractState(this, _stateMachine, _playerData, "InteractHuman","InteractWolf");
            _specialAttack = new PlayerSpecialAttackState(this, _stateMachine, _playerData, "SpecialWolf","SpecialWolf");
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

            MainQuest mainQuest = MainQuest.Instance;
            if (BoltNetwork.IsServer && mainQuest != null)
            {
                mainQuest.AddPlayer(entity);
            }

            //get core components and initialize the fsm
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _col = gameObject.GetComponent<Collider2D>();
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
            state.AddCallback("dmgReductionDebuff",DamageReducionCallback);
            state.AddCallback("fogDebuff",FogDebuffCallback);
            state.AddCallback("slowDebuff",SlowDebuffCallback);
            state.AddCallback("Health", HealthCallback);
            state.AddCallback("Nickname", NicknameCallback);
            state.AddCallback("isDowned", IsDownedCallback);
            state.AddCallback("isBeingHealed", IsBeingHealedCallback);

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
            if(_playerData.socialAnimal){
                Collider2D[] col = Physics2D.OverlapCircleAll(transform.position,5f,_playerData.WhatIsPlayer);
                if(col != null){
                    _playerData.damageMultiplier -= _playerData.socialAnimalMultiplier * _playerData.numOfNearPlayer;
                    _playerData.damageMultiplier += _playerData.socialAnimalMultiplier * col.Length;
                    _playerData.numOfNearPlayer = col.Length;
                }
                else {
                    _playerData.damageMultiplier -= _playerData.socialAnimalMultiplier * _playerData.numOfNearPlayer;
                    _playerData.numOfNearPlayer = 0;
                }
            }

            healthSlider.value = _playerData.currentLifePoints;
            healthImage.color = healthGradient.Evaluate(healthSlider.normalizedValue);
            //CheckIfOtherPlayerInRangeMayBeHealed();
        }

        private void Update()
        {
            if (!entity.IsOwner)
            {
                //canvas.transform.rotation = Quaternion.identity;
                //canvas.transform.rotation = Quaternion.Euler(0, 0, 0);
                healthBar.transform.rotation = Quaternion.identity;
            }
            


            if(SceneManager.GetActiveScene().name.Equals("MapScene") && _questPanel == null){
                 _questPanel = GameObject.Find("Canvas").GetComponent<HiddenCanvas>().GetQuestPanel();
                 _questReward = GameObject.Find("Canvas").GetComponent<HiddenCanvas>().GetReward();
                 _questTitleText = GameObject.Find("Canvas").GetComponent<HiddenCanvas>().GetTitle();
                 _questDescriptionText = GameObject.Find("Canvas").GetComponent<HiddenCanvas>().GetDescription();
                 _questAction = GameObject.Find("Canvas").GetComponent<HiddenCanvas>().GetAction();
                 _questHandler = GameObject.Find("Canvas").GetComponent<HiddenCanvas>().GetQuestHandler();
            }

        }

        public override void Detached()
        {
            MainQuest mainQuest = MainQuest.Instance;
            if (BoltNetwork.IsServer && mainQuest != null)
            {
                mainQuest.RemovePlayer(entity);
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
        /// apply slow debuff for time : time -- can't jump and cant dash
        ///</summary>
        public void ApplicateSlow(float time){
            Debug.LogError("[SLOW BULLET] applico il debuff");
            state.slowDebuff = true;
            _playerData.timeOfSlow = time;
            _playerData.cantDash = true;
            _playerData.cantJump = true;
            _playerData.debuffStartTime = Time.time;
        }

        ///<summary>
        /// remove slow debuff
        ///</summary>
        public void RemoveSlowDebuff(){
            Debug.LogError("[SLOW BULLET]tempo scaduto, rimuovo il debuff");
            state.slowDebuff = false;
            _playerData.timeOfSlow = 0f;
            _playerData.cantDash = false;
            _playerData.cantJump = false;
        }

        public void ApplicateForce(Vector2 direction, float power){
            _rb.AddForce(direction * power);
        }

        public void NextBullet(string bullet){
            _nextBullet = bullet;
        }

        ///<summary>
        /// apply fog of war debuff for time : time, the circle : circle
        ///</summary>
         public void ApplicateFogDebuff(float time,float circleSize){
            state.fogDebuff = true;
            _fogCircle.transform.localScale = new Vector3(circleSize,circleSize,_fogCircle.transform.localScale.z);
            _playerData.timeOffogDebuff = time;
            _playerData.debuffFogStartTime = Time.time;
        }

         ///<summary>
        /// remove fog of war debuff
        ///</summary>
         public void RemoveFogDebuff(){
            state.fogDebuff = false;
            _fogCircle.transform.localScale = new Vector3(_playerData.standardCircleSize,_playerData.standardCircleSize,_fogCircle.transform.localScale.z);
            _playerData.timeOffogDebuff = 0f;
        }

        ///<summary>
        /// apply damage reduction debuff for time : time
        ///</summary>
         public void ApplicateDamageReductionDebuff(float time,float dmgReduction){
            state.dmgReductionDebuff = true;
            _playerData.dmgReduction = dmgReduction;
            _playerData.timeOfDmgReduction = time;
            _playerData.damageReductionDebuffStartTime = Time.time;
        }

        ///<summary>
        /// remove damage reduction debuff
        ///</summary>
        public void RemoveDmgReductionDebuff(){
            state.dmgReductionDebuff = false;
            _playerData.timeOfDmgReduction = 0f;
        }

        ///<summary>
        /// sets the diamater of the fog of war circle to size
        ///</summary>
        public void SetFogOfWarDiameter(float size)
        {
            _fogCircle.transform.localScale = new Vector3(size, size, _fogCircle.transform.localScale.z);
        }


        ///<summary>
        /// subtracts the player's health component by "damage" input field
        ///</summary>
        public void ApplyDamage(float damage)
        {
            //Debug.Log("[HEALTH] apply damage: " + entity.IsOwner);
            if (entity.IsOwner)
            {
                state.Health -= damage;
                if(_playerData.tasteLikeIron && _playerData.tasteLikeIronStart.Count == 0){
                    _playerData.tasteLikeIronStart = new List<float>();
                     _playerData.tasteLikeIronStart.Add(Time.time);
                    _playerData.TateLikeIronStack = 1;
                    _playerData.movementVelocityMultiplier += 0.05f;
                }
                else if (_playerData.tasteLikeIron && _playerData.TateLikeIronStack > 0){
                    _playerData.TateLikeIronStack++;
                     _playerData.tasteLikeIronStart.Add(Time.time);
                    _playerData.movementVelocityMultiplier += 0.05f;
                }

                if (GetIsHuman())
                {
                    _humanHurtSfx.GetComponent<AudioSource>().Play();
                }
                else
                {
                    _wolfHurtSfx.GetComponent<AudioSource>().Play();
                }
            }
        }

        ///<summary>
        /// heal the player by 30% of his max hp
        ///</summary>
        public void Heal(){
            if(entity.IsOwner){
                Debug.LogError("[HEALED]");
                state.Health = _playerData.maxLifePoints * 0.3f;
                SetIsBeingHealed(false);
            }
        }

        ///<summary>
        /// heals the player by the specified percentage of his max hp
        ///</summary>
        public void FountainHeal(float amount)
        {
            if (entity.IsOwner)
            {
                Debug.LogError("[HEALED]");

                _drinkSfx.GetComponent<AudioSource>().Play();

                float newHealth = state.Health + _playerData.maxLifePoints * amount;

                if(newHealth > _playerData.maxLifePoints)
                {
                    newHealth = _playerData.maxLifePoints;
                }
                state.Health = newHealth;
            }
        }

        ///<summary>
        /// add experience and spendable time to the player
        ///</summary>
        public void ObtainRewards(float exp, float time)
        {
            AudioSource.PlayClipAtPoint(_completeSfx.GetComponent<AudioSource>().clip, Camera.main.transform.position);

            Debug.Log("[QUEST] rewards obtained. Exp: " + exp + ", Time: " + time);
            _spendableTime += time;
            _spendableExp += exp;

            _activeQuest = null;
            hasActiveQuest = false;
            SetFogOfWarDiameter(_playerData.standardCircleSize);
        }


        ///<summary>
        /// changes the state to death state
        ///</summary>
        public void Die()
        {
            if (GetIsHuman())
            {
                AudioSource.PlayClipAtPoint(_humanDeathSfx.GetComponent<AudioSource>().clip, Camera.main.transform.position);
            }
            else
            {
                AudioSource.PlayClipAtPoint(_wolfDeathSfx.GetComponent<AudioSource>().clip, Camera.main.transform.position);
            }

            AbandonQuest();

            BoltNetwork.Destroy(gameObject);
        }
        #endregion

        #region callbacks

        ///<summary>
        /// Callback called when state.Health is modified -> we update the local health and do checks on it
        ///</summary>
        private void HealthCallback()
        {
            _playerData.currentLifePoints = state.Health;
            healthSlider.value = _playerData.currentLifePoints;
            healthImage.color = healthGradient.Evaluate(healthSlider.normalizedValue);

            if (entity.IsOwner && _playerData.currentLifePoints <= 0)
            {
                if(_isDowned)
                    Die();
                else{

                    var evnt = PlayPlayerDeathSoundEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.ReliableOrdered);
                    evnt.Position = transform.position;
                    evnt.IsHuman = GetIsHuman();
                    evnt.Send();

                    state.isDowned = true;
                }
            }
        }

        private void IsDownedCallback()
        {
            _isDowned = state.isDowned;
        }

        private void SlowDebuffCallback(){
            _playerData.isSlowed = state.slowDebuff;
        }

        private void FogDebuffCallback(){
            _playerData.isFogDebuffActive = state.fogDebuff;
        }

        private void DamageReducionCallback(){
            _playerData.isDmgReductionDebuffActive = state.dmgReductionDebuff;
        }



        private void IsBeingHealedCallback()
        {
            _isBeingHealed = state.isBeingHealed;
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
            return Physics2D.OverlapBox(_ceilingCheck.position,new Vector3(0.10f, 0.1f, 0f), 0f, _playerData.whatIsGround|_playerData.whatIsWall);
        }

        public void OnDrawGizmos(){
            //
            Gizmos.color = Color.green;
            Gizmos.DrawCube(_ledgeCheck.position,new Vector3(0.08f, 0.1f, 0f));
            Gizmos.DrawCube(_ceilingCheck.position,new Vector3(0.10f, 0.1f, 0f));
            Gizmos.DrawCube(_wallCheck.position,new Vector3(0.12f,0.5f,0f));
            Gizmos.DrawCube(_wallCheckWolf.position,new Vector3(0.12f,0.5f,0f));

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_attackPoint.position, _playerBaseData.rangeBaseWerewolf);

            /*
            Gizmos.color = Color.yellow;
            Vector2 target = new Vector2(_col.bounds.center.x, _col.bounds.center.y + _col.bounds.size.y / 2 - _col.bounds.size.y / 4);
            Vector2 origin = Vector2.zero;
            Vector2 direction = target - origin;
            Ray r = new Ray(origin, direction);
            Gizmos.DrawRay(r);
            */
        }

        ///<summary>
        ///method that returns true if the player is grounded, false instead
        ///<summary>
         public bool CheckIfGrounded()
        {   
            
            _isGrounded = Physics2D.OverlapBox(_ledgeCheck.position,new Vector3(0.08f, 0.1f, 0f), 0f, _playerData.whatIsGround);
            return _isGrounded;
        }

        ///<summary>
        /// method that returns true if the player is on other players, false instead
        ///</summary>
        public bool CheckIfGroundOnOtherPlayer(){
            
            Collider2D col = Physics2D.OverlapBox(_ledgeCheck.position, new Vector3(0.15f, 0.005f, 0f), 0f, 
                _playerData.WhatIsPlayer);
            if(col.transform.root.gameObject != gameObject)
                return true;
            else return false;
        }

        ///<summary>
        /// method that returns true if the player is on an enemy, false instead
        ///</summary>
        public bool CheckIfGroundedOnEnemy(){
             
              return Physics2D.OverlapBox(_ledgeCheck.position, new Vector3(0.15f, 0.005f, 0f), 0f, 
                _playerData.WhatIsEnemy);
        }

        ///<summary>
        /// method that returns true if the player is near a downed player that needs healing, false instead
        ///</summary>
        public bool CheckIfOtherPlayerInRangeMayBeHealed(){
            Collider2D col = Physics2D.OverlapCircle(transform.position,12f,_playerData.WhatIsPlayer);
            if(col != null){
                if(col.gameObject != this.gameObject && 
                    col.gameObject.transform.root.GetComponent<Player>()._isDowned &&
                    !col.gameObject.transform.root.GetComponent<Player>()._isBeingHealed){
                        if(!_interactTooltip.activeSelf){
                            _interactTooltip.transform.position = new Vector2(col.transform.position.x,col.transform.position.y + 1f);
                              _interactTooltip.SetActive(true);
                        }
                        return true;
                }
            }
            if(_interactTooltip.activeSelf)
                _interactTooltip.SetActive(false);
            return false;
        }


        ///<summary>
        ///method that returns true if the player is touching a wall (in the direction that he's facing), false instead
        ///</summary>
        public bool CheckIfTouchingWall()
        {
            if(_isHuman)
                return Physics2D.OverlapBox(_wallCheck.position, new Vector3(0.12f,0.5f,0f), 0f, 
                    _playerData.whatIsWall);
            else return Physics2D.OverlapBox(_wallCheckWolf.position, new Vector3(0.12f,0.5f,0f), 0f, 
                    _playerData.whatIsWall);
        }

         public bool CheckIfTouchingPlayer()
        {
            if(_isHuman){
                Collider2D col = Physics2D.OverlapBox(_wallCheck.position, new Vector3(0.1f,0.5f,0f), 0f, 
                    _playerData.WhatIsPlayer);
                if(!col.Equals(null))
                    if(!col.gameObject.Equals(gameObject))
                        return true;
            }
            else{
                Collider2D col = Physics2D.OverlapBox(_wallCheckWolf.position, new Vector3(0.1f,0.5f,0f), 0f, 
                    _playerData.WhatIsPlayer);
                if(!col.Equals(null))
                    if(!col.gameObject.Equals(gameObject))
                        return true;
            }
            return false;
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

        #region sfx

        public void PlayGunshotSFX()
        {
            _gunshotSfx.GetComponent<AudioSource>().Play();
        }

        public void PlayWalkSFX()
        {
            _walkSfx.GetComponent<AudioSource>().Play();
        }

        public void PlayJumpSFX()
        {
            _jumpSfx.GetComponent<AudioSource>().Play();
        }

        public void PlayAttackSFX()
        {
            _attackSfx.GetComponent<AudioSource>().Play();
        }

        public void PlayPickUpSFX()
        {
            _pickUpSfx.GetComponent<AudioSource>().Play();
        }

        public void PlayTransformationSFX()
        {
            _transformationSfx.GetComponent<AudioSource>().Play();
        }

        public void PlayWolfJumpSFX()
        {
            _wolfJumpSfx.GetComponent<AudioSource>().Play();
        }

        public void PlayDashSFX()
        {
            _dashSfx.GetComponent<AudioSource>().Play();
        }

        public void PlayInteractSFX()
        {
            _interactSfx.GetComponent<AudioSource>().Play();
        }

        #endregion

        #region others

        ///<summary>
        /// if player has not an active quest, the methods trigger the quest to be initialized and ready
        ///</summary>
        public void AcceptQuest(Quest quest)
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
                _joinSfx.GetComponent<AudioSource>().Play();

                _activeQuest = quest;
                hasActiveQuest = true;
                quest.LocalPartecipate();

                Debug.Log("[QUEST] player joined the quest " + quest._title);
                if(_questTitleText == null)
                    Debug.LogError("the object is null");
                if(quest._title.Equals(null)){
                    Debug.LogError("title is is null");
                }
                _questTitleText.text = quest._title;
                _questDescriptionText.text = quest._description;
                _questReward.text = quest._timeReward.ToString();
                _questAction.text = "QUEST JOINED";
                _questPanel.SetActive(true);
                _questHandler.SetActiveQuest(quest);
            }
        }

        ///<summary>
        ///TODO add summary
        ///</summary>
        public void AbandonQuest()
        {
            if (hasActiveQuest)
            {
                _abandonSfx.GetComponent<AudioSource>().Play();

                _activeQuest = null;
                hasActiveQuest = false;
                SetFogOfWarDiameter(_playerData.standardCircleSize);

                Debug.Log("[QUEST] player abandoned the quest " + _activeQuest._title);

                //TODO remove quest UI
            }
        }

        public void DisableQuestPanel(){
            _questPanel.SetActive(false);
        }
  
        ///<summary>
        ///method that flips the playera
        ///</summary>
        private void Flip()
        {
            _facingDirection *= -1;
           
            transform.Rotate(0.0f, 180.0f, 0.0f);
            //_humanObject.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            //_wolfObject.transform.localRotation = Quaternion.identity;
            _interactTooltip.transform.Rotate(0.0f, 180.0f, 0.0f);
            canvas.transform.rotation = Quaternion.Euler(0,0,0);
        }

          

        public void ActivateFogCircle()
        {
            _fogCircle.SetActive(true);
            
        } 
        #endregion


        #region triggers

        private void AnimationTrigger() => _stateMachine._currentState.AnimationTrigger();

        private void AnimtionFinishTrigger() => _stateMachine._currentState.AnimationFinishTrigger();

        #endregion

        #region others
        public void SetEnabledInput(bool value)
        {
            _playerInput.enabled = value;
        }

        #endregion

        #region getter
        public Transform GetAttackPoint()
        {
            return _attackPoint;
        }
        public Transform GetSpecialAttackPoint()
        {
            return _specialAttackPoint;
        }
        public GameObject GetBullet()
        {
            return _bullet;
        }

        public GameObject GetFeleBullet(){
            return _feleBullet;
        }
        public GameObject GetAyatanaBullet(){
            return _ayatanaBullet;
        }

        public GameObject GetCeuinBullet(){
            return _ceuinBullet;
        }

        public GameObject GetHerinBullet(){
            return _herinBullet;
        }
        public GameObject GetNaturiaBullet(){
            return _naturiaBullet;
        }
        public GameObject GetMoonsighterBullet(){
            return _moonsightersBullet;
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

        public string GetNextBullet(){
            return _nextBullet;
        }

        public GameObject GetHumanObject(){
            return _humanObject;
        }

        public GameObject GetWolfObject(){
            return _wolfObject;
        }

        public bool isImpostor(){
            return _isImpostor;
        }

        public float GetSpendableTime(){
            return _spendableTime;
        }

         public float GetSpendableExp(){
            return _spendableExp;
        }

        #endregion

        #region setter

        public void SetSpendableTime(float value){
            _spendableTime = value;
        }

         public void SetSpendableExp(float value){
            _spendableExp = value;
        }
        
        public void SetIsHuman(bool value)
        {
            _isHuman = value;
        }

        public void SetIsImpostor(bool value)
        {
            _isImpostor = value;
        }


        public void SetIsBeingHealed(bool value){
            if(entity.IsOwner)
                state.isBeingHealed = value;
        }

        public void SetTransformationVFXActive(){
            ActivateVFXEvent evnt;
            evnt = ActivateVFXEvent.Create(GlobalTargets.Everyone);
            evnt.TargetPlayerNetworkID = this.entity.NetworkId;
            evnt.Active = true;
            evnt.VFXName = "transformation";
            evnt.Send();
        }
        public void SetTransformationVFXNotActive(){
            ActivateVFXEvent evnt;
            evnt = ActivateVFXEvent.Create(GlobalTargets.Everyone);
            evnt.TargetPlayerNetworkID = this.entity.NetworkId;
            evnt.Active = false;
            evnt.VFXName = "transformation";
            evnt.Send();
        }
        public void SetHitVFXActive(){
            ActivateVFXEvent evnt;
            evnt = ActivateVFXEvent.Create(GlobalTargets.Everyone);
            evnt.TargetPlayerNetworkID = this.entity.NetworkId;
            evnt.Active = true;
            evnt.VFXName = "hit";
            evnt.Send();
        }
        public void SetHitVFXNotActive(){
            ActivateVFXEvent evnt;
            evnt = ActivateVFXEvent.Create(GlobalTargets.Everyone);
            evnt.TargetPlayerNetworkID = this.entity.NetworkId;
            evnt.Active = false;
            evnt.VFXName = "hit";
            evnt.Send();
        }

         public void SetShootVFXActive(){
            ActivateVFXEvent evnt;
            evnt = ActivateVFXEvent.Create(GlobalTargets.Everyone);
            evnt.TargetPlayerNetworkID = this.entity.NetworkId;
            evnt.Active = true;
            evnt.VFXName = "special";
            evnt.Send();
        }
        public void SetShootVFXNotActive(){
            ActivateVFXEvent evnt;
            evnt = ActivateVFXEvent.Create(GlobalTargets.Everyone);
            evnt.TargetPlayerNetworkID = this.entity.NetworkId;
            evnt.Active = false;
            evnt.VFXName = "special";
            evnt.Send();
        }

        public void SetTransformationVFX(bool value){
            _transformationVfx.SetActive(value);
        }
        public void SetHitVFX(bool value){
            _hitVfx.SetActive(value);
        }
        public void SetSpecialVFX(bool value){
            _shootVfx.SetActive(value);
        }


        #endregion

        #endregion
    }
}
