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
        }

        public override void OnExit()
        {
        }

        public override int StateUpdate()
        {
            ropeStateController.ropeRelayBelowHandleTransform.position = ropeStateController.fixedPosition;
            ropeStateController.ropeRelayBelowHandleTransform.rotation = ropeStateController.fixedRotation;

            if ((int)ropeStateController.masterStateController.CurrentState == (int)MasterStateController.StateType.DuringFishing_FishOnTheHook){
                return (int)RopeStateController.StateType.FollowsFish;
            }

            return (int)StateType;
        }
    }

}
