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
    public class MasterStateController : StateControllerBase
    {

        public enum StateType
        {
            SetUp,
            Ready,
            Fight,
            GameSet,
        }

        // 初期化処理
        public override void Initialize(int initializeStateType)
        {
            // ここ自動化したいな
            stateDic[(int)StateType.SetUp] = gameObject.AddComponent<SetUp>();
            stateDic[(int)StateType.SetUp].Initialize((int)StateType.SetUp);

            stateDic[(int)StateType.Ready] = gameObject.AddComponent<Ready>();
            stateDic[(int)StateType.Ready].Initialize((int)StateType.Ready);

            stateDic[(int)StateType.Fight] = gameObject.AddComponent<Fight>();
            stateDic[(int)StateType.Fight].Initialize((int)StateType.Fight);

            stateDic[(int)StateType.GameSet] = gameObject.AddComponent<GameSet>();
            stateDic[(int)StateType.GameSet].Initialize((int)StateType.GameSet);

            CurrentState = initializeStateType;
            stateDic[CurrentState].OnEnter();
        }
    }
}
