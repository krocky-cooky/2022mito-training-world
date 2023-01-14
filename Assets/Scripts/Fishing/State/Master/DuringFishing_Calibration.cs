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

        // キャリブレーション用の変数
        // // 計測したトルクのリスト
        // private List<float> _measuredTorques = new List<float>();
        // // 計測したポジションのリスト
        // private List<float> _measuredPositions = new List<float>();
        // 速度指令して強制的にネガティブ動作をさせるかどうかのフラグ
        bool _isNegativeAction = false;

        // キャリブレーション後に魚を再修正
        public Fish reacquiredFish = new Fish();

        // キャリブレーション後の切り返し時にすっぽ抜けないように、段々負荷を下げる
        private float _timeCountForLighteningSlowly = 0.0f;
        private float _timeForLighteningSlowly = 3.0f;
        private float _lastTorqueAtCalibration;

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

            // キャリブレーション前に、ユーザーの最低パワーを基準値に再設定
            master.minUserPower = master.firstTorqueBeforeCalibration;

            // 記録を初期化
            master.measuredTorques = new List<float>(); // 計測したトルクのリスト
            master.measuredPositions = new List<float>(); // 計測したポジションのリスト
            master.measuredNormalizedTorques = new List<float>(); // 計測した正規化トルクのリスト
        }

        public override void OnExit()
        {
            master.FishSoundOnTheHook.Stop();
            OVRInput.SetControllerVibration(0.0f, 0.0f, OVRInput.Controller.RTouch);

            master.fish.splash.SetActive(false);


            // キャリブレーションした結果の筋力に合わせた魚を表示
            reacquiredFish = master.GetFishesOfSpecifiedWeight(master.fishSpecies, 1, master.minUserPower * 0.9f, master.minUserPower * 1.1f)[0];
            master.fish.isFishShadow = false;
            master.fish.splash.SetActive(false);
            master.fish = reacquiredFish;
            master.fish.isFishBody = true;
        }

        public override int StateUpdate()
        {
            _currentTimeCount += Time.deltaTime;

     
            // モータへの指令用のフラグの切り替え
            if(!(_isNegativeAction) & (master.measuredTorques.Count == 0) & (master.trainingDevice.currentNormalizedPosition > 0.9f)){
                _isNegativeAction = true;
            }else if(_isNegativeAction & master.trainingDevice.currentNormalizedPosition < 0.1f){
                _isNegativeAction = false;
            }

            // モータへの指令
            if(!(_isNegativeAction) & (master.minUserPower == master.firstTorqueBeforeCalibration)){
                // 最初は一定のトルクを与えて、まず最高位置まで持ち上げる
                master.device.SetTorqueMode(master.minUserPower);
            }else if(!(_isNegativeAction) & (master.minUserPower != master.firstTorqueBeforeCalibration)){
                // トルク計測後はユーザーの最低パワーを代入
                // ただし、すっぽ抜けないようにゆっくり負荷を下げる
                _timeCountForLighteningSlowly += Time.deltaTime;
                float _sendTorque = Mathf.Lerp(master.minUserPower, _lastTorqueAtCalibration, 1.0f -Mathf.Clamp01(_timeCountForLighteningSlowly / _timeForLighteningSlowly));
                master.device.SetTorqueMode(_sendTorque);
            }else{
                master.device.SetSpeedMode(master.velocityAtNegativeAction, 6.0f);
                master.measuredTorques.Add(master.device.torque);
                master.measuredPositions.Add(master.trainingDevice.currentNormalizedPosition);
            }

            // ユーザーの最高出力と最低出力を更新
            // トルク計算後もまだユーザーの出力記録が更新されていなければ、更新する
            if (!(_isNegativeAction) & (master.measuredTorques.Count != 0) & (master.minUserPower == master.firstTorqueBeforeCalibration)){
                // 計測データを処理したもの
                List<float> _processedMeasuredTorques;

                // 最初はユーザーの力が立ち上がる途中なのでカット
                int _cutoffIndex = (int)((float)master.measuredTorques.Count * master.cutoffRatioOfTime);
                _processedMeasuredTorques = master.measuredTorques.GetRange(_cutoffIndex, master.measuredTorques.Count - _cutoffIndex);

                // 計測したトルクをソート
                _processedMeasuredTorques.Sort();

                // 上から5%の値を最高値とする
                int _maxIndex = _processedMeasuredTorques.Count - (int)((float)_processedMeasuredTorques.Count * master.topPercentile);
                master.maxUserPower = _processedMeasuredTorques[_maxIndex];

                // 下から5%の値を最低値とする
                int _minIndex = (int)((float)_processedMeasuredTorques.Count * master.bottomPercentile);
                master.minUserPower = _processedMeasuredTorques[_minIndex];

                // 計測した正規化トルクのリストを作成
                foreach(float measuredTorque in master.measuredTorques)
                {
                    master.measuredNormalizedTorques.Add(Mathf.InverseLerp(master.minUserPower, master.maxUserPower, measuredTorque));
                }

                // 動摩擦力 = 0.4f, よって(ネガティブ時の動摩擦力 - ポジティブ時の動摩擦力) = 0.8f
                // ポジティブ動作の筋力 = ネガティブ動産の筋力 * 0.714f
                // 10RM ≒ 1RM * 0.8f
                master.minUserPower -= 0.8f;
                master.maxUserPower -= 0.8f;
                master.minUserPower *= 0.714f;
                master.maxUserPower *= 0.714f;
                // master.minUserPower *= 0.714f * 0.8f;
                // master.maxUserPower *= 0.714f * 0.8f;
                if (master.minUserPower < master.firstTorqueBeforeCalibration){
                    master.minUserPower = master.firstTorqueBeforeCalibration + 0.1f;
                    master.maxUserPower = master.minUserPower + 0.1f;
                }

                // キャリブレーション中の最後のトルク
                _lastTorqueAtCalibration = master.device.torque;

            }

            
            // トルクを負荷ゲージで表示
            // トルクの値の約4.0倍が負荷(kg)
            // master.tensionSlider.value = master.sendingTorque * 4.0f;

            // 魚が暴れる強さ
            master.fish.currentIntensityOfMovements = Mathf.Clamp01(0.2f * (master.device.torque / master.firstTorqueBeforeCalibration));

            // 魚がカラダをひねる強さを変化
            master.fish.twistSpeed = (master.maxSpeedOfFishTwist - master.minSpeedOfFishTwist) * master.fish.currentIntensityOfMovements + master.minSpeedOfFishTwist;

            // 魚が円軌道で動く
            // 魚の暴れる強さと、円軌道上での速さは比例
            _angleVelocity = master.minAngularVelocity + (master.maxAngularVelocity - master.minAngularVelocity) * master.fish.currentIntensityOfMovements;
            master.fish.transform.Rotate(0.0f, - _angleVelocity * Time.deltaTime, 0.0f, Space.World);
            _fishAngle = (master.fish.transform.rotation.eulerAngles.y + master.initialAngle) *  Mathf.PI / 180.0f + Mathf.PI;
            master.fish.transform.position = master.centerOfRotation + (new Vector3(Mathf.Sin(_fishAngle), 0.0f, Mathf.Cos(_fishAngle))) * master.radius;

            // ロープの音の大きさとピッチを変更
            // 音もピッチもトルクのp乗に比例。これで高域をシャープにする
            _p = 2.32f;
            master.FishSoundOnTheHook.volume = master.minRopeSoundVolume + (1.0f - master.minRopeSoundVolume) * (Mathf.Pow(master.fish.currentIntensityOfMovements, _p));
            master.FishSoundOnTheHook.pitch = master.minRopePitch + (3.0f - master.minRopePitch) * (Mathf.Pow(master.fish.currentIntensityOfMovements, _p));

            // トルクに応じて右のリモコンの振動を生成
            OVRInput.SetControllerVibration(0.01f, master.fish.currentIntensityOfMovements, OVRInput.Controller.RTouch); 
            OVRInput.SetControllerVibration(0.01f, 0.5f, OVRInput.Controller.RTouch);

            // トルクをゲージで表示
            if (master.tensionSliderIsOn){
                master.tensionSlider.value = master.fish.currentIntensityOfMovements;
            }

            // リールの色は白
            master.rope.targetRopeColor = new Color32((byte)(255.0f),(byte)(255.0f), (byte)(255.0f), 1);       

            // 魚を最高高さまで引き揚げたらAdfterFishingに移行
            if (master.trainingDevice.currentNormalizedPosition >= 0.99 & !(_isNegativeAction) & (master.measuredTorques.Count != 0)){
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
