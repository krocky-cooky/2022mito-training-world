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
    public class Fight : MasterStateBase
    {
        private string superiorityMessage;

        private BeamController _centerFlareController;

        public override void OnEnter()
        {
            Debug.Log("Fight");

            if ((int)masterForForceGauge.opponentData.superiority == masterForForceGauge.myDeviceId){
                superiorityMessage = "Advantageous";
            }else{
                superiorityMessage = "DisAdvantageous";
            }

            // masterForForceGauge.frontViewUI.text = "Fight !!\n" + "Time " + masterForForceGauge.opponentData.timeCount.ToString() + "\n" + superiorityMessage;
            masterForForceGauge.frontViewUI.text = "Fight !!";
            
            masterForForceGauge.myBeam.isFired = true;
            masterForForceGauge.OpponentPlayer.beamController.isFired = true;

            _centerFlareController = masterForForceGauge.centerFlare.GetComponent<BeamController>();
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

            // 自分と相手のビーム出力の合計値と、中央のフレアの強さを相関
            _centerFlareController.normalizedScale = Mathf.Clamp01((masterForForceGauge.myBeam.normalizedScale + masterForForceGauge.OpponentPlayer.beamController.normalizedScale) / 2.0f);

            // masterForForceGauge.centerFlare.SetActive(masterForForceGauge.myBeam.reachCenter);
            masterForForceGauge.centerFlare.GetComponent<CreateBeamLine>().enabled = masterForForceGauge.myBeam.reachCenter;

            // centerFlareの移動
            // 通信をしてないときはここでエラーが出てUpdate()処理が止まる
            // float normalizedDevicePos = _coordinator.getOpponentValue();
            float normalizedDevicePos = masterForForceGauge.opponentData.normalizedData;
            Vector3 cubePos = masterForForceGauge.cubeStartPosition;
            cubePos.z += (normalizedDevicePos - 0.5f) * masterForForceGauge.moveParameter;
            masterForForceGauge.centerFlare.transform.position = cubePos;
            Debug.Log("normalizedDevicePos is " + normalizedDevicePos.ToString());

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
