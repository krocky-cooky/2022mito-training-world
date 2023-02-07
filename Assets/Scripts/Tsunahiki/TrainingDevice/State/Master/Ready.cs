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
    public class Ready : MasterStateBase
    {
        private int _countDownSeconds = 3;
        private bool _gameStart = false;
        private int restSeconds = 4;
        private bool _duringCoroutine = false;


        public override void OnEnter() 
        {
            restore();  
            stateController.master.addLog("Ready");
            stateController.master.resetTurnip();

        }

        public override void OnExit() 
        {
            _duringCoroutine = false;
            _gameStart = false;

        }

        public override int StateUpdate()
        {
            RemoteTsunahikiDataFormat opponentData = stateController.coordinator.getOpponentData();
            if(opponentData.stateId == (int)MasterStateController.StateType.Ready || stateController.testMode)
            {
                if(!_duringCoroutine)
                    StartCoroutine(DecideSuperiorityCoroutine());
            }

            if(_gameStart)
            {
                int nextState = (int)MasterStateController.StateType.Calibration;
                stateController.coordinator.communicationData.stateId = nextState;
                return nextState;
            }
            

            
            return (int)StateType;
        }

        //カウントダウン用コルーチン
        private IEnumerator GameStartCountdownCoroutine()
        {
            _duringCoroutine = true;
            for(int i = 0;i < _countDownSeconds; ++i) 
            {
                //画面表示する秒数
                restSeconds = _countDownSeconds - i;
                DynamicTextManager.CreateText(stateController.countDownTextPosition,restSeconds.ToString(),stateController.countDownTextData);
                yield return new WaitForSeconds(1);
            }
            DynamicTextManager.CreateText(stateController.countDownTextPosition, "Start !!", stateController.countDownTextData);
            _gameStart = true;
        }

        //優劣決定用コルーチン。この関数が終了後カウントダウンが開始する
        private IEnumerator DecideSuperiorityCoroutine()
        {
            //ルーレットみたいな描写入れたいからコルーチンにしてます
            //現状はただ乱数でどっちか優勢か決めるだけ
            stateController.master.decideSuperiority();
            StartCoroutine(GameStartCountdownCoroutine());
            yield break;
        }

  

    }
}