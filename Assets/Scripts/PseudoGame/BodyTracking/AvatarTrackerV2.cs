using System.Collections.Generic;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;


public class AvatarTrackerV2 : MonoBehaviour
{
    [SerializeField] private TrackerHandler trackerHandler;
    private Animator animator;
    [SerializeField] private GameObject pelvis;

    void Awake()
    {
        this.animator = this.GetComponent<Animator>();
    }
    void Update()
    {
        MoveBody();
    }

    private void MoveBody()
    {
        transform.localPosition = pelvis.transform.localPosition;
        transform.localRotation = pelvis.transform.localRotation;

        

        foreach(var bone in upperBodyBoneMap)
        {
            var nextRot = trackerHandler.GetRelativeJointRotation(bone.Value);
            var prevRot = animator.GetBoneTransform(bone.Key).localRotation;
            animator.GetBoneTransform(bone.Key).localRotation = new Quaternion(nextRot.x, nextRot.y, nextRot.z, nextRot.w);
        }

        foreach(var bone in lowerBodyBoneMap)
        {
            var nextRot = trackerHandler.GetRelativeJointRotation(bone.Value);
            var prevRot = animator.GetBoneTransform(bone.Key).localRotation;
            animator.GetBoneTransform(bone.Key).localRotation = new Quaternion(nextRot.x, nextRot.y, nextRot.z, nextRot.w);
        }

        foreach(var bone in leftHandBoneMap)
        {
            var nextRotZ = trackerHandler.GetRelativeJointRotation(bone.Value).eulerAngles.z;
            var nextZ = nextRotZ <= 0 ? 0 : nextRotZ > 70 ? 70 : nextRotZ;
            var nextRotVec = new Vector3(0, 0, nextZ);
            var nextRot = Quaternion.Euler(nextRotVec);
            animator.GetBoneTransform(bone.Key).localRotation = new Quaternion(nextRot.x, nextRot.y, nextRot.z, nextRot.w);
        }

        foreach(var bone in rightHandBoneMap)
        {
            var nextRotZ = trackerHandler.GetRelativeJointRotation(bone.Value).eulerAngles.z;
            var nextZ = 0 >= nextRotZ ? 0 : nextRotZ >= 360 ? 360 : nextRotZ < 290 ? 290 : nextRotZ;
            var nextRotVec = new Vector3(0, 0, nextZ);
            var nextRot = Quaternion.Euler(nextRotVec);
            animator.GetBoneTransform(bone.Key).localRotation = new Quaternion(nextRot.x, nextRot.y, nextRot.z, nextRot.w);
        }

        foreach(var bone in leftThumbBoneMap)
        {
            var nextRotX = trackerHandler.GetRelativeJointRotation(bone.Value).eulerAngles.x;
            var nextX = nextRotX <= 0 ? 0 : nextRotX > 70 ? 70 : nextRotX;
            var nextRotVec = new Vector3(nextX, 0, 0);
            var nextRot = Quaternion.Euler(nextRotVec);
            animator.GetBoneTransform(bone.Key).localRotation = new Quaternion(nextRot.x, nextRot.y, nextRot.z, nextRot.w);
        }

        foreach(var bone in rightThumbBoneMap)
        {
            var nextRotX = trackerHandler.GetRelativeJointRotation(bone.Value).eulerAngles.x;
            var nextX = 0 >= nextRotX ? 0 : nextRotX >= 360 ? -360 : nextRotX < 290 ? -290 : -nextRotX;
            var nextRotVec = new Vector3(nextX, 0, 0);
            var nextRot = Quaternion.Euler(nextRotVec);
            animator.GetBoneTransform(bone.Key).localRotation = new Quaternion(nextRot.x, nextRot.y, nextRot.z, nextRot.w);
        }
    }

    private readonly Dictionary<HumanBodyBones, JointId> upperBodyBoneMap =
        new Dictionary<HumanBodyBones, JointId>
        {
            {HumanBodyBones.Head, JointId.Head},
            {HumanBodyBones.Neck, JointId.Neck},
            {HumanBodyBones.Chest, JointId.SpineChest},
            {HumanBodyBones.Spine, JointId.SpineNavel},
            {HumanBodyBones.LeftShoulder, JointId.ClavicleLeft},
            {HumanBodyBones.RightShoulder, JointId.ClavicleRight},
            {HumanBodyBones.LeftUpperArm, JointId.ShoulderLeft},
            {HumanBodyBones.RightUpperArm, JointId.ShoulderRight},
            {HumanBodyBones.LeftLowerArm, JointId.ElbowLeft},
            {HumanBodyBones.RightLowerArm, JointId.ElbowRight},
            {HumanBodyBones.LeftHand, JointId.WristLeft},
            {HumanBodyBones.RightHand, JointId.WristRight},
        };

    private readonly Dictionary<HumanBodyBones, JointId> lowerBodyBoneMap =
        new Dictionary<HumanBodyBones, JointId>
        {
            {HumanBodyBones.Hips, JointId.SpineNavel},
            {HumanBodyBones.LeftUpperLeg, JointId.HipLeft},
            {HumanBodyBones.RightUpperLeg, JointId.HipRight},
            {HumanBodyBones.LeftLowerLeg, JointId.KneeLeft},
            {HumanBodyBones.RightLowerLeg, JointId.KneeRight},
            {HumanBodyBones.LeftFoot, JointId.FootLeft},
            {HumanBodyBones.RightFoot, JointId.FootRight},
            {HumanBodyBones.LeftToes, JointId.FootLeft},
            {HumanBodyBones.RightToes, JointId.FootRight},
        };

    private readonly Dictionary<HumanBodyBones, JointId> leftHandBoneMap =
        new Dictionary<HumanBodyBones, JointId>
        {
            {HumanBodyBones.LeftIndexProximal, JointId.HandLeft},
            {HumanBodyBones.LeftIndexIntermediate, JointId.HandLeft},
            {HumanBodyBones.LeftIndexDistal, JointId.HandLeft},
            {HumanBodyBones.LeftMiddleProximal, JointId.HandLeft},
            {HumanBodyBones.LeftMiddleIntermediate, JointId.HandLeft},
            {HumanBodyBones.LeftMiddleDistal, JointId.HandLeft},
            {HumanBodyBones.LeftRingProximal, JointId.HandLeft},
            {HumanBodyBones.LeftRingIntermediate, JointId.HandLeft},
            {HumanBodyBones.LeftRingDistal, JointId.HandLeft},
            {HumanBodyBones.LeftLittleProximal, JointId.HandLeft},
            {HumanBodyBones.LeftLittleIntermediate, JointId.HandLeft},
            {HumanBodyBones.LeftLittleDistal, JointId.HandLeft},
        };

    private readonly Dictionary<HumanBodyBones, JointId> rightHandBoneMap =
        new Dictionary<HumanBodyBones, JointId>
        {
            {HumanBodyBones.RightIndexProximal, JointId.HandRight},
            {HumanBodyBones.RightIndexIntermediate, JointId.HandRight},
            {HumanBodyBones.RightIndexDistal, JointId.HandRight},
            {HumanBodyBones.RightMiddleProximal, JointId.HandRight},
            {HumanBodyBones.RightMiddleIntermediate, JointId.HandRight},
            {HumanBodyBones.RightMiddleDistal, JointId.HandRight},
            {HumanBodyBones.RightRingProximal, JointId.HandRight},
            {HumanBodyBones.RightRingIntermediate, JointId.HandRight},
            {HumanBodyBones.RightRingDistal, JointId.HandRight},
            {HumanBodyBones.RightLittleProximal, JointId.HandRight},
            {HumanBodyBones.RightLittleIntermediate, JointId.HandRight},
            {HumanBodyBones.RightLittleDistal, JointId.HandRight},
        };

    private readonly Dictionary<HumanBodyBones, JointId> leftThumbBoneMap =
        new Dictionary<HumanBodyBones, JointId>
        {
            {HumanBodyBones.LeftThumbProximal, JointId.ThumbLeft},
            {HumanBodyBones.LeftThumbIntermediate, JointId.ThumbLeft},
            {HumanBodyBones.LeftThumbDistal, JointId.ThumbLeft},
        };

    private readonly Dictionary<HumanBodyBones, JointId> rightThumbBoneMap =
        new Dictionary<HumanBodyBones, JointId>
        {
            {HumanBodyBones.RightThumbProximal, JointId.ThumbRight},
            {HumanBodyBones.RightThumbIntermediate, JointId.ThumbRight},
            {HumanBodyBones.RightThumbDistal, JointId.ThumbRight},
        };

    /* NOT ASSIGN */
    // {HumanBodyBones.UpperChest, JointId.SpineChest},
    // {HumanBodyBones.LeftEye, JointId.EyeLeft},
    // {HumanBodyBones.RightEye, JointId.EyeRight},
    // {HumanBodyBones.Jaw, JointId.Nose},
}