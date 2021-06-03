using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerTouchingWallState : PlayerState
    {
        protected bool isGrounded;
        protected bool isTouchingWall;
        protected bool grabInput;
        protected bool jumpInput;
        protected bool isTouchingLedge;
        protected int xInput;
        protected int yInput;

        public PlayerTouchingWallState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName, string wolfAnimBool) : base(player, stateMachine, playerData, animBoolName,wolfAnimBool)
        {
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();
        }

        public override void AnimationTrigger()
        {
            base.AnimationTrigger();
        }

        public override void Checks()
        {
            base.Checks();

            isGrounded = _player.CheckIfGrounded();
            isTouchingWall = _player.CheckIfTouchingWall();
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            xInput = _player._inputHandler._normInputX;
            yInput = _player._inputHandler._normInputY;
            if(!isTouchingWall) Debug.LogError("no touching wall");
           
            if (isGrounded)
            {
                _stateMachine.ChangeState(_player._idleState);
            }
            else if(!isTouchingWall || (xInput != _player._facingDirection))
            {

                _stateMachine.ChangeState(_player._inAirState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}