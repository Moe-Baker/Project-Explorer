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
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CollisionEventsRewind))]
    [DefaultExecutionOrder(-100)]
    public class Player : MonoBehaviour
	{
        new public CapsuleCollider collider { get; protected set; }
        public float Height { get { return collider.height; } }
        public float Radius { get { return collider.radius; } }

        public Vector3 GroundPosition
        {
            get
            {
                return transform.position + Vector3.down * Height / 2f;
            }
        }

        new public Rigidbody rigidbody { get; protected set; }

        public CollisionEventsRewind CollisionEventsRewind { get; protected set; }

        public CameraRig CameraRig { get; protected set; }
        new public Camera camera { get { return CameraRig.camera; } }

        public PlayerBody Body { get; protected set; }

        public PlayerInteract Interact { get; protected set; }

        public PlayerMove Move { get; protected set; }
        public PlayerLook Look { get; protected set; }

        public PlayerNavigator Navigator { get; protected set; }

        public PlayerHandsIK HandsIK { get; protected set; }

        void Awake()
        {
            collider = GetComponent<CapsuleCollider>();

            rigidbody = GetComponent<Rigidbody>();

            CollisionEventsRewind = GetComponent<CollisionEventsRewind>();

            CameraRig = FindObjectOfType<CameraRig>();
            if (CameraRig == null) throw new NullReferenceException("Player cannot find a Camera Rig in scene");

            Body = GetComponentInChildren<PlayerBody>();
            Body.Init(this);

            Interact = GetComponentInChildren<PlayerInteract>();
            Interact.Init(this);

            Move = GetComponentInChildren<PlayerMove>();
            Move.Init(this);

            Look = GetComponentInChildren<PlayerLook>();
            Look.Init(this);

            Navigator = GetComponentInChildren<PlayerNavigator>();
            Navigator.Init(this);

            HandsIK = GetComponentInChildren<PlayerHandsIK>();
            HandsIK.Init(this);
        }

        public float DistanceTo(Transform target)
        {
            return Vector3.Distance(transform.position, target.position);
        }
	}
}