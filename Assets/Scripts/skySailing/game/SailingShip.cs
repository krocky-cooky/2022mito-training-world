using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace skySailing.game
{
    
    public class SailingShip : MonoBehaviour
    {
        public Transform pillarTransform;
        public Transform shipTransform;
        public Transform nextShipTransform;
        public float maxXRotationOfPillar;
        public float minXRotationOfPillar;

        private Vector3 moveVector;

        public SailingShip(GameObject sailingShip, float inputMaxXRotationOfPillar, float inputMinXRotationOfPillar)
        {
            pillarTransform = GameObject.FindWithTag("pillar").GetComponent<Transform>();
            shipTransform = sailingShip.GetComponent<Transform>();
            nextShipTransform = shipTransform;
            maxXRotationOfPillar = inputMaxXRotationOfPillar;
            minXRotationOfPillar = inputMinXRotationOfPillar;
        }

        // 柱の角度を変化
        // x軸周りの角度の相対位置を受け取ると、それに合わせて柱の角度変更
        // 例えばx軸周りの角度の可動範囲が-30~30で、相対位置で0.3を受け取ったら、-30+60*0.3で-12に帆の角度が変更
        public void changePillarRotation(float relativeXRotation){
            pillarTransform.Rotate(minXRotationOfPillar + (maxXRotationOfPillar - minXRotationOfPillar) * relativeXRotation - pillarTransform.localEulerAngles.x, 0, 0);
            Debug.Log("xRotation is "+(pillarTransform.localEulerAngles.x).ToString());
            Debug.Log("next angular is " + (minXRotationOfPillar + (maxXRotationOfPillar - minXRotationOfPillar) * relativeXRotation - pillarTransform.localEulerAngles.x).ToString());
        }

        public void move(Master master){
            moveVector = Quaternion.Euler(pillarTransform.eulerAngles.x, 0.0f, 0.0f) * new Vector3(0.0f, 0.0f, -master.windSpeed);
            shipTransform.position += moveVector;
            Debug.Log("moveVector is " + moveVector.ToString());
        }
       
    }
}