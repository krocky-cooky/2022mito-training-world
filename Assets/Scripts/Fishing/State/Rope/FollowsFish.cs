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

    public class FollowsFish : RopeStateBase
    {

        public override void OnEnter()
        {
            Debug.Log("FollowsFish");
        }

        public override void OnExit()
        {
        }

        public override int StateUpdate()
        {
            ropeStateController.ropeRelayBelowHandleTransform.position = ropeStateController.fish.transform.position;
            ropeStateController.ropeRelayBelowHandleTransform.rotation = ropeStateController.fish.transform.rotation;

            if (((int)ropeStateController.masterStateController.CurrentState == (int)MasterStateController.StateType.AfterFishing) || ((int)ropeStateController.masterStateController.CurrentState == (int)MasterStateController.StateType.BeforeFishing)){
                return (int)RopeStateController.StateType.FollowsHandle;
            }
            if ((int)ropeStateController.masterStateController.CurrentState == (int)MasterStateController.StateType.DuringFishing_Wait){
                return (int)RopeStateController.StateType.Fixed;
            }
            
            return (int)StateType;
        }
    }
}
