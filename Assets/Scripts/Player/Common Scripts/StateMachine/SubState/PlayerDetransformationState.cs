using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class PlayerDetransformationState : PlayerAbilityState
    {
        private float _detransformationTime = 0.16f;
        public PlayerDetransformationState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
        }

        public override void Checks()
        {
            base.Checks();
        }

        public override void Enter()
        {
            base.Enter();
            _player.SetIsHuman(true);
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }


        public override void LogicUpdate()
        {
            base.LogicUpdate();
            _player.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = _player.GetPlayerData().humanHat;
            _player.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = _player.GetPlayerData().humanFace;
            _player.transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = _player.GetPlayerData().humanClothes;
            _player.transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = _player.GetPlayerData().humanArms;
            _player.transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = _player.GetPlayerData().humanArms;

            if(Time.time > _startTime + _detransformationTime)
                _isAbilityDone = true;

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

    }
}
