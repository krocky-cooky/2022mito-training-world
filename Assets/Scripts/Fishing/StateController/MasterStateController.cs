using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using Fishing.Game;
using Fishing.State;
using Fishing.StateController;
using Fishing.Object;

namespace Fishing.StateController
{
    public class MasterStateController : StateControllerBase
    {

        public enum StateType
        {
            BeforeFishing,
            DuringFishing_Wait,
            DuringFishing_Nibble,
            DuringFishing_FishOnTheHook,
            DuringFishing_Calibration,
            DuringFishing_HP0,
            DuringFishing_GetAway,
            DuringFishing_FishingLineBreaks,
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

            stateDic[(int)StateType.DuringFishing_Nibble] = gameObject.AddComponent<DuringFishing_Nibble>();
            stateDic[(int)StateType.DuringFishing_Nibble].Initialize((int)StateType.DuringFishing_Nibble);

            stateDic[(int)StateType.DuringFishing_FishOnTheHook] = gameObject.AddComponent<DuringFishing_FishOnTheHook>();
            stateDic[(int)StateType.DuringFishing_FishOnTheHook].Initialize((int)StateType.DuringFishing_FishOnTheHook);

            stateDic[(int)StateType.DuringFishing_Calibration] = gameObject.AddComponent<DuringFishing_Calibration>();
            stateDic[(int)StateType.DuringFishing_Calibration].Initialize((int)StateType.DuringFishing_Calibration);

            stateDic[(int)StateType.DuringFishing_HP0] = gameObject.AddComponent<DuringFishing_HP0>();
            stateDic[(int)StateType.DuringFishing_HP0].Initialize((int)StateType.DuringFishing_HP0);

            stateDic[(int)StateType.DuringFishing_GetAway] = gameObject.AddComponent<DuringFishing_GetAway>();
            stateDic[(int)StateType.DuringFishing_GetAway].Initialize((int)StateType.DuringFishing_GetAway);

            stateDic[(int)StateType.DuringFishing_FishingLineBreaks] = gameObject.AddComponent<DuringFishing_FishingLineBreaks>();
            stateDic[(int)StateType.DuringFishing_FishingLineBreaks].Initialize((int)StateType.DuringFishing_FishingLineBreaks);

            stateDic[(int)StateType.AfterFishing] = gameObject.AddComponent<AfterFishing>();
            stateDic[(int)StateType.AfterFishing].Initialize((int)StateType.AfterFishing);

            CurrentState = initializeStateType;
            stateDic[CurrentState].OnEnter();
        }
    }
}
