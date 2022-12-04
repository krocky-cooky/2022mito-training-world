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

    public class GameSet : MasterStateBase
    {

        // 勝ち越し回数
        private int _winCounts;

        // 結果のメッセージ
        private string _resultMessage;

        public override void OnEnter()
        {
            Debug.Log("Game Set");

            _winCounts = masterForForceGauge.victoryCounts - masterForForceGauge.defeatCounts;
            if (_winCounts > 0){
                _resultMessage = "You Win !!";
            }else if(_winCounts < 0){
                _resultMessage = "You Lose...";
            }else{
                _resultMessage = "Draw";
            }

            masterForForceGauge.frontViewUI.text = _resultMessage + "\nPress A Button to go to Set Up Mode";

            masterForForceGauge.centerFlare.SetActive(false);
        }

        public override void OnExit()
        {
        }

        public override int StateUpdate()
        {

            if (OVRInput.GetDown(OVRInput.RawButton.A) || OVRInput.GetDown(OVRInput.RawButton.X) || Input.GetMouseButtonDown(1))
            {   
                return (int)MasterStateController.StateType.SetUp;
            }

            return (int)StateType;
        }
    }

}
