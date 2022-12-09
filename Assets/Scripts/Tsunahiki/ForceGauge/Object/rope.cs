using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rope : MonoBehaviour
{

    [SerializeField]
    private float _period;
    [SerializeField]
    private float _ampitute;
    [SerializeField]
    private Vector3 _initPosition;

    [SerializeField]
    private float _time;

    // Start is called before the first frame update
    void Start()
    {
        _initPosition = transform.position;

        _time = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        transform.position = _initPosition + new Vector3(_ampitute * Mathf.Sin(2.0f * Mathf.PI * _time / _period), 0.0f, 0.0f);
    }
}
