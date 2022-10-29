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

    public class DuringFishing_FishOnTheHook : MasterStateBase
    {
        // タイムカウント
        float currentTimeCount;

        // 魚のオブジェクト
        public Fish fish;


        // 初期トルク
        private float _fisrtTorque;

        // 魚の逃げるリスクを反映したゲージ
        // リールのテンションが緩いとゲージが減り、テンションが強すぎるとゲージが増える
        // -1になるとリールが緩んで針が取れて魚が逃げ、1になるとリールが切れて魚が逃げる
        private float _escapeGauge;

        // 魚の回転角[rad]
        private float _fishAngle;

        // 最大のHP
        private float _maxHP;

        public override void OnEnter()
        {
            Debug.Log("DuringFishing_FishOnTheHook");

            // トルクの指定
            _fisrtTorque = masterStateController.fish.weight / masterStateController.fishWeightPerTorque;
            masterStateController.gameMaster.sendingTorque = _fisrtTorque;

            // 音声を再生
            masterStateController.FishSoundOnTheHook.Play();

            // 魚をはりに移動
            masterStateController.distanceFromRope = 0.0f;
            masterStateController.fish.transform.position = masterStateController.ropeRelayBelowHandle.transform.position + new Vector3(masterStateController.distanceFromRope, 0.0f, 0.0f);

            // 初期化
            currentTimeCount = 0f;
            _escapeGauge = 0.0f;
            masterStateController.centerOfRotation = masterStateController.fish.transform.position + new Vector3(0.0f, 0.0f, masterStateController.radius);
            _fishAngle = 0.0f;
            _maxHP = masterStateController.fish.HP;
        }

        public override void OnExit()
        {
            masterStateController.FishSoundOnTheHook.Stop();
        }

        public override int StateUpdate()
        {
            currentTimeCount += Time.deltaTime;

            // 魚が円軌道で動く
            // HPに比例
            masterStateController.fish.transform.Rotate(0.0f, masterStateController.angularVelocity * Time.deltaTime * (masterStateController.fish.HP / _maxHP), 0.0f, Space.World);
            _fishAngle = (masterStateController.fish.transform.rotation.eulerAngles.y + masterStateController.initialAngle) *  Mathf.PI / 180.0f;
            // masterStateController.fish.transform.position += (- masterStateController.fish.transform.right) * masterStateController.radius * masterStateController.angularVelocity / 360.0f;
            // masterStateController.fish.transform.position = new Vector3(Mathf.Sin((masterStateController.transform.rotation.y + masterStateController.initialAngle) / 360.0f), 0.0f, Mathf.Cos((masterStateController.transform.rotation.y + masterStateController.initialAngle) / 360.0f)) * masterStateController.radius * masterStateController.angularVelocity / 360.0f;
            masterStateController.fish.transform.position = masterStateController.centerOfRotation + (new Vector3(Mathf.Sin(_fishAngle), 0.0f, Mathf.Cos(_fishAngle))) * masterStateController.radius;

            // 魚の暴れる強さ
            // 0と1の間を周期的に変化する
            masterStateController.fish.currentIntensityOfMovements = Mathf.Abs(Mathf.Sin(currentTimeCount / masterStateController.periodOfFishIntensity));

            // ロープの音の大きさとピッチを変更
            masterStateController.FishSoundOnTheHook.volume = masterStateController.minRopeSoundVolume + (1.0f - masterStateController.minRopeSoundVolume) * masterStateController.fish.currentIntensityOfMovements;
            masterStateController.FishSoundOnTheHook.pitch = masterStateController.FishSoundOnTheHook.volume * 3.0f;

            // トルク送信
            // 魚の暴れ具合に対してバーの高さが、ぴったりなら中間トルク、高ければトルクが大きくなり、低ければトルクが弱くなる
            // 魚の暴れ具合が、強い時はバーをさげ、弱い時はバーをあげながら、リールのテンションを一定に保つ
            // トルクの最大最小範囲を超えないようにする
            masterStateController.gameMaster.sendingTorque = _fisrtTorque + (masterStateController.fish.currentIntensityOfMovements + masterStateController.trainingDevice.currentNormalizedPosition - 1.5f) * masterStateController.torqueReduction;
            masterStateController.gameMaster.sendingTorque = Mathf.Clamp(masterStateController.gameMaster.sendingTorque, _fisrtTorque - masterStateController.torqueReduction, _fisrtTorque);


            // 魚のHPは、リールのテンションの強さ(=トルク)に応じて減少
            masterStateController.fish.HP -= masterStateController.gameMaster.sendingTorque * Time.deltaTime;

            // 逃げるゲージの更新
            // リールのテンションが緩いとゲージが減り、テンションが強すぎるとゲージが増える
            _escapeGauge += (masterStateController.fish.currentIntensityOfMovements + masterStateController.trainingDevice.currentNormalizedPosition - 1.0f) * masterStateController.fish.easeOfEscape * Time.deltaTime;
            Debug.Log("_escapeGauge is " + _escapeGauge.ToString());

            // 魚が逃げる
            if (Mathf.Abs(_escapeGauge) > 1.0f){
                return (int)MasterStateController.StateType.DuringFishing_Wait;
            }

            //HPがゼロになったら次に移行
            if (masterStateController.fish.HP < 0.0f){
                return (int)MasterStateController.StateType.DuringFishing_HP0;
            }

            // 釣りの前に戻る
            if (OVRInput.GetDown(OVRInput.RawButton.X) || Input.GetMouseButtonDown(1))
            {
                return (int)MasterStateController.StateType.BeforeFishing;
            }
            

            return (int)StateType;
        }

    }

}
