using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class moveKey : MonoBehaviour
{
    // Start is called before the first frame update
    //private GameObject cam;
    [SerializeField]
    private float speed = 0.1f;
    
    void Start()
    {
        //this.cam = GameObject.FindGameObjectsWithTag("MainCamera")[0];
    
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 movement = Vector3.zero;
        Vector3 changeRotation = Vector3.zero;
        if(Input.GetKey(KeyCode.RightArrow))
        {
            changeRotation.y += speed;
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            changeRotation.y -= speed;
        }
        if(Input.GetKey(KeyCode.UpArrow))
        {
            movement.z += speed;
        }
        if(Input.GetKey(KeyCode.DownArrow))
        {
            movement.z -= speed;
        }
        if(Input.GetKey(KeyCode.Return))
        {
            movement.y += speed;
        }
        if(Input.GetKey(KeyCode.RightShift))
        {
            movement.y -= speed;
        }

        //Vector3 changeRotation = new Vector3(0,InputTracking.GetLocalRotation(XRNode.Head).eulerAngles.y,0);
        this.transform.position += this.transform.rotation * (Quaternion.Euler(changeRotation) * movement);
    }
}
