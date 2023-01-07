using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using tsunahiki.game;
using tsunahiki.trainingDevice.state;


namespace tsunahiki.trainingDevice.stateController
{
    public class MasterStateController : StateControllerBase
    {

        [System.Serializable]
        public class ButtonAllotment
        {
            public OVRInput.RawButton Ready;
            public OVRInput.RawButton TorqueRegistered;
            public OVRInput.RawButton ReelWire;
        }


        
        //all
        public bool testMode = false; //テストモードではステートの移行に対戦相手の状況を加味しない

        public MasterForDevice master;
        
        public MainCommunicationInterface communicationInterface;
        
        public RemoteCoordinator coordinator;
        
        public TrainingDevice trainingDevice;
        
        public ButtonAllotment buttonAllotment;
        
        public float torqueSendingInterval = 0.2f; //トルク送信の間隔

        public GameObject opponentAvatar;



        //SetUp State
        
        public float maxTorque = 0.0f; //発揮筋力最大値
        
        public bool maxTorqueRegistered = false; //_maxTorqueを決定したか
        

        //Fight State
        
        public GameObject centerCube; //中央のオブジェクト
        
        public GameObject opponentHandle; //対戦相手のハンドル
        
        public float opponentMotionAmplitude = 0.5f; //対戦相手の手の動きの振幅



        
        

        public enum StateType
        {
            SetUp,
            Ready,
            Fight,
            EndOfFight,
            GameSet,
        }

        // 初期化処理
        public override void Initialize(int initializeStateType)
        {
            // ここ自動化したいな
            stateDic[(int)StateType.SetUp] = gameObject.AddComponent<SetUp>();
            stateDic[(int)StateType.SetUp].Initialize((int)StateType.SetUp);

            stateDic[(int)StateType.Ready] = gameObject.AddComponent<Ready>();
            stateDic[(int)StateType.Ready].Initialize((int)StateType.Ready);

            stateDic[(int)StateType.Fight] = gameObject.AddComponent<Fight>();
            stateDic[(int)StateType.Fight].Initialize((int)StateType.Fight);

            stateDic[(int)StateType.EndOfFight] = gameObject.AddComponent<Fight>();
            stateDic[(int)StateType.EndOfFight].Initialize((int)StateType.Fight);

            // stateDic[(int)StateType.GameSet] = gameObject.AddComponent<GameSet>();
            // stateDic[(int)StateType.GameSet].Initialize((int)StateType.GameSet);

            CurrentState = initializeStateType;
            Debug.Log("initialize currentState");
            stateDic[CurrentState].OnEnter();
        }
    }
}