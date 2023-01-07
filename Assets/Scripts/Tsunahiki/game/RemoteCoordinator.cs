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
        [SerializeField]
        private MasterForDevice masterForDevice;
        [SerializeField]
        private Transform _HMD;

        [System.NonSerialized]
        public RemoteTsunahikiDataFormat communicationData;

        //互いに送りあうデータ(正規化したfloat値)
        //握力系 -> 筋トレデバイス　currentForce
        //筋トレデバイス -> 握力系　持ち手のポジション
        private float _currentValue = 0.0f;
        private float _opponentValue = 0.0f;
        private bool _coordinating = true;
        private float time = 0.0f;
        private float prev = 0.0f;

        private RemoteWebSocketClient _websocketClient;

        private float _timeCount;


        void Start()
        {
            _websocketClient = GetComponent<RemoteWebSocketClient>();
            communicationData = new RemoteTsunahikiDataFormat();
        }

        void Update()
        {
            if(_coordinating)
            {

                
                string text = "";
                if(_deviceType == TrainingDeviceType.TrainingDevice) 
                {
                    _currentValue = getValueFromTrainingDevice();
                    communicationData.normalizedData = _currentValue;
                    communicationData.positionX = _HMD.position.x;
                    communicationData.positionY = _HMD.position.y;
                    communicationData.positionZ = _HMD.position.z;
                    communicationData.rotationX = _HMD.eulerAngles.x;
                    communicationData.rotationY = _HMD.eulerAngles.y;
                    communicationData.rotationZ = _HMD.eulerAngles.z;
                    text = JsonUtility.ToJson(communicationData);
                }
                else if(_deviceType == TrainingDeviceType.ForceGauge) 
                {
                    _currentValue = getValueFromForceGauge();
                    communicationData.normalizedData = _currentValue;
                    communicationData.stateId = masterForForceGauge.masterStateController.CurrentState;
                    communicationData.positionX = _HMD.position.x;
                    communicationData.positionY = _HMD.position.y;
                    communicationData.positionZ = _HMD.position.z;
                    communicationData.rotationX = _HMD.eulerAngles.x;
                    communicationData.rotationY = _HMD.eulerAngles.y;
                    communicationData.rotationZ = _HMD.eulerAngles.z;
                    text = JsonUtility.ToJson(communicationData);
                }
                Debug.Log("hoge"+text);
                if(time - prev > 0.1f)
                {
                    _websocketClient.sendData(text);
                    prev = time;
                }
                time += Time.deltaTime;
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
            Debug.Log(retval);
            return retval;
        }

        public void startCoordinate()
        {
            _coordinating = true;
        }


        public float getOpponentValue() 
        {
            string receivedText = _websocketClient.getReceivedData();
            Debug.Log("receivedText is " + receivedText);
            RemoteTsunahikiDataFormat data = JsonUtility.FromJson<RemoteTsunahikiDataFormat>(receivedText);
            if (data is null) _opponentValue = 0.0f;
            else _opponentValue = data.normalizedData;
            //_opponentValue = _opponentCoordinator.getCurrentValue();
            return _opponentValue;
        }

        public RemoteTsunahikiDataFormat getOpponentData()
        {
            string receivedText = _websocketClient.getReceivedData();
            RemoteTsunahikiDataFormat data = JsonUtility.FromJson<RemoteTsunahikiDataFormat>(receivedText);
            Debug.Log("receivedText is " + receivedText);
            if(data is null) return new RemoteTsunahikiDataFormat();
            else return data;  
        }

        public float getCurrentValue()
        {
            return _currentValue;
        }
    }
}