using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Legacy.Client
{
    public class SoundsPlayerManager : MonoBehaviour
    {
        public static SoundsPlayerManager Instance;

        [SerializeField] private AudioInfo[] AudioClipsArr;
        [Space]
        [SerializeField] private GameObject audioSourcesHolder;

        private List<AudioSource> audioSourcesList;

        private void Awake()
        {
            Instance = this;
            audioSourcesList = new List<AudioSource>(32);
        }

        public void PlaySound(SoundName name)
        {
            foreach (var info in AudioClipsArr)
            {
                if(info.soundName == name)
                {
                    var source = GetSource();
                    source.clip = info.audioClip;
                    source.outputAudioMixerGroup = info.AudioMixerGroup;
                    source.Play();
                }
            }

        }

        private AudioSource GetSource()
        {
            for (int i = 0; i < audioSourcesList.Count; i++)
            {
                if (!audioSourcesList[i].isPlaying)
                {
                    return audioSourcesList[i];
                }
            }
            var newSource = audioSourcesHolder.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            audioSourcesList.Add(newSource);
            return newSource;
        }

        [Serializable]
        private struct AudioInfo
        {
            public SoundName soundName;
            public AudioMixerGroup AudioMixerGroup;
            public AudioClip audioClip;
        }
    }

    public enum SoundName
    {
        Buy,
        Card_change_menu,
        Card_use_menu,
        Chest_to_slot,

    }
}
 