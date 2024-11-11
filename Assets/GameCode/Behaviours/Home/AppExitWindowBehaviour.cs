using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class AppExitWindowBehaviour : WindowBehaviour
    {
        [SerializeField] 
        private Animator WindowAnimator;
        
        public override void Init(Action callback)
        {
            callback();
        }

        public void MissClick()
        {
            WindowManager.Instance.ClosePopUp();
        }

        public void YesButtonClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void CancelButtonClick()
        {
            //WindowManager.Instance.ClosePopUp();
            WindowManager.Instance.Back();
        }
        
        protected override void SelfOpen()
        {
            gameObject.SetActive(true);
        }

        protected override void SelfClose()
        {
            WindowAnimator.Play("Close");
        }
    }
}