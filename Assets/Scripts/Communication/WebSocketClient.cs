using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using pseudogame.trainingObjects;
using pseudogame.util;
using pseudogame.game;


using WebSocketSharp;


namespace communication
{
    public class WebSocketClient : MonoBehaviour
    {
        // Start is called before the first frame update
        private WebSocket _socket; //websocketオブジェクト
        private ReceivingDataFormat receivedData; //websocketで受信したデータを格納
        // private Fishing.Game.Master gameMaster; //ゲームマスターオブジェクト
        private bool isChanged = false; 
        private List<float> torqueList = new List<float>();
        private List<int> timestampList = new List<int>();

        public bool isConnected = false;


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
            // gameMaster = transform.parent.gameObject.GetComponent<Fishing.Game.Master>();
        }
        
        void Start()
        {
            Connect();
        }

        // Update is called once per frame
        void Update()
        {
            if(isChanged)
            {
                isChanged = false;
                this.weight.GetComponent<shortTrainingBar>().changeBarStatusFlag(receivedData);
            }
            
            
        }

        private void changeText(string txt)
        {
        }

        public void Connect()
        {
            if(isConnected)
            {
                // gameMaster.addLog("already connected");
                return;
            }
            _socket.OnOpen += (sender,e) => 
            {
                // gameMaster.addLog("web socket opened");
                isConnected = true;
            };

            _socket.OnMessage += (s,e) => 
            {
                receivedData = JsonUtility.FromJson<ReceivingDataFormat>(e.Data);
                checkData(receivedData);
                isChanged = true;


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
                // gameMaster.addLog("web socket closed");
                isConnected = false;
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
            return receivedData;
        }

        //データの送信
        public void sendData(string data) 
        {
            if(isConnected)
            {
                try
                {
                    _socket.Send(data);
                    Debug.Log(data);
                }
                catch (Exception e)
                {
                    // gameMaster.addLog("web socket not opened");
                }
            }
            else
            {
                // gameMaster.addLog("web socket not opened");
            }
        }

        private void checkData(ReceivingDataFormat data) 
        {
            return;
        }
        
        

        

        public void saveRegisteredTorque(string username)
        {
            SaveManager.saveTorque(torqueList, timestampList, username);
            torqueList = new List<float>();
            timestampList = new List<int>();
        }

        
    }
}