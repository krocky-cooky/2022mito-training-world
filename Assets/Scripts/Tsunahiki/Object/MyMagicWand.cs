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
    [SerializeField]
    private Transform _centerBeam;
    [SerializeField]
    private MasterForForceGauge _masterForForceGauge;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        // _myBeam.normalizedScale = _myForceGauge.outputPosition;

        // // fight状態のときのみビーム発射
        // if(_masterForForceGauge.opponentData.stateId == (int)TsunahikiStateType.Fight){
        //     _myBeam.isFired = true;
        //     _myBeam.endPoint = _centerBeam.position;
        // }else{
        //     _myBeam.isFired = false;
        // }

    }
}
