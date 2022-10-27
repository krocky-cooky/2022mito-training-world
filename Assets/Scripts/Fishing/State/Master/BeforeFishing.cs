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

    public class BeforeFishing : MasterStateBase
    {

        public override void OnEnter()
        {
            Debug.Log("BeforeFishing");

            masterStateController.frontViewUiText.text = "Press X button to start";

            masterStateController.gameMaster.sendingTorque = 0.0f;
        }

        public override void OnExit()
        {
        }

        public override int StateUpdate()
        {
            if (OVRInput.GetDown(OVRInput.RawButton.X) || Input.GetMouseButtonDown(1))
            {
                Debug.Log("move to fishing");
                masterStateController.LureLandingSound.Play();
                return (int)MasterStateController.StateType.DuringFishing_Wait;
            }

            return (int)StateType;
        }
    }

}
