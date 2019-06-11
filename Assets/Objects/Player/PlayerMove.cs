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
    public class PlayerMove : MonoBehaviour
    {
        Player player;
        public void Init(Player player)
        {
            this.player = player;
        }

        public NavMeshAgent NavAgent { get { return player.NavAgent; } }
        public Vector3 Destination { get { return NavAgent.destination; } }

        new public Rigidbody rigidbody { get { return player.rigidbody; } }

        public Animator Animator { get { return player.Body.Animator; } }

        public SpeedData speed = new SpeedData(5f);
        [Serializable]
        public struct SpeedData
        {
            [SerializeField]
            float value;
            public float Value { get { return value; } }

            [SerializeField]
            AnimationCurve distanceMultiplier;
            public AnimationCurve DistanceMultipler { get { return distanceMultiplier; } }

            public float Evaluate(float distance)
            {
                return value * distanceMultiplier.Evaluate(distance);
            }

            public SpeedData(float value)
            {
                this.value = value;

                distanceMultiplier = new AnimationCurve()
                {
                    keys = new Keyframe[] { new Keyframe(0f, 0.2f), new Keyframe(2f, 1f) },
                    postWrapMode = WrapMode.Clamp,
                    preWrapMode = WrapMode.Clamp
                };
            }
        }

        #region Distance
        public float TotalDistance { get; set; }

        public float DistanceLeft { get; set; }

        public float DistanceTraveled { get { return TotalDistance - DistanceLeft; } }

        public float DistanceRate { get { return DistanceTraveled / TotalDistance; } }
        #endregion

        public void To(Vector3 target)
        {
            if (Vector3.Distance(target, Destination) <= 0.1 + 0.05f)
                return;

            TotalDistance = DistanceLeft = Vector3.Distance(player.GroundPosition, target);

            if (IsProcessing)
                NavAgent.SetDestination(target);
            else
                coroutine = StartCoroutine(Procedure(target));
        }

        Coroutine coroutine;
        public bool IsProcessing { get { return coroutine != null; } }
        IEnumerator Procedure(Vector3 destination)
        {
            NavAgent.isStopped = false;
            NavAgent.SetDestination(destination);

            rigidbody.isKinematic = false;

            while (true)
            {
                DistanceLeft = Vector3.Distance(player.GroundPosition, Destination);

                NavAgent.speed = speed.Evaluate(DistanceLeft);

                rigidbody.velocity = NavAgent.velocity;

                if(NavAgent.velocity.magnitude * Time.deltaTime >= DistanceLeft)
                {
                    rigidbody.velocity = Vector3.zero;
                    rigidbody.position = NavAgent.path.corners[0];
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            DistanceLeft = 0f;

            rigidbody.isKinematic = true;

            NavAgent.isStopped = true;
            coroutine = null;
        }

        public void Stop()
        {
            if (IsProcessing)
            {
                NavAgent.isStopped = true;
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        void Update()
        {
            Animator.SetFloat("Speed", NavAgent.velocity.magnitude);
        }
    }
}