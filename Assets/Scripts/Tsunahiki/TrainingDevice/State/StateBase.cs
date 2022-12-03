using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using tsunahiki.game;
using tsunahiki.trainingDevice.stateController;

namespace tsunahiki.trainingDevice.state 
{
    public abstract class StateBase : MonoBehaviour
    {
        protected MasterStateController controller;

        protected int StateType { set; get; }

        

        public virtual void Initialize(int stateType)
        {
            StateType = stateType;
            controller = GetComponent<MasterStateController>();
        }

        public abstract void OnEnter();

        public abstract void OnExit();

        public abstract int StateUpdate();
        
    }
}