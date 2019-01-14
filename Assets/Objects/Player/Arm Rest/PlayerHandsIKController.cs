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
	public class PlayerHandsIKController : MonoBehaviour
	{
        public Player player;

        public PlayerBody Body { get { return player.Body; } }
        public Animator Animator { get { return Body.Animator; } }

        [SerializeField]
        protected CustomIKController right;
        public CustomIKController Right { get { return right; } }

        [SerializeField]
        protected CustomIKController left;
        public CustomIKController Left { get { return left; } }

        [Serializable]
        public class CustomIKController
        {
            public IKController IKController { get; protected set; }

            public Transform Transform { get; protected set; }

            public virtual void Init(Animator animator, AvatarIKGoal goal, Transform transform)
            {
                IKController = new IKController(animator, goal);

                this.Transform = transform;
            }

            protected Collision collision;
            public Collision Collision
            {
                get
                {
                    return collision;
                }
                set
                {
                    collision = value;
                }
            }

            public float heightOffset = 0.25f;

            public float normalOffset = 0f;

            public SpeedData speed = new SpeedData(2f, 2f);
            [Serializable]
            public struct SpeedData
            {
                [SerializeField]
                float set;
                public float Set { get { return set; } }

                [SerializeField]
                float reset;
                public float Reset { get { return reset; } }

                public SpeedData(float set, float reset)
                {
                    this.set = set;
                    this.reset = reset;
                }
            }

            protected TargetData target;
            public TargetData Target
            {
                get
                {
                    return target;
                }
                set
                {
                    target = value;
                }
            }

            Vector3 point;
            Vector3 localPoint;

            public void Process()
            {
                var weightTarget = 0f;

                if(target == null)
                {
                    weightTarget = 0f;

                    IKController.Position = Transform.TransformPoint(localPoint);
                }
                else
                {
                    point = target.Point;
                    point.y = Transform.position.y + heightOffset;

                    point += target.Normal * normalOffset;

                    localPoint = Transform.InverseTransformPoint(point);

                    IKController.Position = point;
                    weightTarget = 1f;
                }

                var delta = weightTarget < IKController.Weight ? speed.Reset : speed.Set;
                IKController.Weight = Mathf.MoveTowards(IKController.Weight, weightTarget, delta * Time.deltaTime);
            }
        }


        void Start()
        {
            Body.AnimatorIKEvent += AnimatorIK;

            right.Init(Animator, AvatarIKGoal.RightHand, player.transform);
            left.Init(Animator, AvatarIKGoal.LeftHand, player.transform);
        }


        void OnCollisionEnter(Collision collision)
        {
            var target = collision.gameObject.GetComponent<IPlayerHandsIKTarget>();

            if (target == null) return;

            if (!target.Active) return;

            for (int i = 0; i < Targets.Count; i++)
            {
                if (Targets[i].Collision.gameObject == collision.gameObject)
                    break;
            }

            Targets.Add(new TargetData(collision));
        }

        void OnCollisionStay(Collision collision)
        {
            var target = collision.gameObject.GetComponent<IPlayerHandsIKTarget>();

            if (target == null) return;

            for (int i = 0; i < Targets.Count; i++)
            {
                if (Targets[i].Collision.gameObject == collision.gameObject)
                {
                    if (target.Active)
                        Targets[i].Collision = collision;
                    else
                        Targets.RemoveAt(i);

                    return;
                }
            }

            if(target.Active)
                Targets.Add(new TargetData(collision));
        }

        void OnCollisionExit(Collision collision)
        {
            var target = collision.gameObject.GetComponent<IPlayerHandsIKTarget>();

            if (target == null) return;

            for (int i = 0; i < Targets.Count; i++)
            {
                if (Targets[i].Collision.gameObject == collision.gameObject)
                {
                    Targets.RemoveAt(i);
                    break;
                }
            }
        }


        public List<TargetData> Targets { get; protected set; } = new List<TargetData>();
        [Serializable]
        public class TargetData
        {
            protected Collision collision;
            public Collision Collision
            {
                get
                {
                    return collision;
                }
                set
                {
                    collision = value;
                }
            }

            public ContactPoint Contact { get { return collision.contacts.First(); } }

            public Vector3 Point { get { return Contact.point; } }

            public Vector3 Normal { get { return Contact.normal; } }

            public TargetData(Collision collision)
            {
                this.Collision = collision;
            }
        }


        void AnimatorIK(int layerIndex)
        {
            if (!Targets.Contains(right.Target))
                right.Target = null;

            if (!Targets.Contains(left.Target))
                left.Target = null;

            foreach (var target in Targets)
            {
                var localPoint = player.transform.InverseTransformPoint(target.Point);

                if (localPoint.z < -0.4f)
                {
                    if (right.Target == target)
                        right.Target = null;

                    if (left.Target == target)
                        left.Target = null;
                }
                else
                {
                    if (localPoint.x >= 0f) //Right Hand
                    {
                        if (right.Target == target)
                            continue;
                        else
                            right.Target = target;

                        if (left.Target == target)
                            left.Target = null;
                    }
                    else //Left Hand
                    {
                        if (left.Target == target)
                            continue;
                        else
                            left.Target = target;

                        if (right.Target == target)
                            right.Target = null;
                    }
                }
            }

            right.Process();
            left.Process();
        }

        void OnDrawGizmos()
        {
            
        }
    }
}