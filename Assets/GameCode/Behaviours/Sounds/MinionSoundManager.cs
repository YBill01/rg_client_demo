using UnityEngine;

namespace Legacy.Client
{
    public class MinionSoundManager : MonoBehaviour
    {
        public AudioClip CurrentClip
        {
            get
            {
                if (MinionAudioSource != null)
                    return MinionAudioSource?.clip;
                return Appear;
            }
            set { }
        }
        [SerializeField] bool isRandomPitch = true;
        [SerializeField] public AudioSource MinionAudioSource;

        private AudioSource _audioSourceForSteps;
        private AudioSource runTimeCreatedSorceForSteps 
        {
            get
            {
                if (_audioSourceForSteps == null)
                {
                    _audioSourceForSteps = gameObject.AddComponent<AudioSource>();
                    _audioSourceForSteps.outputAudioMixerGroup = MinionAudioSource.outputAudioMixerGroup;
                    _audioSourceForSteps.clip = Step;
                }
                return _audioSourceForSteps;
            }
        }

        [SerializeField] AudioClip Appear;

        [SerializeField] AudioClip Step;

        [SerializeField] AudioClip Hit;

        [SerializeField] AudioClip Die;

        [SerializeField] AudioClip Skill1;
        [SerializeField] AudioClip Skill2;


        [SerializeField] AudioClip state1;
        [SerializeField] AudioClip state2;
        [SerializeField] AudioClip state3;

        /// <summary>
        /// Use For Skills
        /// </summary>
        /// <param name="clip"></param>
        public void PlayCustomClip(AudioClip clip)
        {
            MinionAudioSource.clip = clip;
            MinionAudioSource.Play();
        }
        public void PlayCustomClip(AudioSource source, AudioClip clip)
        {
            source.clip = clip;
            source.Play();
        }

        public void PlayStep()
        {
            if (Step)
            {
                SetRandomPitch(runTimeCreatedSorceForSteps);
                runTimeCreatedSorceForSteps.Play();
            }
        }

        public void SetEmptyClip()
        {
            MinionAudioSource.clip = null;
            ResetPitch();
        }

        public void PlayHit()
        {
            if (Hit)
            {
                SetRandomPitch(MinionAudioSource);
                var canPlay = MinionsSoundsManager.canPlay(GetComponent<MinionPanel>().IsEnemy, Hit.name);
                if (canPlay)
                {
                    PlayCustomClip(Hit);
                }
            }
        }
        public void PlaySkill1()
        {
            ResetPitch();
            if (Skill1)
                PlayCustomClip(Skill1);
        }
        public void PlaySkill2()
        {
            ResetPitch();
            if (Skill2)
                PlayCustomClip(Skill2);
        }

        public void PlayDie()
        {
            if (Die)
            {
            SetRandomPitch(MinionAudioSource);
                if(TryGetComponent<MinionPanel>(out var minionPanel))
                {
                    var canPlay = MinionsSoundsManager.canPlay(minionPanel.IsEnemy, Die.name);
                    if (canPlay)
                        PlayCustomClip(Die);
                }
            }
        }

        public void PlayAppear()
        {
            if (!GetComponent<MinionInitBehaviour>().isHero && Appear)
            {
                var canPlay = MinionsSoundsManager.canPlay(GetComponent<MinionPanel>().IsEnemy, Appear.name);
                if (canPlay)
                {
                    SetRandomPitch(isRandomPitch);
                    PlayCustomClip(Appear);
                }
            }
        }

        public void PlayHpBarState1(AudioSource source)
        {
            if (state1)
                PlayCustomClip(source, state1);
        }
        public void PlayHpBarState2(AudioSource source)
        {
            if (state2)
                PlayCustomClip(source, state2);
        }
        public void PlayHpBarState3(AudioSource source)
        {
            if (state3)
                PlayCustomClip(source, state3);
        }

        public void SetRandomPitch(AudioSource sorce)
        {
            sorce.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        }

        public void SetRandomPitch(bool flag)
        {
            MinionAudioSource.pitch = flag ? UnityEngine.Random.Range(0.9f, 1.1f) : 1f;
        }

        public void ResetPitch(float value = 1f)
        {
            MinionAudioSource.pitch = value;
        }

        public void StopStepsSFX()
        {
            if (_audioSourceForSteps)
            {
                _audioSourceForSteps.enabled = false;
            }
        }
    }
}
