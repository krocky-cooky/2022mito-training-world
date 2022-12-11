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

namespace Fishing.StateController
{
    public class MasterStateController : StateControllerBase
    {
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

        public enum StateType
        {
            BeforeFishing,
            DuringFishing_Wait,
            DuringFishing_Nibble,
            DuringFishing_FishOnTheHook,
            DuringFishing_HP0,
            AfterFishing,
        }

        // 初期化処理
        public override void Initialize(int initializeStateType)
        {
            // ここ自動化したいな
            stateDic[(int)StateType.BeforeFishing] = gameObject.AddComponent<BeforeFishing>();
            stateDic[(int)StateType.BeforeFishing].Initialize((int)StateType.BeforeFishing);

            stateDic[(int)StateType.DuringFishing_Wait] = gameObject.AddComponent<DuringFishing_Wait>();
            stateDic[(int)StateType.DuringFishing_Wait].Initialize((int)StateType.DuringFishing_Wait);

            stateDic[(int)StateType.DuringFishing_Nibble] = gameObject.AddComponent<DuringFishing_Nibble>();
            stateDic[(int)StateType.DuringFishing_Nibble].Initialize((int)StateType.DuringFishing_Nibble);

            stateDic[(int)StateType.DuringFishing_FishOnTheHook] = gameObject.AddComponent<DuringFishing_FishOnTheHook>();
            stateDic[(int)StateType.DuringFishing_FishOnTheHook].Initialize((int)StateType.DuringFishing_FishOnTheHook);

            stateDic[(int)StateType.DuringFishing_HP0] = gameObject.AddComponent<DuringFishing_HP0>();
            stateDic[(int)StateType.DuringFishing_HP0].Initialize((int)StateType.DuringFishing_HP0);

            stateDic[(int)StateType.AfterFishing] = gameObject.AddComponent<AfterFishing>();
            stateDic[(int)StateType.AfterFishing].Initialize((int)StateType.AfterFishing);

            CurrentState = initializeStateType;
            stateDic[CurrentState].OnEnter();
        }
    }
}
