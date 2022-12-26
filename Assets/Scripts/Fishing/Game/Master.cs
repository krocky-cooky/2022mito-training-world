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
        public MasterStateController masterStateController;

        // 魚が釣り上がる位置
        public Transform fishUpPosition;

        // ユーザーの筋力
        public float minUserPower;
        public float maxUserPower;

        [SerializeField]
        private GameObject viewerObject;
        [SerializeField]
        private MainCommunicationInterface communicationInterface;
        [SerializeField]
        private float torqueSendingInterval = 0.1f;

        // 自分が持っているハンドルの位置
        [SerializeField]
        private Transform myHandle;

        // ハンドル位置を合わせる先
        [SerializeField]
        private Transform targetHandle;

        private float time = 0.0f;
        private float _previoussendingTorque = 0.0f;
        private float _previousTorqueSendingTime = 0.0f;
        private bool duringReelingWire = false;


        public Master gameMaster;
        public TrainingDevice trainingDevice;

        // ハンドルの上げ下げのズレの許容量
        public float allowableDifference;

        // 魚の逃げにくさの値の変化速度
        public float changeRateOfEscape;

        // 魚の暴れ具合の変動周期
        public float periodOfFishIntensity;

        // 魚を捕らえるときの竿の振り上げの時間と大きさ
        public float timeOfRaising;
        public float lengthOfRasing;

        // 魚を捕らえたとき
        // 短くなった後の釣り糸の長さ
        public float fishingLineLengthAfterFishing;
        //釣り糸を短くする時間
        public float timeShorteningFishingLine;
        //魚が跳ね上がって静止する位置とカメラの間の距離
        public float distanseFromFishToCamera;
        // 魚が跳ね上がっている時間
        public float timeRasingFish;

        // 水面の位置
        public Transform waterSurfaceTransform;

        // 釣り糸の先端
        public Transform ropeRelayBelowHandle;

        // 魚のオブジェクト
        public Fish fish;
        public GameObject fishGameObject;

        // トルクの範囲
        public float maxTorque;
        public float minTorque;

        // 単位トルクあたりの魚重量
        public float fishWeightPerTorque;

        // 魚によるトルクの振動の振幅と周期
        public float maxAplitudeOfTorque;
        public float periodOfTorque;

        // // トルクのスパイクの周期と大きさ
        // public float spikeInterval;

        // トルクのスパイクの時間
        public float firstSpikePeriod;
        public float firstSpikeSize;
        public float latterSpikePeriod;
        public float latterSpikeSize;

        //  釣り中のベーストルク
        public float baseTorqueDuringFishing = 0.75f;

        // 魚が針を突いているときのパラメータ
        // 魚が突く時間間隔の最大値と最小値
        public float maxIntervalOfNibbling;
        public float minIntervalOfNibbling;
        // 魚が針を突き始めてからしっかりかかるまでの最大時間と最小時間
        public float maxTimeOfNibbling;
        public float minTimeOfNibbling;

        // 魚が半分の確率で引っかかる時
        public float timeUntilFishHitAtHalfChance;

        // 正面のオブジェクト
        public Text frontViewUiText;

        // 魚のトルク変化の下降量
        public float torqueReduction;

        // 待機中の魚の動きの大きさと周期と、基準点のロープとの距離
        public float SizeOfFishMovement;
        public float PeriodOfFishMovement;
        public float BaseDistanceOfFishFromRope;
        public float distanceFromRope;
        public float SizeOfFishNibble;
        public float firstPeriodOfFishNibble;
        public float latterPeriodOfFishNibble;

        // つつきのための時間調整量
        public float buffurTimeForNibble = -0.5f;


        // 魚が突くときの音を視覚や力覚と同期させるためのバッファ
        public float buffurTimeForNibbleSound = -0.5f;

        // サウンド
        // ルアーの着水音
        public AudioSource LureLandingSound;
        // 魚が針を突くときの水の音
        public AudioSource NibbleSound;
        // 魚が針にかかっている最中の音
        public AudioSource FishSoundOnTheHook;
        // 魚の体力がなくなった時の魚の音
        public AudioSource FishSoundWithHP0;
        // 魚が水面に浮かび上がる音
        public AudioSource FishGoOnTheWater;
        // 魚釣りに成功した時の効果音
        public AudioSource FishingSuccess;

        // ロープの音の最小値
        public float minRopeSoundVolume;
        // ロープのピッチの最小値
        public float minRopePitch;

        // 円軌道のパラメータ 
        public float radius;
        public float maxAngularVelocity;
        public float minAngularVelocity;
        public float initialAngle;
        public Vector3 centerOfRotation;

        // 魚の動きのアニメーター
        // public Animator sardineAnimator;
        public float maxSpeedOfFishTwist;
        public float minSpeedOfFishTwist;

        // ロープのステートコントローラ
        public FishingLine rope;

        // 張力ゲージ
        public Slider tensionSlider;
        public GameObject tensionSliderGameObject;
        public bool tensionSliderIsOn;

        // 魚が針に引っかかってから、暴れる強さが変化しはじめるまでの時間
        // それまでは、暴れる強さを一定にして、その時の視聴力覚を覚えてもらう
        public float timeUntillFishIntensityChange;

        // プレイヤーのヘッドの位置
        public GameObject HMD;

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

            // // ハンドルホルダーにハンドルを置いて中指ボタンを押したら、それに合わせてプレイヤーの位置をリセット
            // if(OVRInput.GetDown(OVRInput.RawButton.LHandTrigger))
            // {
            //     ResetPlayerTransform();
            // }
            
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

        // // ハンドルホルダーにハンドルを置いて中指ボタンを押したら、それに合わせてプレイヤーの位置をリセット
        // void ResetPlayerTransform(){
        //     Transform player;
        //     player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //     player.position += (targetHandle.position - myHandle.position);
        //     // player.eulerAngles += (targetHandle.eulerAngles - myHandle.eulerAngles);
        //     player.eulerAngles += new Vector3(0.0f, (targetHandle.eulerAngles - myHandle.eulerAngles).y, 0.0f);
        // }
        
    }
}
