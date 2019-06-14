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
    [SelectionBase]
	public class Interactable : MonoBehaviour
	{
		[SerializeField]
        protected string text;
        public string Text { get { return text; } }

        [SerializeField]
        protected bool active = true;
        public bool Active { get { return active; } }

        [SerializeField]
        protected float range = 2f;
        public float Range { get { return range; } }

        public event Action OnAction;
        public virtual void Action()
        {
            if (OnAction != null) OnAction();
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}