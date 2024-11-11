using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class NameWindowBehaviour : WindowBehaviour
    {
        public static bool IsInputFocused;
        [SerializeField] Animator WindowAnimator;

        [SerializeField] TMP_InputField NameInput;

        [SerializeField] RectTransform EnterButton;

        Vector2 CurrentPosition = Vector2.zero;
        [SerializeField] RectTransform PopUpRect;
        float yDelta = 0.0f;
        private ProfileInstance profile;

        public override void Init(Action callback)
        {
            yDelta = GetComponent<RectTransform>().rect.height / 4;
            profile = ClientWorld.Instance.Profile;
            NameInput.text = profile.name;
            
            callback();
        }

        private void OnChanged(string newName)
        {
            string Cleaned = "";
            foreach (char c in newName)
            {
                if (Char.IsLetterOrDigit(c) || c == '_' || c == '@' || c == ' ' || c == '-')
                {
                    if (Cleaned.Length < 16)
                    {
                        Cleaned += c;
                    }
                    else
                    {
                       // Camera.main.transform.W(

                           PopupAlertBehaviour.ShowHomePopupAlert(Camera.main.WorldToScreenPoint(NameInput.transform.position), Locales.Get("locale:2380"));
                    }
                }
            }
            NameInput.text = Cleaned;
        }

        protected override void SelfClose()
        {
            NameInput.onValueChanged.RemoveListener(OnChanged);
            WindowAnimator.Play("Close");

			if (!isFinishTime)
                StopCoroutine(StatBlick());

        }

        public void MissClick()
        {
            if (!MissClickEnable)
            {
                return;
            }

            if (WindowManager.Instance.CurrentWindow.parent is SettingsWindowBehaviour)
            {
                if (!NameInput.isFocused)
                {
                    WindowManager.Instance.ClosePopUp();
                };
            }
            else
            {
				WindowManager.Instance.ClosePopUp();
            }
        }

        bool isFinishTime = true;
        protected override void SelfOpen()
        {
            NameInput.onValueChanged.AddListener(OnChanged);
            EnterButton.GetComponentInChildren<BlickControl>().Disable();
            gameObject.SetActive(true);

            if (!profile.IsBattleTutorial && 
                !profile.HasSoftTutorialState(SoftTutorial.SoftTutorialState.EnterName))
            {
                MissClickEnable = false;
                StartCoroutine(MissClickEnableWait());
            }

            if (WindowManager.Instance.CurrentWindow.parent is SettingsWindowBehaviour)
            { }
            else
                StartCoroutine(StatBlick());


        }

        IEnumerator StatBlick()
        {
            isFinishTime = false;
            yield return new WaitForSeconds(3);
            EnterButton.GetComponentInChildren<BlickControl>().Enable();
            isFinishTime = true;
        }
        // Запрет на клик по фейдеру
        protected bool MissClickEnable = true;
        IEnumerator MissClickEnableWait()
        {
            yield return new WaitForSeconds(2.0f);

            MissClickEnable = true;
        }


        void Update()
        {
            if (NameInput.text.Length > 16)
            {
                NameInput.text = NameInput.text.Substring(0, 16);
            }
                IsInputFocused = NameInput.isFocused;
            CurrentPosition.y = NameInput.isFocused ? yDelta : 0.0f;
            if (PopUpRect.anchoredPosition != CurrentPosition)
                PopUpRect.anchoredPosition = Vector2.Lerp(PopUpRect.anchoredPosition, CurrentPosition, 0.4f);
        }

        public void ClickContinue()
        {
            if (NameInput.text.Length < 1)
            {
                PopupAlertBehaviour.ShowHomePopupAlert(Input.mousePosition, Locales.Get("locale:2380"));
                return;
            }
            AnalyticsManager.Instance.NameChosen(NameInput.text);
            profile.UpdateName(NameInput.text);
            WindowManager.Instance.ClosePopUp();
            SoftTutorialManager.Instance.CompliteTutorial(SoftTutorial.SoftTutorialState.EnterName);
        }
    }
}
