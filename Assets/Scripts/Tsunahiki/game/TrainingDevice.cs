using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using communication;

namespace tsunahiki.game
{
    
    public class TrainingDevice : MonoBehaviour
    {
        // 入力方法の切り替え
        public enum InputInterface
        {
            Mouse,
            OculusController,
        }
        public InputInterface inputInterface;

        // ハンドル等の最大位置と最小位置
        public float maxAbsPosition = 0.0f;
        public float minAbsPosition = 0.0f;

        // ハンドル等の絶対位置
        public float currentAbsPosition = 0.0f;

        // ハンドル等がストローク全体の中で相対的にどの位置にあるかを返す
        // 例えば、ハンドルの最低位置が10cm、最高位置が110cmで、現在地が20cmなら、ストローク全体100cmの中で下から10cmのところにあるので、0.1である
        public float currentNormalizedPosition = 0.0f;

        [SerializeField]
        private GameObject rightControllerAnchor;
        [SerializeField]
        private MainCommunicationInterface communicationInterface;

        void Start(){
        }

        void Update(){
            if (inputInterface == InputInterface.Mouse){
                currentAbsPosition = Input.mousePosition.y;
            }else{
                currentAbsPosition = rightControllerAnchor.transform.position.y;
            }

            currentNormalizedPosition = Mathf.Clamp01((currentAbsPosition - minAbsPosition) / (maxAbsPosition - minAbsPosition));
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