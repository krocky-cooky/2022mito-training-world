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

            master.frontViewUiText.text = "Press X button to start";

            master.sendingTorque = 0.0f;

            master.tensionSliderGameObject.SetActive(false);
        }

        public override void OnExit()
        {
        }

        public override int StateUpdate()
        {
            // トルクを負荷ゲージで表示
            // トルクの値の約4.0倍が負荷(kg)
            master.tensionSlider.value = master.sendingTorque * 4.0f;

            if (OVRInput.GetDown(OVRInput.RawButton.X) || Input.GetMouseButtonDown(1))
            {
                Debug.Log("move to fishing");
                
                return (int)MasterStateController.StateType.DuringFishing_Wait;
            }

            return (int)StateType;
        }
    }

}
