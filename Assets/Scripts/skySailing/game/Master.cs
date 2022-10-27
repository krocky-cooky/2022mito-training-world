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
        public Text timerText;
        public int numberOfCheckpointsPassed = 0;

        // レース中かどうかのフラグ
        public bool duringRace = false;

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

        private float time = 0.0f;
        private Vector3  _initShipPosition;
        private Quaternion _initShipRotaion;
        public float _previousTorqueDuringRace = 0.0f;
        private string checkPointMessage;
        private string forceMessage;

        // Start is called before the first frame update
        void Start()
        {
            _initShipPosition = SailingShip.transform.position;
            _initShipRotaion = SailingShip.transform.rotation;

        }

        // Update is called once per frame
        void Update()
        {
            //ワイヤ巻き取り用ボタンイベント
            if(OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
            {
                reelWire(0.75f);
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
            checkPointMessage = "\nYou passed " + numberOfCheckpointsPassed.ToString() + " check points";
            forceMessage = "\nleft force=" + forceGauge.currentForce.ToString() + "kg  right force=" + torqueDuringRace.ToString() + "kg";
            timerText.text = time.ToString() + checkPointMessage + forceMessage;
            
            // レース中のトルク指令
            if (torqueDuringRace != _previousTorqueDuringRace){
                float spdLimit = 1.0f;
                reelWire(torqueDuringRace, spdLimit);
                _previousTorqueDuringRace = torqueDuringRace;
                // Debug.Log("send torque" + torqueDuringRace.ToString());
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
        private void reelWire(float torque, float speed = 4.0f)
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
