using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using communication;
using tsunahiki.game;
using tsunahiki.forceGauge.state;
using tsunahiki.forceGauge.stateController;
using VolumetricLines;

public class BeamController : MonoBehaviour
{
    // 0~1で正規化されたビームのスケール
    public float normalizedScale;

    // ビームの終点
    public Vector3 endPoint;

    // ビームが発射状態かどうかのフラグ
    public bool isFired = false;

    // フレアまたはビームの衝撃音を発生させるフラグ
    public bool playShockSound = false;

    // ビームが中央のフレアに達しているかどうかを示すフラグ
    public bool reachCenter = false;

    [SerializeField]
    private float _minScale;
    [SerializeField]
    private float _maxScale;
    [SerializeField]
    private float _minSoundVolume;
    [SerializeField]
    private float _maxSoundVolume;

    // フレア(火球)の音
    [SerializeField]
    private AudioSource _flareSound;

    // ビーム発射の音
    [SerializeField]
    private AudioSource _fireSound;

    // 衝撃音
    [SerializeField]
    private AudioSource _shockSound;

    // ビームの発射開始から終点までの到達時間
    [SerializeField]
    private float _timeToEndPoint;

    [SerializeField]
    private Transform _centerFlare;

    private CreateBeamLine _volumetricLineBehavior;
    private float _scale;
    private float _initScale;
    private float _timeCount = 0.0f;

    // ビームの発射音を一回鳴らすためのフラグ
    private bool _fireSoundIsPlayed = false;

    // 終点のローカル座標
    private new Vector3 _localEndPoint;

    // Start is called before the first frame update
    void Start()
    {
        _volumetricLineBehavior = this.gameObject.GetComponent<CreateBeamLine>();
        _initScale = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        // スケールを反映
        normalizedScale = Mathf.Clamp01(normalizedScale);
        _scale = _minScale + (_maxScale - _minScale) * normalizedScale;
        _volumetricLineBehavior.m_lineWidth = _scale;

        // 音量にスケールを反映
        _flareSound.volume = _minSoundVolume + (_maxSoundVolume - _minSoundVolume) * normalizedScale;
        
        // 発射状態なら終点のローカル変換を代入し、非発射状態なら球形にする
        // 発射直後は、始点から終点まで連続的に進む
        if(isFired){
            _timeCount += Time.deltaTime;
            endPoint = _centerFlare.position;
            _localEndPoint = transform.InverseTransformPoint(endPoint);
            _localEndPoint = _localEndPoint * Mathf.Clamp01(_timeCount / _timeToEndPoint);
            reachCenter = (_timeCount > _timeToEndPoint);
        }else{
            _timeCount = 0.0f;
            _localEndPoint = new Vector3(0.0f, 0.0001f, 0.0f);
            reachCenter = false;
        }

        // ビームの発射音を再生
        if(isFired & !(_fireSoundIsPlayed)){
            _fireSound.Play();
            _fireSoundIsPlayed = true;
        }
        if(!isFired){
            _fireSoundIsPlayed = false;
        }

        // ビームの衝撃音を再生
        if (playShockSound){
            _shockSound.Play();
            playShockSound = false;
        }

        // レーザーの終点を反映
        _volumetricLineBehavior.m_endPos = _localEndPoint;
    }
}