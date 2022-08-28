using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        const int MAX_REGISTER_TIME = 20;
        const float REEL_SPEED = 3.0f;
        const int ESP_DATA_RATE_MS = 100; 
        const int MAX_LOG_LINES = 10;

        
        public GameState state = GameState.Idle;
        public Queue<string> viewerTextQueue = new Queue<string>();
        public string username = "username";
        public string loadingFileName = "loadingFileName";

        [SerializeField]
        private int registerSeconds = 3;
        [SerializeField]
        private GameObject viewerObject;

        private webSocketClient _socketClient;
        private Battle _battle;
        private Text FrontViewUI;

        // Start is called before the first frame update
        void Start()
        {
            _socketClient = GameObject.FindWithTag("webSocketClient").GetComponent<webSocketClient>();
            _battle = GameObject.FindWithTag("controller").GetComponent<Battle>();
            FrontViewUI = GameObject.FindWithTag("FrontViewUI").GetComponent<Text>();

            // 正面のUIを初期化
            SetTextOnFrontViewUI("TRAVE");

            addLog("process started");

            
            
        }

        // Update is called once per frame
        void Update()
        {
            //ワイヤ巻き取り用ボタンイベント
            if(OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
            {
                reelWire();
            }
            if(OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger))
            {
                restore();
            }

            //websocketコネクション確立用ボタンイベント
            /*
            if(OVRInput.GetDown(OVRInput.RawButton.LThumbstick))
            {
                _socketClient.Connect();
            }
            */

            writeLog();
            
        }


        public void startRegisterMode()
        {
            state = GameState.Register;

            //ゲームロジックコルーチンの開始
            StartCoroutine(registerModeCoroutine(registerSeconds));
        }

        public void startSendTorqueMode()
        {
            state = GameState.SendTorque;

            //ゲームロジックコルーチンの開始
            StartCoroutine(sendTorqueModeCoroutine());
        }


        //VR空間上のログ情報に追加
        public void addLog(string message)
        {
            
            viewerTextQueue.Enqueue(message);

            if(viewerTextQueue.Count > MAX_LOG_LINES)
            {
                
                string hoge = viewerTextQueue.Dequeue();
            }
        }

        private void writeLog()
        {
            string[] arr = viewerTextQueue.ToArray();
            string writeText = "";
            for(int i = 0;i < arr.Length; ++i)
            {
                writeText += arr[i] + "\n";
            }
            viewerObject.GetComponent<Text>().text = writeText;
        }

        //ワイヤを巻き取る
        private void reelWire()
        {
            SendingDataFormat data = new SendingDataFormat();
            data.setSpeed(REEL_SPEED);
            _socketClient.sendData(data);
        }


        //モーターの現状復帰用関数(速度0指令)
        private void restore()
        {
            SendingDataFormat data = new SendingDataFormat();
            data.setSpeed(0.0f);
            _socketClient.sendData(data);
        }

        public IEnumerator registerModeCoroutine(int seconds)
        {
            if(!_socketClient.isConnected)
            {
                addLog("web socket not opened");
                state = GameState.Idle;
                yield break;
            }

            //速度指令の送信
            SendingDataFormat data = new SendingDataFormat();
            data.setSpeed(0.0f);
            _socketClient.sendData(data);
            addLog("register mode started");
            SetTextOnFrontViewUI("register mode started");


            //記録開始前の3秒カウントダウン
            for(int i = 0; i < 3; ++i)
            {
                addLog($"count down : {3-i}");
                SetTextOnFrontViewUI($"count down : {3-i}");
                yield return new WaitForSeconds(1.0f);
            } 

            int endTimestamp = -1;

            for(int i = 0;i < MAX_REGISTER_TIME;i++)
            {
                ReceivingDataFormat receivedData = _socketClient.getReceivedData();
                
                if(receivedData != null)
                {
                    if(receivedData.trq >= 0.5f && endTimestamp < 0)
                    {
                        _socketClient.registerTorqueMode = true;
                        endTimestamp = receivedData.timestamp + 1000*seconds;
                        addLog("data logging start !!");
                        SetTextOnFrontViewUI("data logging start !!");

                    }
                }
                if(receivedData.timestamp >= endTimestamp && endTimestamp > 0)break;
                addLog("1 sec passed");
                yield return new WaitForSeconds(1.0f);
            }

            _socketClient.registerTorqueMode = false;
            addLog("before torque register ");
            _socketClient.saveRegisteredTorque(username);
            addLog("after torque register ");

            state = GameState.Idle;

            restore();
            addLog("register mode end");
            SetTextOnFrontViewUI("register mode end");

        }

        public IEnumerator sendTorqueModeCoroutine()
        {

            if(!_socketClient.isConnected)
            {
                addLog("web socket not opened");
                state = GameState.Idle;
                yield break;
            }
            
            SendingDataFormat data = new SendingDataFormat();
            List<float> torqueList = new List<float>();
            List<int> timestamp = new List<int>();
            
            addLog("send torque mode start !!!");
            SetTextOnFrontViewUI("send torque mode start !!!");


            SaveManager.getRegisteredTorque(ref torqueList,ref timestamp, loadingFileName);
            timestamp.Add(timestamp[timestamp.Count-1] + ESP_DATA_RATE_MS);
            for(int i = 0;i < torqueList.Count;i++)
            {
                float torque = torqueList[i];
                int interval = timestamp[i+1] - timestamp[i];
                data.setTorque(torque);
                _socketClient.sendData(data);
                yield return new WaitForSeconds((float)interval/1000.0f);
            }
            
            restore();
            addLog("send torque mode end !!!");
            SetTextOnFrontViewUI("send torque mode end !!!");

            state = GameState.Idle;
            
        }

        
        // 正面のUIのテキスト表示を更新する
        void SetTextOnFrontViewUI(string textOnFrontViewUI){
            FrontViewUI.text = textOnFrontViewUI;
        }
        
        
    }
}
