using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using tsunahiki.game;
using tsunahiki.forceGauge.state;
using tsunahiki.forceGauge.stateController;


namespace tsunahiki.forceGauge.stateController
{
    public class MasterStateController : StateControllerBase
    {

        public enum StateType
        {
            SetUp,
            Ready,
            Fight,
            EndOfFight,
            GameSet,
            Calibration,
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

            stateDic[(int)StateType.EndOfFight] = gameObject.AddComponent<EndOfFight>();
            stateDic[(int)StateType.EndOfFight].Initialize((int)StateType.EndOfFight);

            stateDic[(int)StateType.GameSet] = gameObject.AddComponent<GameSet>();
            stateDic[(int)StateType.GameSet].Initialize((int)StateType.GameSet);

            stateDic[(int)StateType.Calibration] = gameObject.AddComponent<Calibration>();
            stateDic[(int)StateType.Calibration].Initialize((int)StateType.Calibration);

            CurrentState = initializeStateType;
            stateDic[CurrentState].OnEnter();
        }
    }
}
