using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game
{
    public class OpponentAvatar : MonoBehaviour
    {

        private Animator animationTarget;
        private HumanPose targetHumanPose;

        [SerializeField]
        private Transform IKTarget;
        [SerializeField]
        private Transform leftControllerAnchor;

        
    
        // Start is called before the first frame update
        void Start()
        {
            animationTarget = GetComponent<Animator>();
            animationTarget.enabled = false;
            HumanPoseHandler handler = new HumanPoseHandler(animationTarget.avatar, animationTarget.transform);
            HumanPose humanpose = new HumanPose();
            handler.GetHumanPose(ref humanpose);

            for(int i = 55;i <= 94; ++i)
            {
                humanpose.muscles[i] = -1.0f;
            }

            handler.SetHumanPose(ref humanpose);

            animationTarget.enabled = true;
            
        }

        // Update is called once per frame
        void Update()
        {
            /*
            Vector3 pos = IKTarget.transform.position;
            pos.y = leftControllerAnchor.position.y;
            IKTarget.transform.position = pos;
            */

            animationTarget = GetComponent<Animator>();
            animationTarget.enabled = false;
            HumanPoseHandler handler = new HumanPoseHandler(animationTarget.avatar, animationTarget.transform);
            HumanPose humanpose = new HumanPose();
            handler.GetHumanPose(ref humanpose);

            for(int i = 55;i <= 94; ++i)
            {
                humanpose.muscles[i] = -1.0f;
            }

            handler.SetHumanPose(ref humanpose);

            animationTarget.enabled = true;

            
        }

        void OnAnimatorIK()
        {
            if (IKTarget == null)
            {
                return ;
            }

            animationTarget.SetIKPositionWeight(AvatarIKGoal.RightHand,1.0f);
            animationTarget.SetIKRotationWeight(AvatarIKGoal.RightHand,1.0f);

            Quaternion palmUp = Quaternion.Euler(0,0,180); //* Quaternion.Euler(180,0,0);
            animationTarget.SetIKPosition(AvatarIKGoal.RightHand,IKTarget.position);
            animationTarget.SetIKRotation(AvatarIKGoal.RightHand,palmUp);
        }
    }
}