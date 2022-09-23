using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game;


namespace skySailing.game
{
    
    public class TrainingDevice : MonoBehaviour
    {
        // ハンドル等の最大位置と最小位置
        public float maxAbsPosition = 0.0f;
        public float minAbsPosition = 0.0f;

        // ハンドル等の絶対位置
        public float currentAbsPosition = 0.0f;

        // ハンドル等がストローク全体の中で相対的にどの位置にあるかを返す
        // 例えば、ハンドルの最低位置が10cm、最高位置が110cmで、現在地が20cmなら、ストローク全体100cmの中で下から10cmのところにあるので、0.1である
        public float currentRelativePosition = 0.0f;

        [SerializeField]
        private GameObject rightControllerAnchor;
        [SerializeField]
        private SailingShip SailingShip;

        private webSocketClient _socketClient;


        void Start(){
            // maxAbsPosition = (float)Screen.height;
            _socketClient = GameObject.FindWithTag("webSocketClient").GetComponent<webSocketClient>();

        }

        void Update(){
            // currentAbsPosition = Input.mousePosition.y;
            // currentAbsPosition = _socketClient.integrationAngleForSkySailing;
            currentAbsPosition = rightControllerAnchor.transform.position.y - SailingShip.transform.position.y;
            currentRelativePosition = (currentAbsPosition - minAbsPosition) / (maxAbsPosition - minAbsPosition);
            // マシンのハンドル等のストロークポジション登録
            if(OVRInput.GetDown(OVRInput.RawButton.Y) || Input.GetMouseButtonDown(2))
            {
                minAbsPosition = currentAbsPosition;
                maxAbsPosition = currentAbsPosition;
                Debug.Log("Input.GetMouseButtonDown(2)");
            }
            if(OVRInput.Get(OVRInput.RawButton.Y) || Input.GetMouseButton(2))
            {
                Debug.Log("Input.GetMouseButton(2)");
                if (minAbsPosition > currentAbsPosition){
                    minAbsPosition = currentAbsPosition;
                }
                if (maxAbsPosition < currentAbsPosition){
                    maxAbsPosition = currentAbsPosition;
                }
            }
        }

       
    }
}