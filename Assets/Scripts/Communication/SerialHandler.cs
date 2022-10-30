using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;

namespace communication
{
    public class SerialHandler : MonoBehaviour
    {
        public bool isRunning = false;

        [SerializeField]
        private string portName = "COM1";
        [SerializeField]
        private int baudRate = 115200;

        private SerialPort _serialPort;
        private Thread _thread;
        private ReceivingDataFormat receivedData;
        private string receivedText;

        //ポート名
        //例
        //Linuxでは/dev/ttyUSB0
        //windowsではCOM1
        //Macでは/dev/tty.usbmodem1421など

        private bool _isNewMessageReceived = false;
        private string _message;

        void Awake()
        {
            Open();
        }

        void Update()
        {
            if (_isNewMessageReceived) {
                OnDataReceived(_message);
            }
            _isNewMessageReceived = false;
        }

        void OnDestroy()
        {
            Close();
        }

        public void Open()
        {
            if(isRunning) 
            {
                Debug.Log("serial already open");
                return;
            }
            _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            //または
            //_serialPort = new SerialPort(portName, baudRate);
            _serialPort.Open();

            isRunning = true;

            _thread = new Thread(Read);
            _thread.Start();
        }

        private void Close()
        {
            _isNewMessageReceived = false;
            isRunning = false;

            if (_thread != null && _thread.IsAlive) {
                _thread.Join();
            }

            if (_serialPort != null && _serialPort.IsOpen) {
                _serialPort.Close();
                _serialPort.Dispose();
            }
        }

        private void Read()
        {
            while (isRunning && _serialPort != null && _serialPort.IsOpen) {
                try 
                {
                    _message = _serialPort.ReadLine();
                    _isNewMessageReceived = true;
                    Debug.Log("serial message is " + _message);
                } 
                catch (System.Exception e) 
                {
                    Debug.LogWarning(e.Message);
                }
            }
        }

        public ForceGaugeDataFormat getReceivedDataOfForceGauge()
        {
            return JsonUtility.FromJson<ForceGaugeDataFormat>(receivedText);
        }

        public ReceivingDataFormat getReceivedData()
        {
            receivedData = JsonUtility.FromJson<ReceivingDataFormat>(receivedText);
            return receivedData;
        }

        public void sendData(string data)
        {
            if(isRunning)
            {
                Write(data);
            }
            else
            {
                Debug.Log("serial port is not opened");
            }
        }

        public void Write(string message)
        {
            message += '\n';
            try {
                Debug.Log(message);
                _serialPort.Write(message);
            } 
            catch (System.Exception e) 
            {
                Debug.LogWarning(e.Message);
            }
        }

        public bool IsOpen()
        {
            return _serialPort.IsOpen;
        }

        private void OnDataReceived(string message)
        {
            receivedText = _message;
        }
    }
}