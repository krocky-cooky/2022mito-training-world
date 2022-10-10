using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Text;

using game;
using util;
using trainingObjects;

namespace communication
{
    public class BluetoothCentral : MonoBehaviour
    {
        private string receivedText;
        private Master gameMaster;
        private bool isChanged = false;
        private List<float> torqueList = new List<float>();
        private List<int> timestampList = new List<int>();


        private bool isScanningDevices = false;
        private bool isScanningServices = false;
        private bool isScanningCharacteristics = false;
        private bool isSubscribed = false;
        private string deviceId;
        //private BleApi.ScanStatus status;
        public Dictionary<string, Dictionary<string, string>> devices = new Dictionary<string, Dictionary<string, string>>();
        public List<string> services = new List<string>();
        private Dictionary<string, string> characteristicName = new Dictionary<string, string>();
        private string lastError;

        public bool isConnected = false;
        public bool registerTorqueMode = false;

        

        [SerializeField]
        private string deviceName = "ESP32-Machine";
        [SerializeField]
        private string serviceId = "{91bad492-b950-4226-aa2b-4ede9fa42f59}";
        [SerializeField]
        private string characteristicId = "{f78ebbff-c8b7-4107-93de-889a6a06d408}";
        [SerializeField]
        private bool StartScan = false;





        



        void Start()
        {

            gameMaster = transform.parent.gameObject.GetComponent<Master>();
            //Connect();
            
            
            
        }

        // Update is called once per frame
        void Update()
        {
            
            
            if(StartScan)
            {
                Connect();
                StartScan = false;
            }
            ScanDevice();
            ScanServices();
            ScanCharacteristics();
            Subscribe();
            
            {
                // log potential errors
                BleApi.ErrorMessage res = new BleApi.ErrorMessage();
                BleApi.GetError(out res);
                if (lastError != res.msg)
                {
                    Debug.LogError(res.msg);
                    lastError = res.msg;
                }
            }
        }

        private void OnApplicationQuit()
        {
            BleApi.Quit();
        }

        private void OnDestroy()
        {
            BleApi.Quit();
        }

        private void ScanDevice()
        {
            BleApi.ScanStatus status;
            if(isScanningDevices)
            {
                BleApi.DeviceUpdate res = new BleApi.DeviceUpdate();

                do 
                {
                    status = BleApi.PollDevice(ref res, false);

                    if(status == BleApi.ScanStatus.AVAILABLE)
                    {
                        
                        if(!devices.ContainsKey(res.id))
                            devices[res.id] = new Dictionary<string, string>() {{"name", ""}, {"isConnectable", "False"}};
                        if(res.nameUpdated)
                            devices[res.id]["name"] = res.name;
                        if(res.isConnectableUpdated)
                            devices[res.id]["isConnectable"] = res.isConnectable.ToString();
                        
                        Debug.Log(devices[res.id]["isConnectable"]);
                        foreach(KeyValuePair<string,Dictionary<string,string>> kvp in devices)
                        {
                            if(kvp.Value["name"] == deviceName && kvp.Value["isConnectable"] == "True")
                            {
                                deviceId = kvp.Key;
                                Debug.Log("Device found");
                                BleApi.StopDeviceScan();
                                isScanningDevices = false;
                                StartServiceScan();
                                return;
                            }
                            
                        }

                    }
                    else if(status == BleApi.ScanStatus.FINISHED)
                    {
                        isScanningDevices = false;
                    }

                } while(status == BleApi.ScanStatus.AVAILABLE);

                


            }
        }

        private void ScanServices()
        {
            BleApi.ScanStatus status;

            
            if(isScanningServices)
            {
                BleApi.Service res = new BleApi.Service();
                do
                {
                    status = BleApi.PollService(out res, false);
                    Debug.Log(status);
                    if(status == BleApi.ScanStatus.AVAILABLE)
                    {
                        services.Add(res.uuid);
                        if(res.uuid == serviceId)
                        {
                            Debug.Log("service found!!");
                            StartCharacteristicScan();
                        }
                    }
                    else if(status == BleApi.ScanStatus.FINISHED)
                    {
                        isScanningServices = false;
                    }
                }while(status == BleApi.ScanStatus.AVAILABLE);
            }
        }

        private void ScanCharacteristics()
        {
            BleApi.ScanStatus status;
            if(isScanningCharacteristics)
            {
                BleApi.Characteristic res = new BleApi.Characteristic();
                do
                {
                    status = BleApi.PollCharacteristic(out res, false);
                    
                    if(status == BleApi.ScanStatus.AVAILABLE)
                    {
                        string name = res.userDescription != "no description available" ? res.userDescription : res.uuid;
                        characteristicName[name] = res.uuid;
                        Debug.Log(res.uuid);
                        if(res.uuid == characteristicId){
                            Debug.Log("characterstic found!!");
                            StartSubscribe();
                        }
                        
                    }
                    else if(status == BleApi.ScanStatus.FINISHED)
                    {
                        isScanningCharacteristics = false;
                    }
                   
                }while(status == BleApi.ScanStatus.AVAILABLE);
            }
        }

        private void Subscribe()
        {
            if(isSubscribed)
            {
                BleApi.BLEData res = new BleApi.BLEData();
                string receivedText = "{}";
                while(BleApi.PollData(out res, false))
                {
                    receivedText = Encoding.ASCII.GetString(res.buf,0,res.size);
                }
                if(receivedText != "{}")
                {
                    Debug.Log(receivedText);
                }
            }
        }

        public void StartSubscribe()
        {
            BleApi.SubscribeCharacteristic(deviceId,serviceId,characteristicId,false);
            isSubscribed = true;
        }

        public void StartDeviceScan()
        {
            if(!isScanningDevices)
            {
                BleApi.StartDeviceScan();
                isScanningDevices = true;
            }
        }

        public void StopDeviceScan()
        {

            BleApi.StopDeviceScan();
            isScanningDevices = false;

        }

        public void StartServiceScan()
        {
            if(!isScanningServices)
            {
                BleApi.ScanServices(deviceId);
                isScanningServices = true;
                Debug.Log(deviceId);
                
            }

        }

        public void StartCharacteristicScan()
        {
            if (!isScanningCharacteristics)
            {
                Debug.Log("characteristics scan start");

                BleApi.ScanCharacteristics(deviceId, serviceId);
                isScanningCharacteristics = true;

            }
        }   


        public void Connect()
        {
            StartDeviceScan();

        }

        

        public ReceivingDataFormat getReceivedData()
        {
            return JsonUtility.FromJson<ReceivingDataFormat>(receivedText);
        }

        public string getReceivedText()
        {
            return receivedText;
        }


        public void sendData(SendingDataFormat sendingData) 
        {
            if(isSubscribed)
            {
                string dataJson = JsonUtility.ToJson(sendingData);
                byte[] payload = Encoding.ASCII.GetBytes(dataJson);
                BleApi.BLEData data = new BleApi.BLEData();
                data.buf = new byte[512];
                data.size = (short)payload.Length;
                data.deviceId = deviceId;
                data.serviceUuid = serviceId;
                data.characteristicUuid = characteristicId;
                for (int i = 0; i < payload.Length; i++)
                    data.buf[i] = payload[i];
                // no error code available in non-blocking mode
                BleApi.SendData(in data, false);
            }
            else
            {
                Debug.Log("web socket not opened");
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

        public void Write()
        {
            
        }
    }
}