using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using tsunahiki.game;
using tsunahiki.state;
using tsunahiki.stateController;


namespace tsunahiki.stateController
{
    public abstract class StateControllerBase : MonoBehaviour
    {
        public Dictionary<int, StateBase> stateDic = new Dictionary<int, StateBase>();

        // 現在のステート
        public int CurrentState { protected set; get; }

        // 初期化処理
        public abstract void Initialize(int initializeStateType);

        // 更新処理
        public void UpdateSequence()
        {
            int nextState = (int)stateDic[CurrentState].StateUpdate();
            AutoStateTransitionSequence(nextState);

            Debug.Log("Current state is " + stateDic[CurrentState].GetType());
        }

        // ステートの自動遷移
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