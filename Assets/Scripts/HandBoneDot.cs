using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HandBoneDot : MonoBehaviour
{
    // Start is called before the first frame update
    public List<BoneDot> _boneDots;
    public List<FingerDot> _fingerDots;
    public enum FingerIndex {Thumb, Index, Middle, Ring, Pinky};
    public Vector3 handCenter;
    public Vector3 handParallel;
    private OVRSkeleton _ovrSkeleton;
    private bool _isInitialized;
    int[,] BoneDirectionIndex = {
    { (int)OVRSkeleton.BoneId.Hand_Thumb1,(int)OVRSkeleton.BoneId.Hand_Thumb2, (int)OVRSkeleton.BoneId.Hand_Thumb3 },
    { (int)OVRSkeleton.BoneId.Hand_Thumb2,(int)OVRSkeleton.BoneId.Hand_Thumb3, (int)OVRSkeleton.BoneId.Hand_ThumbTip },//Thumb
    { (int)OVRPlugin.BoneId.Hand_WristRoot,(int)OVRSkeleton.BoneId.Hand_Index1, (int)OVRSkeleton.BoneId.Hand_Index2 },
    { (int)OVRSkeleton.BoneId.Hand_Index1,(int)OVRSkeleton.BoneId.Hand_Index2, (int)OVRSkeleton.BoneId.Hand_Index3 },
    { (int)OVRPlugin.BoneId.Hand_Index2,(int)OVRSkeleton.BoneId.Hand_Index3, (int)OVRSkeleton.BoneId.Hand_IndexTip },//Index
    { (int)OVRPlugin.BoneId.Hand_WristRoot,(int)OVRSkeleton.BoneId.Hand_Middle1, (int)OVRSkeleton.BoneId.Hand_Middle2 },
    { (int)OVRSkeleton.BoneId.Hand_Middle1,(int)OVRSkeleton.BoneId.Hand_Middle2, (int)OVRSkeleton.BoneId.Hand_Middle3 },
    { (int)OVRSkeleton.BoneId.Hand_Middle2,(int)OVRSkeleton.BoneId.Hand_Middle3, (int)OVRSkeleton.BoneId.Hand_MiddleTip },//Middle
    { (int)OVRPlugin.BoneId.Hand_WristRoot,(int)OVRSkeleton.BoneId.Hand_Ring1, (int)OVRSkeleton.BoneId.Hand_Ring2 },
    { (int)OVRSkeleton.BoneId.Hand_Ring1,(int)OVRSkeleton.BoneId.Hand_Ring2, (int)OVRSkeleton.BoneId.Hand_Ring3 },
    { (int)OVRSkeleton.BoneId.Hand_Ring2,(int)OVRSkeleton.BoneId.Hand_Ring3, (int)OVRSkeleton.BoneId.Hand_RingTip },//Ring
    { (int)OVRPlugin.BoneId.Hand_WristRoot,(int)OVRSkeleton.BoneId.Hand_Pinky1, (int)OVRSkeleton.BoneId.Hand_Pinky2 },
    { (int)OVRSkeleton.BoneId.Hand_Pinky1,(int)OVRSkeleton.BoneId.Hand_Pinky2, (int)OVRSkeleton.BoneId.Hand_Pinky3 },
    { (int)OVRSkeleton.BoneId.Hand_Pinky2,(int)OVRSkeleton.BoneId.Hand_Pinky3, (int)OVRSkeleton.BoneId.Hand_PinkyTip }//Pinky
    };

    public class BoneDot
    {
        Transform BoneBegin,BoneMiddle,BoneEnd;
        public float dot;
        public BoneDot(Transform begin, Transform middle, Transform end)
        {
            BoneBegin = begin;
            BoneMiddle = middle;
            BoneEnd = end;

            this.Update();
        }

        public void Update()
        {
            dot = Vector3.Dot((BoneMiddle.position - BoneBegin.position).normalized,(BoneEnd.position - BoneMiddle.position).normalized);

        }
    }

    public class FingerDot
    {
        BoneDot[] bones;
        public float dot;
        public FingerDot(params BoneDot[] bone)
        {
            bones = bone;
        }
        public void Update()
        {
            dot = 1;
            for (int i = 0; i < bones.Length; ++i)
                dot *= bones[i].dot;
        }
    }

    private void Awake()
    {
        if(_ovrSkeleton == null)
        {
            _ovrSkeleton = GetComponent<OVRSkeleton>();
        }
    }

    private void Start()
    {
        
        if(_ovrSkeleton == null)
        {
            this.enabled = false;
            return;
        }

        //this.Initialize();
    }

    private void Initialize()
    {
        //Debug.Log("hello");
        _boneDots = new List<BoneDot>();
        _fingerDots = new List<FingerDot>();
        _ovrSkeleton = GetComponent<OVRSkeleton>();

        //Debug.Log(BoneDirectionIndex.Length);

        for (int i = 0; i < BoneDirectionIndex.Length / 3; i++)
        {
            var boneVis = new BoneDot(
                _ovrSkeleton.Bones[BoneDirectionIndex[i, 0]].Transform,
                _ovrSkeleton.Bones[BoneDirectionIndex[i, 1]].Transform,
                _ovrSkeleton.Bones[BoneDirectionIndex[i, 2]].Transform);

            _boneDots.Add(boneVis);
        }

        for (int i = 0; i < Enum.GetNames(typeof(FingerIndex)).Length; i++)
        {
            FingerDot Fing;
            if (i == 0)
                Fing = new FingerDot(_boneDots[0], _boneDots[1]);
            else
                Fing = new FingerDot(_boneDots[2 + 3 * (i - 1)], _boneDots[3 + 3 * (i - 1)], _boneDots[4 + 3 * (i - 1)]);
            _fingerDots.Add(Fing);
        }

        _isInitialized = true;
    }

    public void Update()
    {
        //Debug.Log(_ovrSkeleton.Bones.Count);
        if (_isInitialized)
        {
            for (int i = 0; i < _boneDots.Count; i++)
                _boneDots[i].Update();
            for (int i = 0; i < _fingerDots.Count; i++)
                _fingerDots[i].Update();
            this.handCenter = (_ovrSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Middle1].Transform.position + _ovrSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Middle3].Transform.position) / 2;
            this.handParallel = _ovrSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index1].Transform.position - _ovrSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Ring1].Transform.position;
        }else if(_ovrSkeleton.Bones.Count > 0)
            Initialize();
        
        
        
    }
}