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

    // 釣り糸の先端
    public Transform ropeRelayBelowHandleTransform;

    // マスターのコントローラー
    public MasterStateController masterStateController;

    // ロープの固定位置
    public Vector3 fixedPosition;
    public Quaternion fixedRotation;

    // ハンドルの位置
    public Transform centerOfHandle;

    // 釣り中のロープの長さ
    public float ropeLengthDuringFishing;
    // 釣り中以外のロープの長さ
    public float ropeLengthWhenNotFishing;

    // 魚
    public Fish fish;

    // ロープのカラー
    public Color targetRopeColor;

    // ルアーの落下時間
    public float lureDropTime;

    // タイムカウント
    public float time;

    // Start is called before the first frame update
    void Start()
    {
        ropeStateController.Initialize((int)RopeStateController.StateType.FollowsHandle);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        ropeStateController.UpdateSequence();
        Debug.Log("Rope State is "+ropeStateController.stateDic[ropeStateController.CurrentState].GetType());
    }
}
