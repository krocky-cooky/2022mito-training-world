using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using communication;

namespace tsunahiki.game {
    public enum DeviceType {
        TrainingDevice,
        ForceGauge,
    }

    public class RemoteCommunicator : MonoBehaviour {

        [SerializeField]
        private RemoteCommunicator _opponentCommunicator;
        [SerializeField]
        private DeviceType _deviceType;
        [SerializeField]
        private TrainingDevice _trainingDevice;
        [SerializeField]
        private ForceGauge _forceGauge;

        private float _currentPosition = 0.0f;

        [System.NonSerialized]
        public float opponentPosition;

        void Start()
        {

        }

        void Update()
        {
            if(_deviceType == DeviceType.TrainingDevice) 
            {
                _currentPosition = _trainingDevice.currentNormalizedPosition;
            }
            else if(_deviceType == DeviceType.ForceGauge) 
            {
                _currentPosition = _forceGauge.outputPosition;
            }
        }


        public void getOpponentPosition() 
        {
            opponentPosition = _opponentCommunicator.getCurrentPosition();
        }

        public float getCurrentPosition()
        {
            return _currentPosition;
        }
    }
}