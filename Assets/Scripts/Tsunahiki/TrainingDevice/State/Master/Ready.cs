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
        private int _countDownSeconds = 4;
        private bool _gameStart = false;


        public override void OnEnter() 
        {
            restore();  
            stateController.master.addLog("Ready");

        }

        public override void OnExit() 
        {

        }

        public override int StateUpdate()
        {
            RemoteTsunahikiDataFormat opponentData = stateController.coordinator.getOpponentData();
            if(opponentData.stateId == (int)MasterStateController.StateType.Ready)
            {
                StartCoroutine(DecideSuperiorityCoroutine());

            }

            if(_gameStart)
            {
                int nextState = (int)MasterStateController.StateType.Fight;
                stateController.coordinator.communicationData.stateId = nextState;
                return nextState;
            }
            

            
            return (int)StateType;
        }

        //カウントダウン用コルーチン
        private IEnumerator GameStartCountdownCoroutine()
        {
            for(int i = 0;i < _countDownSeconds; ++i) 
            {
                //画面表示する秒数
                int restSeconds = _countDownSeconds - i - 1;

                yield return new WaitForSeconds(1);
            }
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