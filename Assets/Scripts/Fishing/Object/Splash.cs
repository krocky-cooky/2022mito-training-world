using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour
{
    public float normalizedSplashVolume;

    [SerializeField]
    private float _minSizeOfSplash;
    [SerializeField]
    private float _maxSizeOfSplash;
    [SerializeField]
    private float _minSoundOfSplash;
    [SerializeField]
    private float _maxSoundOfSplash;

    private ParticleSystem _particleSystem;
    private ParticleSystem.MainModule _mainModule;
    private AudioSource _splashSoundSource;
    private bool _isActive;

    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = this.gameObject.GetComponent<ParticleSystem>();
        _splashSoundSource = this.gameObject.GetComponent<AudioSource>();
        _mainModule = _particleSystem.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(_isActive)
        {
            _mainModule.startSize = Mathf.Lerp(_minSizeOfSplash, _maxSizeOfSplash, normalizedSplashVolume);
            _splashSoundSource.volume = Mathf.Lerp(_minSoundOfSplash, _maxSoundOfSplash, normalizedSplashVolume);
        }else{
            _mainModule.startSize = 0.0f;
            _splashSoundSource.volume = 0.0f;
        }
    }

    public void SetActive(bool active){
        _isActive = active;
    }

}
