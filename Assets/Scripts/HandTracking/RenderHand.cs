using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UniRx;
//using UniRx.Triggers;


namespace handTracking 
{
    public class RenderHand : MonoBehaviour
    {
        // Start is called before the first frame update
        //[SerializeField] private GameObject _headVisual;
        [SerializeField] private GameObject _handVisual;
        //[SerializeField] private GameObject _hand;

        private readonly List<Transform> _bonesArm = new List<Transform>();
        private List<Transform> _listOfChildren = new List<Transform>();

        private List<OVRBone> _bones;
        private List<OVRBone> _bindPoses;

        private SkinnedMeshRenderer _skinMeshRenderer;

        private OVRSkeleton.SkeletonPoseData _data;

        private bool _isInitializedBone;
        private bool _isInitializedHand;

        private OVRSkeleton.IOVRSkeletonDataProvider dataProvider;
        private OVRPlugin.Skeleton skeleton;

        private Quaternion wristFixupRotation;

        

        //public IList<OVRBone> Bones { get; protected set; }
        
        private void Awake()
        {
            //Bones = _bones.AsReadOnly();
        }

        void Start()
        {
            _skinMeshRenderer = _handVisual.GetComponent<SkinnedMeshRenderer>();

            var ovrSkeleton = this.GetComponent<OVRSkeleton>();
            dataProvider = ovrSkeleton.GetComponent<OVRSkeleton.IOVRSkeletonDataProvider>();

            skeleton = new OVRPlugin.Skeleton();
            OVRPlugin.GetSkeleton((OVRPlugin.SkeletonType) dataProvider.GetSkeletonType(), out skeleton);
            InitializeBones(skeleton,_handVisual,out _isInitializedBone);

            ReadyHand(_handVisual,_bonesArm,out _isInitializedHand);
            
            wristFixupRotation = new Quaternion(0.0f,-1.0f,0.0f,0.0f);


            _skinMeshRenderer.enabled = true;
        }

        void Update()
        {
            if(!_isInitializedBone)
            {
                OVRPlugin.GetSkeleton((OVRPlugin.SkeletonType) dataProvider.GetSkeletonType(), out skeleton);
                InitializeBones(skeleton,_handVisual,out _isInitializedBone);
            }

            if(!_isInitializedHand)
            {
                ReadyHand(_handVisual,_bonesArm,out _isInitializedHand);
            }

            //Debug.Log(_bonesArm.Count);

            _data = dataProvider.GetSkeletonPoseData();
            
            if(_data.IsDataValid && _data.IsDataHighConfidence)
            {
                _handVisual.transform.localPosition = _data.RootPose.Position.FromFlippedZVector3f();
                _handVisual.transform.localRotation = _data.RootPose.Orientation.FromFlippedZQuatf();
                _handVisual.transform.localScale = new Vector3(_data.RootScale,_data.RootScale,_data.RootScale);
                
                
                for(int i = 0; i < _bonesArm.Count; ++i)
                {
                    _bonesArm[i].transform.localRotation = _data.BoneRotations[i].FromFlippedXQuatf();

                    if(_bonesArm[i].name == OVRSkeleton.BoneId.Hand_WristRoot.ToString())
                    {
                        _bonesArm[i].transform.localRotation *= wristFixupRotation;
                    }
                }
                
                
            }
            /*
            this.transform.localPosition = _handVisual.transform.localPosition;
            this.transform.localRotation = _handVisual.transform.localRotation;
            Debug.Log(_handVisual.GetComponent<OVRSkeleton>().Bones.Count);
            Debug.Log(_bonesArm.Count);

            for(int i = 0;i < _bonesArm.Count; ++i)
            {
                _bonesArm[i].transform.localRotation = _handVisual.GetComponent<OVRSkeleton>().Bones[i].Transform.rotation;
            }
            */
            
            
        }

        private void ReadyHand(GameObject hand, List<Transform> bones, out bool isInitialize)
        {
            
            //'Bones'と名の付くオブジェクトからリストを作成する
            foreach (Transform child in hand.transform)
            {
                _listOfChildren = new List<Transform>();
                GetChildRecursive(child.transform);

                //まずは指先以外のリストを作成
                var fingerTips = new List<Transform>();
                foreach (var bone in _listOfChildren)
                {
                    if (bone.name.Contains("Tip"))
                    {
                        fingerTips.Add(bone);
                    }
                    else
                    {
                        bones.Add(bone);
                    }
                }

                //指先もリストに追加
                bones.AddRange(fingerTips);
            }

            var skinMeshRenderer = _handVisual.GetComponent<SkinnedMeshRenderer>();
            var ovrMesh = _handVisual.GetComponent<OVRMesh>();

            var bindPoses = new Matrix4x4[bones.Count];
            var localToWorldMatrix = transform.localToWorldMatrix;
            Debug.Log("bones.Count");
            Debug.Log(bones.Count);
            for(var i = 0;i < bones.Count; ++i)
            {
                bindPoses[i] = bones[i].worldToLocalMatrix * localToWorldMatrix;
            }
            ovrMesh.Mesh.bindposes = bindPoses;
            skinMeshRenderer.bones = bones.ToArray();
            skinMeshRenderer.sharedMesh = ovrMesh.Mesh;

            isInitialize = true;
        }

        private void GetChildRecursive(Transform obj)
        {
            if (null == obj) return;

            foreach (Transform child in obj.transform)
            {
                if (null == child)
                    continue;

                if (child != obj)
                {
                    _listOfChildren.Add(child);
                }

                GetChildRecursive(child);
            }
        }

        private void InitializeBones(OVRPlugin.Skeleton skeleton, GameObject hand, out bool isInitialize)
        {
            _bones = new List<OVRBone>(new OVRBone[skeleton.NumBones]);

            var bonesGO = new GameObject("Bones");
            bonesGO.transform.SetParent(hand.transform, false);
            bonesGO.transform.localPosition = Vector3.zero;
            bonesGO.transform.localRotation = Quaternion.identity;

            for (var i = 0; i < skeleton.NumBones; ++i)
            {
                var id = (OVRSkeleton.BoneId) skeleton.Bones[i].Id;
                var parentIdx = skeleton.Bones[i].ParentBoneIndex;
                var pos = skeleton.Bones[i].Pose.Position.FromFlippedXVector3f();
                var rot = skeleton.Bones[i].Pose.Orientation.FromFlippedXQuatf();

                var boneGO = new GameObject(id.ToString());
                boneGO.transform.localPosition = pos;
                boneGO.transform.localRotation = rot;
                _bones[i] = new OVRBone(id, parentIdx, boneGO.transform);
            }

            for (var i = 0; i < skeleton.NumBones; ++i)
            {
                if (((OVRPlugin.BoneId) skeleton.Bones[i].ParentBoneIndex) == OVRPlugin.BoneId.Invalid)
                {
                    _bones[i].Transform.SetParent(bonesGO.transform, false);
                }
                else
                {
                    _bones[i].Transform.SetParent(_bones[_bones[i].ParentBoneIndex].Transform, false);
                }
            }
            /*
            Debug.Log("skeleton.Numbones");
            Debug.Log(skeleton.NumBones);
            */

            for(int i = 0;i < skeleton.NumBones; ++i)
            {

            }

            isInitialize = true;
        }


        // Update is called once per frame
        
    }
}
