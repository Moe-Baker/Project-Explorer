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
    public class Door : MonoBehaviour
    {
        public HingeJoint Joint { get; protected set; }

        public GameObject Panel { get { return Joint.gameObject; } }

        new public Rigidbody rigidbody { get; protected set; }

        public CollisionEventsRewind CollisionEventsRewind { get; protected set; }

        public bool IsLocked
        {
            get
            {
                return rigidbody.isKinematic;
            }
            set
            {
                rigidbody.isKinematic = value;

                if (OnLockStatChanged != null) OnLockStatChanged(IsLocked);
            }
        }
        public event Action<bool> OnLockStatChanged;

        Player player;

        void Awake()
        {
            GetComponents();

            CollisionEventsRewind = Panel.AddComponent<CollisionEventsRewind>();
            CollisionEventsRewind.EnterEvent += PanelCollisionEnter;

            player = FindObjectOfType<Player>();
        }

        void GetComponents()
        {
            Joint = GetComponentInChildren<HingeJoint>();

            rigidbody = Panel.GetComponent<Rigidbody>();
        }

        void PanelCollisionEnter(Collision collision)
        {
            if(collision.gameObject == player.gameObject)
            {
                if (IsLocked)
                {
                    var direction = (player.transform.position - transform.position).normalized;
                    direction.y = 0.5f;

                    direction = transform.InverseTransformDirection(direction);

                    direction.x = direction.y = 0.5f;

                    direction = transform.TransformDirection(direction);

                    player.Move.To(transform.position + direction * 2f);
                }
            }
        }

        [CustomEditor(typeof(Door))]
        [CanEditMultipleObjects]
        public class Inspector : Editor
        {
            new Door target;

            void OnEnable()
            {
                target = (Door)base.target;

                target.GetComponents();
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (targets.Length == 1)
                    target.IsLocked = EditorGUILayout.Toggle(nameof(target.IsLocked), target.IsLocked);
            }
        }
    }
}