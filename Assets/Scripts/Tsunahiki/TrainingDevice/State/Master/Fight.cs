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
    public class Fight : MasterStateBase
    {
        private Vector3 _cubeStartPosition;
        private Vector3 _opponentHandleStartPosition;
        private float _fromLastTorqueUpdated = 0.0f;

        public override void OnEnter() 
        {
            Debug.Log("Fight");
            _cubeStartPosition = centerCube.transform.position;
            _opponentHandleStartPosition = opponentHandle.transform.position;
        }



        public override void OnExit()
        {}

        public override int StateUpdate()
        {

            
            {
                //cubeの位置をコントローラの位置に合わせて動かす
                Vector3 cubePos = _cubeStartPosition;
                float controllerPositionFromCenter = trainingDevice.currentAbsPosition - (trainingDevice.maxAbsPosition + trainingDevice.minAbsPosition)/2;
                cubePos.z += controllerPositionFromCenter;
                centerCube.transform.position = cubePos;

                //対戦相手側を自身のコントローラ＋握力系の動きに合わせて動かす
                float normalizedForceGaugePos = coordinator.getOpponentValue();
                Vector3 opponentHandlePos = _opponentHandleStartPosition;
                opponentHandlePos.z -= (normalizedForceGaugePos - 0.5f)*opponentMotionAmplitude;
                opponentHandlePos.z += controllerPositionFromCenter;
                opponentHandle.transform.position = opponentHandlePos;
            }

            {
                //握力系トルクの反映
                float opponentValue = coordinator.getOpponentValue();
                float sendingTorque = opponentValue * master.gripStrengthMultiplier;
                _fromLastTorqueUpdated += Time.deltaTime;
                if(_fromLastTorqueUpdated > torqueSendingInterval)
                {
                    UpdateTorque(sendingTorque);
                    _fromLastTorqueUpdated = 0.0f;
                }
            }

            {
                //勝敗がついたとき
                if(false)
                {
                    if(master.updateResult())
                    {
                        int nextState = (int)MasterStateController.StateType.GameSet;
                        coordinator.communicationData.stateId = nextState;
                        return nextState;
                    }
                    else
                    {
                        int nextState = (int)MasterStateController.StateType.Ready;
                        coordinator.communicationData.stateId = nextState;
                        return nextState;
                    }
                }

            }
            
            return (int)StateType;
        }

        private void UpdateTorque(float torque,float speed = 10.0f)
        {
            SendingDataFormat data = new SendingDataFormat();
            data.setTorque(torque,speed);
            communicationInterface.sendData(data);

            Debug.Log($"Torque {torque} has sent to Training Device");
        }
        
    }
}