using DG.Tweening;
using Legacy.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegacyButtonPunkController : MonoBehaviour
{
    [SerializeField] private WindowBehaviour additionalWindow;
    private void OnEnable()
    {
        if (WindowManager.Instance && WindowManager.Instance.PreviousWindow && WindowManager.Instance.PreviousWindow == additionalWindow)
        {
            transform.GetComponent<LegacyButton>().animator.Play("Punk");
        }
    }
    public void ResetTriggers()
    {
        transform.GetComponent<LegacyButton>().animator.ResetTrigger(transform.GetComponent<LegacyButton>().animationTriggers.normalTrigger);
    }
    public void SetTriggers()
    {
        transform.GetComponent<LegacyButton>().animator.SetTrigger(transform.GetComponent<LegacyButton>().animationTriggers.normalTrigger);
    }
}
