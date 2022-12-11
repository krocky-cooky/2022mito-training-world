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


        // 最大トルク. これは魚の重量に比例
        private float _maxTorque;
        // 最小トルク
        private float _minTorque;

        // 正規化トルク. 最小なら0.0f, 最大なら1.0f
        private float _normalizedTorque;

        // 魚の逃げるリスクを反映したゲージ
        // リールのテンションが緩いとゲージが減り、テンションが強すぎるとゲージが増える
        // -1になるとリールが緩んで針が取れて魚が逃げ、1になるとリールが切れて魚が逃げる
        private float _escapeGauge;

        // 魚の回転角[rad]と回転速度
        private float _fishAngle;
        private float _angleVelocity;

        // 最大のHP
        private float _maxHP;

        // トルクのp乗に音やピッチを比例させる
        private float _p;

        // ロープの色の濃度
        private float _colorIntensity;

        // トルクの減少量
        private float _torqueDecrease;

        public override void OnEnter()
        {
            Debug.Log("DuringFishing_FishOnTheHook");

            // トルクの指定
            // _maxTorque = masterStateController.fish.weight / masterStateController.fishWeightPerTorque;
            _maxTorque = masterStateController.fish.torque;
            masterStateController.gameMaster.sendingTorque = _maxTorque;

            // 音声を再生
            masterStateController.FishSoundOnTheHook.Play();

            // 魚をはりに移動
            masterStateController.distanceFromRope = 0.0f;
            masterStateController.fish.transform.position = masterStateController.ropeRelayBelowHandle.transform.position + new Vector3(masterStateController.distanceFromRope, 0.0f, 0.0f);

            // 初期化
            currentTimeCount = 0f;
            _escapeGauge = 0.0f;
            masterStateController.centerOfRotation = masterStateController.fish.transform.position - new Vector3(0.0f, 0.0f, masterStateController.radius);
            _fishAngle = 0.0f;
            _maxHP = masterStateController.fish.HP;
            _torqueDecrease = Mathf.Min(masterStateController.torqueReduction, _maxTorque - 0.75f);
            _minTorque = _maxTorque - _torqueDecrease;
            _normalizedTorque = 0.0f;
            masterStateController.tensionSliderGameObject.SetActive(masterStateController.tensionSliderIsOn);
        }

        public override void OnExit()
        {
            masterStateController.FishSoundOnTheHook.Stop();
            OVRInput.SetControllerVibration(0.0f, 0.0f, OVRInput.Controller.RTouch);
            masterStateController.fish.HP = _maxHP;
        }

        public override int StateUpdate()
        {
            currentTimeCount += Time.deltaTime;

            // トルクを負荷ゲージで表示
            // トルクの値の約4.0倍が負荷(kg)
            masterStateController.tensionSlider.value = masterStateController.gameMaster.sendingTorque * 4.0f;

            // 魚の暴れる強さ
            // 0と1の間を周期的に変化する
            // HPが小さくなると、振幅も小さくなる
            // 最初の数秒間は、暴れる強さを一定にして、その時の視聴力覚を覚えてもらう
            if (currentTimeCount < masterStateController.timeUntillFishIntensityChange){
                masterStateController.fish.currentIntensityOfMovements = 1.0f;
            }else{
                masterStateController.fish.currentIntensityOfMovements = Mathf.Pow((masterStateController.fish.HP / _maxHP), 0.3f) * Mathf.Abs(Mathf.Sin(currentTimeCount / masterStateController.periodOfFishIntensity));
            }

            // 魚がカラダをひねる強さを変化
            masterStateController.fish.twistSpeed = (masterStateController.maxSpeedOfFishTwist - masterStateController.minSpeedOfFishTwist) * masterStateController.fish.currentIntensityOfMovements + masterStateController.minSpeedOfFishTwist;

            // 魚が円軌道で動く
            // 魚の暴れる強さと、円軌道上での速さは比例
            _angleVelocity = masterStateController.minAngularVelocity + (masterStateController.maxAngularVelocity - masterStateController.minAngularVelocity) * masterStateController.fish.currentIntensityOfMovements;
            masterStateController.fish.transform.Rotate(0.0f, _angleVelocity * Time.deltaTime, 0.0f, Space.World);
            _fishAngle = (masterStateController.fish.transform.rotation.eulerAngles.y + masterStateController.initialAngle) *  Mathf.PI / 180.0f;
            masterStateController.fish.transform.position = masterStateController.centerOfRotation + (new Vector3(Mathf.Sin(_fishAngle), 0.0f, Mathf.Cos(_fishAngle))) * masterStateController.radius;


            // トルク送信
            // 魚の暴れ具合に対してバーの高さが、ぴったりなら中間トルク、高ければトルクが大きくなり、低ければトルクが弱くなる
            // 魚の暴れ具合が、強い時はバーをさげ、弱い時はバーをあげながら、リールのテンションを一定に保つ
            // トルクの最大最小範囲を超えないようにする
            _normalizedTorque = masterStateController.fish.currentIntensityOfMovements + masterStateController.trainingDevice.currentNormalizedPosition - 0.5f;
            _normalizedTorque = Mathf.Clamp01(_normalizedTorque);
            masterStateController.gameMaster.sendingTorque = _minTorque + _normalizedTorque * _torqueDecrease;

            // ロープの音の大きさとピッチを変更
            // 音もピッチもトルクのp乗に比例。これで高域をシャープにする
            _p = 2.32f;
            masterStateController.FishSoundOnTheHook.volume = masterStateController.minRopeSoundVolume + (1.0f - masterStateController.minRopeSoundVolume) * (Mathf.Pow(_normalizedTorque, _p));
            masterStateController.FishSoundOnTheHook.pitch = masterStateController.minRopePitch + (3.0f - masterStateController.minRopePitch) * (Mathf.Pow(_normalizedTorque, _p));

            // トルクに応じて右のリモコンの振動を生成
            OVRInput.SetControllerVibration(0.01f, _normalizedTorque, OVRInput.Controller.RTouch);

            // トルクをゲージで表示
            if (masterStateController.tensionSliderIsOn){
                masterStateController.tensionSlider.value = _normalizedTorque;
            }

            // トルクに応じてリールの色を調整
            _colorIntensity = Mathf.Abs(_normalizedTorque - 0.5f) * 2.0f;
            if (_normalizedTorque < 0.5f){
                masterStateController.rope.targetRopeColor = new Color32((byte)(255.0f - 255.0f * _colorIntensity),(byte)(255.0f - 162.0f * _colorIntensity), (byte)(255.0f), 1);
            }else{
                masterStateController.rope.targetRopeColor = new Color32((byte)(255.0f),(byte)(255.0f - 175.0f * _colorIntensity), (byte)(255.0f - 255.0f * _colorIntensity), 1);
            }


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
