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
	public class CollisionEventsRewind : MonoBehaviour
	{
        public delegate void Delegate(Collision collision);

        public event Delegate EnterEvent;
        void OnCollisionEnter(Collision collision)
        {
            if (EnterEvent != null) EnterEvent(collision);
        }

        public event Delegate StayEvent;
        void OnCollisionStay(Collision collision)
        {
            if (StayEvent != null) StayEvent(collision);
        }

        public event Delegate ExitEvent;
        void OnCollisionExit(Collision collision)
        {
            if (ExitEvent != null) ExitEvent(collision);
        }
    }
}