using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ThePackt{
    public class newInputHandler : MonoBehaviour
    {
        private PlayerInput playerInput;
        private Werewolf player;
        private Camera cam;

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
        public bool _attackInput { get; private set; }
        public bool _attackInputStop { get; private set; }
        public Dictionary<string, bool> _attackInputs { get; private set; }

        [SerializeField]
        private float _inputHoldTime = 0.2f;

        private float _jumpInputStartTime;
        private float _dashInputStartTime;
        private float _attackInputStartTime;

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            player = GetComponent<Werewolf>();
            _attackInputs = new Dictionary<string, bool>();

            //int count = Enum.GetValues(typeof(CombatInputs)).Length;
            //_attackInputs = new bool[count];

            cam = Camera.main;
        }

        private void Update()
        {
            CheckJumpInputHoldTime();
            CheckDashInputHoldTime();
            CheckAttackInputHoldTime();
        }
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
        public void OnMoveInput(InputAction.CallbackContext context)
        {
            _rawMovementInput = context.ReadValue<Vector2>();

            _normInputX = Mathf.RoundToInt(_rawMovementInput.x);
            _normInputY = Mathf.RoundToInt(_rawMovementInput.y);       
            
        }

        public void OnJumpInput(InputAction.CallbackContext context)
        {
            Debug.Log("input jump");
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

        public void OnDashDirectionInput(InputAction.CallbackContext context)
        {
            _raw_dashDirectionInput = context.ReadValue<Vector2>();

            if(playerInput.currentControlScheme == "Keyboard")
            {
                _raw_dashDirectionInput = cam.ScreenToWorldPoint((Vector3)_raw_dashDirectionInput) - transform.position;
            }

            _dashDirectionInput = Vector2Int.RoundToInt(_raw_dashDirectionInput.normalized);
        }

        public void OnAttackInput(InputAction.CallbackContext context)
        {
            Debug.Log("attack");

            if (context.started)
            {
                if (SetAttackDirection())
                {
                    _attackInput = true;
                    _attackInputStop = false;
                    _attackInputStartTime = Time.time;
                }
            }
            else if (context.canceled)
            {
                _attackInputStop = true;
            }
        }

        private bool SetAttackDirection()
        {
            Transform attPoint = player.GetAttackPoint();
            bool clickedLeft = true;
            bool pointedLeft = true;
            bool facingLeft = player._facingDirection == -1 ? true : false;

            if (playerInput.currentControlScheme == "Keyboard")
            {
                Vector2 mousePos = cam.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));

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

                Debug.Log("pointed left: " + pointedLeft + " -- " + _attackDirectionInput.x);
                Debug.Log("facing left: " + facingLeft);

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
            Debug.Log("input attack direction");

            _attackDirectionInput = context.ReadValue<Vector2>();

            if (playerInput.currentControlScheme == "Keyboard")
            {
                _attackDirectionInput = cam.ScreenToWorldPoint((Vector3) _attackDirectionInput) - transform.position;
            }

            // Debug.Log("direction (left): " + _attackDirectionInput.x);
        }

        public void OnMouseXInput(InputAction.CallbackContext context)
        {
            float a = context.ReadValue<float>();
            Debug.Log("mouse x: " + a);
        }

        public void OnMouseYInput(InputAction.CallbackContext context)
        {
            float a = context.ReadValue<float>();
            Debug.Log("mouse y: " + a);
        }

        public void UseJumpInput() => _jumpInput = false;

        public void UseDashInput() => _dashInput = false;

        public void UseBaseAttackInput() => _attackInputs["base"] = false;

        private void CheckJumpInputHoldTime()
        {
            if(Time.time >= _jumpInputStartTime + _inputHoldTime)
            {
                _jumpInput = false;
            }
        }

        private void CheckDashInputHoldTime()
        {
            if(Time.time >=  _dashInputStartTime + _inputHoldTime)
            {
                _dashInput = false;
            }
        }

        private void CheckAttackInputHoldTime()
        {
            if (Time.time >= _dashInputStartTime + _inputHoldTime)
            {
                _attackInput = false;
            }
        }
    }
}