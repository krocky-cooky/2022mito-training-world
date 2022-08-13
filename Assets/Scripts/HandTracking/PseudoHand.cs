using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoHand : MonoBehaviour
{

    public Transform handCenter;
    public GameObject ovrCameraRig;
    private Transform pseudoHandTransform;
    private MeshRenderer pseudoHandRenderer;
    private HandGeneral _handGeneral;
    [SerializeField] float pseudoRange = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.color = Color.blue;
        this.pseudoHandRenderer = this.GetComponent<MeshRenderer>();
        this.pseudoHandRenderer.enabled = false;
        this.pseudoHandTransform = this.transform;
        this._handGeneral = ovrCameraRig.gameObject.GetComponent<HandGeneral>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isTraining = this._handGeneral.isTraining;
        if(isTraining)
            this.pseudoHandRenderer.enabled = true;
        else
            this.pseudoHandRenderer.enabled = false;
        
        Vector3 handMove = this._handGeneral.handCenterPosition - this._handGeneral.handCenterPositionOrigin;
        handMove = Vector3.Scale(handMove,new Vector3(pseudoRange,pseudoRange,pseudoRange));
        this.pseudoHandTransform.position = _handGeneral.handCenterPositionOrigin + handMove;


    }
}
