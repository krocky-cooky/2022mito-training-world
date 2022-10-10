using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace communication 
{
    public class GripCommunicationInterface : MonoBehaviour 
    {
        private BluetoothCentral bluetoothCentral;

        public bool isConnected = false;

        void Start() 
        {
            bluetoothCentral = GetComponent<BluetoothCentral>();
        }

        void Update() 
        {
            isConnected = bluetoothCentral.isConnected;
        }

        public ReceivingGripDataFormat getReceivedData()
        {
            string text = bluetoothCentral.getReceivedText();
            return JsonUtility.FromJson<ReceivingGripDataFormat>(text);

        }
    }
}