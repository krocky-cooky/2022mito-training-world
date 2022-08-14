using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace util
{
    public class Setup : MonoBehaviour
    {
        [SerializeField]
        private GameObject trainingDevice;
        [SerializeField]
        private Transform rightControllerTransform;
        [SerializeField]
        private float deviceVerticalDistance = 0.28f;
        [SerializeField]
        private float deviceHorizontalDistance = 0.2f;
        [SerializeField]
        private float deviceDownDistance = 0.08f;
        //[SerializeField]
        private Quaternion initialRotation = Quaternion.Euler(-90.0f,0.0f,180.0f);

        [SerializeField]
        float forwardWaight = 0.527f;
        [SerializeField]
        private GameObject sphere;
        [SerializeField]
        private float spos = 1.0f;

        private Vector3 horizontal;
        private Vector3 down;
        private Vector3 projectionToUp;
        private Vector3 projectionToXZ;
        private Vector3 toDevice;

        // Start is called before the first frame update
        void Start()
        {
            

        }

        // Update is called once per frame
        void Update()
        {
            horizontal = - (forwardWaight*rightControllerTransform.forward + (1-forwardWaight)*rightControllerTransform.up);

            if(OVRInput.GetDown(OVRInput.RawButton.A) || (OVRInput.GetDown(OVRInput.RawButton.LHandTrigger)))
            {
                down = new Vector3(0,-1,0);
                float horizontalDotDown = Vector3.Dot(horizontal,down);
                projectionToUp = Vector3.Scale(down,new Vector3(horizontalDotDown,horizontalDotDown,horizontalDotDown));
                projectionToXZ = (horizontal - projectionToUp).normalized;
                toDevice = Vector3.Scale(Vector3.Cross(down, projectionToXZ).normalized,new Vector3(deviceVerticalDistance,deviceVerticalDistance,deviceVerticalDistance));
                trainingDevice.transform.position = rightControllerTransform.position + toDevice;
                Quaternion rotX = Quaternion.FromToRotation(trainingDevice.transform.right,-projectionToXZ);
                Quaternion rotY = Quaternion.FromToRotation(trainingDevice.transform.forward, new Vector3(0,0,1));
                
                trainingDevice.transform.rotation *= rotX;
                //trainingDevice.transform.rotation *= rotY;

                trainingDevice.transform.position += Vector3.Scale(projectionToXZ,new Vector3(deviceHorizontalDistance,deviceHorizontalDistance,deviceHorizontalDistance));
                trainingDevice.transform.position += Vector3.Scale(down,new Vector3(deviceDownDistance,deviceDownDistance,deviceDownDistance));
                //trainingDevice.transform.rotation *= additionalRotation;
                


            }
            sphere.transform.position = rightControllerTransform.position + Vector3.Scale(horizontal.normalized,new Vector3(spos,spos,spos));

            
        }
    }
}
    
