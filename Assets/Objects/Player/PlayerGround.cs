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
	public class PlayerGround : MonoBehaviour
	{
        [SerializeField]
        protected LayerMask mask = Physics.DefaultRaycastLayers;
        public LayerMask Mask { get { return mask; } }

        protected RaycastHit hit;
        public RaycastHit Hit { get { return hit; } }

        public bool HasHit { get { return hit.collider != null; } }

        public float Angle { get; protected set; }

        Player player;

        new public Rigidbody rigidbody { get { return player.rigidbody; } }

		public virtual void Init(Player reference)
        {
            this.player = reference;
        }

        protected virtual void Update()
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

        public virtual Vector3 Project(Vector3 vector)
        {
            if (HasHit)
                vector = Vector3.Project(vector, hit.normal);

            return vector;
        }
        public virtual Vector3 ProjectOnPlane(Vector3 vector)
        {
            if (HasHit)
                vector = Vector3.ProjectOnPlane(vector, hit.normal);

            return vector;
        }
	}
}