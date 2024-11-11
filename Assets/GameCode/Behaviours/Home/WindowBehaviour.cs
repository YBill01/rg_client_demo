using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Database;
using UnityEditor;
using UnityEngine;
using static Legacy.Client.UpPanelBehaviour;

namespace Legacy.Client
{
    public abstract class WindowBehaviour : MonoBehaviour
    {
        public enum WindowType : byte
        {
            FullScreen = 0,
            PopUp = 1,
            CrossScene = 2
        }        

        [SerializeField]
        protected List<WindowBehaviour> childs_windows;

        [Serializable]
        public struct ScaleByAspect
        {
            [SerializeField, Range(0.0f, 1.0f)]
            public float aspect24_11;
            [SerializeField, Range(0.0f, 1.0f)]
            public float aspect16_9;
            [SerializeField, Range(0.0f, 1.0f)]
            public float aspect3_2;
            [SerializeField, Range(0.0f, 1.0f)]
            public float aspect4_3;
        }

        public ScaleByAspect AspectScaler;

        [Header("3D Render Settings")]
        [SerializeField]
        public bool Need3DHero;
        [SerializeField]
        public RectTransform ParentFor3D;
        [SerializeField, Range(0.0f, 1.0f)]
        public float Scale3DRender;
        public bool CanRotateHero;
        public bool ShowArena;
        [Space(10)]

        [SerializeField]
        public bool HideUpPanel;
        [SerializeField]
        protected List<UpPanelItem> upPanelElements;
        public bool IsOpen { get; private set; }
        public WindowBehaviour parent { get; private set; }

        public Dictionary<string, string> settings;
        [SerializeField]
        public WindowType type;


        public void SetParent(WindowBehaviour parent = null)
        {
            if (parent != null && parent.parent != this)
            {
                this.parent = parent;
            }
        }
        public void Open()
        {            
            IsOpen = true;
            WindowManager.Instance.CurrentWindow = this;
            SelfOpen();
        }

        /// <summary>
        /// Called From AnimationClip Event
        /// </summary>
        public void ClosedAnimationFinish()
        {
            gameObject.SetActive(false);
        }

        public float ScaleMatch
        {
            get
            {
                float scale;
                if (Camera.main.aspect >= 1.9)
                {
                    scale = AspectScaler.aspect24_11;
                }
                else if (Camera.main.aspect >= 1.7)
                {
                    scale = AspectScaler.aspect16_9;
                }
                else if (Camera.main.aspect >= 1.5)
                {
                    scale = AspectScaler.aspect3_2;
                }
                else
                {
                    scale = AspectScaler.aspect4_3;
                }
                return scale;
            }
        }
        protected abstract void SelfOpen();

        public void Close()
        {
            IsOpen = false;
            SelfClose();
        }

        //передача параметров в окно до открытия.
        public void OnSettings(Dictionary<string, string> settings) {
            if (settings != null)
            {
                this.settings = settings;
            }
        }

        protected abstract void SelfClose();

        public abstract void Init(Action callback);

        internal bool NeedHome()
        {
            return parent != null && parent.HasParent();
        }

        internal bool HasParent()
        {
            return parent != null;
        }

        internal int GetUpPanelConfig()
        {
            int result = 0;
            result += HasParent() ? (int)UpPanelItem.BackButton : 0;
            result += NeedHome() ? (int)UpPanelItem.HomeButton : 0;
            
            foreach (UpPanelItem state in upPanelElements)
            {
                result += (int)state;
            }
            
            return result;
        }

        
    }
}