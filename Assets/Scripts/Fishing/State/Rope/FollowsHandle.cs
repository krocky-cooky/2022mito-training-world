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

    public class FollowsHandle : RopeStateBase
    {

        public override void OnEnter()
        {
            Debug.Log("FollowsHandle");
        }

        public override void OnExit()
        {
        }

        public override int StateUpdate()
        {
            // ropeStateController.ropeRelayBelowHandleTransform.position = new Vector3(ropeStateController.centerOfHandle.position.x, -ropeStateController.ropeLength, ropeStateController.centerOfHandle.position.z);

            // if ((int)ropeStateController.masterStateController.CurrentState == (int)MasterStateController.StateType.DuringFishing_Wait){
            //     ropeStateController.fixedPosition = ropeStateController.masterStateController.ropeRelayBelowHandle.transform.position;
            //     ropeStateController.fixedRotation = ropeStateController.masterStateController.ropeRelayBelowHandle.transform.rotation;
            //     return (int)RopeStateController.StateType.Fixed;
            // }

            ropeStateController.ropeRelayBelowHandleTransform.position = new Vector3(ropeStateController.centerOfHandle.position.x, ropeStateController.centerOfHandle.position.y - ropeStateController.ropeLength, ropeStateController.centerOfHandle.position.z);

            if ((int)ropeStateController.masterStateController.CurrentState == (int)MasterStateController.StateType.DuringFishing_Wait){
                ropeStateController.fixedPosition = ropeStateController.masterStateController.ropeRelayBelowHandle.transform.position;
                // ropeStateController.fixedRotation = ropeStateController.masterStateController.ropeRelayBelowHandle.transform.rotation;
                return (int)RopeStateController.StateType.Fixed;
            }

            return (int)StateType;
        }
    }

}
