using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        protected bool _downed = false;

        
        public PlayerState(Player player, PlayerStateMachine stateMachine, PlayerData data, string animationName){
            _player = player;
            _stateMachine = stateMachine;
            _aniamtionName = animationName;
            _playerData = data;
        }

        public virtual void Enter(){
            Checks();

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
           /*Debug.LogWarning("detarnsformation input = "+_detransformationInput +"is human = "+_player.GetIsHuman()+"time = "+Time.time+" startTime + transformduration = "+
                        (_player.GetPlayerData()._startTransformationTime + _player.GetPlayerData().transformStateDuration)+ " startime = "+_player.GetPlayerData()._startTransformationTime);*/
            if(!_player.GetIsHuman()){
                if(Time.time > _player.GetPlayerData()._startTransformationTime + _player.GetPlayerData().transformStateDuration){
                    //Debug.LogError("time = "+Time.time+" startTime + transformduration = "+
                        //_player.GetPlayerData()._startTransformationTime + _player.GetPlayerData().transformStateDuration+ " startime = "+_player.GetPlayerData()._startTransformationTime);
                    //_player._stateMachine.ChangeState(_player._detransformationState);
                    _detransformationInput = true;
                }
            }
        }

        protected void SetAnimatorBools(bool value)
        {
            switch (_aniamtionName)
            {
                case "Idle":
                    _player.state.Idle = value;
                    break;
                case "Move":
                    _player.state.Move = value;
                    break;
                case "InAir":
                    _player.state.InAir = value;
                    break;
                case "CrouchIdle":
                    _player.state.CrouchIdle = value;
                    break;
                case "CrouchMove":
                    _player.state.CrouchMove = value;
                    break;
                case "Land":
                    _player.state.Land = value;
                    break;
                case "WallSlide":
                    _player.state.WallSlide = value;
                    break;
                case "Dash":
                    _player.state.Dash = value;
                    break;
                case "attack":
                    _player.state.attack = value;
                    break;
                case "transformation":
                    _player.state.transformation = value;
                    break;
                case "detrasformation":
                    _player.state.detrasformation = value;
                    break;
                case "Down":
                    _player.state.Down = value;
                    break;
                case "DownMove":
                    _player.state.DownMove = value;
                    break;
                case "Interact":
                    _player.state.Interact = value;
                    break;
            }
        }

        public void SetDowned(bool value){
            _downed = value;
        }
        
        public bool isDowned(){
            return _downed;
        }
        public virtual void AnimationTrigger() { }

        public virtual void AnimationFinishTrigger() => _isAnimationFinished = true;
    }
}
