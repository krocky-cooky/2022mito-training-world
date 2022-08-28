using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace handTracking
{
    public class handTest : MonoBehaviour
    {
        // Start is called before the first frame update
        private OVRSkeleton _ovrSkeleton;
        void Start()
        {
            _ovrSkeleton = GetComponent<OVRSkeleton>();
            
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log(_ovrSkeleton.Bones.Count);
        }
    }
}