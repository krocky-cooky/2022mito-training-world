using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using tsunahiki.game;
using tsunahiki.trainingDevice.stateController;


namespace tsunahiki.trainingDevice.state
{
    public abstract class MasterStateBase : StateBase
    {
        // ステートコントローラー

        [System.Serializable]
        public class ButtonAllotment
        {
            public OVRInput.RawButton Ready;
            public OVRInput.RawButton TorqueRegistered;
        }

        //all
        [SerializeField]
        protected MasterForDevice master;
        [SerializeField]
        protected MainCommunicationInterface communicationInterface;
        [SerializeField]
        protected RemoteCoordinator coordinator;
        [SerializeField]
        protected TrainingDevice trainingDevice;
        [SerializeField]
        protected ButtonAllotment buttonAllotment;
        [SerializeField]
        protected float torqueSendingInterval = 0.2f; //トルク送信の間隔



        //SetUp State
        [SerializeField]
        protected float maxTorque = 0.0f; //発揮筋力最大値
        [SerializeField]
        protected bool maxTorqueRegistered = false; //_maxTorqueを決定したか
        

        //Fight State
        [SerializeField]
        protected GameObject centerCube; //中央のオブジェクト
        [SerializeField]
        protected GameObject opponentHandle; //対戦相手のハンドル
        [SerializeField]
        protected float opponentMotionAmplitude = 0.5f; //対戦相手の手の動きの振幅

        
        

        // 初期化処理
        public override void Initialize(int stateType)
        {
            StateType = stateType;

        }

        //速度ゼロ指令
        protected void restore()
        {
            SendingDataFormat data = new SendingDataFormat();
            data.setSpeed(0.0f);
            communicationInterface.sendData(data);
        }


    }
}