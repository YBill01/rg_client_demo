using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Legacy.Client
{
    public class LevelUpWindowAnimationsController : MonoBehaviour
    {
        [SerializeField] 
        private LevelUpWindowBehaviour levelUpWindow;
        [SerializeField] 
        private Animator LevelIconAnimator;
        [SerializeField] 
        private Animator LevelUpTitleAnimator;
        [SerializeField] 
        private Animator LevelUpSubtitleAnimator;
        [SerializeField] 
        private Animator TapTitleAnimator;
        [SerializeField] 
        private List<Animator> animators;
        [SerializeField] private PlayableDirector MainCardEffectDirector;
        [SerializeField, Range(1.0f, 5.0f)] float mainEffectTime = 4f;


        private static Animator currentAnimator;
        private int currentAnimatorIndex;

        public void Init()
        {
            currentAnimatorIndex = 0;
        }

        public void StartAnimations()
        {
            PlayNextAnimation();
        }

        private void Update()
        {
            CheckMainEffectTimeline();
        }
        private void CheckMainEffectTimeline()
        {
                if (MainCardEffectDirector.time > mainEffectTime)
                {
                    MainCardEffectDirector.Pause();
                }
        }

        private void PlayNextAnimation()
        {
            if (currentAnimatorIndex >= animators.Count) return;

            currentAnimator = animators[currentAnimatorIndex];
            currentAnimator.enabled = true;
            currentAnimatorIndex++;
            StartCoroutine(WaitForAnimation());
        }
        
        private IEnumerator WaitForAnimation()
        {
            yield return new WaitForSeconds(0.34f);
            PlayNextAnimation();
            yield return new WaitForSeconds(2.5f);
            levelUpWindow.AllowClick();
        }

        private void OnDisable()
        {
            if (animators != null)
                for (int i = 0; i < animators.Count; i++)
                {
                    animators[i].enabled = false;
                }

            currentAnimatorIndex = 0;
            WindowManager.Instance.SetUpPanelNextLevel();
        }
    }
}