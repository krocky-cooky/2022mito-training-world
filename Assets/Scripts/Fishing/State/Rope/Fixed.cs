using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using Fishing.Game;
using Fishing.State;
using Fishing.StateController;
using Fishing.Object;


namespace Fishing.State
{

    public class Fixed : RopeStateBase
    {
        float _initTime = 0.0f;
        Vector3 _initPosition;

        public override void OnEnter()
        {
            Debug.Log("Fixed");
            rope.fixedPosition = new Vector3(rope.centerOfHandle.position.x,rope.centerOfHandle.position.y - rope.ropeLengthDuringFishing, rope.centerOfHandle.position.z);
            rope.fixedRotation = rope.master.ropeRelayBelowHandle.transform.rotation;

            _initTime = rope.time;
            _initPosition = rope.ropeRelayBelowHandleTransform.position;
        }

        public override void OnExit()
        {

        }

        public override int StateUpdate()
        {
            rope.ropeRelayBelowHandleTransform.rotation = rope.fixedRotation;

            // ルアーを垂らしていく
            if ((rope.time - _initTime) < rope.lureDropTime){
                rope.ropeRelayBelowHandleTransform.position = _initPosition + (rope.fixedPosition - _initPosition) * (rope.time - _initTime) / rope.lureDropTime;
            }else{
                rope.ropeRelayBelowHandleTransform.position = rope.fixedPosition;
            }


            if ((int)rope.master.masterStateController.CurrentState == (int)MasterStateController.StateType.DuringFishing_FishOnTheHook){
                return (int)RopeStateController.StateType.FollowsFish;
            }

            if ((int)rope.master.masterStateController.CurrentState == (int)MasterStateController.StateType.BeforeFishing){
                return (int)RopeStateController.StateType.FollowsFish;
            }


            return (int)StateType;
        }
    }

}
