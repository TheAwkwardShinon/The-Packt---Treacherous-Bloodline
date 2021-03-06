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
        public PlayerInteractState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName,string wolfAnimBool) : base(player, stateMachine, playerData, animBoolName,wolfAnimBool)
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

            _player.PlayInteractSFX();

            if(_interactionType.Equals("player")){

                if (BoltNetwork.IsServer)
                {
                    var evnt = StartHealingEvent.Create(_interactionTarget.GetComponent<Player>().entity.Source);
                    evnt.TargetPlayerNetworkID = _interactionTarget.GetComponent<Player>().entity.NetworkId;
                    evnt.Send();
                }
                else
                {
                    var evnt = StartHealingEvent.Create(BoltNetwork.Server);
                    evnt.TargetPlayerNetworkID = _interactionTarget.GetComponent<Player>().entity.NetworkId;
                    evnt.Send();
                }
            }
            _player._interactTooltip.SetActive(false);
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
                    Debug.LogWarning("[INTERACTION STATE] heal event started");

                    if (BoltNetwork.IsServer)
                    {
                        var evnt = HealEvent.Create(_interactionTarget.GetComponent<Player>().entity.Source);
                        evnt.TargetPlayerNetworkID = _interactionTarget.GetComponent<Player>().entity.NetworkId;
                        evnt.Send();
                    }
                    else
                    {
                        var evnt = HealEvent.Create(BoltNetwork.Server);
                        evnt.TargetPlayerNetworkID = _interactionTarget.GetComponent<Player>().entity.NetworkId;
                        evnt.Send();
                    }
                }
                else _player.AcceptQuest(_interactionTarget.GetComponentInParent<Quest>());
                _isAbilityDone = true;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public bool CheckIfCanInteract(){
           Collider2D[] col = Physics2D.OverlapCircleAll(_player.transform.position,_player.GetPlayerData().interactRange, 
               _player.GetPlayerData().WhatIsPlayer | _player.GetPlayerData().whatIsRoom);
            
            if (col != null){
                
                foreach(Collider2D hit in col){
                    
                    // Debug.LogError("[INTERACT CHECK] hit object layermask is = "+LayerMask.LayerToName(hit.gameObject.layer));
                    if (LayerMask.LayerToName(hit.transform.root.gameObject.layer).Equals("Players")){
                        if(hit.transform.root.gameObject != _player.gameObject && hit.transform.root.gameObject.GetComponent<Player>()._isDowned &&
                                !hit.transform.root.gameObject.GetComponent<Player>()._isBeingHealed){
                            _interactionType = "player";
                            _interactionTarget = hit.transform.root.gameObject;
                            if(!_player._interactTooltip.activeSelf){
                                _player._interactTooltip.transform.position = new Vector2(hit.transform.position.x,hit.transform.position.y + 2f);
                                _player._interactTooltip.SetActive(true);
                            }
                            return true;   
                        }
                        else continue;
                    }else if(LayerMask.LayerToName(hit.gameObject.layer).Equals("Pillar")){
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
