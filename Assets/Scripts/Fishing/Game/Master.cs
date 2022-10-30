using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using Fishing.Game;
using Fishing.State;
using Fishing.StateController;
using Fishing.Object;

namespace Fishing.Game
{

    public class Master : MonoBehaviour
    {   
        const int MAX_LOG_LINES = 10;

        public Queue<string> viewerTextQueue = new Queue<string>();
        public Text UiText;
        public float sendingTorque = 0.0f;

        [SerializeField]
        private GameObject viewerObject;
        [SerializeField]
        private MainCommunicationInterface communicationInterface;
        [SerializeField]
        private MasterStateController masterStateController;
        [SerializeField]
        private float torqueSendingInterval = 0.1f;

        private float time = 0.0f;
        private float _previoussendingTorque = 0.0f;
        private float _previousTorqueSendingTime = 0.0f;
        private bool duringReelingWire = false;

        // Start is called before the first frame update
        void Start()
        {
            masterStateController.Initialize((int)MasterStateController.StateType.BeforeFishing);
        }

        // Update is called once per frame
        void Update()
        {

            time += Time.deltaTime;

            masterStateController.UpdateSequence();
            Debug.Log("Master State is "+masterStateController.stateDic[masterStateController.CurrentState].GetType());

            // //ワイヤ巻き取り用ボタンイベント
            // reelWire();

            // ログ提示
            writeLog();
            if(communicationInterface.isConnected)
            {
                addLog("web socket opened");
            }else{
                addLog("web socket not opened");
            }
            
            // // プレイ中のトルク指令
            // UpdateTorque(sendingTorque);

            // ワイヤ巻き取りまたはプレイ中のトルク指令
            // ワイヤ巻き取りの操作があればそれを優先し、なければプレイ中のトルク指令を行う
            if(OVRInput.Get(OVRInput.RawButton.LIndexTrigger))
            {
                UpdateTorque(0.75f);
            }else{
                UpdateTorque(sendingTorque);
            }
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

        // //ワイヤを巻き取る
        // private void reelWire(float torque, float speed = 4.0f)
        // {
        //     SendingDataFormat data = new SendingDataFormat();
        //     data.setTorque(torque, speed);
        //     communicationInterface.sendData(data);
        //     Debug.Log("send torque" + sendingTorque.ToString());
        // }
        // private void reelWire(){
        //     if(OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
        //     {
        //         UpdateTorque(0.75f);
        //     }
        //     if(OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger))
        //     {
        //         restore();
        //     }
        // }

        //トルクを更新
        private void UpdateTorque(float torque, float speed = 10.0f)
        {
            // 前回とトルクが同じ、または前回の送信時刻から一定時間経過していなければ、トルクを送信しない
            // if ((torque == _previoussendingTorque) || ((time - _previousTorqueSendingTime) < torqueSendingInterval)){
            //     return;
            // }
            if ((time - _previousTorqueSendingTime) < torqueSendingInterval){
                return;
            }
            SendingDataFormat data = new SendingDataFormat();
            data.setTorque(torque, speed);
            communicationInterface.sendData(data);
            Debug.Log("send torque" + torque.ToString());
            _previoussendingTorque = sendingTorque;
            _previousTorqueSendingTime = time;
        }


        //モーターの現状復帰用関数(速度0指令)
        private void restore()
        {
            SendingDataFormat data = new SendingDataFormat();
            data.setSpeed(0.0f);
            communicationInterface.sendData(data);
        }
        
    }
}
