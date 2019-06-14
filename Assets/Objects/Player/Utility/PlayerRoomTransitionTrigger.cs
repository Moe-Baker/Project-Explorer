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
	public class PlayerRoomTransitionTrigger : MonoBehaviour
	{
		[SerializeField]
        protected Room room1;
        public Room Room1 { get { return room1; } }

        [SerializeField]
        protected Room room2;
        public Room Room2 { get { return room2; } }

        [SerializeField]
        protected float offset = 1.2f;
        public float Offset { get { return offset; } }

        Player player;
        public CameraRig CameraRig { get { return player.CameraRig; } }

        void Awake()
        {
            player = FindObjectOfType<Player>();
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.attachedRigidbody == null) return;
            if (collider.attachedRigidbody.gameObject != player.gameObject) return;

            var distanceToRoom1 = Vector3.Distance(room1.Bounds.ClosestPoint(player.transform.position), player.transform.position);
            var distanceToRoom2 = Vector3.Distance(room2.Bounds.ClosestPoint(player.transform.position), player.transform.position);

            Room target;

            if (distanceToRoom1 > distanceToRoom2) //player is "probably" in room 2
                target = room1;
            else if (distanceToRoom1 < distanceToRoom2) //player is "probably" in room 1
                target = room2;
            else //player is half way between ? where the hell is the player ?
                throw new NotImplementedException("This shouldn't be possible, so GG if you are getting this error");

            StartCoroutine(Procedure(target));
        }

        IEnumerator Procedure(Room target)
        {
            var direction = (transform.position - target.transform.position).normalized;
            direction.y = 0f;

            var closestPoint = target.Bounds.ClosestPoint(transform.position);

            var destination = closestPoint - direction * offset;

            player.Move.To(destination);

            player.Navigator.enabled = false;
            player.Move.To(destination);

            var cameraStartPosition = CameraRig.transform.position;

            var cameraEndPosition = target.Bounds.center;
            cameraEndPosition.y = cameraStartPosition.y;

            while (true)
            {
                CameraRig.transform.position = Vector3.Lerp(cameraStartPosition, cameraEndPosition, player.Move.DistanceRate);

                if (player.Move.IsProcessing)
                    yield return new WaitForEndOfFrame();
                else
                    break;
            }

            player.Navigator.enabled = true;
        }
    }
}