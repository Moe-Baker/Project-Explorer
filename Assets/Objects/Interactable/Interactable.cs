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

using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game
{
    [SelectionBase]
	public class Interactable : MonoBehaviour
	{
        [SerializeField]
        protected bool active = true;
        public bool Active { get { return active; } }

        [SerializeField]
        protected string text;
        public string Text { get { return text; } }

        [SerializeField]
        protected float range = 2f;
        public float Range { get { return range; } }

        [SerializeField]
        protected UnityEvent onAction;
        public UnityEvent OnAction { get { return onAction; } }
        public virtual void Action()
        {
            onAction.Invoke();

            Debug.Log("Interacting with " + name);
        }

        public Bounds Bounds { get; protected set; }

        void Awake()
        {
            Bounds = Utility.CalculateBounds(gameObject);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, range);

            Gizmos.color = Color.red;
            Gizmos.DrawCube(Bounds.center, Bounds.size);
        }
    }
}