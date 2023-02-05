using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using tsunahiki.game;
using tsunahiki.trainingDevice.stateController;
using TRAVE;


namespace tsunahiki.trainingDevice.state 
{
    public class Calibration : MasterStateBase
    {
        private TRAVEDevice _device = TRAVEDevice.GetDevice();
        private float _maxTorque;
        private bool _endRegistration = false;

        public override void OnEnter() 
        {
            _device.SetSpeedMode(0.0f);
            _device.Apply();
            StartCoroutine(CalibrationCoroutine());
            stateController.master.addLog("Calibration");
        }

        public override void OnExit() 
        {
            _endRegistration = false;
        }

        public override int StateUpdate()
        {
            _maxTorque = Mathf.Max(_maxTorque, _device.torque);
            if(_endRegistration)
            {
                stateController.master.setDefaultGripStrengthMultiplier(_maxTorque);
                int nextState = (int)MasterStateController.StateType.Fight;
                stateController.coordinator.communicationData.stateId = nextState;
                return nextState;
            }
            
            return (int)StateType;
        }

        private IEnumerator CalibrationCoroutine()
        {
            yield return new WaitForSeconds(2);
            _endRegistration = true;

        }

        
        
    }
}