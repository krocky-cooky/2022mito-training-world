using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace handTracking 
{
    public class WeightManipulation : MonoBehaviour
    {
        // Start is called before the first frame update

        [SerializeField] 
        private GameObject Weight;
        private GameObject camObject;

        private HandGeneral _handGeneral;
        void Start()
        {
            this.camObject = GameObject.FindGameObjectsWithTag("MainCamera")[0];
            this._handGeneral = this.camObject.GetComponent<HandGeneral>();
        }

        // Update is called once per frame
        void Update()
        {
            this.GetComponent<MeshRenderer>().enabled = this._handGeneral.isTraining;
            if(this._handGeneral.isTraining)
            {
                this.transform.position = this._handGeneral.handCenterPosition;
                
            }
        
            Vector3 aim = this.camObject.transform.position - this.transform.position;
            Quaternion look = Quaternion.LookRotation(aim);
            this.transform.rotation = look;
            this.transform.Rotate(90.0f,90.0f,0.0f);

        }
    }
}