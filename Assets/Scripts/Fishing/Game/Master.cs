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
        public TrainingDevice trainingDevice;

        // ハンドルの上げ下げのズレの許容量
        public float allowableDifference;

        // 魚の逃げにくさの値の変化速度
        public float changeRateOfEscape;

        // 魚のHPの変化速度
        public float changeRateOfHP;

        // 魚の暴れ具合の変動周期
        public float periodOfFishIntensity;

        // 魚を捕らえるときの竿の振り上げの時間と大きさ
        public float timeOfRaising;
        public float lengthOfRasing;

        [SerializeField]
        private GameObject viewerObject;
        [SerializeField]
        private float torqueDuringFishing = 0.0f;
        [SerializeField]
        private CommunicationInterface communicationInterface;
        [SerializeField]
        private MasterStateController masterStateController;
        [SerializeField]
        private float maxTorque = 0.0f;
        [SerializeField]
        private float minTorque = 0.0f;

        private float time = 0.0f;
        public float _previoustorqueDuringFishing = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            masterStateController.Initialize((int)MasterStateController.StateType.BeforeFishing);
        }

        // Update is called once per frame
        void Update()
        {
            masterStateController.UpdateSequence();
            Debug.Log("Master State is "+masterStateController.stateDic[masterStateController.CurrentState].GetType());

            //ワイヤ巻き取り用ボタンイベント
            if(OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
            {
                reelWire(0.75f);
            }
            if(OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger))
            {
                restore();
            }

            writeLog();

            if(communicationInterface.isConnected)
            {
                addLog("web socket opened");
            }else{
                addLog("web socket not opened");
            }
            
            // プレイ中のトルク指令
            if (torqueDuringFishing != _previoustorqueDuringFishing){
                float spdLimit = 1.0f;
                reelWire(torqueDuringFishing, spdLimit);
                _previoustorqueDuringFishing = torqueDuringFishing;
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

        //ワイヤを巻き取る
        private void reelWire(float torque, float speed = 4.0f)
        {
            SendingDataFormat data = new SendingDataFormat();
            data.setTorque(torque, speed);
            communicationInterface.sendData(data);
            Debug.Log("send torque" + torqueDuringFishing.ToString());
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
