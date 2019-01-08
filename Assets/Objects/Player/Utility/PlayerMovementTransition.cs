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
	public class PlayerMovementTransition : MonoBehaviour
	{
        public Player player;

        public Transform destination;

        public bool IsProcessing { get { return player.Move.IsProcessing; } }

        void Reset()
        {
            destination = transform;
        }

        public void Do()
        {
            player.Move.To(destination.position);
        }
	}
}