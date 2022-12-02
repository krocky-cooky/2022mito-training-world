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
    public class Fight : MasterStateBase
    {
        private string superiorityMessage;

        public override void OnEnter()
        {
            Debug.Log("Fight");
            masterForForceGauge.frontViewUI.text = "Fight !!";
            
            masterForForceGauge.myBeam.isFired = true;
            masterForForceGauge.opponentBeam.isFired = true;
        }

        public override void OnExit()
        {
        }

        public override int StateUpdate()
        {

            // //temp
            // if (OVRInput.GetDown(OVRInput.RawButton.X) || Input.GetMouseButtonDown(1))
            // {   
            //     masterForForceGauge.centerFlare.SetActive(false);
            //     masterForForceGauge.myBeam.isFired = false;
            //     masterForForceGauge.opponentBeam.isFired = false;

            //     return (int)MasterStateController.StateType.SetUp; //temp
            // }

            masterForForceGauge.centerFlare.SetActive(masterForForceGauge.myBeam.reachCenter);

            if ((int)masterForForceGauge.opponentData.superiority == masterForForceGauge.myDeviceId){
                superiorityMessage = "Advantageous";
            }else{
                superiorityMessage = "DisAdvantageous";
            }

            masterForForceGauge.frontViewUI.text = "Fight !!\n" + "Time " + masterForForceGauge.opponentData.timeCount.ToString() + "\n" + superiorityMessage;

            if ((int)masterForForceGauge.opponentData.stateId == (int)TsunahikiStateType.EndOfFight)
            {   
                updateResult();
                return (int)MasterStateController.StateType.EndOfFight;
            }      

            return (int)StateType;



        }

        // 勝敗結果を更新
        private void updateResult(){
            if ((int)masterForForceGauge.opponentData.latestWinner == masterForForceGauge.myDeviceId){   
                masterForForceGauge.victoryCounts += 1;
            }else if((int)masterForForceGauge.opponentData.latestWinner == (int)TrainingDeviceType.Nothing){
                masterForForceGauge.drawCounts += 1;
            }else{
                masterForForceGauge.defeatCounts += 1;
            }
        }
    }

}
