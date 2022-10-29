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

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}