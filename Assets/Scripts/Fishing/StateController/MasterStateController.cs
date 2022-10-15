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

        public TrainingDevice trainingDevice;

        // ハンドルの上げ下げのズレの許容量
        public float allowableDifference;

        // 魚の逃げにくさの値の変化速度
        public float changeRateOfEscape;

        // 魚のHPの変化速度
        public float changeRateOfHP;

        // 魚の暴れ具合の変動周期
        public float periodOfFishIntensity;

        // 魚を捕らえるときの竿の振り上げの時間と大きさ
        public float timeOfRaising;
        public float lengthOfRasing;

        // 魚を捕らえたとき
        // 短くなった後の釣り糸の長さ
        public float fishingLineLengthAfterFishing;
        //釣り糸を短くする時間
        public float timeShorteningFishingLine;
        //魚が跳ね上がって静止する位置とカメラの間の距離
        public float distanseFromFishToCamera;
        // 魚が跳ね上がっている時間
        public float timeRasingFish;

        // 水面の位置
        public Transform waterSurfaceTransform;

        // 釣り糸の先端
        public RopeRelayBelowHandle ropeRelayBelowHandle;

        // 魚のオブジェクト
        public Fish fish;

        public enum StateType
        {
            BeforeFishing,
            DuringFishing_Wait,
            DuringFishing_FishOnTheHook,
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

            stateDic[(int)StateType.DuringFishing_FishOnTheHook] = gameObject.AddComponent<DuringFishing_FishOnTheHook>();
            stateDic[(int)StateType.DuringFishing_FishOnTheHook].Initialize((int)StateType.DuringFishing_FishOnTheHook);

            stateDic[(int)StateType.AfterFishing] = gameObject.AddComponent<AfterFishing>();
            stateDic[(int)StateType.AfterFishing].Initialize((int)StateType.AfterFishing);

            CurrentState = initializeStateType;
            stateDic[CurrentState].OnEnter();
        }
    }
}