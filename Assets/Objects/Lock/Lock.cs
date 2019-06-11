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
	public class Lock : MonoBehaviour
	{
		[SerializeField]
        protected LockKey key;
        public LockKey Key { get { return key; } }

        [SerializeField]
        protected bool _isOn = true;
        public bool IsOn
        {
            get
            {
                return _isOn;
            }
            set
            {
                _isOn = value;

                InvokeStateChange();
            }
        }

        protected virtual void InvokeStateChange()
        {
            if (OnStateChanged != null) OnStateChanged(IsOn);
        }
        public event Action<bool> OnStateChanged;

        public virtual bool Open(LockKey key)
        {
            if(key == this.key)
            {
                IsOn = false;

                return true;
            }
            else
            {
                return false;
            }
        }
        public virtual void ForceOpen()
        {
            Open(key);
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(Lock))]
        public class Inspector : Editor
        {
            new Lock target;

            void OnEnable()
            {
                target = base.target as Lock;
            }

            public override void OnInspectorGUI()
            {
                var initial_IsOn = target.IsOn;

                base.OnInspectorGUI();

                if(target.IsOn != initial_IsOn)
                    target.InvokeStateChange();
            }
        }
#endif
    }
}