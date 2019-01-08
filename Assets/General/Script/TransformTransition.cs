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
	public class TransformTransition : MonoBehaviour
	{
        public Transform destination;

        public float duration = 1f;

        public Transform target;

        void Start()
        {
            Do();
        }

        void Reset()
        {
            destination = transform;
        }

        public void Do()
        {
            coroutine = StartCoroutine(Procedure());
        }

        Coroutine coroutine;
        public bool IsProcessing { get { return coroutine != null; } }
        IEnumerator Procedure()
        {
            var startingPosition = target.position;
            var startingRotation = target.rotation;

            var time = 0f;

            while(time != duration)
            {
                time = Mathf.MoveTowards(time, duration, Time.deltaTime);

                target.position = Vector3.Lerp(startingPosition, destination.position, time / duration);
                target.rotation = Quaternion.Lerp(startingRotation, destination.rotation, time / duration);

                yield return new WaitForEndOfFrame();
            }

            coroutine = null;
        }
	}
}