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


            //記録開始前の3秒カウントダウン
            for(int i = 0; i < 3; ++i)
            {
                Debug.Log(i);
                yield return new WaitForSeconds(1.0f);
            } 

            //websocketクラスのトルク記録モードのアクティベート
            _socketClient.registerTorqueMode = true;

            for(int i = 0;i < seconds;i++)
            {
                Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!");

                yield return new WaitForSeconds(1.0f);
            }

            _socketClient.registerTorqueMode = false;
            _socketClient.saveRegisteredTorque();
            state = GameState.Idle;
            Debug.Log("coroutine end !!!");
        }

        public IEnumerator sendTorqueModeCoroutine()
        {
            
            SendingDataFormat data = new SendingDataFormat();
            List<float> torqueList = new List<float>();
            List<int> timestamp = new List<int>();

            SaveManager.getRegisteredTorque(ref torqueList,ref timestamp);
            for(int i = 0;i < torqueList.Count;i++)
            {
                float torque = torqueList[i];
                data.setTorque(torque);
                _socketClient.sendData(data);
                yield return new WaitForSeconds(0.1f);
                

            }
            Debug.Log("send data coroutine done");
            
            
        }

        
    }
}
