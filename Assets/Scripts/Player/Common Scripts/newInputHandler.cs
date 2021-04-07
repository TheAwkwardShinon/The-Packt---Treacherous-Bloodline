using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ThePackt{
    public class newInputHandler : MonoBehaviour
    {
        private PlayerInput playerInput;
        private Camera cam;

        public Vector2 _rawMovementInput { get; private set; }
        public Vector2 _raw_dashDirectionInput { get; private set; }
        public Vector2Int _dashDirectionInput { get; private set; }
        public int _normInputX { get; private set; }
        public int _normInputY { get; private set; }
        public bool _jumpInput { get; private set; }
        public bool _jumpInputStop { get; private set; }
        public bool _dashInput { get; private set; }
        public bool _dashInputStop { get; private set; }

        public bool[] _attackInputs { get; private set; }

        [SerializeField]
        private float _inputHoldTime = 0.2f;

        private float _jumpInputStartTime;
        private float _dashInputStartTime;

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();

            //int count = Enum.GetValues(typeof(CombatInputs)).Length;
            //_attackInputs = new bool[count];

            cam = Camera.main;
        }

        private void Update()
        {
            Check_jump_inputHoldTime();
            CheckDash_inputHoldTime();
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

        public void On_jumpInput(InputAction.CallbackContext context)
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

        public void On_dashDirectionInput(InputAction.CallbackContext context)
        {
            _raw_dashDirectionInput = context.ReadValue<Vector2>();

            if(playerInput.currentControlScheme == "Keyboard")
            {
                _raw_dashDirectionInput = cam.ScreenToWorldPoint((Vector3)_raw_dashDirectionInput) - transform.position;
            }

            _dashDirectionInput = Vector2Int.RoundToInt(_raw_dashDirectionInput.normalized);
        }

        public void Use_jumpInput() => _jumpInput = false;

        public void UseDashInput() => _dashInput = false;

        private void Check_jump_inputHoldTime()
        {
            if(Time.time >= _jumpInputStartTime + _inputHoldTime)
            {
                _jumpInput = false;
            }
        }

        private void CheckDash_inputHoldTime()
        {
            if(Time.time >=  _dashInputStartTime + _inputHoldTime)
            {
            _dashInput = false;
            }
        }
    }
}