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

        // ロープのカラーオブジェクト
        // ここに上記を代入する
        private Color ropeColorProperty;

        public override void OnEnter()
        {
            Debug.Log("FollowsFish");

            // ropeColorProperty = GameObject.Find("fishLine(Clone)").GetComponent<Renderer>().material.color;
            // GameObject.Find("fishLine(Clone)").GetComponent<Renderer>().material.color = new Color32(255, 0, 0, 1);
        }

        public override void OnExit()
        {
            GameObject.Find("fishLine(Clone)").GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 1);
        }

        public override int StateUpdate()
        {
            // ropeColorProperty = ropeStateController.targetRopeColor;
            // ropeColorProperty = Color.red;
            GameObject.Find("fishLine(Clone)").GetComponent<Renderer>().material.color = ropeStateController.targetRopeColor;

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
