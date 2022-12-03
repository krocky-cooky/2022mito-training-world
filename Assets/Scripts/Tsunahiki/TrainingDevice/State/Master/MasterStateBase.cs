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
        protected MasterStateController stateController;


        // 初期化処理
        public override void Initialize(int stateType)
        {
            stateController = GetComponent<MasterStateController>();
            StateType = stateType;

        }

        //速度ゼロ指令
        protected void restore()
        {
            SendingDataFormat data = new SendingDataFormat();
            data.setSpeed(0.0f);
            stateController.communicationInterface.sendData(data);
        }


    }
}