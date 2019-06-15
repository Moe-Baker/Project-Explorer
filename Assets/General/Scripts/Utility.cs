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
	public static class Utility
	{
		public static Bounds CalculateBounds(GameObject gameObject)
        {
            var value = new Bounds();

            var renderers = gameObject.GetComponentsInChildren<Renderer>();

            for (int i = 0; i < renderers.Length; i++)
            {
                if (i == 0)
                    value = renderers[i].bounds;
                else
                    value.Encapsulate(renderers[i].bounds);
            }

            return value;
        }
	}
}