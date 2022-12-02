using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using communication;
using tsunahiki.game;
using tsunahiki.state;
using tsunahiki.stateController;

public class OpponentHand : MonoBehaviour
{

    [SerializeField]
    private MasterForForceGauge _masterForForceGauge;

    // デバッグ用のフラグ
    // 自分の右手リモコンの動きを反映
    [SerializeField]
    private bool _reflectsMyController;

    [SerializeField]
    private Transform _rightController;

    // 相手のリモコンの初期位置
    private Vector3 _initPositionOfController;
    private Vector3 _initEulerAngleOfController;

    // 相手のリモコンの現在位置
    private Vector3 _currentPositionOfController;
    private Vector3 _currentEulerAngleOfController;

    // 相手の手の表示位置の初期位置
    private Vector3 _initPosition;
    private Vector3 _initEulerAngle;  

    //頭の並進運動を一定倍に増幅する際の倍率
    [SerializeField]
    private float _movementScalingFactor;


    // Start is called before the first frame update
    void Start()
    {
        UpdateCurrentControllerTransform();

        _initPositionOfController = _currentPositionOfController;
        _initEulerAngleOfController = _currentEulerAngleOfController;

        _initPosition = transform.position;
        _initEulerAngle = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCurrentControllerTransform();

        // 手の位置を更新
        // transform.position = _initPosition + (_currentPositionOfController - _initPositionOfController) * _movementScalingFactor;
        // transform.eulerAngles = _initEulerAngle + (_currentEulerAngleOfController - _initEulerAngleOfController) * _movementScalingFactor;

        // 相手がready stateになったら初期位置を更新しなおす
        if(_masterForForceGauge.opponentData.stateId == (int)TsunahikiStateType.Ready){
            _initPositionOfController = _currentPositionOfController;
            _initEulerAngleOfController = _currentEulerAngleOfController;
        }
    }


    // リモコンの位置・回転をアップデートする
    // デバッグ時は自分のリモコンを反映させる
    void UpdateCurrentControllerTransform(){
        if (_reflectsMyController){
            _currentPositionOfController = _rightController.position;
            _currentEulerAngleOfController = _rightController.eulerAngles;
        }else{
            _currentPositionOfController = new Vector3(_masterForForceGauge.opponentData.positionX, _masterForForceGauge.opponentData.positionY, _masterForForceGauge.opponentData.positionZ);
            _currentEulerAngleOfController = new Vector3(_masterForForceGauge.opponentData.rotationX, _masterForForceGauge.opponentData.rotationY, _masterForForceGauge.opponentData.rotationZ);
        }
    }

}
