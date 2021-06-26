using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

namespace ThePackt{
    public class PlayerState
    {
        protected Player _player;
        protected PlayerStateMachine _stateMachine;

        protected float _startTime;
        private string _aniamtionName;

        private PlayerData _playerData;

        protected bool _isAnimationFinished;

        protected bool _isExitingState;

        protected bool _detransformationInput = false;

        protected string _wolfAnimationName;
        protected string _aniamtionToShow;

        



        
        public PlayerState(Player player, PlayerStateMachine stateMachine, PlayerData data, string animationName,string wolfAnimationName){
            _player = player;
            _stateMachine = stateMachine;
            _aniamtionName = animationName;
            _wolfAnimationName = wolfAnimationName;
            _playerData = data;
        }

        public virtual void Enter(){
            Checks();
            if(_player.GetIsHuman())
                _aniamtionToShow = _aniamtionName;
            else _aniamtionToShow = _wolfAnimationName;
            SetAnimatorBools(true);

            //_player._anim.SetBool(_aniamtionName,true);

            
                
            _startTime = Time.time;
            _isAnimationFinished = false;
            _isExitingState = false;
        }

        public virtual void Exit(){

            SetAnimatorBools(false);
            //_player._anim.SetBool(_aniamtionName,false);
        }

         public virtual void LogicUpdate()
        {
            Checks();
        }

        public virtual void PhysicsUpdate()
        {
            
        }


        public virtual void FixedUpdate(){

        }

       
        public virtual void Checks(){
            
            if(!_player.GetIsHuman()){
                if(Time.time > _player.GetPlayerData()._startTransformationTime + _player.GetPlayerData().transformStateDuration){
                    _detransformationInput = true;
                }
            }

            if(_playerData.tasteLikeIron && _playerData.tasteLikeIronStart.Count > 0){
                for(int i = 0; i< _playerData.tasteLikeIronStart.Count; i++){
                    if(Time.time >= _playerData.tasteLikeIronStart[i] + _playerData.tasteLikeIronDuration){
                        _playerData.TateLikeIronStack --;
                        _playerData.tasteLikeIronStart.RemoveAt(i);
                        _playerData.movementVelocityMultiplier -= 0.05f;
                    }
                }
            }

            if(_playerData.isSlowed && Time.time > _playerData.debuffStartTime + _playerData.timeOfSlow)
                _player.RemoveSlowDebuff();

            if(_playerData.isFogDebuffActive && Time.time > _playerData.debuffFogStartTime + _playerData.timeOffogDebuff)
                _player.RemoveFogDebuff();
            
            if(_playerData.isDmgReductionDebuffActive && Time.time > _playerData.damageReductionDebuffStartTime+ _playerData.timeOfDmgReduction)
                _player.RemoveDmgReductionDebuff();
        }

        protected void SetAnimatorBools(bool value)
        {
            switch (_aniamtionToShow)
            {
                case "IdleHuman":
                    _player.state.IdleHuman = value;
                    break;
                case "MoveHuman":
                    _player.state.MoveHuman = value;
                    break;
                case "InAirHuman":
                    _player.state.InAirHuman = value;
                    break;
                case "CrouchHuman":
                    _player.state.CrouchHuman = value;
                    break;
                case "CrouchMoveHuman":
                    _player.state.CrouchMoveHuman = value;
                    break;
                case "LandHuman":
                    _player.state.LandHuman = value;
                    break;
                case "WallSlideHuman":
                    _player.state.WallSlideHuman = value;
                    break;
                case "DashHuman":
                    _player.state.DashHuman = value;
                    break;
                case "AttackHuman":
                    _player.state.AttackHuman = value;
                    break;
                case "Transformation":
                    _player.state.Transformation = value;
                    break;
                case "Detransformation":
                    _player.state.Detransformation = value;
                    break;
                case "DownedHuman":
                    _player.state.DownedHuman = value;
                    break;
                case "DownedMoveHuman":
                    _player.state.DownedMoveHuman = value;
                    break;
                case "InteractHuman":
                    _player.state.InteractHuman = value;
                    break;
                case "SpecialWolf":
                    _player.state.SpecialWolf = value;
                    break;
                case "IdleWolf":
                    _player.state.IdleWolf = value;
                    break;
                case "MoveWolf":
                    _player.state.MoveWolf = value;
                    break;
                case "InAirWolf":
                    _player.state.InAirWolf = value;
                    break;
                case "CrouchWolf":
                    _player.state.CrouchWolf = value;
                    break;
                case "CrouchMoveWolf":
                    _player.state.CrouchMoveWolf = value;
                    break;
                case "LandWolf":
                    _player.state.LandWolf = value;
                    break;
                case "WallSlideWolf":
                    _player.state.WallSlideWolf = value;
                    break;
                case "DashWolf":
                    _player.state.DashWolf = value;
                    break;
                case "AttackWolf":
                    _player.state.AttackWolf = value;
                    break;
                case "DownedWolf":
                    _player.state.DownedWolf = value;
                    break;
                case "DownedMoveWolf":
                    _player.state.DownedMoveWolf = value;
                    break;
                case "InteractWolf":
                    _player.state.InteractWolf = value;
                    break;
            }
        }

  
        public virtual void AnimationTrigger() { }

        public virtual void AnimationFinishTrigger() => _isAnimationFinished = true;
        
    }
}
