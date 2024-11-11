using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Client;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class LevelUpWindowBehaviour : WindowBehaviour
    {
        [SerializeField]
        private Animator WindowAnimator;
        [SerializeField]
        private LevelUpWindowAnimationsController animationsController;
        [SerializeField]
        private LevelUpWindowContentBehaviour contentBehaviour;

        private ProfileInstance profile;
        private bool canClick = false;

        public void AllowClick()
        {
            canClick = true;

        }
        public override void Init(Action callback)
        {
            profile = ClientWorld.Instance.Profile;
            animationsController.Init();
            callback();
        }

        IEnumerator WaitClick()
        {
            yield return new WaitForSeconds(4);
            canClick = true;
        }
        public void Click()
        {
            if (canClick)
            {
                /*if (profile.HardTutorialState == 2 && HomeTutorialHelper.Instance.HardHomeTutorStep == 23)
                {
                    WindowManager.Instance.Home();
                    return;
                }*/

                WindowManager.Instance.ClosePopUp();
            }
        }

        protected override void SelfOpen()
        {
            WindowManager.Instance.IsCliCkBack = false;
            AnalyticsManager.Instance.LevelUp();
            contentBehaviour.Init(profile);
            contentBehaviour.SetItemsBeforeAnimation();
            gameObject.SetActive(true);
            canClick = false;
            StartCoroutine(WaitClick());
            animationsController.StartAnimations();
            if(profile.IsBattleTutorial)
                WindowManager.Instance.MainWindow.menuTutorialPointer.HidePointerTemporary();
        }

        protected override void SelfClose()
        {
            WindowManager.Instance.IsCliCkBack = true;
            if (profile.IsBattleTutorial)
                WindowManager.Instance.MainWindow.menuTutorialPointer.UnhidePointer();
            WindowAnimator.Play("Close");
        }
    }
}