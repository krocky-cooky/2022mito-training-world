using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using communication;
using tsunahiki.game;
using tsunahiki.state;
using tsunahiki.stateController;

public class OpponentAvatar : MonoBehaviour
{
    // 表情のマテリアル
    public Material[] faceMaterial = new Material[4];

    // 表情の指定
    public enum OpponentFace
    {
        NormalFace,
        FightFace1,
        FightFace2,
        FightFace3,
        DefeatedFace,
        WinningFace,
    }
    public OpponentFace opponentFace;

    [SerializeField]
    private MasterForForceGauge _masterForForceGauge;

    // デバッグ用のフラグ
    // 自分のHMDの動きを反映
    [SerializeField]
    private bool _reflectsHMD;

    [SerializeField]
    private Transform _HMD;
    [SerializeField]
    private Transform _opponentAvatarHead;
    [SerializeField]
    private Transform _opponentAvatarBody;
    [SerializeField]
    private GameObject _opponentAvatarFace;
    [SerializeField]
    private float _lengthOfNeck;

    // 相手のHMDの初期位置
    private Vector3 _initPositionOfHMD;
    private Vector3 _initEulerAngleOfHMD;

    // 相手のHMDの現在位置
    private Vector3 _currentPositionOfHMD;
    private Vector3 _currentEulerAngleOfHMD;

    // 相手のアバターの頭の表示位置の初期位置
    private Vector3 _initHeadPosition;
    private Vector3 _initHeadEulerAngle;

    // Start is called before the first frame update
    void Start()
    {
        UpdateCurrentHMDTransform();

        _initPositionOfHMD = _currentPositionOfHMD;
        _initEulerAngleOfHMD = _currentEulerAngleOfHMD;

        _initHeadPosition = _opponentAvatarHead.position;
        _initHeadEulerAngle = _opponentAvatarHead.eulerAngles;   
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCurrentHMDTransform();

        // 頭の位置を更新
        _opponentAvatarHead.position = _initHeadPosition + (_currentPositionOfHMD - _initPositionOfHMD);
        _opponentAvatarHead.eulerAngles = _initHeadEulerAngle + (_currentEulerAngleOfHMD - _initEulerAngleOfHMD);

        // 相手がready stateになったら初期位置を更新しなおす
        if(_masterForForceGauge.opponentData.stateId == (int)TsunahikiStateType.Ready){
            _initPositionOfHMD = _currentPositionOfHMD;
            _initEulerAngleOfHMD = _currentEulerAngleOfHMD;
        }

        // 胴体の位置を更新
        _opponentAvatarBody.position = new Vector3(_opponentAvatarHead.position.x, _opponentAvatarHead.position.y + _lengthOfNeck, _opponentAvatarHead.position.z);
        _opponentAvatarBody.eulerAngles = new Vector3(0.0f, _opponentAvatarHead.eulerAngles.y, 0.0f);

        // 表情の更新
        _opponentAvatarFace.GetComponent<MeshRenderer>().material = faceMaterial[(int)opponentFace];

    }

    // HMDの位置・回転をアップデートする
    // デバッグ時は自分のHMDを反映させる
    void UpdateCurrentHMDTransform(){
        if (_reflectsHMD){
            _currentPositionOfHMD = _HMD.position;
            _currentEulerAngleOfHMD = _HMD.eulerAngles;
        }else{
            // _currentPositionOfHMD = new Vector3(_masterForForceGauge.opponentData.positionXOfHMD, _masterForForceGauge.opponentData.positionYOfHMD, _masterForForceGauge.opponentData.positionZOfHMD);
            // _initEulerAngleOfHMD = new Vector3(_masterForForceGauge.opponentData.rotationXOfHMD, _masterForForceGauge.opponentData.rotationYOfHMD, _masterForForceGauge.opponentData.rotationZOfHMD);
        }
    }
}
