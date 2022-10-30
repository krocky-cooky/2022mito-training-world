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

namespace Fishing.StateController
{
    public class RopeStateController : StateControllerBase
    {

        // 釣り糸の先端
        public Transform ropeRelayBelowHandleTransform;

        // マスターのコントローラー
        public MasterStateController masterStateController;

        // ロープの固定位置
        public Vector3 fixedPosition;
        public Quaternion fixedRotation;

        // ハンドルの位置
        public Transform centerOfHandle;

        // ロープの長さ
        public float ropeLength;

        // 魚
        public Fish fish;

        // ロープのカラー
        public Color targetRopeColor;

        public enum StateType
        {
            FollowsHandle,
            Fixed,
            FollowsFish,
        }

        // 初期化処理
        public override void Initialize(int initializeStateType)
        {
            // ここ自動化したいな
            stateDic[(int)StateType.FollowsHandle] = gameObject.AddComponent<FollowsHandle>();
            stateDic[(int)StateType.FollowsHandle].Initialize((int)StateType.FollowsHandle);

            stateDic[(int)StateType.Fixed] = gameObject.AddComponent<Fixed>();
            stateDic[(int)StateType.Fixed].Initialize((int)StateType.Fixed);
            
            stateDic[(int)StateType.FollowsFish] = gameObject.AddComponent<FollowsFish>();
            stateDic[(int)StateType.FollowsFish].Initialize((int)StateType.FollowsFish);

            CurrentState = initializeStateType;
            stateDic[CurrentState].OnEnter();

        }
    }
}
