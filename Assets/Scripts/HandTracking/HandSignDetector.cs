using UnityEngine;
using System.Collections;

public class HandSignDetector : MonoBehaviour
{
    [Range(-1, 1)] public float[] FingerBendThrethold = new float[5] { 0.85f, 0, 0.5f, 0, 0 };
    public int HandSignNum;
    public int sign;
    public float SignEndWaitTime = 0.2f;
    bool IsPosing = false, PoseChangeWaiting = false;
    HandBoneDot handBone;
    void Start()
    {
        handBone = GetComponent<HandBoneDot>();
        if (handBone == null)
            return;
        HandSignNum = 0;
        sign = -1;
        for (int i = 0; i < handBone._fingerDots.Count; i++)
            if (handBone._fingerDots[i].dot > FingerBendThrethold[i])
                HandSignNum += (int)Mathf.Pow(2, i);
        UpdateSignInit();
    }
    void Update()
    {
        HandSignNum = 0;
        for (int i = 0; i < handBone._fingerDots.Count; i++)
            if (handBone._fingerDots[i].dot > FingerBendThrethold[i])
                HandSignNum += (int)Mathf.Pow(2, i);

        if (!IsPosing)
            UpdateSignInit();
        else if (HandSignNum != sign && !PoseChangeWaiting && IsPosing)
            StartCoroutine(PoseEndWait(SignEndWaitTime));
        
        //Debug.Log(HandSignNum);
        string test = this.to_string(HandSignNum);
        //Debug.Log(test);
        //Debug.Log(this.to_string(HandSignNum));
    }

    private string to_string(int num)
    {
        //Debug.Log(num);
        string ret = "handsign : ";
        for(int i = 0;i < 5; ++i)
        {
            int x = num%2;
            ret += x.ToString();
            num /= 2;
        }
        return ret;
    }
    
    void UpdateSignInit()
    {
        sign = HandSignNum;
        IsPosing = true;
    }
    
    IEnumerator PoseEndWait(float waitime)
    {
        float t = Time.time;
        PoseChangeWaiting = true;
        while (Time.time < t + waitime)
        {
            if (HandSignNum == sign)
            {
                PoseChangeWaiting = false;
                yield break;
            }
            yield return null;
        }
        
        IsPosing = false;
        PoseChangeWaiting = false;
    }
}