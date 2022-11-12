using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using communication;

namespace tsunahiki.game 
{
    public enum DeviceType 
    {
        TrainingDevice,
        ForceGauge,
    }

    public class RemoteCoordinator : MonoBehaviour
    {

        [SerializeField]
        private RemoteCoordinator _opponentCoordinator;
        [SerializeField]
        private DeviceType _deviceType;
        [SerializeField]
        private TrainingDevice _trainingDevice;
        [SerializeField]
        private ForceGauge _forceGauge;

        //互いに送りあうデータ(正規化したfloat値)
        //握力系 -> 筋トレデバイス　currentForce
        //筋トレデバイス -> 握力系　持ち手のポジション
        private float _currentValue = 0.0f;
        private bool _coordinating = true;


        [System.NonSerialized]
        public float opponentValue;

        void Start()
        {
        }

        void Update()
        {
            if(_coordinating)
            {
                if(_deviceType == DeviceType.TrainingDevice) 
                {
                    _currentValue = getValueFromTrainingDevice();
                }
                else if(_deviceType == DeviceType.ForceGauge) 
                {
                    _currentValue = getValueFromForceGauge();
                }
            }
        }

        private float getValueFromForceGauge()
        {
            float retval = _forceGauge.outputPosition;
            return retval;
        }

        private float getValueFromTrainingDevice()
        {
            float retval = _trainingDevice.currentNormalizedPosition;
            return retval;
        }

        public void startCoordinate()
        {
            _coordinating = true;
        }


        public float getOpponentValue() 
        {
            opponentValue = _opponentCoordinator.getCurrentValue();
            return opponentValue;
        }

        public float getCurrentValue()
        {
            return _currentValue;
        }
    }
}