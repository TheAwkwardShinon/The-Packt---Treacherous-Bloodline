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
            _player.SetIsHuman(false);

        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            _player.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = _player.GetPlayerData().wolfEars;
            _player.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = _player.GetPlayerData().wolfFace;
            _player.transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = _player.GetPlayerData().wolfBody;
            _player.transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = _player.GetPlayerData().wolfArms;
            _player.transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = _player.GetPlayerData().wolfArms;

            if(Time.time > _startTime + _transformationTime){
                _player.GetPlayerData()._startTransformationTime = Time.time;
                _isAbilityDone = true;
            }


        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

    }
}
