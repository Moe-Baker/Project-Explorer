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
	public class PlayerHandsIK : MonoBehaviour
	{
        [SerializeField]
        protected CustomIKController right;
        public CustomIKController Right { get { return right; } }

        [SerializeField]
        protected CustomIKController left;
        public CustomIKController Left { get { return left; } }

        [Serializable]
        public class CustomIKController
        {
            public IKController Controller { get; protected set; }

            public Transform Transform { get; protected set; }

            public virtual void Init(Animator animator, AvatarIKGoal goal, Transform transform)
            {
                Controller = new IKController(animator, goal);

                this.Transform = transform;
            }

            public float heightOffset = 0.25f;

            public float normalOffset = 0f;

            public SpeedData speed = new SpeedData(new SpeedData.WeightData(2f), new SpeedData.PositionData(2f, 8f));
            [Serializable]
            public struct SpeedData
            {
                [SerializeField]
                WeightData weight;
                public WeightData Weight { get { return weight; } }
                [Serializable]
                public struct WeightData
                {
                    [SerializeField]
                    float set;
                    public float Set { get { return set; } }

                    [SerializeField]
                    float reset;
                    public float Reset { get { return reset; } }

                    public WeightData(float set, float reset)
                    {
                        this.set = set;
                        this.reset = reset;
                    }

                    public WeightData(float value) : this(value, value)
                    {

                    }
                }

                [SerializeField]
                PositionData position;
                public PositionData Position { get { return position; } }
                [Serializable]
                public struct PositionData
                {
                    [SerializeField]
                    float minimum;
                    public float Minimum { get { return minimum; } }

                    [SerializeField]
                    float maximum;
                    public float Maximum { get { return maximum; } }

                    public PositionData(float minimum, float maximum)
                    {
                        this.minimum = minimum;
                        this.maximum = maximum;
                    }
                }

                public SpeedData(WeightData weight, PositionData position)
                {
                    this.weight = weight;
                    this.position = position;
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

            Vector3 targetPoint;
            Vector3 targetLocalPoint;

            Vector3 localPoint;

            public void Process()
            {
                var weightTarget = 0f;

                if(target == null)
                {
                    targetLocalPoint = Transform.InverseTransformPoint(Controller.BoneTransform.position);

                    localPoint = Vector3.Lerp(targetLocalPoint, localPoint, Controller.Weight);

                    Controller.Position = Transform.TransformPoint(localPoint);

                    weightTarget = 0f;
                }
                else
                {
                    targetPoint = target.Point;

                    targetPoint.y = Transform.position.y + heightOffset;
                    targetPoint += target.Normal * normalOffset;

                    targetLocalPoint = Transform.InverseTransformPoint(targetPoint);

                    var positionDelta = Mathf.Lerp(speed.Position.Maximum, speed.Position.Minimum, Controller.Weight);
                    localPoint = Vector3.MoveTowards(localPoint, targetLocalPoint, positionDelta * Time.deltaTime);

                    Controller.Position = Transform.TransformPoint(localPoint);
                    weightTarget = 1f;
                }

                var weightDelta = weightTarget < Controller.Weight ? speed.Weight.Reset : speed.Weight.Set;
                Controller.Weight = Mathf.MoveTowards(Controller.Weight, weightTarget, weightDelta * Time.deltaTime);
            }
        }

        Player player;
        public PlayerBody Body { get { return player.Body; } }
        public Animator Animator { get { return Body.Animator; } }

        public void Init(Player reference)
        {
            this.player = reference;

            player.CollisionEventsRewind.EnterEvent += OnPlayerCollisionEnter;
            player.CollisionEventsRewind.StayEvent += OnPlayerCollisionStay;
            player.CollisionEventsRewind.ExitEvent += OnPlayerCollisionExit;

            Body.AnimatorIKEvent += AnimatorIK;

            right.Init(Animator, AvatarIKGoal.RightHand, reference.transform);
            left.Init(Animator, AvatarIKGoal.LeftHand, reference.transform);
        }


        void OnPlayerCollisionEnter(Collision collision)
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

        void OnPlayerCollisionStay(Collision collision)
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

        void OnPlayerCollisionExit(Collision collision)
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