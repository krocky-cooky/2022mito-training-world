using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showEarlyLift : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject cam;
    void Start()
    {
        cam = GameObject.FindGameObjectsWithTag("MainCamera")[0];
        gameObject.GetComponent<Renderer>().material.color = Color.blue;

    }

    // Update is called once per frame
    void Update()
    {
        if(cam.GetComponent<OVRCameraRig>().getFlag())
        {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
        else{
            gameObject.GetComponent<Renderer>().material.color = Color.blue;
        }
    }
}
