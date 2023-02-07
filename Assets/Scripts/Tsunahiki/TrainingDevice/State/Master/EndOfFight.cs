using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;
using communication;
using tsunahiki.game;
using tsunahiki.trainingDevice.stateController;

namespace tsunahiki.trainingDevice.state 
{
    public class EndOfFight : MasterStateBase 
    {
        private bool _won;
        public override void OnEnter() 
        {
            Debug.Log("FightEndState");
            stateController.master.addLog("FightEnd");
            
            _won = stateController.master.currentWinner == TrainingDeviceType.TrainingDevice;
            string message = _won ? "you win !!" : "you lost";
            DynamicTextManager.CreateText(stateController.countDownTextPosition, message, stateController.countDownTextData);
            Debug.Log(_won ? "you win !!" : "you lost");
        }

        public override void OnExit() 
        {}

        public override int StateUpdate() 
        {

            if(OVRInput.GetDown(stateController.buttonAllotment.Ready) || (stateController.testMode && Input.GetMouseButton(1))) 
            {
                int nextState = (int)MasterStateController.StateType.Ready;
                stateController.coordinator.communicationData.stateId = nextState;
                return nextState;
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
    }
}