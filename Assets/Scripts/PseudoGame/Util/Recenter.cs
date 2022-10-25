using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace pseudogame.util
{
    public class Recenter : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("recenter script started");
        }

        // Update is called once per frame
        void Update()
        {
            if (OVRInput.Get(OVRInput.RawButton.A))
            {
                //InputTracking.Recenter();
                //bool done = XRInputSubsystem.TryRecenter();
                //InputDevices.GetDeviceAtXRNode(XRNode.Head).subsystem.TryRecenter();
            }
        }
    }
}
