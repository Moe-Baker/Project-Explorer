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
        protected float speed = 3f;
        public float Speed { get { return speed; } }

        [SerializeField]
        protected float acceleration = 4;
        public float Acceleration { get { return acceleration; } }

        Player player;
        public void Init(Player reference)
        {
            this.player = reference;

            Destination = player.transform.position + Vector3.down * HeightOffset;

            player.CollisionEventsRewind.EnterEvent += OnPlayerCollisionEnter;

            Path = new NavMeshPath();
        }

        new public CapsuleCollider collider { get { return player.collider; } }
        public float HeightOffset { get { return collider.height / 2f; } }

        new public Rigidbody rigidbody { get { return player.rigidbody; } }
        public PlayerGround Ground { get { return player.Ground; } }
        public PlayerLook Look { get { return player.Look; } }
        public PlayerHandsIK HandsIK { get { return player.HandsIK; } }
        public Animator Animator { get { return player.Body.Animator; } }

        #region Distance
        public float TotalDistance { get; set; }

        public float DistanceLeft { get; set; }
        public float DistanceTraveled { get { return TotalDistance - DistanceLeft; } }

        public float DistanceRate { get { return DistanceTraveled / TotalDistance; } }
        #endregion

        public Vector3 Destination { get; protected set; }

        public NavMeshPath Path { get; protected set; }

        public Vector3 Velocity { get; protected set; }

        public event Action<Vector3> OnMove;
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

                    if (Velocity.magnitude * Time.deltaTime < DistanceLeft)
                    {
                        direction = (Path.corners[1] - player.transform.position); direction.y = 0f;
                        direction = direction.normalized;

                        Look.At(direction);

                        var targetVelocity = direction * speed * Mathf.Clamp(DistanceLeft / 0.5f, 0.4f, 1f);

                        Velocity = Vector3.MoveTowards(Velocity, targetVelocity, acceleration * Time.deltaTime);
                    }
                    else
                    {
                        DistanceLeft = 0f;

                        var position = player.transform.position;
                        {
                            position.x = Path.corners.Last().x;
                            position.z = Path.corners.Last().z;
                        }
                        player.transform.position = position;

                        Velocity = Vector3.zero;

                        break;
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

        void Update()
        {
            Animator.SetFloat("Speed", Velocity.magnitude * Mathf.Clamp01(DistanceLeft / 0.25f));
        }

        void FixedUpdate()
        {
            rigidbody.MovePosition(rigidbody.position + Ground.ProjectOnPlane(Velocity) * Time.fixedDeltaTime);
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

        void OnDrawGizmos()
        {
            if(Path != null)
            {
                Gizmos.color = Color.yellow;

                for (int i = 0; i < Path.corners.Length; i++)
                {
                    Gizmos.DrawSphere(Path.corners[i] + Vector3.up * 0f, 0.1f);

                    if (i < Path.corners.Length - 1)
                        Gizmos.DrawLine(Path.corners[i] + Vector3.up * 0f, Path.corners[i + 1] + Vector3.up * 0f);
                }
            }
        }
    }
}