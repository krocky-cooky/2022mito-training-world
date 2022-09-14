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
    public class BleClient : MonoBehaviour
    {
        private const string MACHINE_DEVICE_NAME = "Machine-ESP32";
        private const string MACHINE_SERVICE_UUID = "91bad492-b950-4226-aa2b-4ede9fa42f59";
        private const string MACHINE_LOG_CHARACTERISTIC_UUID = "f78ebbff-c8b7-4107-93de-889a6a06d408";
        private const string MACHINE_COMMAND_CHARACTERISTIC_UUID = "f78ebbff-c8b7-4107-93de-889a6a06d409";

        // Start is called before the first frame update
        void Start()
        {
            BleApi.StartDeviceScan();
            BleApi.ScanStatus status;
            // 以下、BleWinrtDll Unity の Demo.cs を参考に書く
        }
        
        // Update is called once per frame
        void Update()
        {

        }

        private void OnDestroy()
        {

        }

        public void Connect()
        {

        }

        public void Disconnect()
        {

        }

        public ReceivingDataFormat getReceivedData()
        {

        }

        public void sendData(SendingDataFormat data)
        {

        }

        public void saveRegisteredTorque(string username)
        {

        }
    }
}
