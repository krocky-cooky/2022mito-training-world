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
            // _fisrtTorque = Random.Range(masterStateController.minTorque, masterStateController.maxTorque);
            _fisrtTorque = masterStateController.fish.weight / masterStateController.fishWeightPerTorque;
            masterStateController.gameMaster.sendingTorque = _fisrtTorque;

            // // 魚の初期化
            // masterStateController.fish = GameObject.FindWithTag("fish").GetComponent<Fish>();
            // masterStateController.fish.weight = _fisrtTorque * masterStateController.fishWeightPerTorque;
            // // masterStateController.fish.species = "Sardine";
            // // masterStateController.fish.HP = 1.0f;
            // // masterStateController.fish.difficultyOfEscape = 1.0f;
            // // masterStateController.fish.maxIntensityOfMovements = 1.0f;

            // 音声を再生
            masterStateController.FishSoundOnTheHook.Play();

            // 魚をはりに移動
            masterStateController.distanceFromRope = 0.0f;
            masterStateController.fish.transform.position = masterStateController.ropeRelayBelowHandle.transform.position + new Vector3(masterStateController.distanceFromRope, 0.0f, 0.0f);
        }

        public override void OnExit()
        {
            masterStateController.FishSoundOnTheHook.Stop();
        }

        public override int StateUpdate()
        {
            currentTimeCount += Time.deltaTime;

            // 魚のHPは単調減少
            masterStateController.fish.HP = masterStateController.fish.HP - masterStateController.changeRateOfHP * Time.deltaTime;

            // 魚の暴れる強さ
            // 0と1の間を周期的に変化する
            masterStateController.fish.currentIntensityOfMovements = masterStateController.fish.maxIntensityOfMovements * Mathf.Abs(Mathf.Sin(currentTimeCount / masterStateController.periodOfFishIntensity));
            // masterStateController.fish.currentIntensityOfMovements = 1.0f;
            // masterStateController.fish.currentIntensityOfMovements = masterStateController.fish.maxIntensityOfMovements * Mathf.Abs(((currentTimeCount / masterStateController.periodOfFishIntensity) - Mathf.Floor(currentTimeCount / masterStateController.periodOfFishIntensity)) * 2.0f - 1.0f);

            // ロープの音の大きさとピッチを変更
            masterStateController.FishSoundOnTheHook.volume = masterStateController.minRopeSoundVolume + (1.0f - masterStateController.minRopeSoundVolume) * masterStateController.fish.currentIntensityOfMovements;
            masterStateController.FishSoundOnTheHook.pitch = masterStateController.FishSoundOnTheHook.volume * 3.0f;

            // トルク送信
            // masterStateController.gameMaster.sendingTorque = _fisrtTorque + masterStateController.maxAplitudeOfTorque * masterStateController.fish.currentIntensityOfMovements * Mathf.Sin(currentTimeCount * 2.0f * Mathf.PI / masterStateController.periodOfTorque);
            // masterStateController.gameMaster.sendingTorque = _fisrtTorque + masterStateController.maxAplitudeOfTorque * masterStateController.fish.currentIntensityOfMovements * ((float)(Mathf.CeilToInt(currentTimeCount/ masterStateController.periodOfTorque) % 2) - 0.5f);
            masterStateController.gameMaster.sendingTorque = Mathf.Max(_fisrtTorque - (1.0f - masterStateController.fish.currentIntensityOfMovements) * masterStateController.torqueReduction, 0.75f);

            // 逃げにくさの更新
            if (Mathf.Abs(masterStateController.fish.currentIntensityOfMovements - masterStateController.trainingDevice.currentRelativePosition) > masterStateController.allowableDifference){
                    masterStateController.fish.difficultyOfEscape = masterStateController.fish.difficultyOfEscape - masterStateController.changeRateOfEscape * Time.deltaTime;
                } else {
                    masterStateController.fish.difficultyOfEscape = masterStateController.fish.difficultyOfEscape + masterStateController.changeRateOfEscape * Time.deltaTime;
            }

            // 魚が逃げる
            if (masterStateController.fish.difficultyOfEscape < 0.0f){
                return (int)MasterStateController.StateType.DuringFishing_Wait;
            }

            // 直前の位置の更新
            if ((_whenPreviousPosition - currentTimeCount) > masterStateController.timeOfRaising){
                _previousPosition = masterStateController.trainingDevice.currentRelativePosition;
            }

            //HPがゼロになったら次に移行
            if (masterStateController.fish.HP < 0.0f){
                return (int)MasterStateController.StateType.DuringFishing_HP0;
            }
            

            return (int)StateType;
        }

    }

}
