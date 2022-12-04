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
    public class OpponentPlayer : MonoBehaviour
    {
        public OpponentHead head;
        public OpponentBody body;
        public BeamController beamController;

        // 相手の力の出し具合を正規化値で返す
        public float normalizedPower;

        [SerializeField]
        private BeamController _myBeam;
        [SerializeField]
        private Transform _centerFlare;

        // 表情変化に関する出力の閾値
        [SerializeField]
        private float _firstThresholdForChangingFace;
        [SerializeField]
        private float _secondThresholdForChangingFace;

        // 中央のフレアの速さの最大値
        // フレアの移動速度の正規化のために使う
        [SerializeField]
        private float _maxVelocityOfCenterFlare;

        [SerializeField]
        private MasterForForceGauge _masterForForceGauge;

        private Vector3 _previousPositionOfCenterFlare;


        // Start is called before the first frame update
        void Start()
        {
            _previousPositionOfCenterFlare = _centerFlare.position;
        }

        // Update is called once per frame
        void Update()
        {
            // 出力レベルの計算
            // 対戦中以外は相手から送信される正規化値に比例、対戦中は自分のビームの強さと中央のフレアの動きから逆算
            if (_masterForForceGauge.opponentData.stateId == (int)TsunahikiStateType.Fight){
                // 対戦中の出力計算
                normalizedPower = GetNormalizedPowerByBeamAndCenterFlare();
            }else{
                // 対戦中以外の出力計算
                normalizedPower = _masterForForceGauge.opponentData.normalizedData;
            }


            // ビームの大きさを変更出力レベルに相関させる
            beamController.normalizedScale = normalizedPower;


            // 表情変化
            if (_masterForForceGauge.opponentData.stateId == (int)TsunahikiStateType.Fight){
                // 対戦中の表情変化
                if (normalizedPower < _firstThresholdForChangingFace){
                    head.opponentFace = OpponentHead.OpponentFace.FightFace1;
                }else if (normalizedPower > _secondThresholdForChangingFace){
                    head.opponentFace = OpponentHead.OpponentFace.FightFace3;
                }else{
                    head.opponentFace = OpponentHead.OpponentFace.FightFace2;
                }
                Debug.Log("face changing");
            } else if(_masterForForceGauge.opponentData.stateId == (int)TsunahikiStateType.EndOfFight){
                // 対戦終了時の表情変化
                if ((_masterForForceGauge.opponentData.latestWinner == _masterForForceGauge.myDeviceId)){
                    head.opponentFace = OpponentHead.OpponentFace.DefeatedFace;
                }else if ((_masterForForceGauge.opponentData.latestWinner == (int)TrainingDeviceType.Nothing)){
                    head.opponentFace = OpponentHead.OpponentFace.NormalFace;
                }else{
                    head.opponentFace = OpponentHead.OpponentFace.WinningFace;
                }
            }else{
                // 対戦中・対戦終了時以外の表情
                head.opponentFace = OpponentHead.OpponentFace.NormalFace;
            }
            
            _previousPositionOfCenterFlare = _centerFlare.position;
        }
        

        // 対戦中の相手の出力レベルを計算する
        // 自分のビームの強さと、中央のフレアの動きから、逆算する形をとる
        // 例えば、自分のビームが強くて、それでも中央のフレアが自分の側に移動していれば、相手はかなり強い力を出しているといえる
        float GetNormalizedPowerByBeamAndCenterFlare(){
            // 自分の出力レベル×{キューブの正規化速度(正方向は相手) + 0.5}
            // 例えば、自分が最大の力を出していて、均衡以上なら、相手も最大以上の力を出しているとみなす
            float _power = _myBeam.normalizedScale * (GetNormalizedVelocityOfCenterFlare(_centerFlare.position, _previousPositionOfCenterFlare, _maxVelocityOfCenterFlare) + 0.5f);
            _power = Mathf.Clamp01(_power);
            return _power;
        }

        // 中央のフレアの速度の正規化値を返す
        // 相手に向かうほうを正とする
        float GetNormalizedVelocityOfCenterFlare(Vector3 currentPositionOfCenterFlare, Vector3 previousPositionOfCenterFlare, float maxVelocityOfCenterFlare){
            float _normalizedVelocityOfCenterFlare;
            _normalizedVelocityOfCenterFlare = (- (currentPositionOfCenterFlare.z - previousPositionOfCenterFlare.z)) / (maxVelocityOfCenterFlare * 2.0f) + 0.5f;
            _normalizedVelocityOfCenterFlare = Mathf.Clamp01(_normalizedVelocityOfCenterFlare);
            return _normalizedVelocityOfCenterFlare;
        }
    }
}