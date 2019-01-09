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
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [DefaultExecutionOrder(-100)]
    public class Player : MonoBehaviour
	{
        new public CapsuleCollider collider { get; protected set; }
        public float Height { get { return collider.height; } }
        public float Radius { get { return collider.radius; } }

        public Vector3 FeetPosition
        {
            get
            {
                return transform.position + Vector3.down * Height / 2f;
            }
        }

        new public Rigidbody rigidbody { get; protected set; }

        public Animator Animator { get; protected set; }

        public NavMeshAgent NavAgent { get; protected set; }

        public PlayerMove Move { get; protected set; }

        public PlayerNavigator Navigator { get; protected set; }

        new public Camera camera { get; protected set; }

        void Awake()
        {
            camera = Camera.main;

            collider = GetComponent<CapsuleCollider>();

            rigidbody = GetComponent<Rigidbody>();

            Animator = GetComponentInChildren<Animator>();

            NavAgent = GetComponent<NavMeshAgent>();

            Move = GetComponentInChildren<PlayerMove>();
            Move.Init(this);

            Navigator = GetComponentInChildren<PlayerNavigator>();
            Navigator.Init(this);
        }
	}
}