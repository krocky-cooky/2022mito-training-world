using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using game;
using util;
using trainingObjects;

namespace communication
{
    public class BluetoothCentral : MonoBehaviour
    {
        private ReceivingDataFormat receivedData;
        private Master gameMaster;
        private bool isChanged = false;
        private List<float> torqueList = new List<float>();
        private List<int> timestampList = new List<int>();

        public bool isConnected = false;
        public bool registerTorqueMode = false;


        [SerializeField]
        private Text viewer;
        [SerializeField]
        private GameObject weight;
        private SerialHandler serialHandler;


        void Start()
        {
            serialHandler = GetComponent<SerialHandler>();
            gameMaster = transform.parent.gameObject.GetComponent<Master>();
            Connect();
            
        }

        // Update is called once per frame
        void Update()
        {
            isConnected = serialHandler.IsOpen();
        }

        public void Connect()
        {
            if(!serialHandler.IsOpen())
            {
                serialHandler.Open();
            }

            serialHandler.OnDataReceived += OnDataReceived;
            

        }

        private void OnDataReceived(string message) 
        {
            receivedData = JsonUtility.FromJson<ReceivingDataFormat>(message);
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
        }

        public ReceivingDataFormat getReceivedData()
        {
            return receivedData;
        }


        public void sendData(SendingDataFormat data) 
        {
            string datajson = JsonUtility.ToJson(data);
            
            if(serialHandler.IsOpen())
            {
                try
                {
                    serialHandler.Write(datajson);
                }
                catch (Exception e)
                {
                    gameMaster.addLog("blueooth serial not opened");
                }
            }
            else 
            {
                gameMaster.addLog("blueooth serial not opened");
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