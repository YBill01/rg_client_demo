using UnityEngine;
using System.Collections.Generic;
using Legacy.Client;
using Legacy.Database;
using UnityEngine.UI;
using DG.Tweening;
using Legacy.Visual.NonTweenAnimations;
using Spine.Unity;
using System.Collections;
using System;

public class MenuTutorialPointerBehaviour : MonoBehaviour
{
    #region internal tools
    [SerializeField]
    RectTransform Pointer;

    [SerializeField]
    Animator PointerAnimator;

    [SerializeField]
    SkeletonAnimation PointerSkeletonAnimator;

    [SerializeField]
    GameObject HeroMessage;

    [SerializeField]
    public PopupMessageBehaviour popupMessage;
    #endregion

    private LegacyButton currentTargetButton;

    private RectTransform PointerTarget = null;

    private IEnumerator punkCoroutine;
    bool hideHand = false;

    private Action onTargetClick;

    
    private void Start()
    {
        PointerAnimator.SetBool("active", false);
    }

    void Update()
    {
        if (hideHand && Pointer.gameObject.activeSelf)
            Pointer.gameObject.SetActive(false);

        if (!hideHand && !Pointer.gameObject.activeSelf)
            Pointer.gameObject.SetActive(true);

        if (PointerTarget != null)
        {
            Pointer.position = PointerTarget.position;
            PointerAnimator.SetBool("active", PointerTarget.gameObject.activeInHierarchy);
        }
    }


    public void PointerToRect(RectTransform target, LegacyButton button, Action onTargetClick = null)
    {
        PointerTarget = target;
        currentTargetButton = button;
        this.onTargetClick = onTargetClick;

        Debug.Log($"PointerToRect {PointerTarget?.name}");

        ShowPointer(PointerTarget.position);
        currentTargetButton.onClick.AddListener(OnClick);
    }

    private void ShowPointer(Vector3 position)
    {
        Pointer.gameObject.SetActive(true);

        //TODO use id not string
        bool isActive = PointerTarget?.gameObject?.activeInHierarchy ?? true;
        if (isActive)
        {
            StartCoroutine(WaitShow());
        }
        //  PointerAnimator.SetBool("active", isActive);
        Pointer.position = position;
        PointerSkeletonAnimator.AnimationName = "";
        PointerSkeletonAnimator.AnimationName = "clicks";

        if (punkCoroutine != null)
            StopCoroutine(punkCoroutine);
        punkCoroutine = SimulateButtonClick();
        StartCoroutine(punkCoroutine);
    }

    public void ReleasePointer()
    {
        Debug.Log("HidePointer");
        PointerAnimator.SetBool("active", false);
        if (punkCoroutine != null)
            StopCoroutine(punkCoroutine);

        currentTargetButton?.onClick.RemoveListener(OnClick);
        onTargetClick = null;
        currentTargetButton = null;
        PointerTarget = null;
    }

    private void OnClick()
    {
        Debug.Log($"OnTutorialClick {PointerTarget?.name}");

        onTargetClick?.Invoke();
        ReleasePointer();

        //CreateOnTapEntity();
        if (HeroMessage.activeSelf)
            HeroMessage.SetActive(false);
    }

    private IEnumerator WaitShow()
    {
        yield return new WaitForSeconds(1);
        PointerAnimator.SetBool("active", true);
    }

    private IEnumerator SimulateButtonClick()
    {
        yield return new WaitForSeconds(0.6f);
        while (true)
        {
            if (currentTargetButton.gameObject.activeInHierarchy)
                currentTargetButton.animator?.Play("PressPunk");
            yield return new WaitForSeconds(1.533f);
        }
    }

    public void HideStrongPointer()
    {
        ReleasePointer();
        Pointer.gameObject.SetActive(false);
    }

    public void HidePointerTemporary()
    {
        hideHand = true;
    }

    public void UnhidePointer()
    {
        hideHand = false;
    }

    
    public void ShowHeroMessage()
    {
        if (WindowManager.Instance.CurrentWindow is ArenaWindowBehaviour)
        {
            var window = WindowManager.Instance.CurrentWindow as ArenaWindowBehaviour;
            window.gameObject.GetComponent<ArenaListBehaviour>().StopScrolling();
        }

        WindowManager.Instance.ShowBack(false);
        HeroMessage.SetActive(true);

        foreach (Transform child in HeroMessage.transform)
        {
            if (!child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    public void HideHeroMessage()
    {
        if (HeroMessage.activeSelf)
            HeroMessage.SetActive(false);
    }

    public void SetFlipHandVariant1()
    {
        Pointer.localScale = new Vector3(-1, -1, 1);
        Pointer.localRotation = Quaternion.Euler(0, 0, 90);
    }

    private void SetDefaultHandScale()
    {
        Pointer.localScale = Vector3.one;
        Pointer.localRotation = Quaternion.identity;
    }

}