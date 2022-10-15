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

namespace Fishing.State
{

    public class DuringFishing_FishOnTheHook : MasterStateBase
    {
        // タイムカウント
        float currentTimeCount;

        // 待機時間
        static readonly float waitDuration = 60f;

        // 魚のオブジェクト
        public Fish fish;

        // 直前の位置
        private float _previousPosition;

        // 直前の位置登録の時刻
        private float _whenPreviousPosition;

        public override void OnEnter()
        {
            Debug.Log("DuringFishing_FishOnTheHook");
            currentTimeCount = 0f;

            // 魚の初期化
            fish = GameObject.FindWithTag("fish").GetComponent<Fish>();
            fish.species = "fish";
            fish.weight = 3.0f;
            fish.HP = 1.0f;
            fish.difficultyOfEscape = 1.0f;
            fish.maxIntensityOfMovements = 1.0f;
        }

        public override void OnExit()
        {
            // Do Nothing.
        }

        public override int StateUpdate()
        {
            currentTimeCount += Time.deltaTime;

            // 魚のHPは単調減少
            fish.HP = fish.HP - masterStateController.changeRateOfHP * Time.deltaTime;

            // 魚の暴れる強さ
            // 0と1の間を周期的に変化する
            fish.currentIntensityOfMovements = fish.maxIntensityOfMovements * Mathf.Abs(Mathf.Sin(currentTimeCount / masterStateController.periodOfFishIntensity));
            // fish.currentIntensityOfMovements = fish.maxIntensityOfMovements * Mathf.Abs(((currentTimeCount / masterStateController.periodOfFishIntensity) - Mathf.Floor(currentTimeCount / masterStateController.periodOfFishIntensity)) * 2.0f - 1.0f);

            // 逃げにくさの更新
            // HPがゼロになったら更新しない
            if (fish.HP > 0.0f){
                if (Mathf.Abs(fish.currentIntensityOfMovements - masterStateController.trainingDevice.currentRelativePosition) > masterStateController.allowableDifference){
                    fish.difficultyOfEscape = fish.difficultyOfEscape - masterStateController.changeRateOfEscape * Time.deltaTime;
                } else {
                    fish.difficultyOfEscape = fish.difficultyOfEscape + masterStateController.changeRateOfEscape * Time.deltaTime;
                }
            }

            // 魚が逃げる
            if (fish.difficultyOfEscape < 0.0f){
                return (int)MasterStateController.StateType.DuringFishing_Wait;
            }

            // 直前の位置の更新
            if ((_whenPreviousPosition - currentTimeCount) > masterStateController.timeOfRaising){
                _previousPosition = masterStateController.trainingDevice.currentRelativePosition;
            }

            //HPがゼロになって、かつ竿を振り上げたら、魚ゲット
            if ((fish.HP < 0.0f) && ((masterStateController.trainingDevice.currentRelativePosition - _previousPosition) > masterStateController.lengthOfRasing)){
                return (int)MasterStateController.StateType.AfterFishing;
            }
            

            return (int)StateType;
        }
    }

}
