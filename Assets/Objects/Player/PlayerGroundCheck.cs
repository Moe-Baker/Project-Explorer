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
	public class PlayerGroundCheck : MonoBehaviour
	{
        [SerializeField]
        protected LayerMask mask = Physics.DefaultRaycastLayers;
        public LayerMask Mask { get { return mask; } }

        protected RaycastHit hit;
        public RaycastHit Hit { get { return hit; } }

        public bool HasHit { get { return hit.collider != null; } }

        public Vector3 Normal
        {
            get
            {
                if (HasHit)
                    return hit.normal;
                else
                    return Vector3.up;
            }
        }

        public float Angle { get; protected set; }

        Player player;

        new public Rigidbody rigidbody { get { return player.rigidbody; } }

		public virtual void Init(Player reference)
        {
            this.player = reference;
        }

        void Update()
        {
            if (Physics.Raycast(player.transform.position, Vector3.down, out hit, Mathf.Infinity, mask, QueryTriggerInteraction.Ignore))
            {
                Angle = Vector3.Angle(transform.up, hit.normal);
            }
            else
            {
                Angle = 0f;
            }
        }
	}
}