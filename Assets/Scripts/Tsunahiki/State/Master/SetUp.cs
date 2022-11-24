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
            masterForForceGauge.frontViewUI.text = "Set Up\n最大握力を設定してください";

            masterForForceGauge.centerFlare.SetActive(false);
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

            if (OVRInput.GetDown(OVRInput.RawButton.X) || Input.GetMouseButtonDown(1))
            {   
                isGoingToBattle = !isGoingToBattle;
            }

            if (isGoingToBattle)
            {   
                masterForForceGauge.frontViewUI.text = "Going To Fight...";

                if ((int)masterForForceGauge.opponentData.stateId == (int)TsunahikiStateType.Ready)
                {   
                    return (int)MasterStateController.StateType.Ready;
                }
            }else{
                masterForForceGauge.frontViewUI.text = "Set Up\nSet Maximum Force";
            }


            return (int)StateType;
        }
    }

}
