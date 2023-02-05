using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using communication;
using tsunahiki.game;
using tsunahiki.forceGauge.state;
using tsunahiki.forceGauge.stateController;
using tsunahiki.forceGauge;


namespace tsunahiki.game
{
    public class MasterForForceGauge : MonoBehaviour
    {   
        const int MAX_LOG_LINES = 10;

        public Queue<string> viewerTextQueue = new Queue<string>();
        public Text frontViewUI;
        public Text UIFollowingEyes;

        public float moveParameter;

        public float time = 0.0f;
        public MasterStateController masterStateController;

        // 中央のフレア
        public GameObject centerFlare;

        public BeamController myBeam;

        public OpponentPlayer OpponentPlayer;

        public Vector3 cubeStartPosition;

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
        public RemoteCoordinator _coordinator;
        [SerializeField]
        public ForceGauge myForceGauge;


        // 自他の位置
        public Transform myTransform;
        public Transform opponentTransform;

        // 決着時のフレアの移動時間
        public float flareMovingTime;

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


        // Start is called before the first frame update
        void Start()
        {
            
            masterStateController.Initialize((int)MasterStateController.StateType.SetUp);
            cubeStartPosition = centerFlare.transform.position;
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

            
            // 自分のビームの強度に、握力計の正規化値を代入
            myBeam.normalizedScale = myForceGauge.outputPosition;
           
            {
                // 通信をしてないときはここでエラーが出てUpdate()処理が止まる
                // float normalizedDevicePos = _coordinator.getOpponentValue();
                // float normalizedDevicePos = opponentData.normalizedData;
                // Vector3 cubePos = cubeStartPosition;
                // cubePos.z += (normalizedDevicePos - 0.5f)*moveParameter;
                // centerFlare.transform.position = cubePos;
                // Debug.Log("normalizedDevicePos is " + normalizedDevicePos.ToString());
            }

            Debug.Log("opponent normalized data is " + opponentData.normalizedData.ToString());

        }

        // 相手のデータの手動入力
        void manuallyInputOpponentData(){
            opponentData.normalizedData = Mathf.Clamp01(manualNormalizedData);
            opponentData.deviceInterface = (int)manualDeviceInterface;
            opponentData.stateId = (int)manualStateId;
            opponentData.superiority = (int)manualSuperiority;
            opponentData.latestWinner = (int)manualLatestWinner;
            opponentData.timeCount = manualTimeCount;
            Debug.Log("manuallyInputOpponentData run");
        }

        // UIに表示
        // 一定時間後に削除
        public IEnumerator DisplayOnUI(Text textUI, string message, float displayingTime)
        {
            // メッセージ表示
            textUI.text = message;

            // 一定時間待つ
            yield return new WaitForSeconds(displayingTime);

            // メッセージ削除
            textUI.text = "";
        }
    }
}
