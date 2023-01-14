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
using TRAVE;
using System.Linq;

namespace Fishing.Game
{

    public class Master : MonoBehaviour
    {   
        public TRAVEDevice device = TRAVEDevice.GetDevice();

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

        //　出現しうる魚のPrefabのリスト
        public List<GameObject> fishSpecies;


        [SerializeField]
        private GameObject viewerObject;
        [SerializeField]
        private MainCommunicationInterface communicationInterface;
        [SerializeField]
        private float commandSendingInterval = 0.1f;


        // 自分が持っているハンドルの位置
        [SerializeField]
        private Transform myHandle;

        // ハンドル位置を合わせる先
        [SerializeField]
        private Transform targetHandle;

        private float time = 0.0f;
        private float _previoussendingTorque = 0.0f;
        private float _previousCommandSendingTime = 0.0f;
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

        //  釣り中のトルク範囲
        public float minTorqueDuringFishing;
        public float maxTorqueDuringFishing;


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
        // 魚が逃げるときの効果音
        public AudioSource FishGetAway;
        // リールが切れるときの効果音
        public AudioSource FishingLineBreaks;     

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

        // 釣り上げ予定の魚
        public Fish fishToBeCaught;

        // 釣り上げ予定ではなく、周囲を泳いでいる魚
        public List<Fish> swimmingAroundFishes;

        // 回遊の基準点
        public Vector3 basePointForSwimmingAround;

        // 出現する魚の数
        public int numberOfApearanceFishes = 4;

        // 釣果のリスト
        public List<string> fishingRecord = new List<string>();

        // 左コントローラにあるUI
        public Text UIByLeftController;
        
        // 魚が針から逃げるまでのタイムリミット
        public float timeLimitToEscape;

        // リールが切れるまでの速度制限
        public float normalizedSpeedLimitToBreakFishingLine;

        // ルアーのゲームオブジェクト
        public GameObject lure;

        // ファイト回数(=釣った回数＆逃げられた回数)
        public int fightingCount = 0;

        // // キャリブレーション開始直後の速度0指令時間
        // public float staticTimeAtCalibration;

        // キャリブレーション用の変数
        public float velocityAtNegativeAction; // ネガティブ動作時の速度
        public float cutoffRatioOfTime; // ネガティブ動作の初期はユーザーの力が立ち上がる途中なので、いくつかデータを切り落とす
        public float topPercentile; // 上位?%のデータを最高値とする
        public float bottomPercentile;// 下位?%のデータを最高値とする
        public List<float> measuredTorques = new List<float>(); // 計測したトルクのリスト
        public List<float> measuredPositions = new List<float>(); // 計測したポジションのリスト
        public List<float> measuredNormalizedTorques = new List<float>(); // 計測した正規化トルクのリスト

        // Start is called before the first frame update
        void Start()
        {
            masterStateController.Initialize((int)MasterStateController.StateType.BeforeFishing);

            //Make connection with TRAVE device if connection hasn't been made.
            if(!device.isConnected)
            {
                device.ReConnectToDevice();
            }
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
            // if(OVRInput.Get(OVRInput.RawButton.LIndexTrigger))
            // {
            //     UpdateTorque(0.75f);
            // }else{
            //     UpdateTorque(sendingTorque);
            // }
            if(OVRInput.Get(OVRInput.RawButton.LIndexTrigger))
            {
                device.SetTorqueMode(0.75f);
            }

            // モータへの制御指令を一定時間間隔で送信
            UpdateCommand();

            // 回遊用の魚を動かす
            if(swimmingAroundFishes.Count > 0){
                float _count = 0.0f;
                foreach(Fish _swimmingAroundFish in swimmingAroundFishes)
                {
                    _count += 1.0f;
                    MoveFishOnEllipse(_swimmingAroundFish, time, 9.0f + 3.0f * _count, 1.0f, 0.5f, - 60.0f * (_count - 1.0f), Mathf.PI * 0.4f * _count);
                }
            }

            // 釣果を表示
            UIByLeftController.text = "本日の釣果\n";
            int _numberOfDisplayedFishes = Mathf.Min(fishingRecord.Count, 8);
            foreach(string oneRecored in fishingRecord.Skip(fishingRecord.Count - _numberOfDisplayedFishes)) {
                UIByLeftController.text += oneRecored + "\n";
            }

            // モータのデータ読み取り
            Debug.Log("read torque is " + device.torque.ToString());
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

        //トルクを更新
        // private void UpdateTorque(float torque, float speed = 10.0f)
        // {
        //     if ((time - _previousTorqueSendingTime) < torqueSendingInterval){
        //         return;
        //     }
        //     // SendingDataFormat data = new SendingDataFormat();
        //     // data.setTorque(torque, speed);
        //     // communicationInterface.sendData(data);
        //     device.SetTorqueMode(torque);
        //     device.Apply();

        //     Debug.Log("send torque" + torque.ToString());
        //     _previoussendingTorque = sendingTorque;
        //     _previousTorqueSendingTime = time;
            
        // }

        // 指令を更新
        private void UpdateCommand(){
            if ((time - _previousCommandSendingTime) <commandSendingInterval){
                return;
            }
            device.Apply();
            _previousCommandSendingTime = time;
        }


        //モーターの現状復帰用関数(速度0指令)
        private void restore()
        {
            // SendingDataFormat data = new SendingDataFormat();
            // data.setSpeed(0.0f);
            // communicationInterface.sendData(data);
            device.SetSpeedMode(0.0f);
            device.Apply();
        }

        // 魚を楕円軌道で動かす
        // fish:魚のオブジェクト, period:周期,  longDiameterOfSwimmingTrack:長径, shortDiameterOfSwimmingTrack:短径, angleAroundNeedle:楕円中心位置の、針に対する偏角, initPhase:初期位相
        public void MoveFishOnEllipse(Fish fish, float time, float period, float longDiameterOfSwimmingTrack, float shortDiameterOfSwimmingTrack, float angleAroundNeedle, float initPhase)
        {
            // 位相
            float _phase;
            _phase = - 2.0f * Mathf.PI * time / period + initPhase;

            // 針回りに回転させる前の位置と回転
            fish.transform.position = basePointForSwimmingAround + new Vector3(longDiameterOfSwimmingTrack * Mathf.Sin(_phase), 0.0f, shortDiameterOfSwimmingTrack * Mathf.Cos(_phase)) +  new Vector3(- longDiameterOfSwimmingTrack, 0.0f, - shortDiameterOfSwimmingTrack);
            fish.transform.rotation = Quaternion.Euler(0.0f, _phase * Mathf.Rad2Deg, 0.0f);

            // 針回りに回転
            fish.transform.RotateAround(basePointForSwimmingAround, Vector3.up, angleAroundNeedle);
        }


        // 指定の重量の魚を、指定の匹数だけ出現させて返す
        public List<Fish> GetFishesOfSpecifiedWeight(List<GameObject> _fishSpecies,int _numberOfFishes, float _minTorque, float _maxTorque){
            List<Fish> _appearingFishes = new List<Fish>();
            int tryCount = 0;

            // 指定した匹数だけ繰り返す
            while (_appearingFishes.Count < _numberOfFishes & tryCount < 10000){
                tryCount += 1;

                // 魚をランダムに取得
                Fish _candidateFishPrefab;
                _candidateFishPrefab = _fishSpecies[Random.Range (0, _fishSpecies.Count)].GetComponent<Fish>();

                // 魚のスケールをランダムに変更
                float _scale = Random.Range(0.5f, 2.0f);

                // 魚の釣り上げ時の負荷(トルク)が指定範囲内なら追加
                if ((_candidateFishPrefab.torque * _scale > _minTorque) & (_candidateFishPrefab.torque * _scale < _maxTorque)){
                    Fish _candidateFishInstance;
                    _candidateFishInstance = GameObject.Instantiate(_candidateFishPrefab, transform.position, transform.rotation);

                    // スケールおよび重量を変更
                    _candidateFishInstance.scale = _scale;
                    _candidateFishInstance.torque *= _scale;
                    // _candidateFishInstance.weight *= _scale;

                    // あとで自分の挙上重量がわかるように、weightとtorqueは相関させておく
                    _candidateFishInstance.weight = _candidateFishInstance.torque * 4.0f;

                    // インスタンスを追加
                    _appearingFishes.Add(_candidateFishInstance);

                    // 魚の初期化
                    _candidateFishInstance.isFishShadow = false;
                    _candidateFishInstance.isFishBody = false;
                    _candidateFishInstance.splash.SetActive(false);
                    _candidateFishInstance.twistSpeed = minSpeedOfFishTwist;
                    _candidateFishInstance.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }

            return _appearingFishes;
        }


        /// 目的の値に最も近い値を取得(float用)
        public int GetIndexOfNearestValue(List<float> source, float targetValue){
            if (source.Count == 0) {
            Debug.LogError($"値が入っていないので、最も近い値を取得出来ません");
            return -1;
            }
            
            //目的の値との差の絶対値が最小の値を計算
            var min = source.Min(value => Mathf.Abs(value - targetValue));
            
            //絶対値が最小の値だった物を最も近い値
            var nearestValue = source.First(value => Mathf.Approximately(Mathf.Abs(value - targetValue), min));

            // 最も近い値のindex
            int index = source.IndexOf(nearestValue);

            return index;
        }

    }

    
}
