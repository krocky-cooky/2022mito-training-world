using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using Fishing.Game;
using Fishing.State;
using Fishing.StateController;


namespace Fishing.State
{

    public class AfterFishing : StateBase
    {
        // タイムカウント
        float currentTimeCount;

        // 待機時間
        static readonly float waitDuration = 2f;

        public override void OnEnter()
        {
            Debug.Log("AfterFishing");
            currentTimeCount = 0f;
        }

        public override void OnExit()
        {
            // Do Nothing.
        }

        public override int StateUpdate()
        {
            currentTimeCount += Time.deltaTime;

            if (currentTimeCount >= waitDuration)
            {
                return (int)MasterStateController.StateType.BeforeFishing;
            }

            return (int)StateType;
        }
    }

}
