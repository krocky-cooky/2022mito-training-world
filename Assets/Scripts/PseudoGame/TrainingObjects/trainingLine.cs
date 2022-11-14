using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trainingLine : MonoBehaviour
{
    [SerializeField]
    private List<Transform> relays;

    private LineRenderer renderer;
    private Vector3[] positions;

    // Start is called before the first frame update
    void Start()
    {
        renderer = gameObject.AddComponent<LineRenderer>();
        List<Vector3> positionsSub = new List<Vector3>();

        for(int i = 0;i < relays.Count; ++i)
        {
            positionsSub.Add(relays[i].position);
        }
        positions = positionsSub.ToArray();


        renderer.positionCount = positions.Length;
        renderer.startWidth = 0.03f;
        renderer.endWidth = 0.03f;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        List<Vector3> positionsSub = new List<Vector3>();
        for(int i = 0;i < relays.Count; ++i)
        {
            positionsSub.Add(relays[i].position);
            
        }
        positions = positionsSub.ToArray();

        renderer.SetPositions(positions);
    }
}
