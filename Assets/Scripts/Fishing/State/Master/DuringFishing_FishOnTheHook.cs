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

        // 初期トルク
        private float _fisrtTorque;

        // 前回のスパイクの時間
        private float _previousSpikeTime = 0.0f;

        // スパイクの提示時刻
        private float _spikeEndTime = 0.0f;

        // 魚の水音の切り替えフラグ
        private bool _fishSoundIsChanged = false;

        public override void OnEnter()
        {
            Debug.Log("DuringFishing_FishOnTheHook");
            currentTimeCount = 0f;

            // トルクの指定
            _fisrtTorque = Random.Range(masterStateController.minTorque, masterStateController.maxTorque);
            masterStateController.gameMaster.sendingTorque = _fisrtTorque;

            // 魚の初期化
            fish = GameObject.FindWithTag("fish").GetComponent<Fish>();
            fish.weight = _fisrtTorque * masterStateController.fishWeightPerTorque;
            // fish.species = "Sardine";
            // fish.HP = 1.0f;
            // fish.difficultyOfEscape = 1.0f;
            // fish.maxIntensityOfMovements = 1.0f;

            // 音声を再生
            masterStateController.FishSoundOnTheHook.Play();
        }

        public override void OnExit()
        {
            masterStateController.FishSoundWithHP0.Stop();
        }

        public override int StateUpdate()
        {
            currentTimeCount += Time.deltaTime;

            // 魚のHPは単調減少
            fish.HP = fish.HP - masterStateController.changeRateOfHP * Time.deltaTime;

            // 魚の暴れる強さ
            // 0と1の間を周期的に変化する
            fish.currentIntensityOfMovements = fish.maxIntensityOfMovements * Mathf.Abs(Mathf.Sin(currentTimeCount / masterStateController.periodOfFishIntensity));
            // fish.currentIntensityOfMovements = 1.0f;
            // fish.currentIntensityOfMovements = fish.maxIntensityOfMovements * Mathf.Abs(((currentTimeCount / masterStateController.periodOfFishIntensity) - Mathf.Floor(currentTimeCount / masterStateController.periodOfFishIntensity)) * 2.0f - 1.0f);

            // トルク送信
            // masterStateController.gameMaster.sendingTorque = _fisrtTorque + masterStateController.maxAplitudeOfTorque * fish.currentIntensityOfMovements * Mathf.Sin(currentTimeCount * 2.0f * Mathf.PI / masterStateController.periodOfTorque);
            // masterStateController.gameMaster.sendingTorque = _fisrtTorque + masterStateController.maxAplitudeOfTorque * fish.currentIntensityOfMovements * ((float)(Mathf.CeilToInt(currentTimeCount/ masterStateController.periodOfTorque) % 2) - 0.5f);

            // if ((currentTimeCount - _previousSpikeTime) > masterStateController.spikeInterval){
            //     _spikeEndTime = currentTimeCount + masterStateController.spikePeriod;
            //     _previousSpikeTime = currentTimeCount;
            // }
            // if (currentTimeCount < (_spikeEndTime - masterStateController.spikePeriod * 0.5f)){
            //     masterStateController.gameMaster.sendingTorque = _fisrtTorque + masterStateController.spikeSize;
            // }else if(currentTimeCount < _spikeEndTime){
            //     // masterStateController.gameMaster.sendingTorque = _fisrtTorque - masterStateController.spikeSize;
            //     masterStateController.gameMaster.sendingTorque = 0.0f;
            // }else{
            //     masterStateController.gameMaster.sendingTorque = _fisrtTorque;
            // }

            // if ((currentTimeCount - _previousSpikeTime) > masterStateController.spikeInterval){
            //     _spikeEndTime = currentTimeCount + masterStateController.firstSpikePeriod + masterStateController.latterSpikePeriod;
            //     _previousSpikeTime = currentTimeCount;
                
            // }
            // if (currentTimeCount < (_spikeEndTime - masterStateController.latterSpikePeriod)){
            //     masterStateController.gameMaster.sendingTorque = _fisrtTorque + masterStateController.firstSpikeSize;
            //     Debug.Log("fisrt spike");
            // }else if(currentTimeCount < _spikeEndTime){
            //     // masterStateController.gameMaster.sendingTorque = _fisrtTorque - masterStateController.spikeSize;
            //     masterStateController.gameMaster.sendingTorque = _fisrtTorque + masterStateController.latterSpikeSize;
            //     Debug.Log("latter spike");
            // }else{
            //     masterStateController.gameMaster.sendingTorque = _fisrtTorque;
            // }

            // 逃げにくさの更新
            // HPがゼロになったら更新しない
            if (fish.HP > 0.0f){
                if (Mathf.Abs(fish.currentIntensityOfMovements - masterStateController.trainingDevice.currentRelativePosition) > masterStateController.allowableDifference){
                    fish.difficultyOfEscape = fish.difficultyOfEscape - masterStateController.changeRateOfEscape * Time.deltaTime;
                } else {
                    fish.difficultyOfEscape = fish.difficultyOfEscape + masterStateController.changeRateOfEscape * Time.deltaTime;
                }
            }

            // // 魚が逃げる
            // if (fish.difficultyOfEscape < 0.0f){
            //     return (int)MasterStateController.StateType.DuringFishing_Wait;
            // }

            // 魚の音声の切り替え
            if (fish.HP < 0.0f && !(_fishSoundIsChanged)){
                masterStateController.FishSoundOnTheHook.Stop();
                masterStateController.FishSoundWithHP0.Play();
                _fishSoundIsChanged = true;
            }

            // 直前の位置の更新
            if ((_whenPreviousPosition - currentTimeCount) > masterStateController.timeOfRaising){
                _previousPosition = masterStateController.trainingDevice.currentRelativePosition;
            }

            //HPがゼロになって、かつ竿を振り上げたら、魚ゲット
            if ((fish.HP < 0.0f) && (((masterStateController.trainingDevice.currentRelativePosition - _previousPosition) > masterStateController.lengthOfRasing) || Input.GetMouseButtonDown(1))){
                masterStateController.FishGoOnTheWater.Play();
                return (int)MasterStateController.StateType.AfterFishing;
            }
            

            return (int)StateType;
        }

    }

}
