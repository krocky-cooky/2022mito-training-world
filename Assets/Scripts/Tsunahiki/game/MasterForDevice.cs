using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using communication;
using tsunahiki.trainingDevice.stateController;

namespace tsunahiki.game
{


    public class MasterForDevice : MonoBehaviour
    {   
        const int MAX_LOG_LINES = 10;

        private System.Random _randomGenerator = new System.Random();
        private int _trainingDeviceVictoryCount = 0;
        private int _forceGaugeVictoryCount = 0;
        private int _drawyCount = 0;

        [SerializeField]
        private GameObject viewerObject;
        [SerializeField]
        private GameObject favorableWind;
        [SerializeField]
        private GameObject adverseWind;

        [HideInInspector]
        public Queue<string> viewerTextQueue = new Queue<string>();
        [HideInInspector]
        public TrainingDeviceType currentWinner = TrainingDeviceType.TrainingDevice;
        [HideInInspector]
        public TrainingDeviceType superiority; //各対戦ごとに付与される優勢劣勢
        [HideInInspector]
        public TrainingDeviceType fightCondition; //対戦中の勝敗状況; どちらが勝ちそうな状況か
        [HideInInspector]
        public MasterStateController masterStateController;
        

        // トルク÷握力計の値
        public float gripStrengthMultiplier = 4.0f;

        
    
        



        // Start is called before the first frame update
        void Start()
        {
            masterStateController.Initialize((int)MasterStateController.StateType.SetUp);
            resetWind();
        }

        // Update is called once per frame
        void Update()
        {
            masterStateController.UpdateSequence();
            writeLog();
        }

        public void decideSuperiority()
        {
            int randomOutput = _randomGenerator.Next(0,2);
            superiority = randomOutput == 1 ? 
                TrainingDeviceType.TrainingDevice : 
                TrainingDeviceType.ForceGauge;
            decideGripStrengthMultiplier();
        }

        //勝敗を記録ゲームセットの場合trueを返す
        public bool updateResult()
        {
            
            return false;
        }

        private void decideGripStrengthMultiplier()
        {

        }

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
            print(writeText);
            viewerObject.GetComponent<Text>().text = writeText;
        }

        //対戦相手が負けそうな状況におけるアバターの動き
        public void setLosingAvatarExpression()
        {

        }

        //対戦相手が勝ちそうな状況におけるアバターの動き
        public void setWinningAvatarExpression()
        {

        }

        //追い風
        public void setFavorableWind()
        {
            favorableWind.SetActive(true);
            adverseWind.SetActive(false);
        }

        //向かい風
        public void setAdverseWind()
        {
            adverseWind.SetActive(true);
            favorableWind.SetActive(false);
        }

        public void resetWind()
        {
            favorableWind.SetActive(false);
            adverseWind.SetActive(false);
        }



    }
}
