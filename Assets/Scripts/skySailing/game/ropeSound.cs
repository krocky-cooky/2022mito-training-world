using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ropeSound : MonoBehaviour
{
    [SerializeField]
    private float displacementThresholdForSound;
    [SerializeField]
    private float delayTimeBeforeStopingSound;

    private GameObject _ropeSound;
    public Vector3 _previousPosition;
    private float timeWithoutEnoughVelocity = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _ropeSound = GameObject.FindWithTag("ropeSound");
        _previousPosition = _ropeSound.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log("rope velocily is " + ((_ropeSound.transform.position - _previousPosition).magnitude).ToString());

        if((_ropeSound.transform.position - _previousPosition).magnitude > displacementThresholdForSound)
        {
            Debug.Log("rope velocity is enough");
            if (!_ropeSound.GetComponent<AudioSource>().isPlaying){
                _ropeSound.GetComponent<AudioSource>().Play();
                Debug.Log("rope sound");
            }
            timeWithoutEnoughVelocity = 0.0f;
        }else{
            timeWithoutEnoughVelocity += Time.deltaTime;
        }
        
        if(timeWithoutEnoughVelocity > delayTimeBeforeStopingSound){
            if (_ropeSound.GetComponent<AudioSource>().isPlaying){
                _ropeSound.GetComponent<AudioSource>().Stop();
                Debug.Log("rope sound stop");
            }
        }

        _previousPosition = _ropeSound.transform.position;
    }
}
