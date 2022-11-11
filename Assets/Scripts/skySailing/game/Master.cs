using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using pseudogame.util;
using pseudogame.game;
using skySailing.game;
using communication;

namespace skySailing.game
{

    public class Master : MonoBehaviour
    {   
        const int MAX_LOG_LINES = 10;

        public Queue<string> viewerTextQueue = new Queue<string>();
        public float windSpeed;
        public Text frontViewUI;
        public int numberOfCheckpointsPassed = 0;

        // レース中かどうかのフラグ
        public bool duringRace = false;

        // リモート通信のインターバル
        public float timeIntervalForRemoteCommunication;

        [SerializeField]
        private GameObject viewerObject;
        [SerializeField]
        private SailingShip SailingShip;
        [SerializeField]
        private float torqueDuringRace = 0.0f;
        [SerializeField]
        private ForceGauge forceGauge;
        [SerializeField]
        private MainCommunicationInterface communicationInterface;
        [SerializeField]
        private float speedLimit;
        [SerializeField]
        private RemoteWebSocketClient remoteWebSocketClient;

        private float time = 0.0f;
        private Vector3  _initShipPosition;
        private Quaternion _initShipRotaion;
        public float _previousTorqueDuringRace = 0.0f;
        private string checkPointMessage;
        private string forceMessage;
        private string timerMessage;

        // リモート通信を一定間隔で実行
        private float _timeElapsed = 0.0f;

        private float _timeCount = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            _initShipPosition = SailingShip.transform.position;
            _initShipRotaion = SailingShip.transform.rotation;

        }

        // Update is called once per frame
        void Update()
        {
            _timeCount = Time.deltaTime;

            //ワイヤ巻き取り用ボタンイベント
            if(OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
            {
                reelWire(0.75f, 6.0f);
            }
            if(OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger))
            {
                restore();
            }

            writeLog();

            if(communicationInterface.isConnected)
            {
                addLog("web socket opened");
            }else{
                addLog("web socket not opened");
            }

            // 初期状態に戻る
            if ((OVRInput.GetDown(OVRInput.RawButton.X)) || (Input.GetMouseButtonDown(1))){
                returnToInit();
            }     

            // タイマーとチェックポイント通過数の表示
            if (duringRace){
                time += Time.deltaTime;
                Debug.Log("time is " + time.ToString());
            }

            // forceMessage = "\nleft force=" + forceGauge.currentForce.ToString() + "kg  right force=" + torqueDuringRace.ToString() + "kg";
            forceMessage = "Player1 Level: " + (torqueDuringRace * 3.0f).ToString() + "kg" + "\nPlayer2 Level: " + forceGauge.maxForce.ToString() + "kg";
            timerMessage = "Time: " + time.ToString();
            checkPointMessage = "Check Points:  " + numberOfCheckpointsPassed.ToString();
            frontViewUI.text = forceMessage + "\n" + timerMessage + "\n"  + checkPointMessage;
            
            // レース中のトルク指令
            if (torqueDuringRace != _previousTorqueDuringRace){
                reelWire(torqueDuringRace, speedLimit);
                _previousTorqueDuringRace = torqueDuringRace;
                // Debug.Log("send torque" + torqueDuringRace.ToString());
            }

            // remote Web Socket通信

            if (_timeElapsed > timeIntervalForRemoteCommunication){
                _timeElapsed = 0.0f;
                remoteWebSocketClient.sendData(_timeCount.ToString());
                Debug.Log("message via aws web socket api is " + remoteWebSocketClient.getReceivedData());
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

        //ワイヤを巻き取る
        private void reelWire(float torque, float speed)
        {
            SendingDataFormat data = new SendingDataFormat();
            data.setTorque(torque, speed);
            communicationInterface.sendData(data);
            Debug.Log("send torque" + torqueDuringRace.ToString());
        }


        //モーターの現状復帰用関数(速度0指令)
        private void restore()
        {
            SendingDataFormat data = new SendingDataFormat();
            data.setSpeed(0.0f);
            communicationInterface.sendData(data);
        }

        //初期状態に戻る
        private void returnToInit(){
            SailingShip.transform.position = _initShipPosition;
            SailingShip.transform.rotation = _initShipRotaion;
            time = 0.0f;
            duringRace = false;
            numberOfCheckpointsPassed = 0;
        }
        
    }
}
