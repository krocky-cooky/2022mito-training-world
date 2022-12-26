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

    public class AfterFishing : MasterStateBase
    {
        // タイムカウント
        float currentTimeCount;

        // 釣り糸の初期長さ
        private float _firstLengthOfFishLine;

        // 釣り糸が短くなりきったときの魚の位置
        private Vector3 _fishFirstPosition;

        // 魚の最終位置
        private Vector3 _fishEndPosition;

        public override void OnEnter()
        {
            Debug.Log("AfterFishing");
            currentTimeCount = 0f;

            // _firstLengthOfFishLine = Mathf.Abs(master.waterSurfaceTransform.position.y - master.ropeRelayBelowHandle.centerOfHandle.position.y) + 2.0f;
            // _fishEndPosition = GameObject.FindWithTag("Player").transform.position + new Vector3(-4.0f, 3.0f, -master.distanseFromFishToCamera);
            // _fishEndPosition = new Vector3(master.ropeRelayBelowHandle.transform.position.x, master.HMD.transform.position.y, master.ropeRelayBelowHandle.transform.position.z - 1.0f);
            _fishEndPosition = master.fishUpPosition.position;

            master.frontViewUiText.text = master.fish.species + " " + master.fish.weight.ToString("f2") + "kg";

            master.sendingTorque = 0.0f;

            // 魚の表示を、水中の魚影モードから水上の実体モードに切り替え
            master.fish.isFishShadow = false;
            master.fish.isFishBody = true;

            // 魚の向きを整える
            // master.fishGameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            master.fish.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            // 魚釣りに成功した効果音を鳴らす
            Invoke("PlayFishingSuccess", master.timeRasingFish + master.timeShorteningFishingLine);

            // 水しぶき
            master.fish.splash.SetActive(true);
        }

        public override void OnExit()
        {
            master.frontViewUiText.text = "";

            // 魚の表示しない
            master.fish.isFishShadow = false;
            master.fish.isFishBody = false;
        }

        public override int StateUpdate()
        {
            currentTimeCount += Time.deltaTime;

            // トルクを負荷ゲージで表示
            // トルクの値の約4.0倍が負荷(kg)
            master.tensionSlider.value = master.sendingTorque * 4.0f;

            // 釣り糸と魚を水面の上まであげる
            // そのあと、魚を目の前まで動かす
            // 水しぶきも伴う
            if ((master.timeShorteningFishingLine - currentTimeCount) > 0.0f){
                // master.ropeRelayBelowHandle.ropeLengthDuringFishing = master.fishingLineLengthAfterFishing + (_firstLengthOfFishLine - master.fishingLineLengthAfterFishing) * (master.timeShorteningFishingLine - currentTimeCount) / master.timeShorteningFishingLine;
                // master.fish.transform.position = master.ropeRelayBelowHandle.transform.position;
                _fishFirstPosition = master.fish.transform.position;
            } else if ((master.timeRasingFish + master.timeShorteningFishingLine - currentTimeCount) > 0.0f){
                master.fish.transform.position = _fishEndPosition + (_fishFirstPosition - _fishEndPosition) * (master.timeRasingFish + master.timeShorteningFishingLine - currentTimeCount) / master.timeRasingFish;
            }else{
                master.fish.splash.SetActive(false);
            }

            if (OVRInput.GetDown(OVRInput.RawButton.X) || Input.GetMouseButtonDown(1))
            {
                return (int)MasterStateController.StateType.BeforeFishing;
            }

            return (int)StateType;
        }

        public void PlayFishingSuccess(){
            master.FishingSuccess.Play();
        }

    }

}
