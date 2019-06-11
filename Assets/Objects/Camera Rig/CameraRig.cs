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
	public class CameraRig : MonoBehaviour
	{
		[SerializeField]
        protected Camera _camera;
        new public Camera camera { get { return _camera; } }

        public virtual Ray ScreenPointToRay(Vector3 point)
        {
            point.z = camera.nearClipPlane;

            return camera.ScreenPointToRay(point);
        }
    }
}