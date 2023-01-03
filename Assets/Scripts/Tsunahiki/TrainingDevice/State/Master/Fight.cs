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
            stateController.master.addLog("Fight");
            _cubeStartPosition = stateController.centerCube.transform.position;
            _opponentHandleStartPosition = stateController.opponentHandle.transform.position;
        }



        public override void OnExit()
        {
            stateController.master.resetWind();
        }

        public override int StateUpdate()
        {

            
            {
                //cubeの位置をコントローラの位置に合わせて動かす
                Vector3 cubePos = _cubeStartPosition;
                float controllerPositionFromCenter = stateController.trainingDevice.currentAbsPosition - (stateController.trainingDevice.maxAbsPosition + stateController.trainingDevice.minAbsPosition)/2;
                cubePos.z += controllerPositionFromCenter;
                stateController.centerCube.transform.position = cubePos;

                //対戦相手側を自身のコントローラ＋握力系の動きに合わせて動かす
                float normalizedForceGaugePos = stateController.coordinator.getOpponentValue();
                Vector3 opponentHandlePos = _opponentHandleStartPosition;
                opponentHandlePos.z -= (normalizedForceGaugePos - 0.5f)*stateController.opponentMotionAmplitude;
                opponentHandlePos.z += controllerPositionFromCenter;
                stateController.opponentHandle.transform.position = opponentHandlePos;

            }

            {
                //勝敗状況によるエフェクト
                //要素：対戦相手アバターの表情
                if(stateController.master.fightCondition == TrainingDeviceType.TrainingDevice) //筋トレデバイス優勢
                {
                    stateController.master.setLosingAvatarExpression();
                    

                }
                else if(stateController.master.fightCondition == TrainingDeviceType.ForceGauge) //握力系優勢
                {
                    stateController.master.setWinningAvatarExpression();
                }
                else 
                {

                }
            }

            {
                //ゲームの優劣によるエフェクト
                //要素：風
                if(stateController.master.superiority == TrainingDeviceType.TrainingDevice)
                {
                    stateController.master.setFavorableWind();
                }
                else if(stateController.master.superiority == TrainingDeviceType.ForceGauge)
                {
                    stateController.master.setAdverseWind();
                }
                else
                {

                }
            }


            {
                //握力系トルクの反映
                float opponentValue = stateController.coordinator.getOpponentValue();
                float sendingTorque = opponentValue * stateController.master.gripStrengthMultiplier;
                _fromLastTorqueUpdated += Time.deltaTime;
                if(_fromLastTorqueUpdated > stateController.torqueSendingInterval)
                {
                    UpdateTorque(sendingTorque);
                    _fromLastTorqueUpdated = 0.0f;
                }
            }

            {
                //勝敗がついたとき
                if(false)
                {
                    if(stateController.master.updateResult())
                    {
                        int nextState = (int)MasterStateController.StateType.GameSet;
                        stateController.coordinator.communicationData.stateId = nextState;
                        return nextState;
                    }
                    else
                    {
                        int nextState = (int)MasterStateController.StateType.EndOfFight;
                        stateController.coordinator.communicationData.stateId = nextState;
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
            stateController.communicationInterface.sendData(data);

            Debug.Log($"Torque {torque} has sent to Training Device");
        }
        
    }
}