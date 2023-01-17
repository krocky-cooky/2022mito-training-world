using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using tsunahiki.game;
using tsunahiki.trainingDevice.stateController;


namespace tsunahiki.trainingDevice.state 
{
    public class SetUp : MasterStateBase
    {

        public override void OnEnter() 
        {
            restore();
            Debug.Log("set up start");
            stateController.master.addLog("SetUp");
        }

        public override void OnExit() 
        {

        }

        public override int StateUpdate()
        {
            // if(!stateController.maxTorqueRegistered)
            // {
            //     //最大トルクの記録
            //     ReceivingDataFormat data = stateController.communicationInterface.getReceivedData();
            //     stateController.maxTorque = Mathf.Max(stateController.maxTorque,data.trq);
            // }


            if(OVRInput.GetDown(stateController.buttonAllotment.Ready) || Input.GetMouseButton(0))
            {

                int nextState = (int)MasterStateController.StateType.Ready;
                stateController.coordinator.communicationData.stateId = nextState;
                Debug.Log(stateController.coordinator.communicationData.stateId);
                return nextState;

            }
            else if(OVRInput.GetDown(stateController.buttonAllotment.TorqueRegistered))
            {
                fixMaxTorque(!stateController.maxTorqueRegistered,true);
            }
            else if(OVRInput.GetDown(stateController.buttonAllotment.ReelWire))
            {
                reelWire();
            }
            else if(OVRInput.GetUp(stateController.buttonAllotment.ReelWire))
            {
                restore();
            }
            
            return (int)StateType;
        }

        //トルクを決定するもしくはリセットして変更を再開する
        private void fixMaxTorque(bool fix,bool reset)
        {
            stateController.maxTorqueRegistered = fix;

            if(reset)stateController.maxTorque = 0.0f;
        }
        
    }
}