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

        public override void OnEnter()
        {
            Debug.Log("DuringFishing_Nibble");
            currentTimeCount = 0f;

            _spikeInterval = 0.0f;
            _timeOfNibbling = Random.Range(masterStateController.minTimeOfNibbling, masterStateController.maxTimeOfNibbling);
        }

        public override void OnExit()
        {
            // Do Nothing.
        }

        public override int StateUpdate()
        {
            currentTimeCount += Time.deltaTime;
            _timeCountForNibble += Time.deltaTime;

            // 魚が突く感触をトルクで再現
            if ((currentTimeCount - _previousSpikeTime) > _spikeInterval){
                _spikeEndTime = currentTimeCount + masterStateController.firstSpikePeriod + masterStateController.latterSpikePeriod;
                _previousSpikeTime = currentTimeCount;
                _spikeInterval = Random.Range(masterStateController.minIntervalOfNibbling, masterStateController.maxIntervalOfNibbling);
                _timeCountForNibble = masterStateController.buffurTimeForNibble;
            }
            if (currentTimeCount < (_spikeEndTime - masterStateController.latterSpikePeriod)){
                masterStateController.gameMaster.sendingTorque = masterStateController.firstSpikeSize;
                Debug.Log("fisrt spike");
            }else if(currentTimeCount < _spikeEndTime){
                masterStateController.gameMaster.sendingTorque = masterStateController.latterSpikeSize;
                Debug.Log("latter spike");
                masterStateController.NibbleSound.Play();
            }else{
                masterStateController.gameMaster.sendingTorque = masterStateController.baseTorqueDuringFishing;
            }

            // 魚が突く様子を視覚表現
            // masterStateController.distanceFromRope = masterStateController.SizeOfFishNibble * Mathf.Abs(Mathf.Cos(currentTimeCount * Mathf.PI / (masterStateController.firstPeriodOfFishNibble + masterStateController.latterPeriodOfFishNibble)));
            // masterStateController.fish.transform.position = masterStateController.ropeRelayBelowHandle.transform.position + new Vector3(masterStateController.distanceFromRope, 0.0f, 0.0f);

            if ((_timeCountForNibble > 0.0f) && (_timeCountForNibble < masterStateController.firstPeriodOfFishNibble)){
                masterStateController.distanceFromRope = masterStateController.SizeOfFishNibble * (1.0f - (_timeCountForNibble / masterStateController.firstPeriodOfFishNibble));
            }else if((_timeCountForNibble > 0.0f) && (_timeCountForNibble < (masterStateController.firstPeriodOfFishNibble + masterStateController.latterPeriodOfFishNibble))){
                masterStateController.distanceFromRope = masterStateController.SizeOfFishNibble * ((_timeCountForNibble - masterStateController.firstPeriodOfFishNibble) / masterStateController.latterPeriodOfFishNibble);
            }
            if (_timeCountForNibble > (masterStateController.firstPeriodOfFishNibble + masterStateController.latterPeriodOfFishNibble)){
                _timeCountForNibble =100.0f;
            }
            masterStateController.fish.transform.position = masterStateController.ropeRelayBelowHandle.transform.position + new Vector3(masterStateController.distanceFromRope, 0.0f, 0.0f);




            // 針にかかる
            if (currentTimeCount > _timeOfNibbling)
            {
                return (int)MasterStateController.StateType.DuringFishing_FishOnTheHook;
            }

            return (int)StateType;
        }

    }

}
