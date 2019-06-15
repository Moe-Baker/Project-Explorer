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
	public class Room : MonoBehaviour
	{
        public Bounds Bounds { get; protected set; }

        void Awake()
        {
            Bounds = Utility.CalculateBounds(gameObject);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawCube(Bounds.center, Bounds.size);
        }
    }
}