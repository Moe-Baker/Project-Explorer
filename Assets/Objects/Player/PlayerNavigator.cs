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

        public CameraRig CameraRig { get { return player.CameraRig; } }
        new public Camera camera { get { return CameraRig.camera; } }

        PlayerInteract Interact { get { return player.Interact; } }

        PlayerMove Move { get { return player.Move; } }

        public void Init(Player player)
        {
            this.player = player;
        }

        public event Action<Vector3> OnSelect;

        void Update()
        {
            if(Input.GetMouseButton(0))
            {
                if(Interact.HasHit)
                {
                    NavMeshHit navHit;

                    if(NavMesh.SamplePosition(Interact.Hit.point, out navHit, 4f, NavMesh.AllAreas))
                    {
                        Move.To(navHit.position);

                        if (OnSelect != null) OnSelect(navHit.position);
                    }
                    else
                    {

                    }
                }
                else
                {

                }
            }
        }
	}
}