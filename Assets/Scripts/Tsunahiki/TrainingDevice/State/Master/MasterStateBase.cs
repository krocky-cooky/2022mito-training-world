using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using tsunahiki.game;
using tsunahiki.trainingDevice.stateController;
using TRAVE;


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
            TRAVEDevice device = TRAVEDevice.GetDevice();
            device.SetSpeedMode(0.0f,2.0f);
            device.Apply();
        }

        //ワイヤ巻取り
        protected void reelWire()
        {
            TRAVEDevice device = TRAVEDevice.GetDevice();
            device.SetSpeedMode(2.0f);
            device.Apply();
        }
    }
}