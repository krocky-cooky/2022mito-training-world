using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using communication;

namespace tsunahiki.game 
{
    

    public class RemoteCoordinator : MonoBehaviour
    {

        [SerializeField]
        private RemoteCoordinator _opponentCoordinator;
        [SerializeField]
        private TrainingDeviceType _deviceType;
        [SerializeField]
        private TrainingDevice _trainingDevice;
        [SerializeField]
        private ForceGauge _forceGauge;
        [SerializeField]
        private float _sendPeriod;
        [SerializeField]
        private MasterForForceGauge masterForForceGauge;

        [System.NonSerialized]
        public RemoteTsunahikiDataFormat communicationData;

        //互いに送りあうデータ(正規化したfloat値)
        //握力系 -> 筋トレデバイス　currentForce
        //筋トレデバイス -> 握力系　持ち手のポジション
        private float _currentValue = 0.0f;
        private float _opponentValue = 0.0f;
        private bool _coordinating = true;

        private RemoteWebSocketClient _websocketClient;

        private float _timeCount;


        void Start()
        {
            _websocketClient = GetComponent<RemoteWebSocketClient>();
            communicationData = new RemoteTsunahikiDataFormat();
        }

        void Update()
        {
            _timeCount += Time.deltaTime;
            if (_timeCount > _sendPeriod){
                _timeCount = 0.0f;
                if(_coordinating)
                {
                    if(_deviceType == TrainingDeviceType.TrainingDevice) 
                    {
                        _currentValue = getValueFromTrainingDevice();
                        communicationData.normalizedData = _currentValue;
                    }
                    else if(_deviceType == TrainingDeviceType.ForceGauge) 
                    {
                        _currentValue = getValueFromForceGauge();
                        communicationData.normalizedData = _currentValue;
                        communicationData.deviceInterface = (int)_deviceType;
                        communicationData.stateId = (int)masterForForceGauge.masterStateController.CurrentState;
                        
                    }

                    string text = JsonUtility.ToJson(communicationData);
                    _websocketClient.sendData(text);
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
            string receivedText = _websocketClient.getReceivedData();
            RemoteTsunahikiDataFormat data = JsonUtility.FromJson<RemoteTsunahikiDataFormat>(receivedText);
            _opponentValue = data.normalizedData;
            //_opponentValue = _opponentCoordinator.getCurrentValue();
            return _opponentValue;
        }

        public RemoteTsunahikiDataFormat getOpponentData()
        {
            string receivedText = _websocketClient.getReceivedData();
            RemoteTsunahikiDataFormat data = JsonUtility.FromJson<RemoteTsunahikiDataFormat>(receivedText);
            Debug.Log("receivedText is " + receivedText);
            return data;  
        }

        public float getCurrentValue()
        {
            return _currentValue;
        }
    }
}