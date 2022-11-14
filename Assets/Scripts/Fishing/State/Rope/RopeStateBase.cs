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
    public abstract class RopeStateBase : StateBase
    {
        // ステートコントローラー
        protected RopeStateController ropeStateController;

        // 初期化処理
        public override void Initialize(int stateType)
        {
            StateType = stateType;
            ropeStateController = GetComponent<RopeStateController>();
        }
    }
}