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
        private float _currentTimeCount;

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

        // 速度超過時間
        private float _excessSpeedTime;

        public override void OnEnter()
        {
            Debug.Log("DuringFishing_FishOnTheHook");

            // トルクの指定
            // _maxTorque = master.fish.weight / master.fishWeightPerTorque;
            _maxTorque = master.fish.torque;
            master.sendingTorque = _maxTorque;

            // 音声を再生
            master.FishSoundOnTheHook.Play();

            // 魚をはりに移動
            master.distanceFromRope = 0.0f;
            master.fish.transform.position = master.ropeRelayBelowHandle.transform.position + new Vector3(master.distanceFromRope, 0.0f, 0.0f);

            // 初期化
            _currentTimeCount = 0f;
            _escapeGauge = 0.0f;
            master.centerOfRotation = master.fish.transform.position - new Vector3(0.0f, 0.0f, master.radius);
            _fishAngle = 0.0f;
            _maxHP = master.fish.HP;
            _torqueDecrease = Mathf.Min(master.torqueReduction, _maxTorque - 0.75f);
            _minTorque = _maxTorque - _torqueDecrease;
            _normalizedTorque = 0.0f;
            master.tensionSliderGameObject.SetActive(master.tensionSliderIsOn);

            // 水しぶき
            master.fish.splash.SetActive(true);
        }

        public override void OnExit()
        {
            master.FishSoundOnTheHook.Stop();
            OVRInput.SetControllerVibration(0.0f, 0.0f, OVRInput.Controller.RTouch);
            master.fish.HP = _maxHP;

            master.fish.splash.SetActive(false);
        }

        public override int StateUpdate()
        {
            _currentTimeCount += Time.deltaTime;
            
            // トルクを負荷ゲージで表示
            // トルクの値の約4.0倍が負荷(kg)
            master.tensionSlider.value = master.sendingTorque * 4.0f;

            // 魚の暴れる強さ
            // 引きあげるほど強くなる
            master.fish.currentIntensityOfMovements = master.trainingDevice.currentNormalizedPosition;


            // 魚がカラダをひねる強さを変化
            master.fish.twistSpeed = (master.maxSpeedOfFishTwist - master.minSpeedOfFishTwist) * master.fish.currentIntensityOfMovements + master.minSpeedOfFishTwist;

            // 魚が円軌道で動く
            // 魚の暴れる強さと、円軌道上での速さは比例
            _angleVelocity = master.minAngularVelocity + (master.maxAngularVelocity - master.minAngularVelocity) * master.fish.currentIntensityOfMovements;
            master.fish.transform.Rotate(0.0f, - _angleVelocity * Time.deltaTime, 0.0f, Space.World);
            _fishAngle = (master.fish.transform.rotation.eulerAngles.y + master.initialAngle) *  Mathf.PI / 180.0f + Mathf.PI;
            master.fish.transform.position = master.centerOfRotation + (new Vector3(Mathf.Sin(_fishAngle), 0.0f, Mathf.Cos(_fishAngle))) * master.radius;


            // トルク送信
            // 魚の暴れ具合に対してバーの高さが、ぴったりなら中間トルク、高ければトルクが大きくなり、低ければトルクが弱くなる
            // 魚の暴れ具合が、強い時はバーをさげ、弱い時はバーをあげながら、リールのテンションを一定に保つ
            // トルクの最大最小範囲を超えないようにする
            _normalizedTorque = master.fish.currentIntensityOfMovements + master.trainingDevice.currentNormalizedPosition - 0.5f;
            _normalizedTorque = Mathf.Clamp01(_normalizedTorque);
            master.sendingTorque = _minTorque + _normalizedTorque * _torqueDecrease;

            // ロープの音の大きさとピッチを変更
            // 音もピッチもトルクのp乗に比例。これで高域をシャープにする
            _p = 2.32f;
            master.FishSoundOnTheHook.volume = master.minRopeSoundVolume + (1.0f - master.minRopeSoundVolume) * (Mathf.Pow(_normalizedTorque, _p));
            master.FishSoundOnTheHook.pitch = master.minRopePitch + (3.0f - master.minRopePitch) * (Mathf.Pow(_normalizedTorque, _p));

            // トルクに応じて右のリモコンの振動を生成
            OVRInput.SetControllerVibration(0.01f, _normalizedTorque, OVRInput.Controller.RTouch);

            // トルクをゲージで表示
            if (master.tensionSliderIsOn){
                master.tensionSlider.value = _normalizedTorque;
            }

            // トルクに応じてリールの色を調整
            _colorIntensity = Mathf.Abs(_normalizedTorque - 0.5f) * 2.0f;
            if (_normalizedTorque < 0.5f){
                master.rope.targetRopeColor = new Color32((byte)(255.0f - 255.0f * _colorIntensity),(byte)(255.0f - 162.0f * _colorIntensity), (byte)(255.0f), 1);
            }else{
                master.rope.targetRopeColor = new Color32((byte)(255.0f),(byte)(255.0f - 175.0f * _colorIntensity), (byte)(255.0f - 255.0f * _colorIntensity), 1);
            }


            // 魚が針から逃げる
            if (_currentTimeCount > master.timeLimitToEscape){
                return (int)MasterStateController.StateType.DuringFishing_GetAway;
            }

            // リールが切れる
            if (master.trainingDevice.currentNormalizedVelocity > master.normalizedSpeedLimitToBreakFishingLine){
                _excessSpeedTime += Time.deltaTime;
            }else{
                _excessSpeedTime = 0.0f;
            }
            if (_excessSpeedTime > 0.1f){
                return (int)MasterStateController.StateType.DuringFishing_FishingLineBreaks;
            }

            // 魚を最高高さまで引き揚げたらAdfterFishingに移行
            if (master.trainingDevice.currentNormalizedPosition >= 0.99){
                return (int)MasterStateController.StateType.AfterFishing;
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
