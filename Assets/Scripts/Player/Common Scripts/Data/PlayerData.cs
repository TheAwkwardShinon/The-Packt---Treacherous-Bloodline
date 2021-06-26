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

        public float movementMultiplierWhenFullLife = 0f;

        public float movementVelocityMultiplier = 0f;

        [Header("Jump State")]
        public float jumpVelocity = 15f;
        public int amountOfJumps = 1;


        [Header("In Air State")]
        public float variableJumpHeightMultiplier = 0.5f;

        [Header("Wall Slide State")]
        public float wallSlideVelocity = 3f;


        [Header("Dash State")]
        public float dashCooldown = 0.5f;
        public float maxHoldTime = 1f;
        public float holdTimeScale = 0.25f;
        public float dashTime = 0.2f;
        public float dashVelocity = 30f;
        public float dashEndYMultiplier = 0.2f;
        public float distBetweenAfterImages = 0.5f;

        [Header("Crouch States")]
        public float crouchMovementVelocity = 5f;
        public float crouchColliderHeight = 0.8f;
        public float standColliderHeight = 1.6f;
        public float standColliderWidth = 0.8f;
        public float ceilingHeight = 0.5f;

        [Header("Down States")]
        public float downColliderHeight = 0.4f;
        public float downColliderWidth = 1.3f;

        public float downedStartTime = 0f;
        public float bleedOutTime = 15f;
        public float numOfReviveAction = 0f;


        [Header("Attack State")]

        public float damageMultiplier = 0f;
        public float powerBaseWerewolf = 10f;
        public float rangeBaseWerewolf = 0.35f;
        public float powerBaseHuman = 10f;
        public float baseHumanCooldown = 0.5f;
        public float baseWerewolfCooldown = 0.5f;

        public float specialAttackCooldown = 15f;

        public float healAfterHit = 0f;

        [Header("Interact State")]

        public float interactRange = 1f;

        [Header("Layer masks")]
        public LayerMask whatIsGround;
        public LayerMask whatIsCeiling;
        public LayerMask whatIsWall;
        public LayerMask whatIsLedge;
        public LayerMask whatIsRoom;

        public LayerMask WhatIsPlayer;
        public LayerMask WhatIsEnemy;

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

        public bool healing = false;

        [Header("Debuff Slow")]
        public bool isSlowed = false;
        public bool cantJump = false;
        public bool cantDash = false;
        public float velocityWhenSlowed = 4f;
        public float timeOfSlow = 0f;
        public float debuffStartTime = 0f;

        [Header("Debuff Fog Of War")]

        public float timeOffogDebuff = 0f;
        public float debuffFogStartTime = 0f;
        public float sizeOfCircle = 0f;
        public float standardCircleSize = 2f;
        public bool isFogDebuffActive = false;

        [Header("Debuff Damage Reduction")]

        public bool isDmgReductionDebuffActive = false;
        public float damageReductionDebuffStartTime = 0f;
        public float dmgReduction = 0f;
        public float timeOfDmgReduction = 0f;

        #endregion


        [Header("particular abilities")]
        public bool astralconjuntion = false;
        public bool tasteLikeIron = false;
        public float tasteLikeIronDuration = 5f;
        public int TateLikeIronStack = 0;

        public List<float> tasteLikeIronStart;
        public float portalRoomDamageMultiplier = 0.6f;

        public bool socialAnimal = false;

        public float socialAnimalMultiplier = 0.06f;
        public int numOfNearPlayer = 0;

        public List<GameObject> _sprites;

        

        #region methods
        private void Start(){
            points = 0;
            tasteLikeIronStart = new List<float>();
        }

        #endregion
    }
}
