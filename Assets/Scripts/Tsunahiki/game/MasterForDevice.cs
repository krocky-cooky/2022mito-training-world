using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using communication;

namespace tsunahiki.game
{


    public class MasterForDevice : MonoBehaviour
    {   
        private System.Random _randomGenerator = new System.Random();
        private int _trainingDeviceVictoryCount = 0;
        private int _forceGaugeVictoryCount = 0;
        private int _drawyCount = 0;
        public TrainingDeviceType superiority;
        

        // トルク÷握力計の値
        public float gripStrengthMultiplier = 0.5f;

        
    
        



        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

            

        }

        public void decideSuperiority()
        {
            int randomOutput = _randomGenerator.Next(0,2);
            superiority = randomOutput == 1 ? 
                TrainingDeviceType.TrainingDevice : 
                TrainingDeviceType.ForceGauge;
        }

        //勝敗を記録ゲームセットの場合trueを返す
        public bool updateResult()
        {
            
            return false;
        }

        

        




    }
}
