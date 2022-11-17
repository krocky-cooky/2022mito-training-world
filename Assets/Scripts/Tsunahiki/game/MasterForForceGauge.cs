using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using communication;
using tsunahiki.game;
using tsunahiki.state;
using tsunahiki.stateController;

namespace tsunahiki.game
{
    public class MasterForForceGauge : MonoBehaviour
    {   
        const int MAX_LOG_LINES = 10;

        public Queue<string> viewerTextQueue = new Queue<string>();
        public Text frontViewUI;

        public float moveParameter;

        public float time = 0.0f;
        public MasterStateController masterStateController;

        // デバッグ用で、直接相手のデータを手動入力する
        [SerializeField]
        private float manualNormalizedData = 0.0f;
        [SerializeField]
        private TrainingDeviceType manualDeviceInterface = TrainingDeviceType.ForceGauge;
        [SerializeField]
        private TsunahikiStateType manualStateId = TsunahikiStateType.SetUp;
        [SerializeField]
        private TrainingDeviceType manualSuperiority = TrainingDeviceType.ForceGauge;
        [SerializeField]
        private TrainingDeviceType manualLatestWinner = TrainingDeviceType.Nothing;
        [SerializeField]
        private float manualTimeCount = 0.0f;

        // 相手からの通信内容の手動入力するかどうかの切り替え
        [SerializeField]
        private bool opponentDataIsInputedManually = false;


        // トルク÷握力計の値
        public float gripStrengthMultiplier;

        [SerializeField]
        private GameObject viewerObject;
        [SerializeField]
        private GameObject _centerCube;
        [SerializeField]
        public RemoteCoordinator _coordinator;

        // 対戦相手のデータ
        [System.NonSerialized]
        public RemoteTsunahikiDataFormat opponentData;

        // 勝敗の回数
        [System.NonSerialized]
        public int victoryCounts = 0;
        [System.NonSerialized]
        public int defeatCounts = 0;
        [System.NonSerialized]
        public int drawCounts = 0;

        [System.NonSerialized]
        public int myDeviceId = (int)TrainingDeviceType.ForceGauge;


        private Vector3 cubeStartPosition;



        // Start is called before the first frame update
        void Start()
        {
            cubeStartPosition = _centerCube.transform.position;
            masterStateController.Initialize((int)MasterStateController.StateType.SetUp);
        }

        // Update is called once per frame
        void Update()
        {
            time += Time.deltaTime;

            masterStateController.UpdateSequence();
            Debug.Log("Master State is "+masterStateController.stateDic[masterStateController.CurrentState].GetType());

            
            // Debug.Log("stateId is " + ((int)opponentData.stateId).ToString());

            // 相手のデータの手動入力
            if(opponentDataIsInputedManually){
                Debug.Log("manuallyInputOpponentData is true");
                opponentData = new RemoteTsunahikiDataFormat();
                manuallyInputOpponentData();
            }else{
                opponentData = _coordinator.getOpponentData();
            }
           
            {
                float normalizedDevicePos = _coordinator.getOpponentValue();
                Vector3 cubePos = cubeStartPosition;
                cubePos.z += (normalizedDevicePos - 0.5f)*moveParameter;
                _centerCube.transform.position = cubePos;
                Debug.Log("normalizedDevicePos is " + normalizedDevicePos.ToString());
            }
            

        }

        // 相手のデータの手動入力
        void manuallyInputOpponentData(){
            opponentData.normalizedData = manualNormalizedData;
            opponentData.deviceInterface = (int)manualDeviceInterface;
            opponentData.stateId = (int)manualStateId;
            opponentData.superiority = (int)manualSuperiority;
            opponentData.latestWinner = (int)manualLatestWinner;
            opponentData.timeCount = manualTimeCount;
            Debug.Log("manuallyInputOpponentData run");
        }

    }
}
