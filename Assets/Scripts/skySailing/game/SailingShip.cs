using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using skySailing.game;


namespace skySailing.game
{
    
    public class SailingShip: MonoBehaviour
    {
        public Transform pillarTransform;
        public float maxXRotationOfPillar;
        public float minXRotationOfPillar;

        [SerializeField]
        private TrainingDevice trainingDevice;
        [SerializeField]
        private Master gameMaster;

        private Vector3 _moveVector;
        private Transform _initShipTransform;
        private float _time = 0.0f;
        private float _previousCheckPointTime = 0.0f;
        

        void Start()
        {
            _initShipTransform = transform;
        }

        void Update()
        {
            // 柱を回転
            changePillarRotation(trainingDevice.currentRelativePosition);

            //船を動かす
            move(gameMaster.windSpeed);

            _time += Time.deltaTime;
        }

        // 柱の角度を変化
        // x軸周りの角度の相対位置を受け取ると、それに合わせて柱の角度変更
        // 例えばx軸周りの角度の可動範囲が-30~30で、相対位置で0.3を受け取ったら、-30+60*0.3で-12に帆の角度が変更
        public void changePillarRotation(float relativeXRotation){
            float nextAngle = minXRotationOfPillar + (maxXRotationOfPillar - minXRotationOfPillar) * relativeXRotation;
            float rotationAngle = nextAngle - pillarTransform.eulerAngles.x;
            if (minXRotationOfPillar < nextAngle & nextAngle < maxXRotationOfPillar){
                pillarTransform.Rotate(rotationAngle, 0.0f, 0.0f);
            }
            Debug.Log("xRotation is "+(pillarTransform.eulerAngles.x).ToString());
            Debug.Log("next angular is " + (nextAngle).ToString());
        }

        // 帆の傾きに合わせて船を動かす
        public void move(float windSpeed){
            _moveVector =  Quaternion.Euler(pillarTransform.eulerAngles.x, pillarTransform.eulerAngles.y, pillarTransform.eulerAngles.z) * new Vector3(0.0f, 0.0f, -windSpeed);
            _moveVector = Time.deltaTime * _moveVector;
            transform.position += _moveVector;
            Debug.Log("_moveVector is " + _moveVector.ToString());
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "start"){
                gameMaster.duringRace = true;
                Debug.Log("race start");
            }
            if (other.gameObject.tag == "goal"){
                gameMaster.duringRace = false;
                Debug.Log("race end");
            }
            if (other.gameObject.tag == "checkPoint"){
                if ((_time - _previousCheckPointTime) > 0.5){
                    gameMaster.numberOfCheckpointsPassed += 1;
                    _previousCheckPointTime = _time;
                    Debug.Log("passing the check point");
                }
            }
        }

        public void returnToInitTransform(){
            // transform = _initShipTransform;
        }
    }
}