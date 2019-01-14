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
    public interface IPlayerHandsIKTarget
    {
        bool Active { get; }
    }

	public class PlayerHandsIKTarget : MonoBehaviour, IPlayerHandsIKTarget
    {
        public bool Active = true;
        bool IPlayerHandsIKTarget.Active { get { return Active; } }
    }
}