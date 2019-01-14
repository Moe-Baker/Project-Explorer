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
	public class PlayerArmRestController : MonoBehaviour
	{
        public IKController RightHandIK { get; protected set; }
        public float RightHandIKWeightGoal { get; protected set; }

        public IKController LeftHandIK { get; protected set; }
        public float LeftHandIKWeightGoal { get; protected set; }

        public Player player;

        public IkSpeedData speed = new IkSpeedData(2f, 1f);
        [Serializable]
        public struct IkSpeedData
        {
            [SerializeField]
            float set;
            public float Set { get { return set; } }

            [SerializeField]
            float reset;
            public float Reset { get { return reset; } }

            public float Get(float target)
            {
                return Mathf.Approximately(target, 0f) ? reset : set;
            }

            public IkSpeedData(float set, float reset)
            {
                this.set = set;
                this.reset = reset;
            }
        }

        public float heightOffset = 0.1f;

        public float normalOffset = 0.02f;

        public PlayerBody Body { get { return player.Body; } }
        public Animator Animator { get { return Body.Animator; } }

        void Start()
        {
            Body.AnimatorIKEvent += AnimatorIK;

            RightHandIK = new IKController(Animator, AvatarIKGoal.RightHand);
            LeftHandIK = new IKController(Animator, AvatarIKGoal.LeftHand);
        }


        void OnCollisionEnter(Collision collision)
        {
            var target = collision.gameObject.GetComponent<IPlayerArmRest>();

            if (target == null) return;
            if (target == Target) return;

            Target = target;
        }

        void OnCollisionStay(Collision collision)
        {
            if (Target == null) return;

            if (collision.gameObject == Target.gameObject)
            {
                contact = collision.contacts.First().point;
                contact.y = transform.position.y + heightOffset;

                contact += collision.contacts.First().normal * normalOffset;
            }

            localContact = transform.InverseTransformPoint(contact);
        }

        void OnCollisionExit(Collision collision)
        {
            var target = collision.gameObject.GetComponent<IPlayerArmRest>();

            if (target == null) return;

            if (target == Target)
                Target = null;
        }


        public IPlayerArmRest Target { get; protected set; }

        Vector3 contact;
        Vector3 localContact;


        void AnimatorIK(int layerIndex)
        {
            if (Target == null || localContact.z < -0.2f)
            {
                RightHandIKWeightGoal = 0f;
                LeftHandIKWeightGoal = 0f;

                if (localContact.x > 0f) //Right Hand
                {
                    RightHandIK.Position = transform.TransformPoint(localContact);
                }
                else //Left Hand
                {
                    LeftHandIK.Position = transform.TransformPoint(localContact);
                }
            }
            else
            {
                if (localContact.x > 0f) //Right Hand
                {
                    RightHandIK.Position = contact;
                    RightHandIKWeightGoal = 1f;

                    LeftHandIKWeightGoal = 0f;
                }
                else //Left Hand
                {
                    LeftHandIK.Position = contact;
                    LeftHandIKWeightGoal = 1f;

                    RightHandIKWeightGoal = 0f;
                }
            }

            RightHandIK.Weight = Mathf.MoveTowards(RightHandIK.Weight, RightHandIKWeightGoal, speed.Get(RightHandIKWeightGoal) * Time.deltaTime);

            LeftHandIK.Weight = Mathf.MoveTowards(LeftHandIK.Weight, LeftHandIKWeightGoal, speed.Get(LeftHandIKWeightGoal) * Time.deltaTime);
        }


        void OnDrawGizmos()
        {
            if(Target != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(contact, 0.2f);
            }
        }
    }
}