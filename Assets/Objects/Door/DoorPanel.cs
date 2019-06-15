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
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(HingeJoint))]
    public class DoorPanel : MonoBehaviour
	{
        public HingeJoint Joint { get; protected set; }

        new public Rigidbody rigidbody { get; protected set; }

        Door door;
        public Door Door { get { return door; } }
        public bool IsLocked { get { return door.IsLocked; } }

        public virtual void Init(Door door)
        {
            this.door = door;

            rigidbody = GetComponent<Rigidbody>();
            rigidbody.mass = 0f;

            Joint = GetComponent<HingeJoint>();

            SetLock(door.IsLocked);
        }

        public virtual void SetLock(bool value)
        {
            rigidbody.isKinematic = value;
        }

        void OnCollisionEnter(Collision collision)
        {
            door.OnPanelCollisionEnter(this, collision);
        }
    }
}