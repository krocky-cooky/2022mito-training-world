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
        private const int TOTAL_WIN_COUNT = 3;
        private System.Random _randomGenerator = new System.Random();
        private int _trainingDeviceVictoryCount = 0;
        private int _forceGaugeVictoryCount = 0;
        private int _drawyCount = 0;
        private Vector3 _initialTurnipVelocity = new Vector3(0,20,2);
        private Quaternion _initialTurnipRotation;
        private Vector3 _initialTurnipPosition;

        [SerializeField]
        private GameObject viewerObject;
        [SerializeField]
        private GameObject favorableWind;
        [SerializeField]
        private GameObject adverseWind;
        [SerializeField]
        private float multiplierOffset = 1.5f;
        [SerializeField]
        private GameObject turnip;
        [SerializeField]
        private float _debugNormalizedValue = 0.5f;
        
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
        public float defaultGripStrengthMultiplier = 4.0f;
        public float gripStrengthMultiplier = 4.0f;

        
    
        



        // Start is called before the first frame update
        void Start()
        {
            masterStateController.Initialize((int)MasterStateController.StateType.SetUp);
            _initialTurnipRotation = turnip.transform.rotation;
            _initialTurnipPosition = turnip.transform.position;
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
        }

        //勝敗を記録ゲームセットの場合trueを返す
        public bool updateResult(bool deviceWin)
        {
            currentWinner = deviceWin ? TrainingDeviceType.TrainingDevice : TrainingDeviceType.ForceGauge;
            if(deviceWin)_trainingDeviceVictoryCount++;
            else _forceGaugeVictoryCount++;
            if(_trainingDeviceVictoryCount == TOTAL_WIN_COUNT || _forceGaugeVictoryCount == TOTAL_WIN_COUNT)
            {
                return true;
            }
            return false;
        }

        public void setDefaultGripStrengthMultiplier(float input)
        {
            //defaultGripStrengthMultiplier = input;
            decideGripStrengthMultiplier();

        }

        private void decideGripStrengthMultiplier()
        {
            //gripStrengthMultiplier = defaultGripStrengthMultiplier*multiplierOffset;
        }

        public void addLog(string message)
        {
            viewerTextQueue.Enqueue(message);
            

            if(viewerTextQueue.Count > MAX_LOG_LINES)
            {
                
                string hoge = viewerTextQueue.Dequeue();
            }
        }

        public void rotateTurnip(float normalizedValue)
        {
            float zeroCenter = (normalizedValue - 0.5f) * 2;
            float rotationValue = zeroCenter*60;
            turnip.transform.rotation = _initialTurnipRotation* Quaternion.Euler(rotationValue,0,0);
            Debug.Log(Mathf.Abs(zeroCenter));
            if(Mathf.Abs(zeroCenter) >= 0.6f)
            {
                vibrateTurnip(1.0f);
            }
            
        }

        public void vibrateTurnip(float vibrationAmplitude)
        {
            Vector3 turnipPosition = _initialTurnipPosition;
            float x_diff = Random.Range(-vibrationAmplitude,vibrationAmplitude) * vibrationAmplitude;
            float y_diff = Random.Range(-vibrationAmplitude,vibrationAmplitude) * vibrationAmplitude;
            float z_diff = Random.Range(-vibrationAmplitude,vibrationAmplitude) * vibrationAmplitude;
            turnipPosition.x += x_diff;
            turnipPosition.y += y_diff;
            turnipPosition.z += z_diff;
            turnip.transform.position = turnipPosition;
        }

        public void resultTurnipAction(bool won)
        {
            Rigidbody rb = turnip.AddComponent<Rigidbody>();
            rb.velocity = _initialTurnipVelocity;
        }

        public void resetTurnip()
        {
            Destroy(turnip.GetComponent<Rigidbody>());
        }

        private void writeLog()
        {
            string[] arr = viewerTextQueue.ToArray();
            string writeText = "";
            for(int i = 0;i < arr.Length; ++i)
            {
                writeText += arr[i] + "\n";
            }
            //print(writeText);
            viewerObject.GetComponent<Text>().text = writeText;
        }
        
        public float calculateSendingTorque(float opponentNormalizedValue)
        {
            return opponentNormalizedValue*gripStrengthMultiplier;
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
