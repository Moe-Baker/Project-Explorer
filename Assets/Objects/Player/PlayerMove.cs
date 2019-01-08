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
	public class PlayerMove : MonoBehaviour
	{
        Player player;

        public NavMeshAgent NavAgent { get { return player.NavAgent; } }
        public Vector3 Destination { get { return NavAgent.destination; } }

        public Animator Animator { get { return player.Animator; } }

        public void Init(Player player)
        {
            this.player = player;
        }

        void Update()
        {
            Animator.SetFloat("Speed", NavAgent.velocity.magnitude);
        }

        public void To(Vector3 target)
        {
            if (Vector3.Distance(target, Destination) <= 0.1 + 0.05f)
                return;

            if (IsProcessing)
                NavAgent.SetDestination(target);
            else
                coroutine = StartCoroutine(Procedure(target));
        }

        Coroutine coroutine;
        public bool IsProcessing { get { return coroutine != null; } }
        IEnumerator Procedure(Vector3 destination)
        {
            NavAgent.SetDestination(destination);
            NavAgent.isStopped = false;

            while (true)
            {
                if (Vector3.Distance(player.transform.position + Vector3.down * player.Height / 2f, NavAgent.destination) <= NavAgent.stoppingDistance)
                    break;

                yield return new WaitForEndOfFrame();
            }

            NavAgent.isStopped = true;
            coroutine = null;
        }

        public void Stop()
        {
            if (IsProcessing)
            {
                NavAgent.isStopped = true;
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}