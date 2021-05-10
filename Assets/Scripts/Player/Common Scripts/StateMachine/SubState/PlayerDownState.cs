using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class PlayerDownState : PlayerGroundedState
    {
        public PlayerDownState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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
            //_player.SetColliderHeight(_player.GetPlayerData().downColliderHeight);
            //_player.SetColliderWidth(_player.GetPlayerData().downColliderWidth);
            Debug.Log("[DOWNED STATE] ENTER");

        }

        public override void Exit()
        {
            base.Exit();
            Debug.LogError("[DOWNED STATE] EXIT ---> stand = "+_isStand);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if(_player.state.Health >= _player.GetPlayerData().maxLifePoints){
                 Debug.LogError("[DOWNED STATE] ---> IDLE");
                _downed = false;
                _isStand = true;
                //_player.SetColliderHeight(_player.GetPlayerData().standColliderHeight);
                //_player.SetColliderWidth(_player.GetPlayerData().standColliderWidth);
                _stateMachine.ChangeState(_player._idleState);
            }else if(_xInput != 0){
                Debug.LogError("[DOWNED STATE] ---> DOWNED MOVE STATE");
                _stateMachine.ChangeState(_player._downMoveState);
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
