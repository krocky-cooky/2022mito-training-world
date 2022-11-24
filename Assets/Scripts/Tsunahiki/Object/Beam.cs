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

    private VolumetricLineBehavior _volumetricLineBehavior;
    private AudioSource _effectSound;
    private float _scale;

    // Start is called before the first frame update
    void Start()
    {
        _volumetricLineBehavior = this.gameObject.GetComponent<VolumetricLineBehavior>();
        _effectSound = this.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // スケールを反映
        normalizedScale = Mathf.Clamp01(normalizedScale);
        _scale = _minScale + (_maxScale - _minScale) * normalizedScale;
        transform.localScale = new Vector3(_scale, _scale, _scale);

        // 音量にスケールを反映
        _effectSound.volume = normalizedScale;
        
        // 発射状態でなければ球状態にする
        if(!isFired){
            endPoint = new Vector3(0.0f, 0.05f, 0.0f);
        }

        // レーザーの終点を反映
        _volumetricLineBehavior.m_endPos = endPoint;
    }
}
