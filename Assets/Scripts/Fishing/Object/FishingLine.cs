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


public class FishingLine : MonoBehaviour
{
    [SerializeField]
    private RopeStateController ropeStateController;

    // Start is called before the first frame update
    void Start()
    {
        ropeStateController.Initialize((int)RopeStateController.StateType.FollowsHandle);
    }

    // Update is called once per frame
    void Update()
    {
        ropeStateController.UpdateSequence();
        Debug.Log("Rope State is "+ropeStateController.stateDic[ropeStateController.CurrentState].GetType());
    }
}
