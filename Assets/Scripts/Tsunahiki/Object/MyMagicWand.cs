using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using communication;
using tsunahiki.game;
using tsunahiki.state;
using tsunahiki.stateController;
using VolumetricLines;


public class MyMagicWand : MonoBehaviour
{

    [SerializeField]
    private Beam _myBeam;
    [SerializeField]
    private ForceGauge _myForceGauge;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _myBeam.normalizedScale = _myForceGauge.outputPosition;
    }
}
