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
        public override void OnEnter()
        {
            Debug.Log("Fixed");
            rope.fixedPosition = new Vector3(rope.centerOfHandle.position.x,rope.centerOfHandle.position.y - rope.ropeLengthDuringFishing, rope.centerOfHandle.position.z);
            rope.fixedRotation = rope.masterStateController.ropeRelayBelowHandle.transform.rotation;
        }

        public override void OnExit()
        {

        }

        public override int StateUpdate()
        {
            rope.ropeRelayBelowHandleTransform.position = rope.fixedPosition;
            rope.ropeRelayBelowHandleTransform.rotation = rope.fixedRotation;

            if ((int)rope.masterStateController.CurrentState == (int)MasterStateController.StateType.DuringFishing_FishOnTheHook){
                return (int)RopeStateController.StateType.FollowsFish;
            }

            if ((int)rope.masterStateController.CurrentState == (int)MasterStateController.StateType.BeforeFishing){
                return (int)RopeStateController.StateType.FollowsFish;
            }


            return (int)StateType;
        }
    }

}
