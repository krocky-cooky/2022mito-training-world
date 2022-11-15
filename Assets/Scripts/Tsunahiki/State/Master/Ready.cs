using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using tsunahiki.game;
using tsunahiki.state;
using tsunahiki.stateController;



namespace tsunahiki.state
{

    public class Ready : MasterStateBase
    {

        public override void OnEnter()
        {
            Debug.Log("Ready");
            masterForForceGauge.frontViewUI.text = "Ready...\n" + "Victory Count: " + masterForForceGauge.victoryCounts.ToString() + "\nDefeat Count: " + masterForForceGauge.defeatCounts.ToString() + "\nDraw Count: " + masterForForceGauge.drawCounts.ToString();
        }

        public override void OnExit()
        {
        }

        public override int StateUpdate()
        {

            if ((int)masterForForceGauge.opponentData.stateId == (int)TsunahikiStateType.Fight)
            {   
                return (int)MasterStateController.StateType.Fight;
            }

            return (int)StateType;
        }
    }

}
