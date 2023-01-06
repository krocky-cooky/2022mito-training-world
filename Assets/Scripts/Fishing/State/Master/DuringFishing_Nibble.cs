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

    public class DuringFishing_Nibble : MasterStateBase
    {
        // タイムカウント
        float currentTimeCount;

        // 前回のスパイクの時間
        private float _previousSpikeTime = 0.0f;

        // スパイクの提示時刻
        private float _spikeEndTime = 0.0f;

        // スパイクの時間周期
        private float _spikeInterval;

        // 魚が針を突き始めてからしっかり引っかかるまでの時間
        private float _timeOfNibbling;

        // つつきのための時間カウント
        public float _timeCountForNibble = 100.0f;

        // 魚が突く音を視覚や力覚と同期させるバッファ
        private float _timeCountForNibbleSound = 100.0f;

        // 針を突く魚の振動の方向ベクトル
        // (魚の位置ベクトル) = (ルアーの位置ベクトル) + (針を突く魚の振動の方向ベクトル)
        private Vector3 _directionVectorOfNibble;

        // 針と魚の間の正規化距離
        private float _normalizedDistanceBetweenFishAndLure;

        public override void OnEnter()
        {
            Debug.Log("DuringFishing_Nibble");

            // 初期化
            currentTimeCount = 0.0f;
            _previousSpikeTime = 0.0f;
            _spikeEndTime = 0.0f;
            _spikeInterval = 0.0f;
            _timeOfNibbling = Random.Range(master.minTimeOfNibbling, master.maxTimeOfNibbling);
            _timeCountForNibble = 100.0f;
            _timeCountForNibbleSound = 100.0f;

            _directionVectorOfNibble = master.fish.transform.position - master.ropeRelayBelowHandle.transform.position;
        }

        public override void OnExit()
        {
            // Do Nothing.
        }

        public override int StateUpdate()
        {
            currentTimeCount += Time.deltaTime;
            _timeCountForNibble += Time.deltaTime;

            // トルクを負荷ゲージで表示
            // トルクの値の約4.0倍が負荷(kg)
            master.tensionSlider.value = master.sendingTorque * 4.0f;

            // 魚が突く感触をトルクで再現
            if ((currentTimeCount - _previousSpikeTime) > _spikeInterval){
                _spikeEndTime = currentTimeCount + master.firstSpikePeriod + master.latterSpikePeriod;
                _previousSpikeTime = currentTimeCount;
                _spikeInterval = Random.Range(master.minIntervalOfNibbling, master.maxIntervalOfNibbling);
                _timeCountForNibble = master.buffurTimeForNibble;

                // 音発生
                Invoke("PlayNibbleSound", master.buffurTimeForNibbleSound);

                // 振動発生
                Invoke("PlayNibbleVibration", master.buffurTimeForNibbleSound);
            }
            if (currentTimeCount < (_spikeEndTime - master.latterSpikePeriod)){
                master.sendingTorque = master.firstSpikeSize;
                Debug.Log("fisrt spike");
            }else if(currentTimeCount < _spikeEndTime){
                master.sendingTorque = master.latterSpikeSize;
                Debug.Log("latter spike");
                // master.NibbleSound.Play();
            }else{
                master.sendingTorque = master.baseTorqueDuringFishing;
            }


            // 魚が突く様子を視覚表現
            if ((_timeCountForNibble > 0.0f) && (_timeCountForNibble < master.firstPeriodOfFishNibble)){
                _normalizedDistanceBetweenFishAndLure = (1.0f - (_timeCountForNibble / master.firstPeriodOfFishNibble));
            }else if((_timeCountForNibble > 0.0f) && (_timeCountForNibble < (master.firstPeriodOfFishNibble + master.latterPeriodOfFishNibble))){
                _normalizedDistanceBetweenFishAndLure = ((_timeCountForNibble - master.firstPeriodOfFishNibble) / master.latterPeriodOfFishNibble);
            }
            if (_timeCountForNibble > (master.firstPeriodOfFishNibble + master.latterPeriodOfFishNibble)){
                _timeCountForNibble =100.0f;
            }
            master.fish.transform.position = master.ropeRelayBelowHandle.transform.position + _normalizedDistanceBetweenFishAndLure * _directionVectorOfNibble;


            // 針にかかる
            if (currentTimeCount > _timeOfNibbling)
            {
                return (int)MasterStateController.StateType.DuringFishing_FishOnTheHook;
            }

            // 釣りの前に戻る
            if (OVRInput.GetDown(OVRInput.RawButton.X) || Input.GetMouseButtonDown(1))
            {
                return (int)MasterStateController.StateType.BeforeFishing;
            }

            return (int)StateType;
        }

        public void PlayNibbleSound(){
            master.NibbleSound.Play();
        }

        public void PlayNibbleVibration(){
            OVRInput.SetControllerVibration(0.01f, 0.5f, OVRInput.Controller.RTouch);
            Invoke("StopNibbleVibration", 0.25f);
        }

        public void StopNibbleVibration(){
            OVRInput.SetControllerVibration(0.0f, 0.0f, OVRInput.Controller.RTouch);
        }

    }

}
