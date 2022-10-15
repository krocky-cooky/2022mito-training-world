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

    public class DuringFishing_Wait : MasterStateBase
    {

        [SerializeField]
        private float timeUntilfFishHitAtHalfChance = 1.0f;

        // タイムカウント
        private float currentTimeCount;
        
        // 出現確率を計算するためのインターバル
        private float interval = 0.5f;

        //　出現確率
        private float probabilityOfFishOnTheHook;

        public override void OnEnter()
        {
            Debug.Log("DuringFish_Wait");
            currentTimeCount = 0.0f;
            probabilityOfFishOnTheHook = 1.0f - Mathf.Pow(2.0f, (-1.0f / (timeUntilfFishHitAtHalfChance / interval)));
            Debug.Log("hit prob is" + probabilityOfFishOnTheHook.ToString());
        }

        public override void OnExit()
        {
        }

        public override int StateUpdate()
        {
            currentTimeCount += Time.deltaTime;

            if (currentTimeCount >= interval)
            {
                currentTimeCount = 0.0f;
                if (Random.value < probabilityOfFishOnTheHook){
                    return (int)MasterStateController.StateType.DuringFishing_FishOnTheHook;
                }
            }

            return (int)StateType;
        }
    }

}
