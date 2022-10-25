using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pseudogame.game;
using communication;

namespace pseudogame.trainingObjects
{
    public class shortTrainingBar : MonoBehaviour
    {
        private GameObject _webSocketClient;
        private ReceivingDataFormat _jsonData;
        private bool _changeBarSizeFlag = false;
        private Gradient _gradient = default;
        private float currentScale = 5.0f;
        private float yVelocity = 0.0f;
        private float smoothTime = 0.3f;

        // Start is called before the first frame update
        void Start()
        {
            _webSocketClient = GameObject.FindWithTag("webSocketClient");    
        }

        // Update is called once per frame
        void Update()
        {
            if(_changeBarSizeFlag)
            {
                changeBarStatus();
                _changeBarSizeFlag = false;
                Debug.Log("??????????????");
            }
        }

        public void changeBarStatusFlag(ReceivingDataFormat data)
        {
            _jsonData = data ;
            _changeBarSizeFlag = true;
        }

        private void changeBarStatus()
        {
            //color
            float colorFrac = _jsonData.trq / 2.0f;
            GameObject child = transform.GetChild(0).gameObject;

            //child.GetComponent<Renderer>().material.color = _gradient.Evaluate(colorFrac);

            //size
            GameObject[] weights = GameObject.FindGameObjectsWithTag("Weight");
            float targetScale = _jsonData.trq*10 + 5.0f;
            float nextScale = Mathf.SmoothDamp(
                currentScale,
                targetScale,
                ref yVelocity,
                smoothTime
            );
            Vector3 nextScaleVector = new Vector3(targetScale,0.1f,targetScale);
            foreach(GameObject weight in weights)
            {
                weight.transform.localScale = nextScaleVector;
            }
            currentScale = nextScale;
        }
    }
}