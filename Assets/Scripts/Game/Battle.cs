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
        public GameState state = GameState.Idle;

        // Start is called before the first frame update
        void Start()
        {
            gameMaster = transform.parent.gameObject.GetComponent<Master>();
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
                }
            }
        }

        

       
    }
}

