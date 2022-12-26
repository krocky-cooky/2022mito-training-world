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

namespace Fishing.State
{
    public abstract class MasterStateBase : StateBase
    {
        // ステートコントローラー
        // protected MasterStateController masterStateController;
        protected Master master;

        // 初期化処理
        public override void Initialize(int stateType)
        {
            StateType = stateType;
            // masterStateController = GetComponent<MasterStateController>();
            master = GameObject.FindWithTag("master").GetComponent<Master>();
        }
    }
}