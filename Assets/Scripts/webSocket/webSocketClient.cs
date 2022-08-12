using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
            spdLimit = -0.1f;
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
            viwer.text = text;

            
            
        }

        // Update is called once per frame
        void Update()
        {
            viwer.text = text;
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

        private void Connect()
        {
            _socket.OnOpen += (sender,e) => 
            {
                Debug.Log("WebSocket Open");
                this.changeText("WebSocket Open");
                connected = true;
            };

            _socket.OnMessage += (s,e) => 
            {
                receivedData = JsonUtility.FromJson<ReceivingDataFormat>(e.Data);
                checkData(receivedData);
                changeText($"target: {receivedData.target}\ntorque: {receivedData.spd}\nposition: {receivedData.trq}\nspeed: {receivedData.spd}");
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
            _socket.Send(datajson);
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