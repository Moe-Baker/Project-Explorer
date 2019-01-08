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
        public Room destination;

        public float cameraTransitionSpeed = 10f;

        public Vector3 playerAnchorOffset = new Vector3(2f, 0f, 0f);
        public Vector3 PlayerAnchorPosition
        {
            get
            {
                return transform.TransformPoint(playerAnchorOffset);
            }
        }

        public TransitionRegion opposingRegion;

        Player player;
        new Camera camera;

        void Awake()
        {
            player = FindObjectOfType<Player>();

            camera = Camera.main;
        }
         
        void OnTriggerEnter(Collider collider)
        {
            if (!enabled) return;

            var player = collider.gameObject.GetComponent<Player>();

            if (player == null) return;

            StartCoroutine(Procedure());
        }

        IEnumerator Procedure()
        {
            opposingRegion.enabled = false;

            player.Navigator.enabled = false;
            player.Move.To(PlayerAnchorPosition);

            var cameraTargetPosition = destination.Bounds.center;
            cameraTargetPosition.y = camera.transform.position.y;

            while (true)
            {
                camera.transform.position = Vector3.MoveTowards(camera.transform.position, cameraTargetPosition, cameraTransitionSpeed * Time.deltaTime);

                var distance = Vector3.Distance(camera.transform.position, cameraTargetPosition);

                if (player.Move.IsProcessing || distance > 0f)
                    yield return new WaitForEndOfFrame();
                else
                    break;
            }

            player.Navigator.enabled = true;
            opposingRegion.enabled = true;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(PlayerAnchorPosition, new Vector3(0.2f, 4f, 0.2f));
        }
    }
}