using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class ErrorWindowBehaviour : WindowBehaviour
    {
        [SerializeField] Animator WindowAnimator;
        [SerializeField] TMP_Text ErrorTxt;
        public override void Init(Action callback)
        {
            callback();
        }

        protected override void SelfClose()
        {
         
            WindowAnimator.Play("Close");
        }

        protected override void SelfOpen()
        {
            ErrorTxt.text ="Error: "+ settings["index"];
            gameObject.SetActive(true);
        }

        public void OnClick()
        {
            ObserverReachableSystem.HardDisconect = true;
        }
    }
}
