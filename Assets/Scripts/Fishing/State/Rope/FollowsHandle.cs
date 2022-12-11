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
            ropeStateController.ropeRelayBelowHandleTransform.position = new Vector3(ropeStateController.centerOfHandle.position.x,ropeStateController.centerOfHandle.position.y - ropeStateController.ropeLengthWhenNotFishing, ropeStateController.centerOfHandle.position.z);
            ropeStateController.ropeRelayBelowHandleTransform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            
            if ((int)ropeStateController.masterStateController.CurrentState == (int)MasterStateController.StateType.DuringFishing_Wait){
                return (int)RopeStateController.StateType.Fixed;
            }

            return (int)StateType;
        }
    }

}
