using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    #region Required Components
    [RequireComponent(typeof(Animator))]
    #endregion

    public class AnimatorHandler : MonoBehaviour
    {
        #region Variables
        private Animator _animator;
        private int _vertical;
        private int _horizontal;

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


        #region Methods
        public void Initialize()
        {
            _animator = GetComponent<Animator>();
            _vertical = Animator.StringToHash("Vertical");
            _horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
        {
            float v, h;
            v = verticalMovement;
            //TODO add Joystick sensitivity//
            h = horizontalMovement;
            //TODO add Joystick sensitivity//


            if (isSprinting & h != 0){
                v = 2f;
                h = 2f;
            }

            _animator.SetFloat(_vertical, v, 0.1f, Time.deltaTime);
            _animator.SetFloat(_horizontal, h, 0.1f, Time.deltaTime);
        }

        public void UpdateParameter (string targetParameter, bool state)
        {
            _animator.SetBool(Animator.StringToHash(targetParameter), state);
        }
        
        public void ActivateTargetTrigger(string targetTrigger)
        { 
            _animator.SetTrigger(Animator.StringToHash(targetTrigger));
        }
        #endregion

        #region Getter and Setter
        public Animator GetAnimator()
        {
            return _animator;
        }
        #endregion
    }
}
