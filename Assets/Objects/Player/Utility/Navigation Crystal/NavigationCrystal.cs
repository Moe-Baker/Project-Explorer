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
	public class NavigationCrystal : MonoBehaviour
	{
        public float duration = 2f;
        float timer = 0f;

        Player player;

        void Start()
        {
            player = FindObjectOfType<Player>();

            player.Navigator.OnSelect += OnSelect;

            gameObject.SetActive(false);
        }

        void OnSelect(Vector3 point)
        {
            transform.position = point;
            timer = duration;
            gameObject.SetActive(true);
        }

        void Update()
        {
            var distanceFromPlayer = Vector3.Distance(player.GroundPosition, transform.position);

            if (!player.Move.IsProcessing || distanceFromPlayer < 0.5f || !player.Navigator.enabled)
                gameObject.SetActive(false);
        }
	}
}