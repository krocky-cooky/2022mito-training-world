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

    public class Ready : MasterStateBase
    {

        public override void OnEnter()
        {
            Debug.Log("Ready");
            masterForForceGauge.frontViewUI.text = "Ready...\n" + "Victory Count: " + masterForForceGauge.victoryCounts.ToString() + "\nDefeat Count: " + masterForForceGauge.defeatCounts.ToString() + "\nDraw Count: " + masterForForceGauge.drawCounts.ToString();
            StartCoroutine(masterForForceGauge.DisplayOnUI(masterForForceGauge.UIFollowingEyes, "Ready...", 5.0f));
            masterForForceGauge.centerFlare.GetComponent<CreateBeamLine>().enabled = false;
        }

        public override void OnExit()
        {
        }

        public override int StateUpdate()
        {
            masterForForceGauge.OpponentPlayer.head.SetInitTransform();

            if ((int)masterForForceGauge.opponentData.stateId == (int)TsunahikiStateType.Calibration)
            {   
                return (int)MasterStateController.StateType.Calibration;
            }

            return (int)StateType;
        }
    }

}
