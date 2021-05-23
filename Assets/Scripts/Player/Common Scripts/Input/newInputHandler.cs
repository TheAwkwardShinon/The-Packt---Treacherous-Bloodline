using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ThePackt{
    public class newInputHandler : MonoBehaviour
    {

        #region variables
        private PlayerInput playerInputComponent;
        // private PlayerInputClass playerInputObject;
        private Player player;

        private GameObject _menuInGameUI;

        private tabButton _firstTabSelected;
        private EventSystem _eventSystem;

        public Vector2 _rawMovementInput { get; private set; }
        public Vector2 _raw_dashDirectionInput { get; private set; }
        public Vector2 _attackDirectionInput { get; private set; }
        public Vector2Int _dashDirectionInput { get; private set; }
        public int _normInputX { get; private set; }
        public int _normInputY { get; private set; }
        public bool _jumpInput { get; private set; }
        public bool _jumpInputStop { get; private set; }
        public bool _dashInput { get; private set; }
        public bool _dashInputStop { get; private set; }
        public bool _transformInput { get; private set; }
        public bool _transformInputStop { get; private set; }

        public bool _interactInput {get; private set;}
        public bool _interactInputStop {get; private set;}

        public Dictionary<string, bool> _attackInputs { get; private set; }
        public Dictionary<string, bool> _attackInputsStop { get; private set; }

        public bool _specialAttackInput { get; private set; }
        public bool _specialAttackInputStop { get; private set; }

        [SerializeField]
        private float _inputHoldTime = 0.2f;

        private float _jumpInputStartTime;
        private float _dashInputStartTime;
        private float _transformInputStartTime;
        private float _interactInputStartTime;

        private float _specialAttackInputStartTime;
        public Dictionary<string, float> _attackInputsStartTime { get; private set; }

        #endregion


        #region methods

        #region core methods

        private void OnEnable(){
            
        }
        private void Start()
        {
            player = GetComponent<Player>();
            _attackInputs = new Dictionary<string, bool>();
            _attackInputsStop = new Dictionary<string, bool>();
            _attackInputsStartTime = new Dictionary<string, float>();

            playerInputComponent = GetComponent<PlayerInput>();
            
            if(SceneManager.GetActiveScene().name.Equals("NetworkTestScene")){ //TODO Change scene name here whene scene name changes
                _menuInGameUI = GameObject.Find("Canvas").GetComponent<HiddenCanvas>().GetMenu();
                _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            }
        }

        private void Update()
        {
            CheckJumpInputHoldTime();
            CheckDashInputHoldTime();
            CheckBaseAttackInputHoldTime();
            if(_menuInGameUI == null && _eventSystem == null && SceneManager.GetActiveScene().name.Equals("NetworkTestScene")){
                _menuInGameUI = GameObject.Find("Canvas").GetComponent<HiddenCanvas>().GetMenu();
                _firstTabSelected = GameObject.Find("Canvas").GetComponent<HiddenCanvas>().GetFirstTab();
                _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            }
        }

        #endregion

        /*
                public void OnPrimaryAttackInput(InputAction.CallbackContext context)
                {
                    if (context.started)
                    {
                        _attackInputs[(int)CombatInputs.primary] = true;
                    }

                    if (context.canceled)
                    {
                        _attackInputs[(int)CombatInputs.primary] = false;
                    }
                }

                public void OnSecondaryAttackInput(InputAction.CallbackContext context)
                {
                    if (context.started)
                    {
                        _attackInputs[(int)CombatInputs.secondary] = true;
                    }

                    if (context.canceled)
                    {
                        _attackInputs[(int)CombatInputs.secondary] = false;
                    }
                }
        */

        #region input 
        
        ///<summary> 
        /// Callabck called when input system detect movement input 
        ///</summary>
        public void OnMoveInput(InputAction.CallbackContext context)
        {
            _rawMovementInput = context.ReadValue<Vector2>();

            _normInputX = Mathf.RoundToInt(_rawMovementInput.x);
            _normInputY = Mathf.RoundToInt(_rawMovementInput.y);       
            
        }
        

        ///<summary> 
        /// Callabck called when input system detect jump input 
        ///</summary>
        public void OnJumpInput(InputAction.CallbackContext context)
        {
 
            if (context.started)
            {
                _jumpInput = true;
                _jumpInputStop = false;
                _jumpInputStartTime = Time.time;
            }

            if (context.canceled)
            {
                _jumpInputStop = true;
            }
        }

         ///<summary> 
        /// Callabck called when input system detects Interact input 
        ///</summary>
        public void OnInteractInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _interactInput = true;
                _interactInputStop = false;
                _interactInputStartTime = Time.time;
            }

            if (context.canceled)
            {
                _interactInputStop = true;
            }
        }
        
        ///<summary> 
        /// Callabck called when input system detect transformation input 
        ///</summary>
        public void OnTransformationInput(InputAction.CallbackContext context)
        {
            //MainQuest mainQuest = MainQuest.Instance;
            //if (mainQuest != null && mainQuest.GetQuestState() == Constants.STARTED)
            //{
                if (context.started)
                {
                    if (player.GetIsHuman())
                    {
                        _transformInput = true;
                        _transformInputStop = false;
                        _transformInputStartTime = Time.time;
                    }
                }

                if (context.canceled)
                {
                    _transformInputStop = true;
                }
           // }
        }
        
        ///<summary> 
        /// Callabck called when input system detect dash input 
        ///</summary>
        public void OnDashInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
            _dashInput = true;
            _dashInputStop = false;
            _dashInputStartTime = Time.time;
            }
            else if (context.canceled)
            {
            _dashInputStop = true;
            }
        }

        ///<summary> 
        /// Callabck called when input system detect dash direction input 
        ///</summary>
        public void OnDashDirectionInput(InputAction.CallbackContext context)
        {
            _raw_dashDirectionInput = context.ReadValue<Vector2>();

            if(playerInputComponent.currentControlScheme == "Keyboard")
            {
                _raw_dashDirectionInput = Camera.main.ScreenToWorldPoint((Vector3)_raw_dashDirectionInput) - transform.position;
            }

            _dashDirectionInput = Vector2Int.RoundToInt(_raw_dashDirectionInput.normalized);
        }

        ///<summary> 
        /// Callabck called when input system detect attack input 
        ///</summary>
        public void OnAttackInput(InputAction.CallbackContext context)
        {
            MainQuest mainQuest = MainQuest.Instance;
            //if (mainQuest != null && mainQuest.GetQuestState() == Constants.STARTED)
            //{
                if (context.started)
                {
                    if (!player.GetIsHuman() || CheckSetAttackDirection())
                    {
                        _attackInputs[Constants.BASE] = true;
                        _attackInputsStop[Constants.BASE] = false;
                        _attackInputsStartTime[Constants.BASE] = Time.time;
                    }
                }
                else if (context.canceled)
                {
                    _attackInputsStop[Constants.BASE] = true;
                }
            //}
        }
         public void OnSpecialAttackInput(InputAction.CallbackContext context)
        {
            //MainQuest mainQuest = MainQuest.Instance;
            //if (mainQuest != null && mainQuest.GetQuestState() == Constants.STARTED)
            //{
                if (context.started)
                {
                    if (player._specialAttack.CheckIfCanAttack() && CheckSetAttackDirection())
                    {
                       _specialAttackInput = true;
                       _specialAttackInputStop = false;
                       _specialAttackInputStartTime = Time.time;
                    }
                }
                else if (context.canceled)
                {
                    _specialAttackInputStop = true;
                }
            //}
        }

         public void OnActivateUIInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {   
                player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UserInterface");
                _menuInGameUI.SetActive(true);
                /*_menuInGameUI.transform.GetChild(0).GetComponent<TabGroup>().
                    OnTabSelected( _menuInGameUI.transform.GetChild(0).GetComponent<TabGroup>().tabButtons[0]);    */            
            }
            else if (context.canceled)
            {
            }
        }

        //TODO ora funziona mouse
        private bool CheckSetAttackDirection()
        {
            Transform attPoint = player.GetAttackPoint();
            bool clickedLeft = true;
            bool pointedLeft = true;
            bool facingLeft = player._facingDirection == -1 ? true : false;

            if (playerInputComponent.currentControlScheme == "Keyboard")
            {
                //da togliere quando funzioner� input sul mouse
                Vector2 mousePos = Camera.main.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));

                Vector2 attPointPos = attPoint.transform.position;
                if (attPointPos.x < mousePos.x)
                {
                    clickedLeft = false;
                }

                //checks if the player is facing towards the point of the click: the attack is executed only if he does
                if (facingLeft == clickedLeft)
                {
                    Vector2 attackDir = mousePos - attPointPos;
                    attPoint.transform.right = attackDir;

                    return true;
                }

                return false;
            }
            else
            {
                if (_attackDirectionInput.x > 0f)
                {
                    pointedLeft = false;
                }
                else if (_attackDirectionInput.x == 0f)
                {
                    facingLeft = pointedLeft;
                }

               

                //checks if the player is facing towards the point of the click: the attack is executed only if he does
                if (_attackDirectionInput == Vector2.zero)
                {
                    Vector2 facingDir = player._facingDirection == -1 ? Vector2.left : Vector2.right;
                    attPoint.transform.right = facingDir;

                    return true;
                }
                else if (facingLeft == pointedLeft)
                {
                    attPoint.transform.right = _attackDirectionInput;

                    return true;
                }

                return false;
            } 
        }

        public void OnAttackDirectionInput(InputAction.CallbackContext context)
        {

            _attackDirectionInput = context.ReadValue<Vector2>();

            //da aggiungere quando funzionerà input sul mouse
            /*
            if (playerInputComponent.currentControlScheme == "Keyboard")
            {
                _attackDirectionInput = cam.ScreenToWorldPoint((Vector3) _attackDirectionInput) - transform.position;
            }
            */

            // Debug.Log("direction (left): " + _attackDirectionInput.x);
        }

        #endregion


        #region Setter
        ///<summary> 
        /// set the jumpImput variable to false. This variables trigger the player fsm states, is important to reset it.
        ///</summary>

        public void UseJumpInput() => _jumpInput = false;

        ///<summary> 
        /// set the DashInput variable to false. This variables trigger the player fsm states, is important to reset it.
        ///</summary>
        public void UseDashInput() => _dashInput = false;

        ///<summary> 
        /// set the TransformationInput variable to false. This variables trigger the player fsm states, is important to reset it.
        ///</summary>
        public void UseTransformInput() => _transformInput = false;

        ///<summary> 
        /// set the InteractInput variable to false. This variables trigger the player fsm states, is important to reset it.
        ///</summary>
        public void UseInteractInput() => _interactInput = false;

        public void UseSpecialAttackInput() => _specialAttackInput = false;


        ///<summary> 
        /// set the Imput variable to false. This variables trigger the player fsm states, is important to reset it.
        ///</summary>
        public void UseBaseAttackInput() => _attackInputs["base"] = false;

        #endregion

        #region checker

        ///<summary> 
        /// method that check the hold time of a jump (the more you press the more hight you can reach within boundaries)
        ///</summary>
        private void CheckJumpInputHoldTime()
        {
            if(Time.time >= _jumpInputStartTime + _inputHoldTime)
            {
                _jumpInput = false;
            }
        }

        ///<summary> 
        ///  method that check the hold time of a dash
        ///</summary>
        private void CheckDashInputHoldTime()
        {
            if(Time.time >=  _dashInputStartTime + _inputHoldTime)
            {
                _dashInput = false;
            }
        }

        private void CheckBaseAttackInputHoldTime()
        {
            if (_attackInputsStartTime.ContainsKey(Constants.BASE))
            {
                if (Time.time >= _attackInputsStartTime[Constants.BASE] + _inputHoldTime)
                {
                    _attackInputs[Constants.BASE] = false;
                }
            }
        }
        #endregion

        #endregion
    }
}