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
    public abstract class StateBase : MonoBehaviour
    {
        // ステートコントローラー
        protected StateControllerBase controller;

        // 登録されたステートタイプ
        protected int StateType { set; get; }

        // 初期化処理
        public virtual void Initialize(int stateType)
        {
            StateType = stateType;
            controller = GetComponent<StateControllerBase>();
        }

        // 入場処理
        public abstract void OnEnter();

        // 退場処理
        public abstract void OnExit();

        /// 更新処理
        public abstract int StateUpdate();
    }
}