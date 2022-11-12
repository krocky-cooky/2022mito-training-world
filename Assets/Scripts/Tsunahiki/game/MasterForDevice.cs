using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using communication;

namespace tsunahiki.game
{


    public class MasterForDevice : MonoBehaviour
    {   
        const int MAX_LOG_LINES = 10;

        public Queue<string> viewerTextQueue = new Queue<string>();
        public Text frontViewUI;


        // トルク÷握力計の値
        public float gripStrengthMultiplier;

        [SerializeField]
        private GameObject viewerObject;
        [SerializeField]
        private float sendingTorque = 0.0f;
        [SerializeField]
        private MainCommunicationInterface communicationInterface;
        [SerializeField]
        private float speedLimit;
        [SerializeField]
        private float torqueSendingInterval = 0.1f;
        [SerializeField]
        private RemoteCoordinator _coordinator;
        [SerializeField]
        private GameObject _centerCube;
        [SerializeField]
        private GameObject _trainingDeviceObject;
        [SerializeField]
        private GameObject _opponentHandle;
    
        

        private float time = 0.0f;
        private float _previoussendingTorque = 0.0f;
        private float _previousTorqueSendingTime = 0.0f;
        private float _opponentValue;
        private TrainingDevice _trainingDevice;
        private Vector3 cubeStartPosition;
        private Vector3 opponentHandleStartPosition;

        // Start is called before the first frame update
        void Start()
        {
            _trainingDevice = _trainingDeviceObject.GetComponent<TrainingDevice>();
            cubeStartPosition = _centerCube.transform.position;
            opponentHandleStartPosition = _opponentHandle.transform.position;
        }

        // Update is called once per frame
        void Update()
        {

            _opponentValue = _coordinator.getCurrentValue();

            // 握力計の値をトルクに代入
            sendingTorque = _opponentValue * gripStrengthMultiplier;

            time += Time.deltaTime;

            {
                Vector3 cubePos = cubeStartPosition;
                float controllerPositionFromCenter = _trainingDevice.currentAbsPosition - (_trainingDevice.maxAbsPosition + _trainingDevice.minAbsPosition)/2;
                cubePos.z += controllerPositionFromCenter;
                _centerCube.transform.position = cubePos;
            }

            {
                float normalizedForceGaugePos = _coordinator.getOpponentValue();
                Debug.Log(normalizedForceGaugePos);
                Vector3 opponentHandlePos = opponentHandleStartPosition;
                opponentHandlePos.z -= (normalizedForceGaugePos - 0.5f)*2.0f;
                _opponentHandle.transform.position = opponentHandlePos;
            }


            // ワイヤ巻き取りまたはプレイ中のトルク指令
            // ワイヤ巻き取りの操作があればそれを優先し、なければプレイ中のトルク指令を行う
            if(OVRInput.Get(OVRInput.RawButton.LIndexTrigger))
            {
                UpdateTorque(0.75f);
            }
            else
            {
                UpdateTorque(sendingTorque);
            }

        }

        //VR空間上のログ情報に追加
        public void addLog(string message)
        {
            
            viewerTextQueue.Enqueue(message);

            if(viewerTextQueue.Count > MAX_LOG_LINES)
            {
                
                string hoge = viewerTextQueue.Dequeue();
            }
        }

        private void writeLog()
        {
            string[] arr = viewerTextQueue.ToArray();
            string writeText = "";
            for(int i = 0;i < arr.Length; ++i)
            {
                writeText += arr[i] + "\n";
            }
            viewerObject.GetComponent<Text>().text = writeText;
        }


        //モーターの現状復帰用関数(速度0指令)
        private void restore()
        {
            SendingDataFormat data = new SendingDataFormat();
            data.setSpeed(0.0f);
            communicationInterface.sendData(data);
        }

        //トルクを更新
        private void UpdateTorque(float torque, float speed = 10.0f)
        {
            // 前回とトルクが同じ、または前回の送信時刻から一定時間経過していなければ、トルクを送信しない
            // if ((torque == _previoussendingTorque) || ((time - _previousTorqueSendingTime) < torqueSendingInterval)){
            //     return;
            // }
            if ((time - _previousTorqueSendingTime) < torqueSendingInterval){
                return;
            }
            SendingDataFormat data = new SendingDataFormat();
            data.setTorque(torque, speed);
            communicationInterface.sendData(data);
            Debug.Log("send torque" + torque.ToString());
            _previoussendingTorque = sendingTorque;
            _previousTorqueSendingTime = time;
        }

    }
}
