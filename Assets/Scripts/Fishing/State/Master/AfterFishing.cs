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

            // _firstLengthOfFishLine = Mathf.Abs(masterStateController.waterSurfaceTransform.position.y - masterStateController.ropeRelayBelowHandle.centerOfHandle.position.y) + 2.0f;
            _fishEndPosition = GameObject.FindWithTag("Player").transform.position + new Vector3(-4.0f, 3.0f, -masterStateController.distanseFromFishToCamera);

            masterStateController.frontViewUiText.text = masterStateController.fish.species + " " + masterStateController.fish.weight.ToString("f2") + "kg";

            masterStateController.gameMaster.sendingTorque = 0.0f;

            // 魚の表示を、水中の魚影モードから水上の実体モードに切り替え
            masterStateController.fish.isFishShadow = false;
            masterStateController.fish.isFishBody = true;

            // 魚の向きを整える
            masterStateController.fishGameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            // 魚釣りに成功した効果音を鳴らす
            Invoke("PlayFishingSuccess", masterStateController.timeRasingFish + masterStateController.timeShorteningFishingLine);

            // 水しぶき
            masterStateController.fish.splash.SetActive(true);
        }

        public override void OnExit()
        {
            masterStateController.frontViewUiText.text = "";

            // 魚の表示しない
            masterStateController.fish.isFishShadow = false;
            masterStateController.fish.isFishBody = false;
        }

        public override int StateUpdate()
        {
            currentTimeCount += Time.deltaTime;

            // トルクを負荷ゲージで表示
            // トルクの値の約4.0倍が負荷(kg)
            masterStateController.tensionSlider.value = masterStateController.gameMaster.sendingTorque * 4.0f;

            // 釣り糸と魚を水面の上まであげる
            // そのあと、魚を目の前まで動かす
            // 水しぶきも伴う
            if ((masterStateController.timeShorteningFishingLine - currentTimeCount) > 0.0f){
                // masterStateController.ropeRelayBelowHandle.ropeLengthDuringFishing = masterStateController.fishingLineLengthAfterFishing + (_firstLengthOfFishLine - masterStateController.fishingLineLengthAfterFishing) * (masterStateController.timeShorteningFishingLine - currentTimeCount) / masterStateController.timeShorteningFishingLine;
                // masterStateController.fish.transform.position = masterStateController.ropeRelayBelowHandle.transform.position;
                _fishFirstPosition = masterStateController.fish.transform.position;
            } else if ((masterStateController.timeRasingFish + masterStateController.timeShorteningFishingLine - currentTimeCount) > 0.0f){
                masterStateController.fish.transform.position = _fishEndPosition + (_fishFirstPosition - _fishEndPosition) * (masterStateController.timeRasingFish + masterStateController.timeShorteningFishingLine - currentTimeCount) / masterStateController.timeRasingFish;
            }else{
                masterStateController.fish.splash.SetActive(false);
            }

            if (OVRInput.GetDown(OVRInput.RawButton.X) || Input.GetMouseButtonDown(1))
            {
                return (int)MasterStateController.StateType.BeforeFishing;
            }

            return (int)StateType;
        }

        public void PlayFishingSuccess(){
            masterStateController.FishingSuccess.Play();
        }

    }

}
