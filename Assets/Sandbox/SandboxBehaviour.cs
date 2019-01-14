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
	public class SandboxBehaviour : MonoBehaviour
	{
        public float speed = 5f;

        public float angularSpeed = 240f;

        public float acceleration = 15f;

        public Transform target;

        new Rigidbody rigidbody;

        NavMeshPath path;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        void Start()
        {
            path = new NavMeshPath();

            if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
            {

            }
            else
            {
                path = null;
            }
        }

        void FixedUpdate()
        {
            if (path != null && path.corners.Length > 1)
            {
                var corner = path.corners[1];

                var direction = (path.corners[1] - transform.position).normalized;
                direction.y = 0f;
                direction = direction.normalized;

                if(direction != Vector3.zero)
                {
                    var position = transform.position;
                    position.y = path.corners[0].y;
                    transform.position = position;

                    var distanceToTarget = Vector3.Distance(transform.position, target.position);
                    var distanceToCorner = Vector3.Distance(transform.position, path.corners[1]);

                    var targetVelocity = direction * speed;
                    // * Mathf.Clamp(distanceToTarget / 2f, 0.4f, 1f)
                    

                    if (path.corners.Length == 2 && targetVelocity.magnitude * Time.fixedDeltaTime >= distanceToCorner)
                    {
                        transform.position = path.corners[1];
                        rigidbody.velocity = Vector3.zero;
                    }
                    else
                    {
                        rigidbody.velocity = targetVelocity;
                    }

                    if(distanceToTarget > 0.5f)
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), angularSpeed * Mathf.Clamp01(targetVelocity.magnitude) * Time.deltaTime);
                }
            }

            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            var offset = Vector3.up * 0.05f;

            if(path != null)
            {
                for (int i = 0; i < path.corners.Length; i++)
                {
                    Gizmos.DrawSphere(path.corners[i] + offset, 0.1f);

                    if(i+1 < path.corners.Length)
                        Gizmos.DrawLine(path.corners[i] + offset, path.corners[i + 1] + offset);
                }
            }
        }
    }
}