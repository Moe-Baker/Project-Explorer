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
	public class TriggerEventsRewind : MonoBehaviour
	{
        public delegate void Delegate(Collider collider);

        public event Delegate EnterEvent;
        void OnTriggerEnter(Collider collider)
        {
            if (EnterEvent != null) EnterEvent(collider);
        }

        public event Delegate StayEvent;
        void OnTriggerStay(Collider collider)
        {
            if (StayEvent != null) StayEvent(collider);
        }

        public event Delegate ExitEvent;
        void OnTriggerExit(Collider collider)
        {
            if (ExitEvent != null) ExitEvent(collider);
        }
    }
}