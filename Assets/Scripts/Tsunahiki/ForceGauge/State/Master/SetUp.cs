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

    public class SetUp : MasterStateBase
    {

        private bool isGoingToBattle = false;

        public override void OnEnter()
        {
            Debug.Log("Set Up");
            masterForForceGauge.frontViewUI.text = "Set Up";

            masterForForceGauge.centerFlare.GetComponent<CreateBeamLine>().enabled = false;

            
        }

        public override void OnExit()
        {
            isGoingToBattle = false;
        }

        public override int StateUpdate()
        {
            // 勝敗をリセット
            masterForForceGauge.victoryCounts = 0;
            masterForForceGauge.defeatCounts = 0;
            masterForForceGauge.defeatCounts = 0;

            if (OVRInput.GetDown(OVRInput.RawButton.X) || OVRInput.GetDown(OVRInput.RawButton.A) || Input.GetMouseButtonDown(1))
            {   
                isGoingToBattle = !isGoingToBattle;
                // return (int)MasterStateController.StateType.Fight; //temp
            }

            if (isGoingToBattle)
            {   
                masterForForceGauge.frontViewUI.text = "Going To Fight...";

                if ((int)masterForForceGauge.opponentData.stateId == (int)TsunahikiStateType.Ready)
                {   
                    return (int)MasterStateController.StateType.Ready;
                }
            }else{
                masterForForceGauge.frontViewUI.text = "Set Up";
            }


            return (int)StateType;
        }
    }

}
