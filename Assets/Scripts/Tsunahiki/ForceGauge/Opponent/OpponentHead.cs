using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using communication;
using tsunahiki.game;
using tsunahiki.forceGauge.state;
using tsunahiki.forceGauge.stateController;
using tsunahiki.forceGauge;

namespace tsunahiki.forceGauge
{
    public class OpponentHead : MonoBehaviour
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
        // [SerializeField]
        // private Transform _opponentAvatarHead;
        // [SerializeField]
        // private Transform _opponentAvatarBody;
        [SerializeField]
        private GameObject _face;
        [SerializeField]
        private float _lengthOfNeck;

        //頭の並進運動を一定倍に増幅する際の倍率
        [SerializeField]
        private float _movementScalingFactor;

        // 位置と回転をリセットする際の閾値
        // 一定の距離以上動いたら、まったく別の場所に移動したとみなして、位置を回転を初期化する
        [SerializeField]
        private float _maxScaleOfHMDPosition;

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

            _initHeadPosition = this.transform.position;
            _initHeadEulerAngle = this.transform.eulerAngles;   
        }

        // Update is called once per frame
        void Update()
        {
            UpdateCurrentHMDTransform();

            if (_currentPositionOfHMD.magnitude > _maxScaleOfHMDPosition){
                SetInitTransform();
            }

            // 頭の位置を更新
            this.transform.position = _initHeadPosition + (_currentPositionOfHMD - _initPositionOfHMD) * _movementScalingFactor;
            this.transform.eulerAngles = _initHeadEulerAngle + (_currentEulerAngleOfHMD - _initEulerAngleOfHMD);


            // 胴体の位置を更新
            // _opponentAvatarBody.position = new Vector3(this.transform.position.x, this.transform.position.y + _lengthOfNeck, this.transform.position.z);
            // _opponentAvatarBody.eulerAngles = new Vector3(0.0f, this.transform.eulerAngles.y, 0.0f);

            // 表情の更新
            _face.GetComponent<MeshRenderer>().material = faceMaterial[(int)opponentFace];

            
            // 相手がready stateになったら初期位置を更新しなおす
            // if(_masterForForceGauge.opponentData.stateId == (int)TsunahikiStateType.Ready){
            //     _initPositionOfHMD = _currentPositionOfHMD;
            //     _initEulerAngleOfHMD = _currentEulerAngleOfHMD;
            // }

        }

        // HMDの位置・回転をアップデートする
        // デバッグ時は自分のHMDを反映させる
        void UpdateCurrentHMDTransform(){
            if (_reflectsHMD){
                _currentPositionOfHMD = _HMD.position;
                _currentEulerAngleOfHMD = _HMD.eulerAngles;
            }else{
                _currentPositionOfHMD = new Vector3(_masterForForceGauge.opponentData.positionX, _masterForForceGauge.opponentData.positionY, _masterForForceGauge.opponentData.positionZ);
                _initEulerAngleOfHMD = new Vector3(_masterForForceGauge.opponentData.rotationX, _masterForForceGauge.opponentData.rotationY, _masterForForceGauge.opponentData.rotationZ);
            }
        }

        public void SetInitTransform(){
            _initPositionOfHMD = _currentPositionOfHMD;
            _initEulerAngleOfHMD = _currentEulerAngleOfHMD;
        }
    }
}