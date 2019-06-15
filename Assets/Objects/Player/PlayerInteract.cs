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
	public class PlayerInteract : MonoBehaviour
	{
        [SerializeField]
        protected LayerMask mask = Physics.DefaultRaycastLayers;
        public LayerMask Mask { get { return mask; } }

        [SerializeField]
        protected float range = 100;
        public float Range { get { return range; } }

        Player player;

        public CameraRig CameraRig { get { return player.CameraRig; } }
        new public Camera camera { get { return CameraRig.camera; } }

        Ray ray;

        RaycastHit hit;
        public RaycastHit Hit { get { return hit; } }
        public bool HasHit { get { return hit.collider != null; } }

        protected Interactable _target;
        public Interactable Target
        {
            get
            {
                return _target;
            }
            set
            {
                if (value == Target) return;

                _target = value;

                if (OnTargetChanged != null) OnTargetChanged(Target);
            }
        }
        public event Action<Interactable> OnTargetChanged;

        public void Init(Player player)
        {
            this.player = player;
        }

        void Update()
        {
            Detect();

            Process();
        }

        void Detect()
        {
            ray = CameraRig.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, range, mask, QueryTriggerInteraction.Ignore))
            {
                var tempTarget = hit.transform.GetComponent<Interactable>();

                if (tempTarget != null && tempTarget.Active == false) tempTarget = null;

                if (tempTarget != null && player.DistanceTo(tempTarget.transform) > tempTarget.Range) tempTarget = null;

                Target = tempTarget;
            }
            else
            {
                Target = null;
            }
        }

        void Process()
        {
            if(Target != null)
            {
                if(Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Target.Action();
                }
            }
        }
    }
}