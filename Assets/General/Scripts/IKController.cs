using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game
{
    public class IKController
    {
        public Animator Animator { get; protected set; }

        public AvatarIKGoal Goal { get; protected set; }

        public HumanBodyBones Bone { get; protected set; }
        public Transform BoneTransform { get; protected set; }

        float weight = 0f;
        public float Weight
        {
            get
            {
                return weight;
            }
            set
            {
                weight = Mathf.Clamp01(value);
                Animator.SetIKPositionWeight(Goal, weight);
            }
        }

        public Vector3 Position
        {
            get
            {
                return Animator.GetIKPosition(Goal);
            }
            set
            {
                Animator.SetIKPosition(Goal, value);
            }
        }

        public IKController(Animator animator, AvatarIKGoal goal)
        {
            this.Animator = animator;
            this.Goal = goal;

            switch (goal)
            {
                case AvatarIKGoal.LeftFoot:
                    Bone = HumanBodyBones.LeftFoot;
                    break;

                case AvatarIKGoal.RightFoot:
                    Bone = HumanBodyBones.RightFoot;
                    break;

                case AvatarIKGoal.LeftHand:
                    Bone = HumanBodyBones.LeftHand;
                    break;

                case AvatarIKGoal.RightHand:
                    Bone = HumanBodyBones.RightHand;
                    break;
            }

            BoneTransform = animator.GetBoneTransform(Bone);
        }
    }
}