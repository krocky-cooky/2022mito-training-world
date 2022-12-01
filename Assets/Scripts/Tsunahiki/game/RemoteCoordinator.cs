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
        private Transform _rightController;
        [SerializeField]
        private Transform _leftController;

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
                    RemoteTsunahikiDataFormat data = new RemoteTsunahikiDataFormat();
                    data.normalizedData = _currentValue;
                    data.positionX = _rightController.position.x;
                    data.positionY = _rightController.position.y;
                    data.positionZ = _rightController.position.z;
                    data.rotationX = _rightController.eulerAngles.x;
                    data.rotationY = _rightController.eulerAngles.y;
                    data.rotationZ = _rightController.eulerAngles.z;
                    text = JsonUtility.ToJson(data);
                }
                else if(_deviceType == TrainingDeviceType.ForceGauge) 
                {
                    _currentValue = getValueFromForceGauge();
                    RemoteTsunahikiDataFormat data = new RemoteTsunahikiDataFormat();
                    data.normalizedData = _currentValue;
                    data.stateId = masterForForceGauge.masterStateController.CurrentState;
                    data.positionX = _leftController.position.x;
                    data.positionY = _leftController.position.y;
                    data.positionZ = _leftController.position.z;
                    data.rotationX = _leftController.eulerAngles.x;
                    data.rotationY = _leftController.eulerAngles.y;
                    data.rotationZ = _leftController.eulerAngles.z;
                    text = JsonUtility.ToJson(data);
                }

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