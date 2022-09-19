using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using skySailing.game;


namespace skySailing.game
{
    
    public class SailingShip: MonoBehaviour
    {
        public Transform pillarTransform;
        public Transform shipTransform;
        public float maxXRotationOfPillar;
        public float minXRotationOfPillar;
        public bool duringRace = false;

        private Vector3 _moveVector;
        private Master gameMaster;

        void Start()
        {
            gameMaster = transform.parent.gameObject.GetComponent<Master>();
        }

        void Update()
        {
            // Debug.Log("ship updating");
            // 柱を回転
            Debug.Log("device position is" + (gameMaster.fittnessDevice.currentRelativePosition).ToString());
            changePillarRotation(gameMaster.fittnessDevice.currentRelativePosition);

            //船を動かす
            move(gameMaster.windSpeed);
        }

        // public SailingShip(GameObject sailingShip, float inputMaxXRotationOfPillar, float inputMinXRotationOfPillar)
        // {
        //     pillarTransform = GameObject.FindWithTag("pillar").GetComponent<Transform>();
        //     shipTransform = sailingShip.GetComponent<Transform>();
        //     maxXRotationOfPillar = inputMaxXRotationOfPillar;
        //     minXRotationOfPillar = inputMinXRotationOfPillar;
        // }

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
            Debug.Log("next angular is " + (minXRotationOfPillar + (maxXRotationOfPillar - minXRotationOfPillar) * relativeXRotation - pillarTransform.localEulerAngles.x).ToString());
        }

        // 帆の傾きに合わせて船を動かす
        public void move(float windSpeed){
            _moveVector = Quaternion.Euler(pillarTransform.eulerAngles.x, pillarTransform.eulerAngles.y, pillarTransform.eulerAngles.z) * new Vector3(0.0f, 0.0f, -windSpeed);
            shipTransform.position += _moveVector;
            Debug.Log("_moveVector is " + _moveVector.ToString());
        }

        // void OnTriggerEnter(Collider other)
        // {
        //     Debug.Log("すり抜けた！ 船が");
        //     if (other.gameObject.tag == "start"){
        //         duringRace = true;
        //         Debug.Log("race start");
        //     }
        // }
    }
}