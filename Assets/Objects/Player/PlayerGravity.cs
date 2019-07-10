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
	public class PlayerGravity : MonoBehaviour
	{
        Player player;
		public void Init(Player reference)
        {
            player = reference;

            rigidbody.useGravity = false;
        }

        new public Rigidbody rigidbody { get { return player.rigidbody; } }

        public PlayerGround Ground { get { return player.Ground; } }

        protected virtual void FixedUpdate()
        {
            rigidbody.AddForce(Ground.Project(Physics.gravity), ForceMode.Acceleration);
        }
	}
}