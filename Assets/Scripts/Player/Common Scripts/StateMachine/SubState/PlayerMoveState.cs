using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerMoveState : PlayerGroundedState
    {
        public PlayerMoveState(Player player, PlayerStateMachine stateMachine,PlayerData data, string animation,string wolfAnimBool) : base(player, stateMachine,data,animation,wolfAnimBool)
        {
        }

        public override void Checks()
        {
            base.Checks();
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

            _player.CheckIfShouldFlip(_xInput);
            if(_player.state.slowDebuff)
                _player.SetVelocityX(_player.GetPlayerData().velocityWhenSlowed * _xInput);
            else _player.SetVelocityX(_player.GetPlayerData().movementVelocity * _xInput);

            if (!_isExitingState)
            {
                if (_xInput == 0)
                {
                    _stateMachine.ChangeState(_player._idleState);
                }
                else if (_yInput == -1 && (!_player.CheckIfTouchingPlayerOrEnemy() && !_player.CheckIfTouchingWall()))
                {
                    if(_player.GetIsHuman()){
                        _player.GetComponent<BoxCollider2D>().offset = new Vector2(-4.783879f,-18.35041f);
                        _player.GetComponent<BoxCollider2D>().size = new Vector2(38.76051f,10.20428f);
                    }else{
                        _player.GetComponent<BoxCollider2D>().offset = new Vector2(-5.583345f,-9.130901f);
                        _player.GetComponent<BoxCollider2D>().size = new Vector2(36.60429f,29.61012f);
                    }
                    _stateMachine.ChangeState(_player._crouchMoveState);
                }
            }        
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

       
       
    }
}
