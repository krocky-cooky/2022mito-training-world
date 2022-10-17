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


            if ((currentTimeCount - _previousSpikeTime) > _spikeInterval){
                _spikeEndTime = currentTimeCount + masterStateController.firstSpikePeriod + masterStateController.latterSpikePeriod;
                _previousSpikeTime = currentTimeCount;
                _spikeInterval = Random.Range(masterStateController.minIntervalOfNibbling, masterStateController.maxIntervalOfNibbling);
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

            
            if (currentTimeCount > _timeOfNibbling)
            {
                return (int)MasterStateController.StateType.DuringFishing_FishOnTheHook;
            }

            return (int)StateType;
        }

    }

}
