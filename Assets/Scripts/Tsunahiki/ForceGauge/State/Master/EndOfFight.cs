using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using tsunahiki.game;
using tsunahiki.forceGauge.state;
using tsunahiki.forceGauge.stateController;



namespace tsunahiki.forceGauge.state
{
    public class EndOfFight : MasterStateBase
    {

        public Vector3 _flareDestinaion;
        private Vector3 _initPosition;
        private float _initTime;


        // フレアが自他に当たった際の衝撃音をならすためのフラグ
        private bool _playedShockSound;

        public override void OnEnter()
        {
            Debug.Log("EndOfFight");

            _initPosition = masterForForceGauge.centerFlare.transform.position;

            _initTime = masterForForceGauge.time;

            // 勝ったら相手に、負けたら自分に、、ドローなら初期位置に、中央のフレアの行先を設定
            if ((int)masterForForceGauge.opponentData.latestWinner == masterForForceGauge.myDeviceId){   
                _flareDestinaion = masterForForceGauge.opponentTransform.position;
                StartCoroutine(masterForForceGauge.DisplayOnUI(masterForForceGauge.UIFollowingEyes, "You Win!", 3.0f));
            }else if ((int)masterForForceGauge.opponentData.latestWinner == (int)TrainingDeviceType.Nothing){
                _flareDestinaion = _initPosition;
                StartCoroutine(masterForForceGauge.DisplayOnUI(masterForForceGauge.UIFollowingEyes, "Draw", 3.0f));
            }else{
                _flareDestinaion = masterForForceGauge.myTransform.position;
                StartCoroutine(masterForForceGauge.DisplayOnUI(masterForForceGauge.UIFollowingEyes, "You Lose...", 3.0f));
            }

            masterForForceGauge.centerFlare.SetActive(true);

            _playedShockSound = false;
        }

        public override void OnExit()
        {
            _playedShockSound = false;

            // 初期位置に戻す
            masterForForceGauge.centerFlare.transform.position = _initPosition;

            // ビームとフレアを非表示・非アクティブ化
            masterForForceGauge.centerFlare.GetComponent<CreateBeamLine>().enabled = false;
            masterForForceGauge.myBeam.isFired = false;
            masterForForceGauge.OpponentPlayer.beamController.isFired = false;
        }

        public override int StateUpdate()
        {
            // 初期位置とEndPointの間を指定時間をかけて移動
            // 衝突して2秒待ち、音が鳴り終わったら、非アクティブ化
            if ((masterForForceGauge.time - _initTime) < masterForForceGauge.flareMovingTime){
                masterForForceGauge.centerFlare.transform.position = _initPosition + (_flareDestinaion - _initPosition) * (masterForForceGauge.time - _initTime) / masterForForceGauge.flareMovingTime;
                Debug.Log("center flare position is "+masterForForceGauge.centerFlare.transform.position.z.ToString());
            }else if ((masterForForceGauge.time - _initTime) < (masterForForceGauge.flareMovingTime + 2.0f)){
                // 衝撃音をならす
                if(!_playedShockSound){
                    masterForForceGauge.centerFlare.GetComponent<BeamController>().playShockSound = true;
                    _playedShockSound = true;
                    Debug.Log("played shock sound");
                }
            }else{
                // ビームとフレアの非表示
                masterForForceGauge.centerFlare.GetComponent<CreateBeamLine>().enabled = false;
                masterForForceGauge.myBeam.isFired = false;
                masterForForceGauge.OpponentPlayer.beamController.isFired = false;
            }


            if ((int)masterForForceGauge.opponentData.stateId == (int)TsunahikiStateType.Ready)
            {   
                return (int)MasterStateController.StateType.Ready;
            }    

            if ((int)masterForForceGauge.opponentData.stateId == (int)TsunahikiStateType.GameSet)
            {   
                return (int)MasterStateController.StateType.GameSet;
            }      

            return (int)StateType;
        }
    }

}
