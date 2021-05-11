using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class PlayerInteractState : PlayerAbilityState
    {
        #region variables
        private float _timeToInteract = 4.018f;
        #endregion

        #region methods
        public PlayerInteractState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
        {
        }

        public override void Checks()
        {
            base.Checks();
        }

        public override void Enter()
        {
            base.Enter();
            _player._inputHandler.UseInteractInput();
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

            //TODO if i am healing a player, if ability is not done, mantein him down
            // else trigger is " stand state"
            //if i am dead while i was interacting adn ability was not done, force death state.

            if(Time.time > _startTime + _timeToInteract){
                //TODO once the animation finished correctly, trigger Quest start/accepted functions
                _isAbilityDone = true;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public bool CheckIfCanInteract(){
            return true; //TODO check if can interact with downed player and so heals him, or if is within a quest room
        }

        #endregion

    }
}
