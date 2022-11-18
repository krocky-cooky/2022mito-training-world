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
    public class RemoteWebSocketClient : MonoBehaviour
    {
        // Start is called before the first frame update
        private WebSocket _socket; //websocketオブジェクト
        private string receivedText;
        // private ReceivingRemoteDataFormat receivedData;
        private string receivedMessage; // AWSのWebSocketAPIを介して受信したメッセージを格納

        public bool isConnected = false;

        [SerializeField]
        private string webSocketURL;

        private enum SslProtocolsHack
        {
        Tls = 192,
        Tls11 = 768,
        Tls12 = 3072
        }
        

        void Awake()
        {
            _socket = new WebSocket(webSocketURL);

            // wssから始まるセキュア通信用プロトコルの場合は以下が必要
            var sslProtocolHack = (System.Security.Authentication.SslProtocols)SslProtocolsHack.Tls12;
            _socket.SslConfiguration.EnabledSslProtocols = sslProtocolHack;
        }
        
        void Start()
        {
            Connect();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void Connect()
        {
            if(isConnected)
            {
                Debug.Log("remote web socket is already connected");
                return;
            }
            _socket.OnOpen += (sender,e) => 
            {
                isConnected = true;
            };

            _socket.OnMessage += (s,e) => 
            {
                Debug.Log(e.Data);

                if(!e.Data.Contains("server error"))
                {
                    receivedText = e.Data;
                }
                // receivedMessage = JsonUtility.FromJson<ReceivingRemoteDataFormat>(receivedText).message;
            };
            
            _socket.OnClose += (sender, e) =>
            {
                isConnected = false;
            };

            _socket.Connect();
        }

        private void OnDestroy()
        {
            _socket.Close();
            _socket = null;
        }

        public string getReceivedData()
        {
            return receivedText;
        }

        //データの送信
        public void sendData(string data) 
        {
            if(isConnected)
            {
                try
                {
                    data = data.Replace("\"","\\\"");
                    data = "{\"action\": \"sendmessage\", \"message\":\""  + data + "\"}";
                    _socket.Send(data);
                    Debug.Log("send data via web socket api is" + data);
                }
                catch (Exception e)
                {
                    Debug.Log("Error");
                }
            }
            else
            {
            }
        }
        
    }
}