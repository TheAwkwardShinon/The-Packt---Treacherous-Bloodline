using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class PlayerDownMoveState : PlayerGroundedState
    {
        public PlayerDownMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName,string wolfAnimBool) : base(player, stateMachine, playerData, animBoolName,wolfAnimBool)
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
            Debug.LogWarning("[DOWNED MOVE STATE] ENTER ---> IS STAND = "+_isStand);

        }

        public override void Exit()
        {
            base.Exit();
            Debug.LogWarning("[DOWNED MOVE STATE] EXIT ---> IS STAND = "+_isStand);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if(Time.time >= _player.GetPlayerData().downedStartTime + _player.GetPlayerData().bleedOutTime){
                _player.Die();
                return;
            }


            _player.SetVelocityX(_player.GetPlayerData().crouchMovementVelocity * _player._facingDirection);
            _player.CheckIfShouldFlip(_xInput);

            if(_player.state.Health >= _player.GetPlayerData().maxLifePoints * 0.3f){
                Debug.LogWarning("[DOWNED MOVE STATE] ---> IDLE");
                _player.state.isDowned = false;
                _isStand = true;
                if(_player.GetIsHuman()){
                    _player.GetComponent<BoxCollider2D>().offset = new Vector2(-0.7352595f,-5.962845f);
                    _player.GetComponent<BoxCollider2D>().size = new Vector2(8.667796f,35.94624f);
                }
                else{
                    _player.GetComponent<BoxCollider2D>().offset = new Vector2(-1.780157f,-5.962845f);
                    _player.GetComponent<BoxCollider2D>().size = new Vector2(24.9682f,35.94624f);
                }
                _stateMachine.ChangeState(_player._idleState);
            } 
            else if(_xInput == 0)
            {
                Debug.LogWarning("[DOWNED MOVE STATE] ---> DOWNED");
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