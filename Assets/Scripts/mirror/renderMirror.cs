using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class renderMirror : MonoBehaviour
{
    [SerializeField]
    private float mirrorSize = 50;
    [SerializeField]
    private GameObject trackingCamera;
    private GameObject reflectionCamera;
    private GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        reflectionCamera = GameObject.FindWithTag("ReflectionCamera");
        parent = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
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
}


