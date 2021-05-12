using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class PlayerDownMoveState : PlayerGroundedState
    {
        public PlayerDownMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
        }

       
        public override void Checks()
        {
            base.Checks();
        }

        public override void Enter()
        {
            base.Enter();
            _isStand = false;
            Debug.Log("[DOWNED MOVE STATE] ENTER ---> IS STAND = "+_isStand);

        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log("[DOWNED MOVE STATE] EXIT ---> IS STAND = "+_isStand);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            _player.SetVelocityX(_player.GetPlayerData().crouchMovementVelocity * _player._facingDirection);
            _player.CheckIfShouldFlip(_xInput);

            if(_player.state.Health >= _player.GetPlayerData().maxLifePoints){
                Debug.LogError("[DOWNED MOVE STATE] ---> IDLE");
                _player.state.isDowned = false;
                _isStand = true;
                //_player.SetColliderHeight(_player.GetPlayerData().standColliderHeight);
                //_player.SetColliderWidth(_player.GetPlayerData().standColliderWidth);
                _stateMachine.ChangeState(_player._idleState);
            } 
            else if(_xInput == 0)
            {
                Debug.LogError("[DOWNED MOVE STATE] ---> DOWNED");
                _stateMachine.ChangeState(_player._downState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}