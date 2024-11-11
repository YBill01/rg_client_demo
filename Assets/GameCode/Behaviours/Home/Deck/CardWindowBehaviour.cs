using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardWindowBehaviour : WindowBehaviour
{
    [SerializeField]
    private Animator WindowAnimator;

    [SerializeField]
    private CardWindowDataBehaviour CardWindowData;

    private BinaryCard currentBinaryCard;

    private ProfileInstance profile;

    public DeckCardBehaviour ClickedCard { get; private set; }

    public override void Init(Action callback)
    {
        profile = ClientWorld.Instance.GetExistingSystem<HomeSystems>().UserProfile;
        callback();
        profile.PlayerProfileUpdated.AddListener(UpdateAll);
    }

    protected override void SelfClose()
    {
        if (parent != null)
        {
            DecksWindowBehaviour desc = (parent as DecksWindowBehaviour);
            //desc.CardTutorUpdate = 0;
        }
        WindowAnimator.Play("Close");
    }

    /// <summary>
    /// Called From AnimationClip Event
    /// </summary>
    
    private void UpdateAll()
    {
        if (ClickedCard && ClickedCard.cardView!=null)
            ClickedCard.cardView.SetLabelNew();
    }

    public void ClosedAnimationFinish()
    {
        gameObject.SetActive(false);
    }

    protected override void SelfOpen()
    {
        if (parent != null)
        {
            ClickedCard = (parent as DecksWindowBehaviour).GetClickedCard();
            if (currentBinaryCard.index != ClickedCard.binaryCard.index)
            {
                //     profile.ViewCard(ClickedCard.binaryCard.index);
                currentBinaryCard = ClickedCard.binaryCard;
                CardWindowData.Init(ClickedCard);
            }
            else
                CardWindowData.UpdateAll();
        }
        gameObject.SetActive(true);
    }

    public void MissClick()
    {
        (parent as DecksWindowBehaviour).ClickedCardReset();
        (parent as DecksWindowBehaviour).ChosenCardForDragReset();        
        WindowManager.Instance.ClosePopUp();
    }

    public void UseClick()
    {
        if (!ClientWorld.Instance.Profile.DecksCollection.IsFullDesc())
        {
            (parent as DecksWindowBehaviour).CardToEmpty(currentBinaryCard.index);
        }
        else
            (parent as DecksWindowBehaviour).CardChoose(ClickedCard);
        MissClick();
    }

    public void UpgradeClick()
    {
        (parent as DecksWindowBehaviour).WindowUpgradeCardOpen();

        if (profile.IsBattleTutorial &&
            profile.HasSoftTutorialState(SoftTutorial.SoftTutorialState.UpgradeCard))
        {
            WindowManager.Instance.GetUpPanel().BackButtonBlick();
        }
    }
}
