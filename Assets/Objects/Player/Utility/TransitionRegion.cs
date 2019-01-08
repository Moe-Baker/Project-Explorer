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
    public class TransitionRegion : MonoBehaviour
    {
        public TransformTransition cameraTransition;

        public PlayerMovementTransition movementTransition;

        void OnTriggerEnter(Collider collider)
        {
            Debug.Log(collider.name);

            var player = collider.gameObject.GetComponent<Player>();

            if (player == null) return;

            StartCoroutine(Procedure());
        }

        IEnumerator Procedure()
        {
            cameraTransition.Do();
            movementTransition.Do();

            while (cameraTransition.IsProcessing || movementTransition.IsProcessing)
                yield return new WaitForEndOfFrame();
        }
	}
}