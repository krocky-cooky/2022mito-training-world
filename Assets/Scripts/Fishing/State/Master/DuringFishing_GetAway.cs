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

    public class DuringFishing_GetAway : MasterStateBase
    {
        // タイムカウント
        private float _currentTimeCount;

        // 魚が逃げるときのスピード
        private float _fishSpeed;

        public override void OnEnter()
        {
            Debug.Log("DuringFishing_GetAway");

            // 現在の魚の速度を、逃げる際も引きつぐ
            _fishSpeed = Mathf.Lerp(master.minAngularVelocity, master.maxAngularVelocity, master.fish.currentIntensityOfMovements) * Mathf.Deg2Rad * master.radius;

            master.FishGetAway.Play();

            // ファイト回数を追加
            master.fightingCount += 1;

        }

        public override void OnExit()
        {
        }

        public override int StateUpdate()
        {
            _currentTimeCount += Time.deltaTime;

            // 魚を直進させる
            // 円運動時の最低速度で逃げる
            {
                master.fish.transform.position +=  Time.deltaTime * _fishSpeed * (- master.fish.transform.right);
            }

            // 釣りの前に戻る
            if (OVRInput.GetDown(OVRInput.RawButton.X) || Input.GetMouseButtonDown(1) || (_currentTimeCount > 5.0f))
            {
                return (int)MasterStateController.StateType.BeforeFishing;
            }

            return (int)StateType;
        }

    }

}
