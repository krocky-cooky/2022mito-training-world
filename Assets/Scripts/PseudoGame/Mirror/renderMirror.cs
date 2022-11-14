using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace pseudogame.mirror
{
    public class renderMirror : MonoBehaviour
    {
        [SerializeField]
        private float mirrorSize = 50;
        [SerializeField]
        private GameObject trackingCameraObject;
        [SerializeField]
        private OVRCameraRig rig;
        [SerializeField]
        private Texture testTexture;

        private RenderTexture  _rightTexture;
        private RenderTexture _leftTexture;
        private Vector3 _eyeOffset;
        private Camera reflectionCamera;
        private GameObject parent;
        private Camera trackingCamera;

        void Awake()
        {
            _rightTexture = new RenderTexture(1024,1024,24);
            _leftTexture = new RenderTexture(1024,1024,24);
        }

        void Start()
        {
            reflectionCamera = GameObject.FindWithTag("ReflectionCamera").GetComponent<Camera>();
            parent = this.transform.parent.gameObject;
            this.transform.position = parent.transform.position;
            trackingCamera = trackingCameraObject.GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            /*
            Vector3 diff = transform.position - trackingCamera.transform.position;
            Vector3 normal = this.parent.transform.forward;
            Vector3 reflection = diff + 2 * (Vector3.Dot(-diff, normal)) * normal;
            reflectionCamera.transform.position = this.parent.transform.position - reflection;
            reflectionCamera.transform.LookAt(this.parent.transform.position);

            float distance = Vector3.Distance(transform.position, reflectionCamera.transform.position);
            reflectionCamera.GetComponent<Camera>().nearClipPlane = distance * 0.8f;

            var angle = Vector3.Angle(-parent.transform.forward, reflectionCamera.transform.forward);
            var specularSize = mirrorSize + Mathf.Sin(angle * Mathf.Deg2Rad) * 2;
            this.transform.localScale = new Vector3(-specularSize, specularSize, 1);

            reflectionCamera.GetComponent<Camera>().fieldOfView = 2 * Mathf.Atan(mirrorSize / (2* distance)) * Mathf.Rad2Deg;

            this.transform.rotation = Quaternion.LookRotation(this.transform.position - trackingCamera.transform.position);
            */
            Vector3 diff = transform.position - trackingCamera.transform.position;
            Vector3 normal = this.parent.transform.forward;
            Vector3 reflection = diff + 2 * (Vector3.Dot(-diff, normal)) * normal;
            reflectionCamera.transform.position = this.parent.transform.position - reflection;
            reflectionCamera.transform.LookAt(this.parent.transform.position);

            float distance = Vector3.Distance(transform.position, reflectionCamera.transform.position);
            reflectionCamera.GetComponent<Camera>().nearClipPlane = distance * 0.8f;

            var angle = Vector3.Angle(-parent.transform.forward, reflectionCamera.transform.forward);
            var specularSize = mirrorSize + Mathf.Sin(angle * Mathf.Deg2Rad) * 2;
            this.transform.localScale = new Vector3(-specularSize, specularSize, 1);

            reflectionCamera.GetComponent<Camera>().fieldOfView = 2 * Mathf.Atan(mirrorSize / (2* distance)) * Mathf.Rad2Deg;

            this.transform.rotation = Quaternion.LookRotation(this.transform.position - trackingCamera.transform.position);
        }

        private void OnWillRenderObject()
        {
            RenderIntoMaterial();
            Debug.Log("hello");
        }

        private void RenderIntoMaterial()
        {
            

            Transform trackingSpace = rig.centerEyeAnchor.transform;
            var leftEye = trackingSpace.localPosition + new Vector3(-trackingCamera.stereoSeparation / 2f,0f,0f);
            var rightEye = trackingSpace.localPosition + new Vector3(-trackingCamera.stereoSeparation / 2f,0f,0f);
            Material material = GetComponent<Renderer>().material;

            if (trackingCamera.stereoTargetEye == StereoTargetEyeMask.Both || trackingCamera.stereoTargetEye == StereoTargetEyeMask.Left)
            {
                Vector3 leftPosition = rig.transform.TransformPoint(leftEye);
                Quaternion leftRot = rig.transform.rotation * trackingSpace.localRotation;
                RenderPlane(reflectionCamera, _leftTexture, leftPosition, leftRot, trackingCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left));
                material.SetTexture("_LeftTex", _leftTexture);
            }
            if (trackingCamera.stereoTargetEye == StereoTargetEyeMask.Both || trackingCamera.stereoTargetEye == StereoTargetEyeMask.Right)
            {
                Vector3 rightPosition = rig.transform.TransformPoint(rightEye);
                Quaternion rightRot = rig.transform.rotation * trackingSpace.localRotation;
                RenderPlane(reflectionCamera, _rightTexture, rightPosition, rightRot, trackingCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right));
                material.SetTexture("_RightTex", _rightTexture);
            }
        }

        protected void RenderPlane (Camera mirrorCamera, RenderTexture targetTexture, Vector3 camPosition, Quaternion camRotation, Matrix4x4 camProjectionMatrix) 
        {
		// Copy camera position/rotation/projection data into the reflectionCamera
            mirrorCamera.transform.position = camPosition;
            mirrorCamera.transform.rotation = camRotation;
            mirrorCamera.targetTexture = targetTexture;
            mirrorCamera.ResetWorldToCameraMatrix ();

            // Change the project matrix to use oblique culling (only show things BEHIND the portal)
            Vector3 pos = transform.position;
            Vector3 normal = transform.forward;
            bool isForward = transform.InverseTransformPoint (mirrorCamera.transform.position).z < 0;
            //Vector4 clipPlane = CameraSpacePlane( mirrorCamera, pos, normal, isForward ? 1.0f : -1.0f );
            Matrix4x4 projection = camProjectionMatrix;
            
            mirrorCamera.projectionMatrix = projection;

            // Hide the other dimensions
            mirrorCamera.enabled = false;
            mirrorCamera.cullingMask = 0;


            // Update values that are used to generate the Skybox and whatnot.
            /*
            mirrorCamera.farClipPlane = mainCamera.farClipPlane;
            mirrorCamera.nearClipPlane = mainCamera.nearClipPlane;
            mirrorCamera.orthographic = mainCamera.orthographic;
            mirrorCamera.fieldOfView = mainCamera.fieldOfView;
            mirrorCamera.aspect = mainCamera.aspect;
            mirrorCamera.orthographicSize = mainCamera.orthographicSize;
            */

            mirrorCamera.Render ();

            mirrorCamera.targetTexture = null;
	    }

        
	}

    
        
    
}

