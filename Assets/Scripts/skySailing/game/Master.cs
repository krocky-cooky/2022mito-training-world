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

        [SerializeField]
        private GameObject viewerObject;
        [SerializeField]
        private GameObject gameObjectOfMyShip;
        [SerializeField]
        private float maxXRotationOfPillar;
        [SerializeField]
        private float minXRotationOfPillar;

        private webSocketClient _socketClient;
        private FittnessDevice _fittnessDevice;
        private SailingShip _mySailingShip;

        // Start is called before the first frame update
        void Start()
        {
            _socketClient = GameObject.FindWithTag("webSocketClient").GetComponent<webSocketClient>();

            // FitnessDeviceとMyShipの初期化
            _fittnessDevice = new FittnessDevice((float)Screen.height, 0.0f);
            _mySailingShip = new SailingShip(gameObjectOfMyShip, maxXRotationOfPillar, minXRotationOfPillar);
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

            // FittnessDeviceの入力の相対位置を得る
            _fittnessDevice.getCurrentRelativePosition(Input.mousePosition.y);
            addLog(_fittnessDevice.currentRelativePosition.ToString());

            // 帆の角度を変更する
            _mySailingShip.changePillarRotation(_fittnessDevice.currentRelativePosition);
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
        
    }
}
