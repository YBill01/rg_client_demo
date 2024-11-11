using Legacy.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SkillUpgradeNimationsController : MonoBehaviour
{
    public SkillUpgradeBehaviour heroWindow;
    [SerializeField] private Animator LevelTextAnimator;
    private Queue<Animator> animatorsQueue;
    public  Animator curAnimator;
    private List<Animator> animators;
    private const string startTriggerName = "Start";
    private const string endTriggerName = "End";
    public void SetStart()
    {
        animators = new List<Animator> { LevelTextAnimator };
        foreach (var a in heroWindow.skillParams)
        {
            animators.Add(a);
        }
        animatorsQueue = new Queue<Animator>(animators);
    }

    public void NextStartAnimation(bool isEnd = false, float time = 0.3f)
    {
        var currentAnimator = animatorsQueue.FirstOrDefault();
        currentAnimator.enabled = false;
        currentAnimator.enabled = true;
        currentAnimator.speed = 1;
        curAnimator = currentAnimator;


        var TriggerName = isEnd ? endTriggerName : startTriggerName;
        currentAnimator.SetTrigger(TriggerName);

        var firstElem = animatorsQueue.Dequeue();
        animatorsQueue.Enqueue(firstElem);

        heroWindow.waitTime = time;
        heroWindow.NextStateOnce();
    }

    public void SetAnimationToEnd()
    {
        if (curAnimator)
            curAnimator.speed = 100;
    }

    public void UpdateParam()
    {
        GetComponent<AudioSource>().Play();
        var param = curAnimator.GetComponent<SkillParametrBehavior>();
        var prevValue = (int)Mathf.Round(param.PrevValue());
        var nextValue = (int)Mathf.Round(param.NextValue());
        var addData = param.GetStrAddData();
        var tmp = param.GetComponentInChildren<ValueParamTextDefiner>().GetComponent<TextMeshProUGUI>();
        heroWindow.StartCoroutine(heroWindow.LerpCoroutine(prevValue, nextValue, tmp, addData));
    }

    public void UpdateLevel()
    {
        this.gameObject.GetComponentInChildren<ProfileUpdatedValueDelayBehavior>()?.SetCanUpdate(true);
    }



    private void OnDisable()
    {
        if (animatorsQueue != null)
            foreach (var a in animatorsQueue)
            {
                if (a) a.enabled = false;
            }
    }
}
