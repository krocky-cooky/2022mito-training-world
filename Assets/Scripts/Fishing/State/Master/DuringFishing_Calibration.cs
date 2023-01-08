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

    public class DuringFishing_Calibration : MasterStateBase
    {
        // タイムカウント
        private float _currentTimeCount;

        // 魚の回転角[rad]と回転速度
        private float _fishAngle;
        private float _angleVelocity;


        // トルクのp乗に音やピッチを比例させる
        private float _p;

        // 速度超過時間
        private float _excessSpeedTime;

        public override void OnEnter()
        {
            Debug.Log("DuringFishing_FishOnTheHook");

            // 音声を再生
            master.FishSoundOnTheHook.Play();

            // 魚をはりに移動
            master.fish.transform.position = master.ropeRelayBelowHandle.transform.position + new Vector3(0.0f, 0.0f, 0.0f);

            // 初期化
            _currentTimeCount = 0f;
            master.centerOfRotation = master.fish.transform.position - new Vector3(0.0f, 0.0f, master.radius);
            _fishAngle = 0.0f;
            master.tensionSliderGameObject.SetActive(master.tensionSliderIsOn);

            // 水しぶき
            master.fish.splash.SetActive(true);
        }

        public override void OnExit()
        {
            master.FishSoundOnTheHook.Stop();
            OVRInput.SetControllerVibration(0.0f, 0.0f, OVRInput.Controller.RTouch);

            master.fish.splash.SetActive(false);
        }

        public override int StateUpdate()
        {
            _currentTimeCount += Time.deltaTime;


            // モータへの指令
            // if (_currentTimeCount < master.staticTimeAtCalibration){
            //     device.SetSpeedMode(0.0f, 4.5f);
            // }else{
            //     device.SetTorqueMode()
            // }
            // device.Apply();

            
            // トルクを負荷ゲージで表示
            // トルクの値の約4.0倍が負荷(kg)
            // master.tensionSlider.value = master.sendingTorque * 4.0f;

            // 魚がカラダをひねる強さを変化
            master.fish.twistSpeed = (master.maxSpeedOfFishTwist - master.minSpeedOfFishTwist) * master.fish.currentIntensityOfMovements + master.minSpeedOfFishTwist;

            // 魚が円軌道で動く
            // 魚の暴れる強さと、円軌道上での速さは比例
            _angleVelocity = master.minAngularVelocity + (master.maxAngularVelocity - master.minAngularVelocity) * master.fish.currentIntensityOfMovements;
            master.fish.transform.Rotate(0.0f, - _angleVelocity * Time.deltaTime, 0.0f, Space.World);
            _fishAngle = (master.fish.transform.rotation.eulerAngles.y + master.initialAngle) *  Mathf.PI / 180.0f + Mathf.PI;
            master.fish.transform.position = master.centerOfRotation + (new Vector3(Mathf.Sin(_fishAngle), 0.0f, Mathf.Cos(_fishAngle))) * master.radius;


            // トルク送信
            // master.sendingTorque = _minTorque + _normalizedTorque * _torqueDecrease;


            // ロープの音の大きさとピッチを変更
            // 音もピッチもトルクのp乗に比例。これで高域をシャープにする
            // _p = 2.32f;
            // master.FishSoundOnTheHook.volume = master.minRopeSoundVolume + (1.0f - master.minRopeSoundVolume) * (Mathf.Pow(_normalizedTorque, _p));
            // master.FishSoundOnTheHook.pitch = master.minRopePitch + (3.0f - master.minRopePitch) * (Mathf.Pow(_normalizedTorque, _p));

            // トルクに応じて右のリモコンの振動を生成
            // OVRInput.SetControllerVibration(0.01f, _normalizedTorque, OVRInput.Controller.RTouch);
            OVRInput.SetControllerVibration(0.01f, 0.5f, OVRInput.Controller.RTouch);

            // トルクをゲージで表示
            // if (master.tensionSliderIsOn){
            //     master.tensionSlider.value = _normalizedTorque;
            // }

            // リールの色は白
            master.rope.targetRopeColor = new Color32((byte)(255.0f),(byte)(255.0f), (byte)(255.0f), 1);       


            // // 魚が針から逃げる
            // if (_currentTimeCount > master.timeLimitToEscape){
            //     return (int)MasterStateController.StateType.DuringFishing_GetAway;
            // }

            // // リールが切れる
            // if (master.trainingDevice.currentNormalizedVelocity > master.normalizedSpeedLimitToBreakFishingLine){
            //     _excessSpeedTime += Time.deltaTime;
            // }else{
            //     _excessSpeedTime = 0.0f;
            // }
            // if (_excessSpeedTime > 0.1f){
            //     return (int)MasterStateController.StateType.DuringFishing_FishingLineBreaks;
            // }

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
