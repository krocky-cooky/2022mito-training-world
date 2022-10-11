using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using Fishing.Game;
using Fishing.State;
using Fishing.StateController;

namespace Fishing.StateController
{
    public class MasterStateController : StateControllerBase
    {
        public enum StateType
        {
            BeforeFishing,
            DuringFishing_Wait,
            DuringFishing_HoldFish,
            AfterFishing,
        }

        // 初期化処理
        public override void Initialize(int initializeStateType)
        {
            // ここ自動化したいな
            stateDic[(int)StateType.BeforeFishing] = gameObject.AddComponent<BeforeFishing>();
            stateDic[(int)StateType.BeforeFishing].Initialize((int)StateType.BeforeFishing);

            stateDic[(int)StateType.DuringFishing_Wait] = gameObject.AddComponent<DuringFishing_Wait>();
            stateDic[(int)StateType.DuringFishing_Wait].Initialize((int)StateType.DuringFishing_Wait);

            stateDic[(int)StateType.DuringFishing_HoldFish] = gameObject.AddComponent<DuringFishing_HoldFish>();
            stateDic[(int)StateType.DuringFishing_HoldFish].Initialize((int)StateType.DuringFishing_HoldFish);

            stateDic[(int)StateType.AfterFishing] = gameObject.AddComponent<AfterFishing>();
            stateDic[(int)StateType.AfterFishing].Initialize((int)StateType.AfterFishing);

            CurrentState = initializeStateType;
            stateDic[CurrentState].OnEnter();
        }
    }
}