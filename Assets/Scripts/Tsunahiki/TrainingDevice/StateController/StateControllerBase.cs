using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using tsunahiki.game;
using tsunahiki.trainingDevice.state;


namespace tsunahiki.trainingDevice.stateController 
{
    public abstract class StateControllerBase : MonoBehaviour
    {
        public Dictionary<int, StateBase> stateDic = new Dictionary<int, StateBase>();

        public int CurrentState { protected set; get;}

        public abstract void Initialize(int initializeStateType);



        public void UpdateSequence()
        {
            int nextState = (int)stateDic[CurrentState].StateUpdate();
            AutoStateTransitionSequence(nextState);

            Debug.Log("Current state is " + stateDic[CurrentState].GetType());
        }


        protected void AutoStateTransitionSequence(int nextState)
        {
            if (CurrentState == nextState)
            {
                return;
            }

            stateDic[CurrentState].OnExit();
            CurrentState = nextState;
            stateDic[CurrentState].OnEnter();
        }

    }
}