using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using Fishing.Game;
using Fishing.State;
using Fishing.StateController;


namespace Fishing.State
{

    public class DuringFishing_Wait : StateBase
    {
        public override void OnEnter()
        {
            Debug.Log("DuringFish_Wait");
        }

        public override void OnExit()
        {
        }

        public override int StateUpdate()
        {

            return (int)StateType;
        }
    }

}
