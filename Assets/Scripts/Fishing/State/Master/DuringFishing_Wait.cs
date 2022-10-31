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

    public class DuringFishing_Wait : MasterStateBase
    {

        // タイムカウント
        private float currentTimeCount;

        // 前回の確率計算時刻からの経過時間
        private float _timeSinceLastCalculatingProbability;
        
        // 出現確率を計算するためのインターバル
        private float interval = 0.5f;

        //　出現確率
        private float probabilityOfFishOnTheHook;

        // 魚のオブジェクトの配列
        private GameObject[] _fishGameObjects;

        // // 魚のゲームオブジェクト
        // private GameObject _fishGameObject;

        public override void OnEnter()
        {
            Debug.Log("DuringFish_Wait");
            currentTimeCount = 0.0f;
            _timeSinceLastCalculatingProbability = 0.0f;
            probabilityOfFishOnTheHook = 1.0f - Mathf.Pow(2.0f, (-1.0f / (masterStateController.timeUntilFishHitAtHalfChance / interval)));
            Debug.Log("hit prob is" + probabilityOfFishOnTheHook.ToString());

            // 釣りモード時のトルク指令
            masterStateController.gameMaster.sendingTorque = masterStateController.baseTorqueDuringFishing;

            masterStateController.frontViewUiText.text = "During fishing";

            // ルアーが着水する音
            masterStateController.LureLandingSound.Play();


            // 魚の初期化
            _fishGameObjects = GameObject.FindGameObjectsWithTag("fish");
            masterStateController.fishGameObject = _fishGameObjects[Random.Range(0, _fishGameObjects.Length)];
            masterStateController.fish = masterStateController.fishGameObject.GetComponent<Fish>();
            // masterStateController.fish.SetActive(true);
            masterStateController.fish.isFishShadow = true;
            masterStateController.fish.isFishBody = false;
            masterStateController.fish.weight = Random.Range(masterStateController.minTorque, masterStateController.maxTorque) * masterStateController.fishWeightPerTorque;
            masterStateController.fish.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            masterStateController.fishGameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            masterStateController.fish.twistSpeed = masterStateController.minSpeedOfFishTwist;
            masterStateController.ropeStateController.fish = masterStateController.fish;
            Debug.Log("fish name is " + masterStateController.fish.species);

            // 選んだ魚以外はすべて魚影、ボディを非アクティブ化
            foreach(GameObject fishGameObject in _fishGameObjects){
                if (fishGameObject != masterStateController.fishGameObject){
                    fishGameObject.GetComponent<Fish>().isFishShadow = false;
                    fishGameObject.GetComponent<Fish>().isFishBody = false;
                }
            }

            masterStateController.tensionSliderGameObject.SetActive(false);
        }

        public override void OnExit()
        {
        }

        public override int StateUpdate()
        {
            currentTimeCount += Time.deltaTime;
            _timeSinceLastCalculatingProbability += Time.deltaTime;

            // 魚を単振動で動かす
            masterStateController.distanceFromRope = masterStateController.BaseDistanceOfFishFromRope + masterStateController.SizeOfFishMovement * Mathf.Sin(currentTimeCount * Mathf.PI / masterStateController.PeriodOfFishMovement);
            masterStateController.fish.transform.position = masterStateController.ropeRelayBelowHandle.transform.position + new Vector3(masterStateController.distanceFromRope, 0.0f, 0.0f);
            
            if (_timeSinceLastCalculatingProbability >= interval)
            {
                _timeSinceLastCalculatingProbability = 0.0f;
                if (Random.value < probabilityOfFishOnTheHook){
                    return (int)MasterStateController.StateType.DuringFishing_Nibble;
                }
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
