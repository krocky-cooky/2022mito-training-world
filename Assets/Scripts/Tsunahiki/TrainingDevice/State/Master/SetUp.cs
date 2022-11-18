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
        }

        public override void OnExit() 
        {

        }

        public override int StateUpdate()
        {
            if(!maxTorqueRegistered){
                //最大トルクの記録
                ReceivingDataFormat data = communicationInterface.getReceivedData();
                maxTorque = Mathf.Max(maxTorque,data.trq);
            }


            if(OVRInput.GetDown(buttonAllotment.Ready))
            {
                if(maxTorqueRegistered)
                {   
                    int nextState = (int)MasterStateController.StateType.Ready;
                    coordinator.communicationData.stateId = nextState;
                    return nextState;
                }
            }
            else if(OVRInput.GetDown(buttonAllotment.TorqueRegistered))
            {
                fixMaxTorque(!maxTorqueRegistered,true);

            }

            
            return (int)StateType;
        }

        //トルクを決定するもしくはリセットして変更を再開する
        private void fixMaxTorque(bool fix,bool reset)
        {
            maxTorqueRegistered = fix;

            if(reset)maxTorque = 0.0f;
        }
        
    }
}