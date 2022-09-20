using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using game;
using skySailing.game;

namespace skySailing.game
{

    public class Master : MonoBehaviour
    {   
        const int MAX_LOG_LINES = 10;

        public Queue<string> viewerTextQueue = new Queue<string>();
        public float windSpeed;
        public Text timerText;

        // レース中かどうかのフラグ
        public bool duringRace = false;

        [SerializeField]
        private GameObject viewerObject;
        [SerializeField]
        private SailingShip SailingShip;

        private webSocketClient _socketClient;
        private float time = 0.0f;
        private Vector3  _initShipPosition;
        private Quaternion _initShipRotaion;

        // Start is called before the first frame update
        void Start()
        {
            _socketClient = GameObject.FindWithTag("webSocketClient").GetComponent<webSocketClient>();
            _initShipPosition = SailingShip.transform.position;
            _initShipRotaion = SailingShip.transform.rotation;

        }

        // Update is called once per frame
        void Update()
        {
            //ワイヤ巻き取り用ボタンイベント
            if(OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
            {
                reelWire();
            }
            if(OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger))
            {
                restore();
            }

            writeLog();

            if (duringRace){
                time += Time.deltaTime;
                Debug.Log("time is " + time.ToString());
            }
            timerText.text = time.ToString();

            if(_socketClient.isConnected)
            {
                addLog("web socket opened");
            }else{
                addLog("web socket not opened");
            }

            if (Input.GetMouseButtonDown(1)){
                returnToInit();
                Debug.Log("右ボタンが押されました。");
            }
            if (OVRInput.GetDown(OVRInput.RawButton.X)){
                returnToInit();
                Debug.Log("xボタンが押されました。");
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
        private void reelWire()
        {
            SendingDataFormat data = new SendingDataFormat();
            data.setTorque(0.75f);
            _socketClient.sendData(data);
        }


        //モーターの現状復帰用関数(速度0指令)
        private void restore()
        {
            SendingDataFormat data = new SendingDataFormat();
            data.setSpeed(0.0f);
            _socketClient.sendData(data);
        }

        //初期状態に戻る
        private void returnToInit(){
            SailingShip.transform.position = _initShipPosition;
            SailingShip.transform.rotation = _initShipRotaion;
            time = 0.0f;
            duringRace = false;
        }
        
    }
}
