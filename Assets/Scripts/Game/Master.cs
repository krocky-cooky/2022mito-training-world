using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;


namespace game
{

    public enum GameState
    {
        Idle,
        Register,
        SendTorque,
    }

    public class Master : MonoBehaviour
    {
        public GameState state = GameState.Idle;


        [SerializeField]
        private int registerSeconds = 3;
        private webSocketClient _socketClient;
        private Battle _battle;
        const int MAX_REGISTER_TIME = 20;
        const int ESP_DATA_RATE_MS = 100; 
        


        // Start is called before the first frame update
        void Start()
        {
            _socketClient = GameObject.FindWithTag("webSocketClient").GetComponent<webSocketClient>();
            _battle = GameObject.FindWithTag("controller").GetComponent<Battle>();
            
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }


        public void startRegisterMode()
        {
            Debug.Log("registermode start");
            state = GameState.Register;

            //ゲームロジックコルーチンの開始
            StartCoroutine(registerModeCoroutine(registerSeconds));
        }

        public void startSendTorqueMode()
        {
            Debug.Log("send torque mode start");
            state = GameState.SendTorque;

            //ゲームロジックコルーチンの開始
            StartCoroutine(sendTorqueModeCoroutine());
        }

        public IEnumerator registerModeCoroutine(int seconds)
        {
            //速度指令の送信
            SendingDataFormat data = new SendingDataFormat();
            data.setSpeed(0.0f);
            _socketClient.sendData(data);
            Debug.Log("coroutine start");
            _socketClient.message = "coroutine start";


            //記録開始前の3秒カウントダウン
            for(int i = 0; i < 3; ++i)
            {
                Debug.Log(i);
                yield return new WaitForSeconds(1.0f);
            } 

            int endTimestamp = -1;

            for(int i = 0;i < MAX_REGISTER_TIME;i++)
            {
                Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!");
                ReceivingDataFormat receivedData = _socketClient.getReceivedData();
                
                if(receivedData != null)
                {
                    if(receivedData.trq >= 0.5f && endTimestamp < 0)
                    {
                        _socketClient.registerTorqueMode = true;
                        endTimestamp = receivedData.timestamp + 1000*seconds;
                        Debug.Log($"data logging start!! until : {endTimestamp}");
                        _socketClient.message = $"data logging start!! until : {endTimestamp}";



                    }
                }
                if(receivedData.timestamp >= endTimestamp && endTimestamp > 0)break;
                yield return new WaitForSeconds(1.0f);
            }

            _socketClient.registerTorqueMode = false;
            _socketClient.saveRegisteredTorque();
            state = GameState.Idle;
            Debug.Log("coroutine end !!!");
            _socketClient.message = "coroutine end !!!";

        }

        public IEnumerator sendTorqueModeCoroutine()
        {
            
            SendingDataFormat data = new SendingDataFormat();
            List<float> torqueList = new List<float>();
            List<int> timestamp = new List<int>();
            Debug.Log("Coroutine start");
            _socketClient.message = "coroutine start !!!";


            SaveManager.getRegisteredTorque(ref torqueList,ref timestamp);
            timestamp.Add(timestamp[timestamp.Count-1] + ESP_DATA_RATE_MS);
            for(int i = 0;i < torqueList.Count;i++)
            {
                float torque = torqueList[i];
                int interval = timestamp[i+1] - timestamp[i];
                data.setTorque(torque);
                _socketClient.sendData(data);
                yield return new WaitForSeconds((float)interval/1000.0f);
                

            }
            
            data.setSpeed(0.0f);
            _socketClient.sendData(data);
            Debug.Log("send data coroutine done");
            _socketClient.message = "send data coroutine done !!!";
            state = GameState.Idle;

            
            
        }

        
    }
}
