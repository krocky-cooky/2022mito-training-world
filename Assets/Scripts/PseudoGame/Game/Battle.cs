using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace game
{
    
    public class Battle : MonoBehaviour
    {
        private Master gameMaster;
        private string registerStartCommand = "s0";
        private string endCommand = "end";
        private Vector3 battleStartPosition;
        private Vector3 opponentStartPosition;

        public GameState state = GameState.Idle;

        [SerializeField]
        private GameObject rightControllerAnchor;

        [SerializeField]
        private GameObject opponentTrainingBar;

        // Start is called before the first frame update
        void Start()
        {
            gameMaster = transform.parent.gameObject.GetComponent<Master>();
            if(opponentTrainingBar != null)
            {
                opponentStartPosition = opponentTrainingBar.transform.position;
                //battleStartTransform = opponentStartTransform;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(OVRInput.GetDown(OVRInput.RawButton.X))
            {
                if(gameMaster.state == GameState.Idle)
                {
                    gameMaster.startRegisterMode();
                }
                
            }
            if(OVRInput.GetDown(OVRInput.RawButton.Y))
            {
                if(gameMaster.state == GameState.Idle)
                {
                    gameMaster.startSendTorqueMode();
                    battleStartPosition = rightControllerAnchor.transform.position;
                }
            }

            //対戦相手のアバターのトレーニングバーの位置を上下させる
            if(opponentTrainingBar != null )//&& gameMaster.state == GameState.SendTorque)
            {
                float ygap = rightControllerAnchor.transform.position.y - battleStartPosition.y;
                ygap *= 0.8f;
                Vector3 pos = opponentTrainingBar.transform.position;
                pos.y = opponentStartPosition.y - ygap;
                opponentTrainingBar.transform.position = pos;
            }
        }

        

       
    }
}

