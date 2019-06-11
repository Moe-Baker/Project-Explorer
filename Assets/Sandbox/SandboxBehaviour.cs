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
        new public Rigidbody rigidbody;

        public Transform target;

        public float speed = 50f;

        void Start()
        {
            rigidbody.centerOfMass = Vector3.zero;
        }

        void Update()
        {
            float angle = CalculateAngle();

            var direction = (target.position - rigidbody.transform.position).normalized;

            var targetAngles = Quaternion.LookRotation(direction, rigidbody.transform.up).eulerAngles;

            var angularDirection = (targetAngles - rigidbody.transform.eulerAngles).normalized;

            rigidbody.angularVelocity = angularDirection * speed * Time.deltaTime;
        }

        float CalculateAngle()
        {
            return Quaternion.Angle(rigidbody.transform.rotation, target.rotation);
        }
    }
}