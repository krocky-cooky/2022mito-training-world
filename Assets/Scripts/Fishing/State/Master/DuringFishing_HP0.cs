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
            // _fisrtTorque = master.fish.weight / master.fishWeightPerTorque;
            // master.sendingTorque = Mathf.Max(_fisrtTorque - master.torqueReduction, 0.75f);
            master.sendingTorque = 1.0f;

            // 音声を再生
            master.FishSoundWithHP0.Play();

            // 魚をはりに移動
            master.distanceFromRope = 0.0f;
            master.fish.transform.position = master.ropeRelayBelowHandle.transform.position + new Vector3(master.distanceFromRope, 0.0f, 0.0f);

            // 初期化
            _previousPosition = master.trainingDevice.currentNormalizedPosition;
            _whenPreviousPosition = 0.0f;
            master.tensionSliderGameObject.SetActive(false);

            // ファイト回数を追加
            master.fightingCount += 1;
        }

        public override void OnExit()
        {
            master.FishSoundWithHP0.Stop();
        }

        public override int StateUpdate()
        {
            currentTimeCount += Time.deltaTime;

            // トルクを負荷ゲージで表示
            // トルクの値の約4.0倍が負荷(kg)
            master.tensionSlider.value = master.sendingTorque * 4.0f;

            // 直前の位置の更新
            if ((currentTimeCount - _whenPreviousPosition) > master.timeOfRaising){
                _previousPosition = master.trainingDevice.currentNormalizedPosition;
                _whenPreviousPosition = currentTimeCount;
            }

            //HPがゼロになって、かつ竿を振り上げたら、魚ゲット
            if (((master.trainingDevice.currentNormalizedPosition - _previousPosition) > master.lengthOfRasing) || Input.GetMouseButtonDown(1)){
                master.FishGoOnTheWater.Play();
                return (int)MasterStateController.StateType.AfterFishing;
            }

            return (int)StateType;
        }

    }

}
