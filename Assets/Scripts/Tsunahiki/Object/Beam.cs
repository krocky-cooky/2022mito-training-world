using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using communication;
using tsunahiki.game;
using tsunahiki.state;
using tsunahiki.stateController;
using VolumetricLines;

public class Beam : MonoBehaviour
{

    // 0~1で正規化されたビームのスケール
    public float normalizedScale;

    // ビームの終点
    public Vector3 endPoint;

    // ビームが発射状態かどうかのフラグ
    public bool isFired = false;

    [SerializeField]
    private float _minScale;
    [SerializeField]
    private float _maxScale;
    [SerializeField]
    private float _minSoundVolume;
    [SerializeField]
    private float _maxSoundVolume;

    private VolumetricLineBehavior _volumetricLineBehavior;
    private AudioSource _effectSound;
    private float _scale;
    private float _initScale;

    // 終点のローカル座標
    private new Vector3 _localEndPoint;

    // Start is called before the first frame update
    void Start()
    {
        _volumetricLineBehavior = this.gameObject.GetComponent<VolumetricLineBehavior>();
        _effectSound = this.gameObject.GetComponent<AudioSource>();
        _initScale = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        // スケールを反映
        // ただし長さ方向であるy方向は変えない
        normalizedScale = Mathf.Clamp01(normalizedScale);
        _scale = _minScale + (_maxScale - _minScale) * normalizedScale;
        transform.localScale = new Vector3(_scale, _initScale, _scale);

        // 音量にスケールを反映
        _effectSound.volume = _minSoundVolume + (_maxSoundVolume - _minSoundVolume) * normalizedScale;
        
        // 発射状態なら終点のローカル変換を代入し、非発射状態なら球形にする
        if(isFired){
            _localEndPoint = transform.InverseTransformPoint(endPoint);
        }else{
            _localEndPoint = new Vector3(0.0f, 0.01f, 0.0f);
        }

        // レーザーの終点を反映
        _volumetricLineBehavior.m_endPos = _localEndPoint;
    }
}
