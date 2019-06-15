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
    [DefaultExecutionOrder(ExecutionOrder)]
	public class Level : MonoBehaviour
	{
        public const int ExecutionOrder = -200;

        public static Level Instance { get; protected set; }

        public Player Player { get; protected set; }

        public LevelMenu Menu { get; protected set; }

        void Awake()
        {
            Instance = this;

            Player = FindObjectOfType<Player>();

            Menu = FindObjectOfType<LevelMenu>();
        }

        void Start()
        {

        }
	}
}