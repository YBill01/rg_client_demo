using Legacy.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HeroLevelUpAnimationsController : MonoBehaviour
{
    [SerializeField] private HeroWindowBehaviour heroWindow;
    [SerializeField] private Animator SkillsAnimator;
    [SerializeField] private Animator ParamsAnimator;
    [SerializeField] private Animator LevelTextAnimator;
    [SerializeField] private Animator ParamHpAnimator;
    [SerializeField] private Animator ParamDmgAnimator;
    [SerializeField] private Animator ParamDpsAnimator;
    private List<Animator> animators;
    private static Queue<Animator> animatorsQueue;
    private static Animator curAnimator;
    private const string startTriggerName = "Start";
    private const string endTriggerName = "End";
    public void SetStart()
    {
        animators = new List<Animator> { SkillsAnimator, ParamsAnimator, LevelTextAnimator, ParamHpAnimator, ParamDmgAnimator, ParamDpsAnimator };
        animatorsQueue = new Queue<Animator>(animators);

    }

    public static void NextStartAnimation(bool isEnd = false, float time = 0.3f)
    {

        var currentAnimator = animatorsQueue.FirstOrDefault();
        currentAnimator.speed = 1;
        curAnimator = currentAnimator;
        currentAnimator.enabled = true;

        var TriggerName = isEnd ? endTriggerName : startTriggerName;
        currentAnimator.SetTrigger(TriggerName);

        var firstElem = animatorsQueue.Dequeue();
        animatorsQueue.Enqueue(firstElem);

        LevelUpHeroBehavior.Instance.waitTime = time;
        LevelUpHeroBehavior.Instance.NextStateOnce();
    }

    public static void SetAnimationToEnd()
    {
        if (curAnimator)
            curAnimator.speed = 100;
    }

    public void SetAlpha(CanvasGroup canvasGroup, bool toZero, float elapsedTime)
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, Convert.ToInt32(!toZero), elapsedTime);
    }

    //public void UpdateParam()
    //{
    //    GetComponent<AudioSource>().Play();
    //    var paramType = this.gameObject.GetComponent<HeroParamBehaviour>().GetType();
    //    this.gameObject.GetComponent<ProfileUpdatedValueDelayBehavior>()?.SetCanUpdate(true);
    //    heroWindow.SetCurrentParametr((byte)paramType);
    //}
    public void UpdateParam()
    {
        LevelUpHeroBehavior.Instance.StartCoroutine(LevelUpHeroBehavior.Instance.ShakeCoroutine(0.25f, 4, 1.5f));
        if (this.gameObject && this.gameObject.gameObject.GetComponent<HeroParamBehaviour>())
        {
        var heroParam = this.gameObject.gameObject.GetComponent<HeroParamBehaviour>();
        var tmpDefiner = this.gameObject.gameObject.GetComponentInChildren<ValueParamTextDefiner>();
        var tmpAddDefiner = this.gameObject.gameObject.GetComponentInChildren<AdditionalValueParamTextDefiner>();
        var tmp = tmpDefiner.gameObject.GetComponent<TextMeshProUGUI>();
        var tmpAdd = tmpAddDefiner.gameObject.GetComponent<TextMeshProUGUI>();
        int number;

        GetComponent<AudioSource>().Play();
        bool success = Int32.TryParse(tmpAdd.text, out number);
        LevelUpHeroBehavior.Instance.StartCoroutine(LevelUpHeroBehavior.Instance.LerpCoroutine(
            (int)heroParam.GetLvlValue(), number,
            (int)heroParam.GetNextLvlValue(), (int)heroParam.GetNextDifferenceValue(),
            tmp, tmpAdd));

        }
    }
    public void UpdateLevel()
    {
        LevelUpHeroBehavior.Instance.StopAllCoroutines();
    }
    private void OnDisable()
    {
        if (animatorsQueue != null)
            foreach (var a in animatorsQueue)
            {
               if(a && a.enabled) a.enabled = false;
            }
    }
}
