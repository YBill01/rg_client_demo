using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class MobileInputManager : MonoBehaviour
    {
        public static MobileInputManager Instance = null;

        public event Action BackButtonDown;

        void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            CheckForBackButtonDown();
        }

        private void CheckForBackButtonDown()
        {
			if (Input.GetKeyDown(KeyCode.Escape))
			{
                if (ClientWorld.Instance.Profile.IsBattleTutorial || WindowManager.Instance.CurrentWindow is NameWindowBehaviour
                    || WindowManager.Instance.CurrentWindow is BattleStartWindowBehaviour)
                {
                    return;
                }

                BackButtonDown?.Invoke();
            }
                
        }
    }
}