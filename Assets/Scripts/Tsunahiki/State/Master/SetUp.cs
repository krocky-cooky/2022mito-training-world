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

    public class SetUp : MasterStateBase
    {

        private bool isGoingToBattle = false;

        public override void OnEnter()
        {
            Debug.Log("Set Up");
            masterForForceGauge.frontViewUI.text = "Set Up";
        }

        public override void OnExit()
        {
        }

        public override int StateUpdate()
        {
            if (OVRInput.GetDown(OVRInput.RawButton.X) || Input.GetMouseButtonDown(1))
            {   
                isGoingToBattle = !isGoingToBattle;
            }     

            if (isGoingToBattle)
            {   
                masterForForceGauge.frontViewUI.text = "Going To Battle...";

                if ((int)masterForForceGauge.coordinator.getOpponentData().stateId == (int)TsunahikiStateType.GetReady)
                {   
                    return (int)MasterStateController.StateType.GetReady;
                }               
            }


            return (int)StateType;
        }
    }

}
