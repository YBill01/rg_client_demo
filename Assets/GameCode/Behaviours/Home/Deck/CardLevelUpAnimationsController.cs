using Legacy.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class CardLevelUpAnimationsController : MonoBehaviour
    {
        [SerializeField] private LevelUpCardBehaviour heroWindow;
        [SerializeField] private Animator LevelTextAnimator;
        [SerializeField] private Animator ParamHpAnimator;
        [SerializeField] private Animator ParamDmgAnimator;
        [SerializeField] private Animator ParamDpsAnimator;
        [SerializeField] private Animator ParamDmgSturmAnimator;
        private static Queue<Animator> animatorsQueue;
        private static Animator curLevelAnimator;
        private static Animator curAnimator;
        private List<Animator> animators;
        private const string startTriggerName = "Start";
        private const string endTriggerName = "End";

        public void SetStart()
        {
            curLevelAnimator = LevelTextAnimator;
            animators = new List<Animator> {/*LevelTextAnimator, */ParamHpAnimator, ParamDmgAnimator, ParamDpsAnimator, ParamDmgSturmAnimator };
            animatorsQueue = new Queue<Animator>(animators);
        }

        public static bool NextStartAnimation(bool isEnd = false, float time = 0.3f)
        {
			if (animatorsQueue.Count == 0)
			{
                LevelUpCardBehaviour.Instance.TapToContinue(true);
                LevelUpCardBehaviour.Instance.StopAllCoroutines();

                return false;
			}

            var currentAnimator = animatorsQueue.Dequeue();
            //var currentAnimator = animatorsQueue.FirstOrDefault();
            currentAnimator.speed = 1.0f;
            curAnimator = currentAnimator;

            return true;

            //LevelUpCardBehaviour.Instance.StartNextLerp();
            //currentAnimator.enabled = true;

            //var TriggerName = isEnd ? endTriggerName : startTriggerName;
            //currentAnimator.SetTrigger(TriggerName);

            //var firstElem = animatorsQueue.Dequeue();
            //animatorsQueue.Enqueue(firstElem);

            //LevelUpCardBehaviour.Instance.waitTime = time;
            //LevelUpCardBehaviour.Instance.NextStateOnce();
        }

        public static void StartLevelAnimation()
        {
            curLevelAnimator.enabled = true;
            curLevelAnimator.speed = 1.0f;
            curLevelAnimator.SetTrigger(startTriggerName);
        }

        public static void SetAnimationToEnd()
        {
            if (curAnimator)
			{
                curAnimator.speed = 100;
            }
        }

        public void UpdateParam()
        {
        }

        public void UpdateLevel()
        {
            CardUpgradeWindowBehavior.Instance.SetLevel();
            LevelUpCardBehaviour.Instance.UpdateProgreessBar();
            CardLevelUpAnimationsController.NextStartAnimation(false, 0.1f);
            Lerp();
        }

        public void Lerp()
        {
            var heroParam = curAnimator.gameObject.GetComponent<HeroParamBehaviour>();
            var tmpDefiner = curAnimator.gameObject.GetComponentInChildren<ValueParamTextDefiner>();
            var tmpAddDefiner = curAnimator.gameObject.GetComponentInChildren<AdditionalValueParamTextDefiner>();
            var tmp = tmpDefiner.gameObject.GetComponent<TextMeshProUGUI>();
            var tmpAdd = tmpAddDefiner.gameObject.GetComponent<TextMeshProUGUI>();
            int number;

			if (curAnimator != curLevelAnimator)
			{
                curAnimator.gameObject.GetComponent<AudioSource>().Play();
            }

            bool success = Int32.TryParse(tmpAdd.text, out number);
            LevelUpCardBehaviour.Instance.StartCoroutine(LevelUpCardBehaviour.Instance.LerpCoroutine(
                (int) heroParam.GetLvlValue(), number,
                (int) heroParam.GetNextLvlValue(), (int) heroParam.GetDifferenceValue(),
                tmp, tmpAdd));
        }

        private void OnDisable()
        {
            if (animatorsQueue != null)
                foreach (var a in animatorsQueue)
                {
                    a.enabled = false;
                }
        }
    }
}