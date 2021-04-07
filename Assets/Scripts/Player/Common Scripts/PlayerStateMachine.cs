using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class PlayerStateMachine 
    {
        #region variables
        public PlayerState _currentState {get; private set;}

        #endregion
        
        #region methods
        /* method that initialize the first state */
        public void Initialize(PlayerState state){
            _currentState =state;
            _currentState.Enter();
        }

        /* change from one state to another one */
        public void ChangeState(PlayerState state){
            _currentState.Exit();
            _currentState = state;
            _currentState.Enter();
        }
        #endregion
    }
}