using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using communication;

namespace tsunahiki.game
{
    public class MasterForForceGauge : MonoBehaviour
    {   
        const int MAX_LOG_LINES = 10;

        public Queue<string> viewerTextQueue = new Queue<string>();
        public Text frontViewUI;


        // トルク÷握力計の値
        public float gripStrengthMultiplier;

        [SerializeField]
        private GameObject viewerObject;
        [SerializeField]
        private RemoteCoordinator _coordinator;
        [SerializeField]
        private GameObject _centerCube;

        private Vector3 cubeStartPosition;
        // Start is called before the first frame update
        void Start()
        {
            cubeStartPosition = _centerCube.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
           
            {
                float normalizedDevicePos = _coordinator.getOpponentValue();
                Vector3 cubePos = cubeStartPosition;
                cubePos.z += (normalizedDevicePos - 0.5f)*2.0f;
                _centerCube.transform.position = cubePos;
            }
            

        }

        

    }
}
