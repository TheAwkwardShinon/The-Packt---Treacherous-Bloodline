using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class PlayerInteractState : PlayerAbilityState
    {
        #region variables
        private float _timeToInteract = 4.018f;
        private GameObject _interactionTarget;

        private string _interactionType;
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
            if(_interactionType.Equals("player")){
                var evnt = StartHealingEvent.Create(BoltNetwork.Server);
                evnt.TargetPlayerNetworkID = _interactionTarget.GetComponent<Player>().entity.NetworkId;
                evnt.Send();
            }
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

            //TODO STOP HEALING VBOLT EVENT ON Interruption

            if(Time.time > _startTime + _timeToInteract){
                if(_interactionType.Equals("player")){
                    var evnt = HealEvent.Create(BoltNetwork.Server);
                    evnt.TargetPlayerNetworkID = _interactionTarget.GetComponent<Player>().entity.NetworkId; //però devo farlo dell col testa di cazzo ricordatelo. COL = QUELLO CHE HAI COLPITO.
                    evnt.Send();
                }
                else _player.AcceptQuest(_interactionTarget.GetComponent<Quest>());
                _isAbilityDone = true;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public bool CheckIfCanInteract(){
           Collider2D[] col = Physics2D.OverlapCircleAll(_player.transform.position,12f, LayerMask.GetMask(
               LayerMask.LayerToName(_player.GetPlayerData().WhatIsPlayer),LayerMask.LayerToName(_player.GetPlayerData().whatIsRoom)));
            if(col != null){
                foreach(Collider2D hit in col){
                    Debug.LogError("[INTERACT CHECK] hit object layermask is = "+hit.gameObject.layer);
                    if(hit.gameObject.layer == _player.GetPlayerData().WhatIsPlayer){
                        Debug.LogError("[INTERACT CHECK] layermask = player ("+_player.GetPlayerData().WhatIsPlayer+")");
                        if(hit.gameObject != _player.gameObject && hit.gameObject.GetComponent<Player>()._isDowned &&
                                !hit.gameObject.GetComponent<Player>()._isBeingHealed){
                            _interactionType = "player";
                            _interactionTarget = hit.gameObject;
                            if(!_player._interactTooltip.activeSelf){
                                _player._interactTooltip.transform.position = new Vector2(hit.transform.position.x,hit.transform.position.y + 2f);
                                _player._interactTooltip.SetActive(true);
                            }
                            return true;   
                        }
                        else continue;
                    }else if(hit.gameObject.layer == _player.GetPlayerData().whatIsRoom){
                        Debug.LogError("[INTERACT CHECK] layermask = Room("+_player.GetPlayerData().whatIsRoom+")");
                        //TODO IF THE QUEST IS NOT ALREADY ACCEPTED.
                        _interactionType = "quest";
                        _interactionTarget = hit.gameObject;
                        if(_player._interactTooltip.activeSelf)
                            _player._interactTooltip.SetActive(false);
                        return true;
                    }
                    else continue;
                }
                if(_player._interactTooltip.activeSelf)
                    _player._interactTooltip.SetActive(false);
                return false;
            }
            else{
                if(_player._interactTooltip.activeSelf)
                        _player._interactTooltip.SetActive(false);
                 return false;
            }
        }
        #endregion
    }
}