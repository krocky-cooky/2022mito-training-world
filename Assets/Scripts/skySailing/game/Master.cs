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

        [SerializeField]
        private GameObject viewerObject;

        private webSocketClient _socketClient;

        // レース中かどうかのフラグ
        private bool duringRace = false;

        // Start is called before the first frame update
        void Start()
        {
            _socketClient = GameObject.FindWithTag("webSocketClient").GetComponent<webSocketClient>();
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

            Debug.Log("master update");
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
