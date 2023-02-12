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
    public class Calibration : MasterStateBase
    {
        private string superiorityMessage;

        private BeamController _centerFlareController;

        public override void OnEnter()
        {
            Debug.Log("Calibration");

            masterForForceGauge.frontViewUI.text = "Fight !!";
            StartCoroutine(masterForForceGauge.DisplayOnUI(masterForForceGauge.UIFollowingEyes, "Fight!!", 3.0f));
            
            masterForForceGauge.myBeam.isFired = true;
            masterForForceGauge.OpponentPlayer.beamController.isFired = true;

            _centerFlareController = masterForForceGauge.centerFlare.GetComponent<BeamController>();

            masterForForceGauge.myForceGauge.maxForce = 5.0f;
        }

        public override void OnExit()
        {
        }

        public override int StateUpdate()
        {
            // 自分と相手のビーム出力の合計値と、中央のフレアの強さを相関
            _centerFlareController.normalizedScale = Mathf.Clamp01((masterForForceGauge.myBeam.normalizedScale + masterForForceGauge.OpponentPlayer.beamController.normalizedScale) / 2.0f);

            // 2つのビームが中央に達したらセンターフレアが表示
            masterForForceGauge.centerFlare.GetComponent<CreateBeamLine>().enabled = masterForForceGauge.myBeam.reachCenter;

            // 最大値を更新
            masterForForceGauge.myForceGauge.maxForce = Mathf.Max(masterForForceGauge.myForceGauge.maxForce, masterForForceGauge.myForceGauge.currentForce);

            if ((int)masterForForceGauge.opponentData.stateId == (int)TsunahikiStateType.Fight)
            {   
                return (int)MasterStateController.StateType.Fight;
            }      

            return (int)StateType;


        }
    }

}
