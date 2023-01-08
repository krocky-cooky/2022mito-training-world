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
            _fishSpeed = Mathf.Lerp(master.minAngularVelocity, master.maxAngularVelocity, master.fish.currentIntensityOfMovements) * Mathf.Deg2Rad * master.radius * 2.0f;

            master.FishGetAway.Play();

            // ファイト回数を追加
            master.fightingCount += 1;

            // 負荷を小さくする
            // master.sendingTorque = Mathf.Max(master.sendingTorque - 1.0f, master.baseTorqueDuringFishing);
            master.device.SetTorqueMode(Mathf.Max(master.sendingTorque - 1.0f, master.baseTorqueDuringFishing));

        }

        public override void OnExit()
        {
            master.fish.isFishShadow = false;
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
            if (OVRInput.GetDown(OVRInput.RawButton.X) || Input.GetMouseButtonDown(1) || (_currentTimeCount > 7.0f))
            {
                return (int)MasterStateController.StateType.BeforeFishing;
            }

            return (int)StateType;
        }

    }

}
