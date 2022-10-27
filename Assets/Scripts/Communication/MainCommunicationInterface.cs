using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace communication 
{
    public enum CommunicationType 
    {
        webSocket,
        bluetooth,
        serial
    }
    
    public class MainCommunicationInterface : MonoBehaviour
    {
        private BluetoothCentral bluetoothCentral;
        private WebSocketClient webSocketClient;
        private SerialHandler serialHandler;
        
        public bool isConnected = false;

        [SerializeField]
        private CommunicationType communicationType;

        
        
        
        void Start()
        {
            bluetoothCentral = GetComponent<BluetoothCentral>();
            webSocketClient = GetComponent<WebSocketClient>();
            serialHandler = GetComponent<SerialHandler>();
            toggleMotor(true);
        }

        // Update is called once per frame
        void Update()
        {
            switch(communicationType)
            {
                case CommunicationType.webSocket:
                    isConnected = webSocketClient.isConnected;
                    break;
                case CommunicationType.bluetooth:
                    isConnected = bluetoothCentral.isConnected;
                    break;
                case CommunicationType.serial:
                    isConnected = serialHandler.IsOpen();
                    break;
            }

        }

        public void Connect()
        {
            switch(communicationType)
            {
                case CommunicationType.webSocket:
                    webSocketClient.Connect();
                    break;
                case CommunicationType.bluetooth:
                    bluetoothCentral.Connect();
                    break;
                case CommunicationType.serial:
                    serialHandler.Open();
                    break;
            }
        }

        public ReceivingDataFormat getReceivedData()
        {
            switch(communicationType)
            {
                case CommunicationType.webSocket:
                    return webSocketClient.getReceivedData();
                    break;
                case CommunicationType.bluetooth:
                    return bluetoothCentral.getReceivedData();
                    break;
                case CommunicationType.serial:
                    return serialHandler.getReceivedData();
                    break;
                default:
                    return null;
            }       
        }

        public void sendData(SendingDataFormat data)
        {
            string dataJson = JsonUtility.ToJson(data);
            switch(communicationType)
            {
                case CommunicationType.webSocket:
                    webSocketClient.sendData(dataJson);
                    break;
                case CommunicationType.bluetooth:
                    bluetoothCentral.sendData(dataJson);
                    break;
                case CommunicationType.serial:
                    serialHandler.sendData(dataJson);
                    break;
                
            }
            Debug.Log("send");
        }

        public void saveRegisteredTorque(string username)
        {
            switch(communicationType)
            {
                case CommunicationType.webSocket:
                    webSocketClient.saveRegisteredTorque(username);
                    break;
                case CommunicationType.bluetooth:
                    bluetoothCentral.saveRegisteredTorque(username);
                    break;
            }   
        }

        public void toggleRegisterTorqueMode(bool flag)
        {
            switch(communicationType)
            {
                case CommunicationType.webSocket:
                    webSocketClient.registerTorqueMode = flag;
                    break;
                case CommunicationType.bluetooth:
                    bluetoothCentral.registerTorqueMode = flag;
                    break;
            }   
        }

        public void toggleMotor(bool turnOn) 
        {
            int turnOnInt = Convert.ToInt32(turnOn);
            SwitchMotorFormat motor = new SwitchMotorFormat(turnOnInt);
            string dataJson = JsonUtility.ToJson(motor);
            switch(communicationType)
            {
                case CommunicationType.webSocket:
                    webSocketClient.sendData(dataJson);
                    break;
                case CommunicationType.bluetooth:
                    bluetoothCentral.sendData(dataJson);
                    break;
                case CommunicationType.serial:
                    serialHandler.sendData(dataJson);
                    break;
            }

        }
    }
}
