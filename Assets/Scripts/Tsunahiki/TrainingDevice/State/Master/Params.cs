using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;
using communication;
using tsunahiki.game;
using tsunahiki.trainingDevice.stateController;

namespace tsunahiki.trainingDevice.state 
{
    public class Params : MasterStateBase 
    {
        public override void OnEnter()
        {}

        public override void OnExit()
        {}

        public override int StateUpdate()
        {return (int)StateType;}
    }
}