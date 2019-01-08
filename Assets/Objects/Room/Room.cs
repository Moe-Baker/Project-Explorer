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
        void CalculateBounds()
        {
            var temp = new Bounds();

            var renderers = GetComponentsInChildren<Renderer>();

            for (int i = 0; i < renderers.Length; i++)
            {
                if (i == 0)
                    temp = renderers[i].bounds;
                else
                    temp.Encapsulate(renderers[i].bounds);
            }

            Bounds = temp;
        }

        void Awake()
        {
            CalculateBounds();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawCube(Bounds.center, Bounds.size);
        }
    }
}