using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Legacy.Client
{
    public class SoundManager : MonoBehaviour
    {
        [Serializable]
        public struct MinMaxAttenuation
        {
            [Range(-80, 0)]
            public short min;
            [Range(0, 10)]
            public byte max;
        }

        [SerializeField] MinMaxAttenuation AttenuationSettings;

        [SerializeField] AudioMixer Mixer;
        [SerializeField] AudioClip MainMenuTheme;
        [SerializeField] AudioClip MainBattleTheme;
        [SerializeField] AudioClip MainBattleThemeActive;
        [SerializeField] AudioClip MainBattleThemeSuperActive;
        [SerializeField] AudioMixerGroup menugroup;
        [SerializeField] AudioMixerGroup battlegroup;
        [SerializeField] AudioSource MusicAudioSource;
        [Space]
        [SerializeField] [Range(0, 1)] private float menuMusicVol;
        [SerializeField] [Range(0, 1)] private float menuEffectsVol;
        [SerializeField] [Range(0, 1)] private float battleMusicVol;
        [SerializeField] [Range(0, 1)] private float battleAmbientVol;
        [SerializeField] [Range(0, 1)] private float battleEffectsVol;
        [Space]
        [SerializeField] [Range(0, 1)] private float fadeOutVolume;// how low channel will be lowered while fadeout
        [SerializeField] private float fadeInLasting = 0.2f; // duration of smoth lowering music volume
        [Tooltip("Depends on sound clip lasting on which fade out was called, 1 == fadeOut starts after effect fully played")]
        [SerializeField] [Range(0, 1)] private float fadeOutLastingMult = 0.75f;

        private Coroutine FadeOutRoutineManager;
        private Coroutine FadeOutRoutine_1;
        private Coroutine FadeOutRoutine_2;

        private bool _isMusicOn;
        private bool _isEffectsOn;

        public static SoundManager Instance;

        private void Start()
        {
            Instance = this;
            if (!PlayerPrefs.HasKey(PlayerPrefsVariable.SETTINGS_MUSIC.ToString()))
            {
                MusicOn();
            }
            else
            {
                if (PlayerPrefs.GetInt(PlayerPrefsVariable.SETTINGS_MUSIC.ToString()) > 0)
                {
                    MusicOn();
                }
                else
                {
                    MusicOff();
                }
            }
            if (!PlayerPrefs.HasKey(PlayerPrefsVariable.SETTINGS_SFX.ToString()))
            {
                EffectsOn();
            }
            else
            {
                if (PlayerPrefs.GetInt(PlayerPrefsVariable.SETTINGS_SFX.ToString()) > 0)
                {
                    EffectsOn();
                }
                else
                {
                    EffectsOff();
                }
            }
        }

        public void PlayMenuMusic()
        {
            MusicAudioSource.outputAudioMixerGroup = menugroup;
            MuteMusic(false);
            MusicAudioSource.clip = MainMenuTheme;
            MusicAudioSource.Play();
        }

        public void MuteMusic(bool value)
        {
            MusicAudioSource.mute = value;
        }

        public void PlayBattleMusic(bool muteMusic = false)
        {
            MusicAudioSource.outputAudioMixerGroup = battlegroup;
            MuteMusic(muteMusic);
            MusicAudioSource.clip = MainBattleTheme;
            MusicAudioSource.Play();
        }

        public void PlayBattleMusicActive(bool isSuperActive = false)
        {
            MusicAudioSource.clip = isSuperActive ? MainBattleThemeSuperActive : MainBattleThemeActive;
            MusicAudioSource.Play();
        }

        void SetChannelVolume(string ChannelVolumeExposedParameterName, float value)
        {
            Mixer.SetFloat(ChannelVolumeExposedParameterName, Mathf.Lerp(AttenuationSettings.min, AttenuationSettings.max, value));
        }

        public void SetMusicVolume(int value)
        {
            PlayerPrefs.SetInt(PlayerPrefsVariable.SETTINGS_MUSIC.ToString(), value > 0 ? 1 : 0);

            SetChannelVolume("BattleMusicVolume", value > 0 ? battleMusicVol : 0);
            SetChannelVolume("MenuMusicVolume"  , value > 0 ? menuMusicVol   : 0);
        }

        public void SetEffectsVolume(int value)
        {
            PlayerPrefs.SetInt(PlayerPrefsVariable.SETTINGS_SFX.ToString(), value > 0 ? 1 : 0);

            SetChannelVolume("BattleAmbientVolume", value > 0 ? battleAmbientVol : 0);
            SetChannelVolume("BattleEffectsVolume", value > 0 ? battleEffectsVol : 0);
            SetChannelVolume("MenuEffectsVolume"  , value > 0 ? menuEffectsVol   : 0);
        }

        public void MusicOn()
        {
            _isMusicOn = true;
            StopAllFadeOutRoutines();
            SetMusicVolume(1);
        }

        public void MusicOff()
        {
            _isMusicOn = false;
            SetMusicVolume(0);
        }

        public void EffectsOn()
        {
            _isEffectsOn = true;
            SetEffectsVolume(1);
        }

        public void EffectsOff()
        {
            _isEffectsOn = false;
            SetEffectsVolume(0);
        }

        public void FadeOutMenuMusic(float lasting)
        {
            if (_isMusicOn)
            {
                StopAllFadeOutRoutines();
                FadeOutRoutineManager = StartCoroutine(FadeOutManagerRout(lasting));
            }
        }
        private IEnumerator FadeOutManagerRout(float lasting)
        {
            lasting *= fadeOutLastingMult;
            Mixer.GetFloat("MenuMusicVolume", out var currentVolumeLVL);
            currentVolumeLVL = 1 - (currentVolumeLVL / -80); // to get from 0 to 1 value
            yield return FadeOutRoutine_1 = StartCoroutine(FadeOutRoutine(currentVolumeLVL, fadeOutVolume, fadeInLasting));
            yield return new WaitForSeconds(lasting);
            yield return FadeOutRoutine_2 = StartCoroutine(FadeOutRoutine(fadeOutVolume, menuMusicVol, lasting));
        }
        private IEnumerator FadeOutRoutine(float from, float to, float lasting)
        {
            var timer = 0f;
            while (timer < lasting)
            {
                var progress = timer / lasting;
                var value = Mathf.Lerp(from, to, progress);

                SetChannelVolume("MenuMusicVolume", value);
                yield return null;
                timer += Time.deltaTime;
            }
            SetChannelVolume("MenuMusicVolume", to);
        }
        private void StopAllFadeOutRoutines()
        {
            if (FadeOutRoutineManager != null)
            {
                StopCoroutine(FadeOutRoutineManager);
                FadeOutRoutineManager = null;
            }
            if (FadeOutRoutine_1 != null)
            {
                StopCoroutine(FadeOutRoutine_1);
                FadeOutRoutine_1 = null;
            }
            if (FadeOutRoutine_2 != null)
            {
                StopCoroutine(FadeOutRoutine_2);
                FadeOutRoutine_2 = null;
            }
        }
    }
}
 