using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using tsunahiki.game;
using tsunahiki.state;
using tsunahiki.stateController;



namespace tsunahiki.state
{

    public class GetReady : MasterStateBase
    {

        public override void OnEnter()
        {
            Debug.Log("Set Up");
        }

        public override void OnExit()
        {
        }

        public override int StateUpdate()
        {

            if (OVRInput.GetDown(OVRInput.RawButton.X) || Input.GetMouseButtonDown(1))
            {   
                return (int)MasterStateController.StateType.GetReady;
            }

            return (int)StateType;
        }
    }

}
