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
	public class PlayerLook : MonoBehaviour
	{
        [SerializeField]
        protected SpeedData speed = new SpeedData(60f, 260f);
        public SpeedData Speed { get { return speed; } }
        [Serializable]
        public struct SpeedData
        {
            [SerializeField]
            float min;
            public float Min { get { return min; } }

            [SerializeField]
            float max;
            public float Max { get { return max; } }

            public float Sample(float rate)
            {
                return Mathf.Lerp(min, max, rate);
            }

            public SpeedData(float min, float max)
            {
                this.min = min;
                this.max = max;
            }
        }

        Player player;

        public void Init(Player refrence)
        {
            this.player = refrence;
        }

        public float At(Vector3 direction)
        {
            direction.y = 0f;

            if(direction == Vector3.zero) return 0f;

            var rotation = Quaternion.LookRotation(direction);

            var angle = Quaternion.Angle(transform.rotation, rotation);

            player.transform.rotation =
                Quaternion.RotateTowards(transform.rotation, rotation, speed.Sample(Mathf.Clamp01(angle / 90f)) * Time.deltaTime);

            return Quaternion.Angle(transform.rotation, rotation);
        }
    }
}