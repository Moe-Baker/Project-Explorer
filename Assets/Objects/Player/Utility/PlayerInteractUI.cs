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
	public class PlayerInteractUI : UIElement
	{
        [SerializeField]
        protected Text label;
        public Text Label { get { return label; } }

        public Level Level { get { return Level.Instance; } }

        public Player Player { get { return Level.Player; } }

        public Interactable Target { get { return Player.Interact.Target; } }

        new public Camera camera { get { return Player.camera; } }

        public RectTransform Rect { get; protected set; }

        public override void Init()
        {
            base.Init();

            Rect = transform as RectTransform;

            Player.Interact.OnTargetChanged += OnTargetChanged;

            Hide();
        }

        void OnTargetChanged(Interactable interact)
        {
            if(interact == null)
            {
                Hide();
            }
            else
            {
                Show();

                label.text = interact.Text;


            }
        }

        void Update()
        {
            var screenPosition = camera.WorldToScreenPoint(Target.Bounds.center + Vector3.forward * Target.Bounds.size.z);

            Rect.position = screenPosition + Vector3.up * Rect.sizeDelta.y / 2f;
        }
	}
}