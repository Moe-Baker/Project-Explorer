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
        [SerializeField]
        protected float baseOffset = 0.9f;
        public float BaseOffset { get { return baseOffset; } }

        Player player;
        public void Init(Player reference)
        {
            this.player = reference;

            player.CollisionEventsRewind.EnterEvent += OnPlayerCollisionEnter;

            Path = new NavMeshPath();
        }

        new public Rigidbody rigidbody { get { return player.rigidbody; } }

        public Vector3 Velocity
        {
            get
            {
                return Vector3.Scale(rigidbody.velocity, Vector3.right + Vector3.forward);
            }
            set
            {
                rigidbody.velocity = new Vector3(value.x, rigidbody.velocity.y, value.z);
            }
        }

        public PlayerLook Look { get { return player.Look; } }

        public Animator Animator { get { return player.Body.Animator; } }

        [SerializeField]
        protected float speed = 3f;
        public float Speed { get { return speed; } }

        [SerializeField]
        protected float acceleration;
        public float Acceleration { get { return acceleration; } }

        #region Distance
        public float TotalDistance { get; set; }

        public float DistanceLeft { get; set; }

        public float DistanceTraveled { get { return TotalDistance - DistanceLeft; } }

        public float DistanceRate { get { return DistanceTraveled / TotalDistance; } }
        #endregion

        public Vector3 Destination { get; protected set; }

        public NavMeshPath Path { get; protected set; }

        public void To(Vector3 target)
        {
            if (Vector3.Distance(target, Destination) <= 0.1 + 0.05f)
                return;

            if (IsProcessing) Stop();

            if (NavMesh.CalculatePath(player.transform.position, target, NavMesh.AllAreas, Path))
            {
                TotalDistance = DistanceLeft = CalculateDistance(Path);

                target = Path.corners.Last();

                if (OnMove != null) OnMove(target);

                coroutine = StartCoroutine(Procedure(target));
            }
            else
            {
                Debug.LogError("Navigation Error");
            }
        }
        public event Action<Vector3> OnMove;

        Coroutine coroutine;
        public bool IsProcessing { get { return coroutine != null; } }
        IEnumerator Procedure(Vector3 target)
        {
            Destination = target;

            var direction = Vector3.zero;

            while (true)
            {
                if (NavMesh.CalculatePath(player.transform.position, Destination, NavMesh.AllAreas, Path))
                {
                    DistanceLeft = CalculateDistance(Path);

                    if (rigidbody.velocity.magnitude * Time.deltaTime >= DistanceLeft)
                    {
                        DistanceLeft = 0f;

                        transform.position = Path.corners.Last() + Vector3.up * baseOffset;

                        Velocity = Vector3.zero;

                        break;
                    }
                    else
                    {
                        direction = (Path.corners[1] - player.transform.position + Vector3.up * baseOffset).normalized;

                        Look.At(direction);

                        var distanceMultiplier = Mathf.Clamp(DistanceLeft / 1f, 0.4f, 1f);

                        Velocity = Vector3.MoveTowards(Velocity, direction * speed * distanceMultiplier, acceleration * Time.deltaTime);
                    }
                }
                else
                {
                    Debug.LogError("Navigation Error");

                    break;
                }

                yield return null;
            }

            while(Look.At(direction) != 0f)
                yield return null;

            coroutine = null;
        }
        
        protected float CalculateDistance(NavMeshPath path)
        {
            var value = 0f;

            for (int i = 0; i < path.corners.Length - 1; i++)
                value += Vector3.Distance(path.corners[i], path.corners[i + 1]);

            return value;
        }

        void OnPlayerCollisionEnter(Collision collision)
        {
            if(collision.rigidbody == null)
            {

            }
            else
            {
                if (collision.rigidbody.isKinematic)
                {
                    var direction = (player.transform.position - collision.contacts.First().point).normalized;

                    To(player.transform.position + direction * 1f);
                }
            }
        }

        public void Stop()
        {
            TotalDistance = DistanceLeft = 0f;

            if (IsProcessing)
            {
                StopCoroutine(coroutine);

                coroutine = null;
            }
        }

        void Update()
        {
            Animator.SetFloat("Speed", rigidbody.velocity.magnitude * Mathf.Clamp01(DistanceLeft / 0.25f));
        }
    }
}