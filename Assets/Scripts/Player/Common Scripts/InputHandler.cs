using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ThePackt.Werewolf;

namespace ThePackt{
    public class InputHandler : MonoBehaviour
    {
        #region variables
        private Werewolf _player;

        private MovementController _moveControl;
        private CombatSystem _combat;
        private AnimatorHandler _anim;
        private bool _allowInput = true; //true until we implement aniamtor
        private Vector2 _movement;
        [SerializeField] private GameObject _camera;

        #endregion

        #region costants
        private const string PLAYER_IDLE = "idle";
        private const string PLAYER_MOVE = "move";
        private const string PLAYER_JUMP = "jump";

        private const string PLAYER_DASH = "dash";

        private const string PLAYER_JUMP_ATTACK = "jump_attack";
        private const string PLAYER_CROUCH = "crouch";

        private const string PLAYER_CROUCH_MOVE = "crouch_move";

        private const string PLAYER_CROUCH_ATTACK = "crouch_attack";

        private const string PLAYER_HUMAN_ATTACK = "human_attack";

        private const string PLAYER_WEREWOLF_ATTACK = "werewolf_attack";

        private const string PLAYER_USE_ITEM = "using_item";

        private const string PLAYER_SPECIAL_ABILITY = "special_ability";

        private const string PLAYER_ULTIMATE = "ultimate";

        private const string PLAYER_UNCONSCIOUS = "unconscious";

        private const string PLAYER_INTERACT = "interact";

        private const string PLAYER_HEALING_BUDDY = "healing_buddy";

        private const string PLAYER_COMEBACK = "comeback"; //from unconscious state

        private const string PLAYER_DEATH = "death";

        private const string PLAYER_TRANSFORMATION = "transformation";

        private const string PLAYER_BACK_HUMAN = "detrasfromation";
        #endregion


        #region methods

        private void Start()
        {
            _player = GetComponent<Werewolf>();
            _combat = GetComponent<CombatSystem>();
            _moveControl = GetComponent<MovementController>();
        }

        /* in the update we shoul check if the palyer is performing inputs */
        private void Update()
        {
            
            if (_allowInput) //true until we implement aniamtor
            {
                /*
                StartCoroutine("HandleMovementInput");
                StartCoroutine("HandleDashInput");
                StartCoroutine("HandleJumpInput");
                StartCoroutine("HandleCrouchInput");
                StartCoroutine("HandleAttackInput");
                StartCoroutine("HandleTransformationInput");
                */
                HandleMovementInput();
                HandleDashInput();
                HandleJumpInput();
                HandleCrouchInput();
                HandleTransformationInput();
                HandleAttackInput();

            }
        }

        /* method that check if is possible to move, changes state, activates the aniamtion trigger and then call the handler in order to perform teh action */
         private void HandleMovementInput()
        {
            bool validPos = _player.GetIsGrounded() || _player.GetIsOnEnemy();
             if (_player.GetCurrentState().Equals(State.IDLE) && validPos || _player.GetCurrentState().Equals(State.CROUCH) && validPos)
            {
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)){
                    _movement.x = Input.GetAxisRaw("Horizontal");
                    _movement.y = 0;
                    _movement.Normalize();
                    _player.SetDirection(_movement);
                    if( _player.GetCurrentState().Equals(State.CROUCH)){
                        _player.SetCurrentState(State.CROUCH_MOVE);
                        //TODO ANIAMTION TRIGGER
                    }
                    else{ 
                        _player.SetCurrentState(State.MOVE);
                        //TODO ANIMATION TRIGGER
                    }
                    _moveControl.Moving();
                }
            }
            else if(_player.GetCurrentState().Equals(State.MOVE) && !validPos || _player.GetCurrentState().Equals(State.IDLE) && !validPos){
                if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)){  /* falling in the right direction --> seems more realistic */
                    _movement.x = Input.GetAxisRaw("Horizontal");
                    _movement.y = Physics2D.gravity.y *100f;
                    _movement.Normalize();
                    _player.SetDirection(_movement);
                    _player.SetCurrentState(State.MOVE);
                    _moveControl.Moving();
                }
                else if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
                    _player.SetCurrentState(State.IDLE);
            }
        }


        /* method that check if is possible to dash, changes state, activates the aniamtion trigger and then call the handler in order to perform teh action */

        private void HandleDashInput(){
            if(_player.GetCurrentState().Equals(State.IDLE)||_player.GetCurrentState().Equals(State.MOVE)){
                if (Input.GetKey(KeyCode.Space)){ //TODO adding stamina??? not a good idea to dash-spamming.
                    _player.SetCurrentState(State.DASH);
                    _moveControl.Dashing();
                    //TODO set trigger like : _animatorHandler.ActivateTargetTrigger("Dash");
                    //if stamina then spend it here.
                }
            }
        }

        /* method that check if is possible to jump, changes state, activates the aniamtion trigger and then call the handler in order to perform teh action */

        private void HandleJumpInput(){
            if(_player.GetCurrentState().Equals(State.IDLE) &&_player.GetIsGrounded()||_player.GetCurrentState().Equals(State.MOVE) && _player.GetIsGrounded()){
                if(Input.GetKeyDown(KeyCode.W)){
                    Debug.Log(_player.GetIsGrounded());
                    _player.SetCurrentState(State.JUMP);
                    _moveControl.Jumping();
                    //TODO set animator trigger
                } 
            }
            //TODO SPECIAL ABILITY DOUBLE JUMP
        }

        /* method that check if is possible to move, changes state, activates the aniamtion trigger. This method checks if you release the input in order to change state */

        private void HandleCrouchInput()
        {
            if(_player.GetCurrentState().Equals(State.IDLE) && _player.GetIsGrounded()||_player.GetCurrentState().Equals(State.MOVE) 
                || _player.GetCurrentState().Equals(State.CROUCH) || _player.GetCurrentState().Equals(State.CROUCH_MOVE) ){
                if (Input.GetKey(KeyCode.S))
                {
                    _player.SetCurrentState(State.CROUCH);
                    Debug.Log("["+Time.time+"]"+ " is crouched");
                    //TODO set animator trigger
                }
                if (Input.GetKeyUp(KeyCode.S))
                {
                    _player.SetCurrentState(State.IDLE);
                     Debug.Log("["+Time.time+"]"+ " not crouched anymore");
                    //TODO set animator trigger
                }
            }
        }

        /* method that check if is possible to attack, changes state, activates the aniamtion trigger and then call the handler in order to perform teh action */

        private void HandleAttackInput()
        {
            // set the right conditions
            if (_player.GetCurrentState().Equals(State.IDLE)||_player.GetCurrentState().Equals(State.MOVE) || _player.GetCurrentState().Equals(State.CROUCH)){
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (_player.GetIsHuman())
                    {
                        Vector2 mousePos = _camera.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));

                        Transform attPoint = _player.GetAttackPoint();
                        Vector2 attPointPos = attPoint.transform.position;
                        bool clickedLeft = true;
                        if (attPointPos.x < mousePos.x)
                        {
                            clickedLeft = false;
                        }

                        //checks if the player is facing towards the point of the click: the attack is executed only if he does
                        if (_player.GetIsFacingLeft() == clickedLeft)
                        {
                            _player.SetCurrentState(State.ATTACK);

                            Vector2 attackDir = mousePos - attPointPos;
                            attPoint.transform.right = attackDir;

                            Debug.Log("[" + Time.time + "]" + "pressed human attack");
                            _combat.BaseHumanAttack();
                        }
                    }
                    else
                    {
                        _player.SetCurrentState(State.ATTACK);

                        Debug.Log("["+Time.time+"]"+"pressed werewolf attack");
                        _combat.BaseWereWolfAttack();
                        //TODO set animator trigger
                    }
                }
            }
        }

        /* method that check if is possible to transform, changes state, activates the aniamtion trigger and then call the handler in order to perform teh action */

        private void HandleTransformationInput()
        {
            // player can transform while jumping and while moving?
            if (_player.GetCurrentState().Equals(State.IDLE) && _player.GetIsGrounded())
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (_player.GetIsHuman())
                    {
                        _player.SetCurrentState(State.TRANSFORM);
                        Debug.Log("[" + Time.time + "]" + "transforming to werewolf");

                        //TODO set animator trigger
                        _player.SetIsHuman(false);

                        _player.SetCurrentState(State.IDLE);
                    }
                    else
                    {
                        _player.SetCurrentState(State.TRANSFORM);

                        Debug.Log("[" + Time.time + "]" + "transforming to human");

                        //TODO set animator trigger
                        _player.SetIsHuman(true);

                        _player.SetCurrentState(State.IDLE);
                    }
                }
            }
        }
        #endregion
    }
}
