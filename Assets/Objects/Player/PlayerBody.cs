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
    [RequireComponent(typeof(Animator))]
	public class PlayerBody : MonoBehaviour
	{
        Player player;
        public void Init(Player player)
        {
            this.player = player;

            Animator = GetComponent<Animator>();
        }

        public Animator Animator { get; protected set; }

        public event Action<int> AnimatorIKEvent;
        void OnAnimatorIK(int layerIndex)
        {
            if (AnimatorIKEvent != null) AnimatorIKEvent(layerIndex);
        }
	}
}