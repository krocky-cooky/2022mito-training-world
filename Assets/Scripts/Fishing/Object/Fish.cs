using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;
using communication;
using Fishing.Game;
using Fishing.State;
using Fishing.StateController;

namespace Fishing.Object{
    public class Fish : MonoBehaviour
    {
        public string species;
        public float weight;
        public float HP;
        public float easeOfEscape;
        public float currentIntensityOfMovements;

        // 魚影およびボディのアクティブ化のフラグ
        public bool isFishShadow = false;
        public bool isFishBody = false;

        // 魚がカラダをねじる速度
        public float twistSpeed;

        // 子オブジェクト
        // private GameObject[] ChildObject;
        private GameObject childObject;

        // 魚影オブジェクト
        public GameObject _shadowObject;

        // 魚のボディのオブジェクト
        public GameObject _bodyObject;

        // Start is called before the first frame update
        void Start()
        {
            // childObjects = GetAllChildObject();

            // foreach(Transform childObject in childObjects) {
            //     if (childObject.tag == "fishShadow")
            //     {
            //         _shadowObject = childObject.transform.GetChild(0).gameObject;
            //     }
            //     if (childObject.tag == "fishBody")
            //     {
            //         _bodyObject = childObject.transform.GetChild(0).gameObject;
            //     }
            // }

            for (int i = 0; i < this.transform.childCount; i++)
            {
                childObject = this.transform.GetChild(i).gameObject;
                if (childObject.tag == "fishShadow")
                {
                    _shadowObject = childObject;
                }
                if (childObject.tag == "fishBody")
                {
                    _bodyObject = childObject;
                }
            }

        }

        // Update is called once per frame
        void Update()
        {
            _shadowObject.SetActive(isFishShadow);
            _bodyObject.SetActive(isFishBody);
            _shadowObject.GetComponent<Animator>().SetFloat("Speed", twistSpeed);
        }
    }
}