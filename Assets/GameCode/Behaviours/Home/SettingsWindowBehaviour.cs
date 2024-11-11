using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class SettingsWindowBehaviour : WindowBehaviour
    {

        [SerializeField] Animator WindowAnimator;
        [SerializeField] TMP_Dropdown LanguageDropDown;

        [SerializeField] TMP_Text IDText;
        [SerializeField] GameObject GoogleIDObject;
        [SerializeField] TMP_Text GoogleIDText;

        [SerializeField] TMP_Text VersionText;

        [SerializeField] SettingsToggleButtonBehaviour SFX_Button;
        [SerializeField] SettingsToggleButtonBehaviour MusicButton;

        [SerializeField] GameObject SaveBttn;
        [SerializeField] GameObject NameBttn;

        ProfileInstance profile;

        public override void Init(Action callback)
        {
            VersionText.text = Locales.Get("locale:1645", Application.version);
            profile = ClientWorld.Instance.Profile;
            IDText.text = Locales.Get("locale:1414", profile.index.ToString());

#if UNITY_ANDROID
            if (GooglePlay.Instance.GPG_Init && Social.localUser.authenticated)
            {
                GoogleIDObject.SetActive(true);
                GoogleIDText.text = Locales.Get("locale:1516", Social.localUser.id);
            }
            else
            {
                GoogleIDObject.SetActive(false);
            }
#endif

            var languages = Enum.GetNames(typeof(Language));
            LanguageDropDown.options.Clear();
            for (int i = 0; i < languages.Length; i++)
            {
                LanguageDropDown.options.Add(new TMP_Dropdown.OptionData() {
                    text = languages[i]
                });
               
            }
            onChangeLanguage();
            callback();
        }

        private Language selectLanguege;
        public void OnSelectLangue()
        {
            var languages = Enum.GetNames(typeof(Language));
            for (int i = 0; i < languages.Length; i++)
            {
                if(languages[i] == LanguageDropDown.captionText.text)
                {
                    if(i+1< languages.Length)
                    {
                        LanguageDropDown.value =(i+1);
                        selectLanguege = (Language)(i+1);
                    }
                    else
                    {
                        LanguageDropDown.value = 0;
                        selectLanguege = (Language)(0);
                    }
                   
                    break;
                }
            }
            if (profile.playerSettings.language != selectLanguege)
            {
                SaveBttn.SetActive(true);
            }
            else
            {
                SaveBttn.SetActive(false);
            }
        }

        public void OnSave()
        {
            profile.UpdateLanguage(selectLanguege);
            SelfClose();
        }

        protected override void SelfClose()
        {
            selectLanguege = profile.playerSettings.language;
            SaveBttn.SetActive(false);
            WindowAnimator.Play("Close");
            //WindowManager.Instance.MainWindow.StartTutor();
        }

        private void onChangeLanguage()
        {
            var languages = Enum.GetNames(typeof(Language));
            for (int i = 0; i < languages.Length; i++)
            {
                selectLanguege = (Language)(i);
                if (selectLanguege == profile.playerSettings.language)
                {
                    LanguageDropDown.value = i;
                }
            }
        }
        protected override void SelfOpen()
        {
            WindowManager.Instance.MainWindow.StopTutor();
             NameBttn.SetActive(!profile.IsBattleTutorial);

            selectLanguege = profile.playerSettings.language;
            SaveBttn.SetActive(false);

            onChangeLanguage();
            gameObject.SetActive(true);
            SetButtonsOnEnabled();
        }

        public void ToggleMusic()
        {
            var now_enabled = PlayerPrefs.GetInt(PlayerPrefsVariable.SETTINGS_MUSIC.ToString()) > 0;

            if (now_enabled)
            {
                SoundManager.Instance.MusicOff();
            }
            else
            {
                SoundManager.Instance.MusicOn();
            }
            MusicButton.Enable(!now_enabled);
        }

        public void ToggleSoundFX()
        {
            var now_enabled = PlayerPrefs.GetInt(PlayerPrefsVariable.SETTINGS_SFX.ToString()) > 0;

            if (now_enabled)
            {
                SoundManager.Instance.EffectsOff();
            }
            else
            {
                SoundManager.Instance.EffectsOn();
            }
            SFX_Button.Enable(!now_enabled);
        }

        private void SetButtonsOnEnabled()
        {
            var now_enabled = PlayerPrefs.GetInt(PlayerPrefsVariable.SETTINGS_SFX.ToString()) > 0;
            SFX_Button.Enable(now_enabled);
            var now_enable = PlayerPrefs.GetInt(PlayerPrefsVariable.SETTINGS_MUSIC.ToString()) > 0;
            MusicButton.Enable(now_enable);
        }

        public void MissClick()
        {
            WindowManager.Instance.ClosePopUp();
        }
        
        public void ChangeName()
        {
            WindowManager.Instance.OpenWindow(childs_windows[0]);
        }
    }
}
