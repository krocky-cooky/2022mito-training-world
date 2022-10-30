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
        public float maxYRotationOfPillar;
        public float minYRotationOfPillar;

        [SerializeField]
        private TrainingDevice trainingDevice;
        [SerializeField]
        private ForceGauge forceGauge;
        [SerializeField]
        private Master gameMaster;
        [SerializeField]
        private AudioSource startSound;
        [SerializeField]
        private AudioSource checkPointSound;
        [SerializeField]
        private AudioSource goalSound;


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
            changePillarRotation(forceGauge.outputPosition, trainingDevice.currentRelativePosition);

            //船を動かす
            move(gameMaster.windSpeed);

            _time += Time.deltaTime;
        }

        // 柱の角度を変化
        public void changePillarRotation(float leftNormalizedPosition, float rightNormalizedPosition){
            // 左側のコントローラー出力位置と右側のコントローラー出力位置の、平均に応じてx軸周りの柱の角度を設定
            float nextXAngle = minXRotationOfPillar + (maxXRotationOfPillar - minXRotationOfPillar) * (rightNormalizedPosition + leftNormalizedPosition) / 2.0f;

            // 左側のコントローラー出力位置と右側のコントローラー出力位置の、差に応じてy軸周りの柱の角度を設定
            float nextYAngle = ((maxYRotationOfPillar + minYRotationOfPillar) / 2.0f) + (maxYRotationOfPillar - minYRotationOfPillar) * (rightNormalizedPosition - leftNormalizedPosition) / 2.0f;

            // 回転させる
            pillarTransform.Rotate(nextXAngle - pillarTransform.eulerAngles.x, nextYAngle - pillarTransform.eulerAngles.y, 0.0f - pillarTransform.eulerAngles.z, Space.World);
            // pillarTransform.Rotate(0.0f, nextYAngle - pillarTransform.eulerAngles.y, 0.0f);
        }

        // 帆の傾きに合わせて船を動かす
        public void move(float windSpeed){
            _moveVector =  Quaternion.Euler(pillarTransform.eulerAngles.x, pillarTransform.eulerAngles.y, pillarTransform.eulerAngles.z) * new Vector3(0.0f, 0.0f, -windSpeed);
            _moveVector = Time.deltaTime * _moveVector;
            transform.position += _moveVector;
            Debug.Log("_moveVector is " + _moveVector.ToString());
        }


        // 枠の通過判定
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "start"){
                gameMaster.duringRace = true;
                startSound.Play();
                Debug.Log("race start");
            }
            if (other.gameObject.tag == "goal"){
                gameMaster.duringRace = false;
                goalSound.Play();
                Debug.Log("race end");
            }
            if (other.gameObject.tag == "checkPoint"){
                if ((_time - _previousCheckPointTime) > 0.5){
                    gameMaster.numberOfCheckpointsPassed += 1;
                    _previousCheckPointTime = _time;
                    checkPointSound.Play();
                    Debug.Log("passing the check point");
                }
            }
        }

        public void returnToInitTransform(){
            // transform = _initShipTransform;
        }
    }
}