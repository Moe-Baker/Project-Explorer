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
	public class PlayerNavigator : MonoBehaviour
	{
        Player player;

        new public Camera camera { get { return player.camera; } }

        PlayerMove Move { get { return player.Move; } }

        public LayerMask mask = Physics.DefaultRaycastLayers;

        public void Init(Player player)
        {
            this.player = player;
        }

        void Update()
        {
            if(Input.GetMouseButton(0))
            {
                var screenPoint = Input.mousePosition;
                screenPoint.z = camera.nearClipPlane;

                var ray = camera.ScreenPointToRay(screenPoint);

                RaycastHit hit;

                if(Physics.Raycast(ray, out hit, Mathf.Infinity, mask, QueryTriggerInteraction.Ignore))
                {
                    Move.To(hit.point);
                }
                else
                {

                }
            }
        }
	}
}