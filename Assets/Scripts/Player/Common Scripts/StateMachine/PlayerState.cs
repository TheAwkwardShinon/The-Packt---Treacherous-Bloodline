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

        
        public PlayerState(Player player, PlayerStateMachine stateMachine, PlayerData data, string animationName){
            _player = player;
            _stateMachine = stateMachine;
            _aniamtionName = animationName;
            _playerData = data;
        }

        public virtual void Enter(){
            Checks();
            _player._anim.SetBool(_aniamtionName,true);
            _startTime = Time.time;
            _isAnimationFinished = false;
            _isExitingState = false;
        }

        public virtual void Exit(){
            _player._anim.SetBool(_aniamtionName,false);
        }

         public virtual void LogicUpdate()
        {
            if(!_player.GetIsHuman())
                if(Time.time > _player.GetPlayerData()._startTransformationTime + _player.GetPlayerData().transformStateDuration)
                    _player._stateMachine.ChangeState(_player._detransformationState);
        }

        public virtual void PhysicsUpdate()
        {
            Checks();
        }


        public virtual void FixedUpdate(){

        }

       
        public virtual void Checks(){

        }
        
         public virtual void AnimationTrigger() { }

        public virtual void AnimationFinishTrigger() => _isAnimationFinished = true;
    }
}
