using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace util
{

    public class CylinderBetweenTwoPoints : MonoBehaviour {
        [SerializeField]
        private Transform cylinderPrefab;
        [SerializeField]
        private Transform transformOfBeginPoint;
        [SerializeField]
        private Transform transformOfEndPoint;

        private GameObject leftSphere;
        private GameObject rightSphere;
        private GameObject cylinder;

        private void Start () {
            InstantiateCylinder(cylinderPrefab, transformOfBeginPoint.position, transformOfEndPoint.position);
        }

        private void Update () {
            UpdateCylinderPosition(cylinder, transformOfBeginPoint.position, transformOfEndPoint.position);
        }

        private void InstantiateCylinder(Transform cylinderPrefab, Vector3 beginPoint, Vector3 endPoint)
        {
            cylinder = Instantiate<GameObject>(cylinderPrefab.gameObject, Vector3.zero, Quaternion.identity, transform.parent.gameObject.transform);
            UpdateCylinderPosition(cylinder, beginPoint, endPoint);
        }

        private void UpdateCylinderPosition(GameObject cylinder, Vector3 beginPoint, Vector3 endPoint)
        {
            Vector3 offset = endPoint - beginPoint;
            Vector3 position = beginPoint + (offset / 2.0f);

            cylinder.transform.position = position;
            cylinder.transform.LookAt(beginPoint);
            cylinder.transform.Rotate(90.0f, 0.0f, 0.0f);
            Vector3 localScale = cylinder.transform.localScale;
            localScale.y = (endPoint - beginPoint).magnitude * 0.5f;
            cylinder.transform.localScale = localScale;
        }
    }

}