using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tsunahiki.game;
using tsunahiki.state;
using tsunahiki.stateController;



namespace tsunahiki.game
{
    public class MagicHand : MonoBehaviour
    {
        // shaftの正規化された長さ
        public float normalizedShaftLength = 0.0f;

        // 各パーツ
        [SerializeField]
        private GameObject caseOfHandDyameter;
        [SerializeField]
        private GameObject grip;
        [SerializeField]
        private GameObject leftArm;
        [SerializeField]
        private GameObject rightArm;
        [SerializeField]
        private GameObject joint;

        // 各パーツの可動域
        [SerializeField]
        public float maxGripPosition;
        [SerializeField]
        public float minGripPosition;
        [SerializeField]
        public float maxArmAngle;
        [SerializeField]
        public float minArmAngle;
        [SerializeField]
        public float maxShaftLength;
        [SerializeField]
        public float minShaftLength;

        // 各パーツの初期位置
        private Vector3 _initGripPosition;
        private Vector3 _initJointPosition;
        private Vector3 _initLeftArmPosition;
        private Vector3 _initRightArmPosition;

        // Start is called before the first frame update
        void Start()
        {
            _initGripPosition = grip.transform.position;
            _initJointPosition = joint.transform.position;
            _initLeftArmPosition = leftArm.transform.position;
            _initRightArmPosition = rightArm.transform.position;
        }

        // Update is called once per frame
        void Update() 
        {
            // 正規化チェック
            normalizedShaftLength = Mathf.Clamp01(normalizedShaftLength);

            // グリップの位置を変更
            grip.transform.position = new Vector3(_initGripPosition.x, _initGripPosition.y, _initGripPosition.z + getAbsPosition(maxGripPosition, minGripPosition, normalizedShaftLength));

            // ジョイントの位置を変更
            joint.transform.position = new Vector3(_initJointPosition.x, _initJointPosition.y, _initJointPosition.z - getAbsPosition(maxShaftLength, minShaftLength, normalizedShaftLength));

            // アームの角度を変更
            leftArm.transform.localEulerAngles = new Vector3(0.0f, 0.0f, getAbsPosition(maxArmAngle, minArmAngle, normalizedShaftLength));
            rightArm.transform.localEulerAngles = new Vector3(0.0f, 0.0f, -getAbsPosition(maxArmAngle, minArmAngle, normalizedShaftLength));
        }

        // 最大値と最小値と正規化位置を受け取ると、絶対値を返す関数
        float getAbsPosition(float maxPosition, float minPosition, float normalizedPosition){
            return minPosition + (maxPosition - minPosition) * normalizedPosition;
        }
    }
}