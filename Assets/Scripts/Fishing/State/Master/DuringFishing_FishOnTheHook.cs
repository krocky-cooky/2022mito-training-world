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

    public class DuringFishing_FishOnTheHook : StateBase
    {
        // タイムカウント
        float currentTimeCount;

        // 待機時間
        static readonly float waitDuration = 60f;

        // 魚のオブジェクト
        public Fish fish;

        private Master _gameMaster;

        // 直前の位置
        private float _previousPosition;

        // 直前の位置登録の時刻
        private float _whenPreviousPosition;

        public override void OnEnter()
        {
            Debug.Log("DuringFishing_FishOnTheHook");
            currentTimeCount = 0f;
            _gameMaster = GameObject.FindWithTag("master").GetComponent<Master>();

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
            fish.HP = fish.HP - _gameMaster.changeRateOfHP * Time.deltaTime;

            // 魚の暴れる強さ
            // 0と1の間を周期的に変化する
            fish.currentIntensityOfMovements = fish.maxIntensityOfMovements * Mathf.Abs(Mathf.Sin(currentTimeCount / _gameMaster.periodOfFishIntensity));
            // fish.currentIntensityOfMovements = fish.maxIntensityOfMovements * Mathf.Abs(((currentTimeCount / _gameMaster.periodOfFishIntensity) - Mathf.Floor(currentTimeCount / _gameMaster.periodOfFishIntensity)) * 2.0f - 1.0f);

            // 逃げにくさの更新
            // HPがゼロになったら更新しない
            if (fish.HP > 0.0f){
                if (Mathf.Abs(fish.currentIntensityOfMovements - _gameMaster.trainingDevice.currentRelativePosition) > _gameMaster.allowableDifference){
                    fish.difficultyOfEscape = fish.difficultyOfEscape - _gameMaster.changeRateOfEscape * Time.deltaTime;
                } else {
                    fish.difficultyOfEscape = fish.difficultyOfEscape + _gameMaster.changeRateOfEscape * Time.deltaTime;
                }
            }

            // 魚が逃げる
            if (fish.difficultyOfEscape < 0.0f){
                return (int)MasterStateController.StateType.DuringFishing_Wait;
            }

            // 直前の位置の更新
            if ((_whenPreviousPosition - currentTimeCount) > _gameMaster.timeOfRaising){
                _previousPosition = _gameMaster.trainingDevice.currentRelativePosition;
            }

            //HPがゼロになって、かつ竿を振り上げたら、魚ゲット
            if ((fish.HP < 0.0f) && ((_gameMaster.trainingDevice.currentRelativePosition - _previousPosition) > _gameMaster.lengthOfRasing)){
                return (int)MasterStateController.StateType.AfterFishing;
            }
            

            return (int)StateType;
        }
    }

}
