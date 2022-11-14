using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pseudogame.game
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
            
            
        }

        // Update is called once per frame
        void Update()
        {

            
        }

        void OnAnimatorIK()
        {
            if (IKTarget == null)
            {
                return ;
            }

            animationTarget.SetIKPositionWeight(AvatarIKGoal.RightHand,1.0f);
            animationTarget.SetIKRotationWeight(AvatarIKGoal.RightHand,1.0f);

            Quaternion palmUp = Quaternion.Euler(0,0,180); 
            animationTarget.SetIKPosition(AvatarIKGoal.RightHand,IKTarget.position);
            animationTarget.SetIKRotation(AvatarIKGoal.RightHand,palmUp);
        }
    }
}