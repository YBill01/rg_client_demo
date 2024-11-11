using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Legacy.Client
{
    public class GameOverAnimatorsBehaviour : MonoBehaviour
    {
        [SerializeField] private Animator LevelTextAnimator;
        [SerializeField] private Animator ParamHpAnimator;
        private static Queue<Animator> animatorsQueue;
        private static Animator curAnimator;
        private List<Animator> animators;
        private const string startTriggerName = "Start";
        private const string endTriggerName = "End";
        public void SetStart()
        {
            animators = new List<Animator> { LevelTextAnimator, ParamHpAnimator };
            animatorsQueue = new Queue<Animator>(animators);
        }

        public static void NextStartAnimation(bool isEnd = false, float time = 0.3f, bool animName = false)
        {
            var currentAnimator = animatorsQueue.FirstOrDefault();
            currentAnimator.speed = 1;
            curAnimator = currentAnimator;
            currentAnimator.enabled = true;

            var TriggerName = isEnd ? endTriggerName : startTriggerName;
            currentAnimator.SetTrigger(TriggerName);
            if (!animName)
            {
                var firstElem = animatorsQueue.Dequeue();
                animatorsQueue.Enqueue(firstElem);
            }

            GameOverWindowBehaviour.Instance.waitTime = time;
            GameOverWindowBehaviour.Instance.NextStateOnce();
        }
        public static void NextStartAnimation(string AnimationName, float time = 0.3f)
        {
            curAnimator.Play(AnimationName);
            GameOverWindowBehaviour.Instance.waitTime = time;
            GameOverWindowBehaviour.Instance.NextStateOnce();
        }

        public static void SetAnimationToEnd()
        {
            if (curAnimator)
                curAnimator.speed = 100;
        }

        public void UpdateParam()
        {
        }

        public void UpdateLevel()
        {

        }



        private void OnDisable()
        {
            if (animatorsQueue != null)
                foreach (var a in animatorsQueue)
                {
                    if (a)
                        a.enabled = false;
                }
        }
    }
}
