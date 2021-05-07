using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
    public class PlayerData : ScriptableObject
    {
        #region variables
        [Header("Move State")]
        public float movementVelocity = 10f;

        [Header("Jump State")]
        public float jumpVelocity = 15f;
        public int amountOfJumps = 1;


        [Header("In Air State")]
        public float coyoteTime = 0.2f;
        public float variableJumpHeightMultiplier = 0.5f;

        [Header("Wall Slide State")]
        public float wallSlideVelocity = 3f;


        [Header("Dash State")]
        public float dashCooldown = 0.5f;
        public float maxHoldTime = 1f;
        public float holdTimeScale = 0.25f;
        public float dashTime = 0.2f;
        public float dashVelocity = 30f;
        public float drag = 10f;
        public float dashEndYMultiplier = 0.2f;
        public float distBetweenAfterImages = 0.5f;

        [Header("Crouch States")]
        public float crouchMovementVelocity = 5f;
        public float crouchColliderHeight = 0.8f;
        public float standColliderHeight = 1.6f;
        public float ceilingHeight = 0.5f;

        [Header("Attack State")]

        public float damageMultiplier = 0f;
        public float powerBaseWerewolf = 10f;
        public float rangeBaseWerewolf = 0.35f;
        public float powerBaseHuman = 10f;
        public float baseHumanCooldown = 0.5f;
        public float baseWerewolfCooldown = 0.5f;

        [Header("Layer masks")]
        public LayerMask whatIsGround;
        public LayerMask whatIsCeiling;
        public LayerMask whatIsWall;
        public LayerMask whatIsLedge;

        public float ceilingCheckRadius = 0.3f;
        public float groundCheckRadius = 0.3f;
        public float wallCheckDistance = 0.5f;

        [Header("gameplay")]

        public float points = 0f;
        public float experienceMultiplier = 0f;
        public float maxLifePoints = 100f;
        public float currentLifePoints = 100f;
        public float transformStateDuration = 10f;

        public float _startTransformationTime;

        #endregion

        #region Sprite
        [Header("Sprites")]

        [Header("Human")]
        public Sprite humanFace;
        public Sprite humanHat;
        public Sprite humanArms;
        public Sprite humanFeet;
        public Sprite humanClothes;

        [Header("wolf")]

        public Sprite wolfFace;
        public Sprite wolfEars;
        public Sprite wolfArms;
        public Sprite wolfFeet;
        public Sprite wolfBody;

        #endregion

        #region methods
        private void Start(){
            points = 0;
        }

        #endregion
    }
}
