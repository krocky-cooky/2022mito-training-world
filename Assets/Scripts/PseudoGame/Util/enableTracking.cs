using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enableTracking : MonoBehaviour
{
    [SerializeField]
    private GameObject handTrackingLeft;
    [SerializeField]
    private GameObject handTrackingRight;
    [SerializeField]
    private GameObject controllerLeft;
    [SerializeField]
    private GameObject controllerRight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        Debug.Log(OVRPlugin.GetHandTrackingEnabled());
        if(Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("button pressed!!!!");
            controllerLeft.SetActive(false);
            controllerRight.SetActive(false);
           
            handTrackingRight.SetActive(true);
            handTrackingLeft.SetActive(true);
        }
        */
    }
}
