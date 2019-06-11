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
    public class Door : MonoBehaviour
    {
        public Lock Lock { get; protected set; }
        public bool HasLock { get { return Lock != null; } }
        public bool IsLocked { get { return HasLock ? Lock.IsOn : false; } }

        public DoorPanel[] Panels { get; protected set; }

        Player player;

        void Awake()
        {
            Lock = GetComponent<Lock>();

            if(HasLock)
            {
                Lock.OnStateChanged += OnLockStateChanged;
            }

            Panels = GetComponentsInChildren<DoorPanel>();
            for (int i = 0; i < Panels.Length; i++)
                Panels[i].Init(this);

            player = FindObjectOfType<Player>();
        }

        void OnLockStateChanged(bool state)
        {
            for (int i = 0; i < Panels.Length; i++)
                Panels[i].SetLock(state);
        }

        public void OnPanelCollisionEnter(DoorPanel panel, Collision collision)
        {
            if(collision.gameObject == player.gameObject)
            {
                if (IsLocked)
                {
                    var localPosition = transform.InverseTransformDirection(player.transform.position);

                    localPosition.x = 0f;
                    localPosition.y = 0f;

                    var alignedPosition = transform.TransformDirection(localPosition);

                    var direction = (alignedPosition - transform.position).normalized;

                    var destination = transform.position + direction * 1f;

                    player.Move.To(destination);

                    Debug.DrawLine(transform.position, destination, Color.green, 10f);
                }
            }
        }
    }
}