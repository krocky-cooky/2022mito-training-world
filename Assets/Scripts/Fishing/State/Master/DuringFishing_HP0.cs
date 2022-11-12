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

    public class DuringFishing_HP0 : MasterStateBase
    {
        // タイムカウント
        float currentTimeCount;

        // 直前の位置
        public float _previousPosition;

        // 直前の位置登録の時刻
        public float _whenPreviousPosition;

        // 初期トルク
        // private float _fisrtTorque;

        public override void OnEnter()
        {
            Debug.Log("DuringFishing_HP0");
            currentTimeCount = 0f;

            // トルクの指定
            // _fisrtTorque = masterStateController.fish.weight / masterStateController.fishWeightPerTorque;
            // masterStateController.gameMaster.sendingTorque = Mathf.Max(_fisrtTorque - masterStateController.torqueReduction, 0.75f);
            masterStateController.gameMaster.sendingTorque = 1.0f;

            // 音声を再生
            masterStateController.FishSoundWithHP0.Play();

            // 魚をはりに移動
            masterStateController.distanceFromRope = 0.0f;
            masterStateController.fish.transform.position = masterStateController.ropeRelayBelowHandle.transform.position + new Vector3(masterStateController.distanceFromRope, 0.0f, 0.0f);

            // 初期化
            _previousPosition = masterStateController.trainingDevice.currentNormalizedPosition;
            _whenPreviousPosition = 0.0f;
            masterStateController.tensionSliderGameObject.SetActive(false);
        }

        public override void OnExit()
        {
            masterStateController.FishSoundWithHP0.Stop();
        }

        public override int StateUpdate()
        {
            currentTimeCount += Time.deltaTime;

            // トルクを負荷ゲージで表示
            // トルクの値の約4.0倍が負荷(kg)
            masterStateController.tensionSlider.value = masterStateController.gameMaster.sendingTorque * 4.0f;

            // 直前の位置の更新
            if ((currentTimeCount - _whenPreviousPosition) > masterStateController.timeOfRaising){
                _previousPosition = masterStateController.trainingDevice.currentNormalizedPosition;
                _whenPreviousPosition = currentTimeCount;
            }

            //HPがゼロになって、かつ竿を振り上げたら、魚ゲット
            if (((masterStateController.trainingDevice.currentNormalizedPosition - _previousPosition) > masterStateController.lengthOfRasing) || Input.GetMouseButtonDown(1)){
                masterStateController.FishGoOnTheWater.Play();
                return (int)MasterStateController.StateType.AfterFishing;
            }

            return (int)StateType;
        }

    }

}
