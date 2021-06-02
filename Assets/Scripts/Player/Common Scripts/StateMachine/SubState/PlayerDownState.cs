using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class PlayerDownState : PlayerGroundedState
    {
        public PlayerDownState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName,string wolfAnimBool) : base(player, stateMachine, playerData, animBoolName,wolfAnimBool)
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
          
            Debug.LogWarning("[DOWNED STATE] ENTER");

        }

        public override void Exit()
        {
            base.Exit();
            Debug.LogWarning("[DOWNED STATE] EXIT ---> stand = "+_isStand);
        }

        public override void AnimationFinishTrigger()
        {
            if(_player.GetIsHuman()){
                _player.GetComponent<BoxCollider2D>().offset = new Vector2(35.94624f,-18.82179f);
                _player.GetComponent<BoxCollider2D>().size = new Vector2(33.21022f,10.22835f);
            }else{
                _player.GetComponent<BoxCollider2D>().offset = new Vector2(4.416998f,-12.08582f);
                _player.GetComponent<BoxCollider2D>().size = new Vector2(33.30902f,23.7003f);
            }
            
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if(_player.state.Health >= _player.GetPlayerData().maxLifePoints * 0.3f){
                 Debug.LogWarning("[DOWNED STATE] ---> IDLE");
                _isStand = true;
                _player.state.isDowned = false;
                if(_player.GetIsHuman()){
                    _player.GetComponent<BoxCollider2D>().offset = new Vector2(-0.7352595f,-5.962845f);
                    _player.GetComponent<BoxCollider2D>().size = new Vector2(8.667796f,35.94624f);
                }
                else{
                    _player.GetComponent<BoxCollider2D>().offset = new Vector2(-1.780157f,-5.962845f);
                    _player.GetComponent<BoxCollider2D>().size = new Vector2(24.9682f,35.94624f);
                }
                _stateMachine.ChangeState(_player._idleState);
            }else if(_xInput != 0){
                Debug.LogWarning("[DOWNED STATE] ---> DOWNED MOVE STATE");
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
