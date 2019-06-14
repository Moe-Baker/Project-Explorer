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

        public float speed = 3f;

        void Start()
        {
            rigidbody.centerOfMass = Vector3.zero;
        }

        void Update()
        {
            
        }
    }
}