using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentHand : MonoBehaviour
{

    [SerializeField]
    private MasterForForceGauge _masterForForceGauge;

    // 相手のリモコンの初期位置
    private Vector3 _initPositionOfController;
    private Vector3 _initEulerAngleOfController;

    // 相手のリモコンの現在位置
    private Vector3 _currentPositionOfController;
    private Vector3 _currentEulerAngleOfController;

    // Start is called before the first frame update
    void Start()
    {
        _currentPositionOfController = new Vector3(_masterForForceGauge.opponentData.positionX, _masterForForceGauge.opponentData.positionY, _masterForForceGauge.opponentData.positionZ);
        _currentEulerAngleOfController = new Vector3(_masterForForceGauge.opponentData.rotationX, _masterForForceGauge.opponentData.rotationY, _masterForForceGauge.opponentData.rotationZ);

        _initPositionOfController = _currentPositionOfController;
        _initEulerAngleOfController = _currentEulerAngleOfController;
    }

    // Update is called once per frame
    void Update()
    {
        _currentPositionOfController = new Vector3(_masterForForceGauge.opponentData.positionX, _masterForForceGauge.opponentData.positionY, _masterForForceGauge.opponentData.positionZ);
        _currentEulerAngleOfController = new Vector3(_masterForForceGauge.opponentData.rotationX, _masterForForceGauge.opponentData.rotationY, _masterForForceGauge.opponentData.rotationZ);

        // 手の位置を更新
        transform.position += _currentPositionOfController - _initPositionOfController;
        transform.eulerAngles += _currentEulerAngleOfController - _initEulerAngleOfController;

        // 相手がready stateになったら初期位置を更新しなおす
        if(masterForForceGauge.opponentData.stateId == TsunahikiStateType.Ready){
            _initPositionOfController = _currentPositionOfController;
            _initEulerAngleOfController = _currentEulerAngleOfController;
        }
    }

}
