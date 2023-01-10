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

        public override void OnEnter()
        {
            Debug.Log("DuringFish_Wait");
            currentTimeCount = 0.0f;
            _timeSinceLastCalculatingProbability = 0.0f;
            probabilityOfFishOnTheHook = 1.0f - Mathf.Pow(2.0f, (-1.0f / (master.timeUntilFishHitAtHalfChance / interval)));
            Debug.Log("hit prob is" + probabilityOfFishOnTheHook.ToString());

            // 釣りモード時のトルク指令
            master.sendingTorque = master.baseTorqueDuringFishing;

            master.frontViewUiText.text = "During fishing";

            // すでにある魚のインスタンスをすべて削除
            _fishGameObjects = GameObject.FindGameObjectsWithTag("fish");
            if(_fishGameObjects.Length > 0){
                foreach(GameObject _fishGameObject in _fishGameObjects)
                {
                    Destroy(_fishGameObject);
                }
            }


            // // 自分の最小筋力の±10%以内の重量の魚を取得
            // // 釣り上げ予定の魚
            master.fishToBeCaught = GetFishesOfSpecifiedWeight(master.fishSpecies, 1, master.minUserPower * 0.8f, master.minUserPower * 1.2f)[0];
            master.fish = master.fishToBeCaught;
            master.rope.fish = master.fish;
            // // 泳ぎ回る予定の魚
            master.swimmingAroundFishes = GetFishesOfSpecifiedWeight(master.fishSpecies, master.numberOfApearanceFishes - 1, master.minUserPower * 0.8f, master.minUserPower * 1.2f);

            // ルアーが着水するまで魚を非表示
            Invoke("SetActiveOfAllFishes", master.rope.lureDropTime);

            master.tensionSliderGameObject.SetActive(false);
        }

        public override void OnExit()
        {
        }

        public override int StateUpdate()
        {
            master.basePointForSwimmingAround = master.ropeRelayBelowHandle.position;

            currentTimeCount += Time.deltaTime;
            _timeSinceLastCalculatingProbability += Time.deltaTime;

            // トルクを負荷ゲージで表示
            // トルクの値の約4.0倍が負荷(kg)
            master.tensionSlider.value = master.sendingTorque * 4.0f;
            
            // 釣り上げ予定の魚を動かす
            master.MoveFishOnEllipse(master.fish, currentTimeCount, 10.0f, 1.0f, 0.5f, -90.0f, 0.0f);

            // 針にかかる予定の魚が針のほうを向いたら、次のステートに移行
            {
                Vector3 _directionToLure;
                float _angleOfLurePositionAndFishDirection;
                _directionToLure = master.ropeRelayBelowHandle.position - master.fish.transform.position;
                _angleOfLurePositionAndFishDirection = Vector3.Angle(_directionToLure, - master.fish.transform.right);
                Debug.Log("_angleOfLurePositionAndFishDirection" + _angleOfLurePositionAndFishDirection.ToString());
                
                if (Mathf.Abs(_angleOfLurePositionAndFishDirection) < 2.0f){
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

        // 指定の重量の魚を、指定の匹数だけ出現させて返す
        public List<Fish> GetFishesOfSpecifiedWeight(List<GameObject> _fishSpecies,int _numberOfFishes, float _minTorque, float _maxTorque){
            List<Fish> _appearingFishes = new List<Fish>();
            int tryCount = 0;

            // 指定した匹数だけ繰り返す
            while (_appearingFishes.Count < _numberOfFishes & tryCount < 10000){
                tryCount += 1;

                // 魚をランダムに取得
                Fish _candidateFishPrefab;
                _candidateFishPrefab = _fishSpecies[Random.Range (0, _fishSpecies.Count)].GetComponent<Fish>();

                // 魚の釣り上げ時の負荷(トルク)が指定範囲内なら追加
                if ((_candidateFishPrefab.torque > _minTorque) & (_candidateFishPrefab.torque < _maxTorque)){
                    Fish _candidateFishInstance;
                    _candidateFishInstance = GameObject.Instantiate(_candidateFishPrefab, transform.position, transform.rotation);
                    _appearingFishes.Add(_candidateFishInstance);

                    // 魚の初期化
                    _candidateFishInstance.isFishShadow = false;
                    _candidateFishInstance.isFishBody = false;
                    _candidateFishInstance.splash.SetActive(false);
                    _candidateFishInstance.twistSpeed = master.minSpeedOfFishTwist;
                    _candidateFishInstance.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }

            return _appearingFishes;
        }

        // すべての魚を表示
        public void SetActiveOfAllFishes(){
            // 釣り上げ予定の魚の表示
            master.fish.isFishShadow = true;
            master.fish.isFishBody = false;
            master.fish.splash.SetActive(false);

            // 回遊用の魚の表示
            if(master.swimmingAroundFishes.Count > 0){
                foreach(Fish _swimmingAroundFish in master.swimmingAroundFishes)
                {
                    _swimmingAroundFish.isFishShadow = true;
                    _swimmingAroundFish.isFishBody = false;
                    _swimmingAroundFish.splash.SetActive(false);
                }
            }
        }

    }



}
