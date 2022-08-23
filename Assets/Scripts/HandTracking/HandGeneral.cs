using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace handTracking
{
    public class HandGeneral : MonoBehaviour
    {
        public enum TrainingType{
            rightHand,
            leftHand,
            bothHands,
        }

        private int rightHandGesture,leftHandGesture;
        public Vector3 handCenterPosition;
        public Quaternion handCenterRotation;
        public Vector3 handCenterPositionOrigin;
        public Vector3 handCenterPositionEnd;
        public GameObject rightHandPrefab,leftHandPrefab;
        public bool isTraining,trainingChangeWaiting;
        private GameObject camObject;
        private GameObject Weight;
        private bool endRegistered = true;
        [SerializeField] private float trainingStartWaitTime = 2f,trainingStopWaitTime = 2f,pseudoRange = 2.0f,afterEarlyLiftScale = 0.99f,pseudoTime = 1.0f,aheadMergin = 0.2f;
        [SerializeField] private TrainingType trainingType = TrainingType.bothHands;
        [SerializeField] private GameObject shortWeight,longWeight,startLineObject,endLineObject;

        // Start is called before the first frame update
        void Start()
        {
            this.rightHandGesture = -1;
            this.leftHandGesture = -1;
            this.isTraining = false;
            this.trainingChangeWaiting = false;
            this.shortWeight.SetActive(false);
            this.longWeight.SetActive(false);
            
            this.camObject = GameObject.FindGameObjectsWithTag("MainCamera")[0];

            if(trainingType == TrainingType.bothHands)
                this.Weight = this.longWeight;
            else if(trainingType == TrainingType.rightHand || trainingType == TrainingType.leftHand)
                this.Weight = this.shortWeight;
            camObject.GetComponent<OVRCameraRig>().setParams(pseudoRange,afterEarlyLiftScale,(int)trainingType,pseudoTime,aheadMergin);
        }

        // Update is called once per frame
        void Update()
        {
            
            rightHandGesture = rightHandPrefab.GetComponent<HandSignDetector>().sign;
            leftHandGesture = leftHandPrefab.GetComponent<HandSignDetector>().sign;
            
            Vector3 rightHandCenterPosition = rightHandPrefab.GetComponent<HandBoneDot>().handCenter;
            Vector3 leftHandCenterPosition =  leftHandPrefab.GetComponent<HandBoneDot>().handCenter;
            
            if(trainingType == TrainingType.rightHand)
            {
                this.handCenterPosition = rightHandCenterPosition;
            }
            else if(trainingType == TrainingType.leftHand)
            {
                this.handCenterPosition = leftHandCenterPosition;
            }
            else if(trainingType == TrainingType.bothHands)
            {
                this.handCenterPosition = (rightHandCenterPosition + leftHandCenterPosition)/2;
            }
            
            //Debug.Log(rightHandGesture);
            //Debug.Log(new Vector2(rightHandGesture,leftHandGesture));
            if(rightHandGesture == 0 && leftHandGesture == 0 && !isTraining && !trainingChangeWaiting && !endRegistered)
                StartCoroutine(RegisterEnd(trainingStartWaitTime));
            else if(rightHandGesture == 0 && leftHandGesture == 0 && !isTraining && !trainingChangeWaiting)
                StartCoroutine(TrainingStartWait(trainingStartWaitTime));
            else if(rightHandGesture == 2 && leftHandGesture == 2 && isTraining && !trainingChangeWaiting)
                StartCoroutine(TrainingStopWait(trainingStopWaitTime));

            //ダンベル操作処理
            this.Weight.SetActive(this.isTraining);
            if(this.isTraining)
                this.Weight.GetComponent<Transform>().position = this.handCenterPosition;
            
            Vector3 parallelVector = Vector3.zero;

            if(trainingType == TrainingType.rightHand)
            {
                parallelVector = rightHandPrefab.GetComponent<HandBoneDot>().handParallel;
            }
            else if(trainingType == TrainingType.leftHand)
            {
                parallelVector = leftHandPrefab.GetComponent<HandBoneDot>().handParallel;

            }
            else if(trainingType == TrainingType.bothHands)
            {
                parallelVector = rightHandCenterPosition - leftHandCenterPosition;
            }

            Quaternion parallelQuaternion = Quaternion.LookRotation(parallelVector);
            Quaternion offsetRotation = Quaternion.FromToRotation(Vector3.forward,new Vector3(0,1,0));
            this.Weight.GetComponent<Transform>().rotation = parallelQuaternion * offsetRotation;
            


            //Debug.Log(isTraining);
            
        }

        IEnumerator RegisterEnd(float waitime)
        {
            float t = Time.time;
            trainingChangeWaiting = true;
            while (Time.time < t + waitime)
            {
                
                if(!(rightHandGesture == 0 && leftHandGesture == 0))
                {
                    this.isTraining = false;
                    trainingChangeWaiting = false;
                    yield break;
                }
                //Debug.Log("Starting");
                yield return null;
                
            }
            this.endRegistered = true;
            handCenterPositionEnd = handCenterPosition;
            trainingChangeWaiting = false;
            Vector3 endLinePosition = camObject.GetComponent<OVRCameraRig>().RegisterEnd();
            endLineObject.transform.position = endLinePosition;
            

        }

        IEnumerator TrainingStartWait(float waitime)
        {
            float t = Time.time;
            trainingChangeWaiting = true;
            while (Time.time < t + waitime)
            {
                
                if(!(rightHandGesture == 0 && leftHandGesture == 0))
                {
                    this.isTraining = false;
                    trainingChangeWaiting = false;
                    yield break;
                }
                //Debug.Log("Starting");
                yield return null;
                
            }
            this.isTraining = true;
            handCenterPositionOrigin = handCenterPosition;
            trainingChangeWaiting = false;
            Vector3 startLinePosition = camObject.GetComponent<OVRCameraRig>().TrainingStart();
            startLineObject.transform.position = startLinePosition;
            Debug.Log("Training start");

        }

        IEnumerator TrainingStopWait(float waitime)
        {
            float t = Time.time;
            trainingChangeWaiting = true;
            while (Time.time < t + waitime)
            {
                if(!(rightHandGesture == 2 && leftHandGesture == 2))
                {
                    this.isTraining = true;
                    trainingChangeWaiting = false;
                    yield break;
                }
                yield return null;
            }
            this.isTraining = false;
            trainingChangeWaiting = false;
            camObject.GetComponent<OVRCameraRig>().TrainingEnd();
            Debug.Log("Training stop");

        }
    }
}
