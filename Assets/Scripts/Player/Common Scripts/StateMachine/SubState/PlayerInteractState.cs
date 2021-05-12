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

            ///TODO CHECK INTERACTION TYPE

            /// acceptQuest
            //TODO if i am healing a player, if ability is not done, resta li per TERRA!!! XDXDXDXDXDXD !111!111
            // else trigger is " stand state"
            
            //if i am dead while i was interacting adn ability was not done, force death state.

            //on enter if target = player
           /* var evnt = StartHealingEvent.Create(BoltNetwork.Server);
            evnt.TargetPlayerNetworkID = _player.entity.NetworkId; //però devo farlo dell col testa di cazzo ricordatelo. COL = QUELLO CHE HAI COLPITO.
            evnt.Send();

           // on exit if target was player
            var evnt = HealEvent.Create(BoltNetwork.Server);
            evnt.TargetPlayerNetworkID = _player.entity.NetworkId; //però devo farlo dell col testa di cazzo ricordatelo. COL = QUELLO CHE HAI COLPITO.
            evnt.Send();*/



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
            if(_player.CheckIfOtherPlayerInRangeMayBeHealed())
                return true;
            return true; //TODO check if can interact with downed player and so heals him, or if is within a quest room
        }

        #endregion

    }
}
