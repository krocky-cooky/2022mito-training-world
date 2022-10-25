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
        
        public bool isConnected = false;

        [SerializeField]
        private CommunicationType communicationType;

        
        
        
        void Start()
        {
            bluetoothCentral = GetComponent<BluetoothCentral>();
            webSocketClient = GetComponent<WebSocketClient>();
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
                default:
                    return null;
            }       
        }

        public void sendData(SendingDataFormat data)
        {
            switch(communicationType)
            {
                case CommunicationType.webSocket:
                    webSocketClient.sendData(data);
                    break;
                case CommunicationType.bluetooth:
                    bluetoothCentral.sendData(data);
                    break;
                
            }     
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
    }
}
