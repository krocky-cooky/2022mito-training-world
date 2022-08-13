using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using util;


using WebSocketSharp;

namespace game
{

    public class ReceivingDataFormat
    {
        public string target;
        public float trq;
        public float spd;
        public float pos;
        public float integrationAngle;
        public int timestamp;
    }

    public class SendingDataFormat
    {
        public SendingDataFormat()
        {
            target = "trq";
            trq = -0.1f;
            spd = -0.1f;
            spdLimit = 2.0f;
        }

        public string target;
        public float trq;
        public float spd;
        public float spdLimit;

        public void setTorque(float torque)
        {
            target = "trq";
            trq = torque;
        }

        public void setSpeed(float speed)
        {
            target = "spd";
            spd = speed;
        }
    }

    public class webSocketClient : MonoBehaviour
    {
        // Start is called before the first frame update
        private WebSocket _socket; //websocketオブジェクト
        private ReceivingDataFormat receivedData; //websocketで受信したデータを格納
        private Master gameMaster; //ゲームマスターオブジェクト
        private string previousState = "neutral"; //直前のESP32の状態
        private string text = "hello";
        private bool changed = false; 
        private List<float> torqueList = new List<float>();
        private List<int> timestampList = new List<int>();

        public bool connected = false;
        public string message = "neutral";

        [SerializeField]
        private string ESP32PrivateIP = "192.168.128.192";
        [SerializeField]
        private Text viwer;
        [SerializeField]
        private GameObject weight;

        public bool registerTorqueMode = false; //トルク記録モードかどうか

        

        void Awake()
        {
            _socket = new WebSocket($"ws://{ESP32PrivateIP}/ws");
            gameMaster = transform.parent.gameObject.GetComponent<Master>();
        }
        
        void Start()
        {
            Connect();
            message = "neutral";

            
            
        }

        // Update is called once per frame
        void Update()
        {
            //viwer.text = text;
            if(changed)
            {
                changed = false;
                Debug.Log(this.weight);
                this.weight.GetComponent<shortTrainingBar>().changeBarStatusFlag(receivedData);
            }
            
            
        }

        private void changeText(string txt)
        {
            //Debug.Log("change start");
            text = txt;
            //Debug.Log("change end");

        }

        public void Connect()
        {
            if(connected)
            {
                gameMaster.addLog("already connected");
                Debug.Log("already connected");
                return;
            }
            _socket.OnOpen += (sender,e) => 
            {
                Debug.Log("WebSocket Open");
                gameMaster.addLog("web socket opened");

                connected = true;
            };

            _socket.OnMessage += (s,e) => 
            {
                receivedData = JsonUtility.FromJson<ReceivingDataFormat>(e.Data);
                checkData(receivedData);
                //changeText($"target: {receivedData.target}\ntorque: {receivedData.trq}\nspeed: {receivedData.spd}\ntimestamp: {receivedData.timestamp}\nmessage: {message}");
                changed = true;


                //トルク記録モードの場合トルクを保存していく
                if(registerTorqueMode)
                {
                    float torque = receivedData.trq;
                    int timestamp = receivedData.timestamp;
                    torqueList.Add(torque);
                    timestampList.Add(timestamp);
                }
            };
            
            _socket.OnClose += (sender, e) =>
            {
                Debug.Log("WebSocket Close");
                gameMaster.addLog("web socket closed");
                connected = false;
            };

            _socket.Connect();
        }

        private void OnDestroy()
        {
            _socket.Close();
            _socket = null;
        }

        public ReceivingDataFormat getReceivedData()
        {
            if(receivedData == null)return null;
            else return receivedData;
        }

        //データの送信
        public void sendData(SendingDataFormat data) 
        {
            string datajson = JsonUtility.ToJson(data);
            if(connected)
            {
                try
                {
                    _socket.Send(datajson);
                }
                catch (Exception e)
                {
                    gameMaster.addLog("web socket not opened");
                }
            }
            else
            {
                gameMaster.addLog("web socket not opened");
            }
        }

        private void checkData(ReceivingDataFormat data) 
        {
            return;
        }
        
        

        

        public void saveRegisteredTorque()
        {
            SaveManager.saveTorque(torqueList,timestampList);
            torqueList = new List<float>();
            timestampList = new List<int>();
        }

        
    }
}