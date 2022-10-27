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
            if (bluetoothCentral.getReceivedText() != ""){
                isConnected = true;
            }
        }

        public ReceivingGripDataFormat getReceivedData()
        {
            string text = bluetoothCentral.getReceivedText();
            Debug.Log("text in grip comunication is " + text);
            return JsonUtility.FromJson<ReceivingGripDataFormat>(text);
        }
    }
}