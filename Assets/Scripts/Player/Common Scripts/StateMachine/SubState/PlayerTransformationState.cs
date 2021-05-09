using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerTransformationState : PlayerAbilityState
    {
        private float _transformationTime = 0.251f;
        public PlayerTransformationState(Player player, PlayerStateMachine stateMachine, PlayerData data, string animationName) : base(player, stateMachine, data, animationName)
        {
        }

        public override void Checks()
        {
            base.Checks();
        }

        public override void Enter()
        {
            base.Enter();
            Debug.LogError("TRANSFORMATION : ABILITY START");
          
            _player._inputHandler.UseTransformInput();

            _player.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = _player.GetPlayerData().wolfEars;
            _player.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = _player.GetPlayerData().wolfFace;
            _player.transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = _player.GetPlayerData().wolfBody;
            _player.transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = _player.GetPlayerData().wolfArms;
            _player.transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = _player.GetPlayerData().wolfArms;

        }

        public override void Exit()
        {
            base.Exit();
            
            Debug.LogError("TRANSFORMATION : ABILITY DONE");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
           

            if(Time.time > _startTime + _transformationTime){
                _isAbilityDone = true;
            }


        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

    }
}
