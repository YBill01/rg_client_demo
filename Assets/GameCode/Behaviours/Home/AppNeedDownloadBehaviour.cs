using UnityEngine;
using System.Collections;
using System;

namespace Legacy.Client
{
    public class AppNeedDownloadBehaviour : WindowBehaviour
    {

        [SerializeField]
        private Animator WindowAnimator;

        public override void Init(Action callback)
        {
            callback();
        }

        public void DownloadButtonClick()
        {
            ObserverReachableSystem.HardDisconect = true;
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