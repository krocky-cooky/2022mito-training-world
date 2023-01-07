using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using tsunahiki.game;
using tsunahiki.forceGauge.state;
using tsunahiki.forceGauge.stateController;


namespace tsunahiki.forceGauge.state
{
    public abstract class MasterStateBase : StateBase
    {
        protected MasterForForceGauge masterForForceGauge;

        // 初期化処理
        public override void Initialize(int stateType)
        {
            StateType = stateType;
            masterForForceGauge = GameObject.FindWithTag("master").GetComponent<MasterForForceGauge>();
        }
    }
}