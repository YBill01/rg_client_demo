using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardUpgradeWindowBehavior : WindowBehaviour
{
    public static CardUpgradeWindowBehavior Instance;
    public DeckCardBehaviour ClickdCard { get; set; }

    [SerializeField] private CardGridUpBehaviour grid;
    [SerializeField] private DeckCardBehaviour CardPrefab;
    [SerializeField] private CardProgressBarBehaviour progressLvlBehaviour;

    private List<HeroParamBehaviour> paramsList;
    private CardUpgradeWindowState state;
    private BinaryCard currentBinaryCard;
    public enum CardUpgradeWindowState : byte
    {
        Start = 0,
        WaitingResult = 1,
        GettingParams = 2,
        Finished = 3
    }

    public override void Init(Action callback)
    {
        paramsList = new List<HeroParamBehaviour>();
        Instance = this;
        state = CardUpgradeWindowState.Start;
        callback();
    }

    protected override void SelfOpen()
    {
        if (parent != null)
        {
            ClickdCard = (parent as DecksWindowBehaviour).GetClickedCard();
            {
                ResetWindow();

                currentBinaryCard = ClickdCard.binaryCard;
                CardPrefab.Init(ClickdCard.binaryCard);
                var cardData = ClientWorld.Instance.Profile.Inventory.GetCardData(ClickdCard.binaryCard.index);
                CardPrefab.SetData(cardData, null, 0.9f, () => { });
                CardPrefab.SetCardName(ClickdCard.binaryCard.title);
                CardPrefab.InDeckBehaviour.GetComponent<Canvas>().sortingLayerName = "UI";
                SetProgressBar();
                CreateParams(ClickdCard.binaryCard);

                AnalyticsManager.Instance.CardUpgrade(currentBinaryCard, CardPrefab.GetPlayerCard());
                ClientWorld.Instance.Profile.UpgradeCard(ClickdCard.binaryCard.index);

                //Так как игрок может улучшить карту до того как успеет начаться туториал улучшения карты - мы проходим тутор по первому же улучшению карты
                SoftTutorialManager.Instance.CompliteTutorial(SoftTutorial.SoftTutorialState.UpgradeCard);
            }
        }
        gameObject.SetActive(true);
        state = CardUpgradeWindowState.Finished;
    }

    protected override void SelfClose()
    {
        gameObject.SetActive(false);
    }

    public void DisableGlows(bool flag)
    {
        CardPrefab.cardView.SetGlow(flag);
        CardPrefab.cardView.StrokeGO(flag);
        CardPrefab.cardView.SetShadow(flag);
        CardPrefab.cardView.SetFrame(flag);
    }

    public void SetLevel()
    {
        CardPrefab.InDeckBehaviour.GetComponentInChildren<CardTextDataBehaviour>().SetLevel(ClientWorld.Instance.Profile.Inventory.GetCardData(ClickdCard.binaryCard.index).level);
    }

    public void SetProgressBar()
    {
        progressLvlBehaviour.ProgressValue.SetMaxSlider();
    }

    public void TapToClose()
    {
        if (GetComponent<LevelUpCardBehaviour>().TapToContinueEnabled)
        {
            StartCoroutine(CloseWindow());
        }
    }

    IEnumerator CloseWindow()
    {
        yield return new WaitForSeconds(0.3f);
        (parent as DecksWindowBehaviour).WindowUpgradeCardClose();
        //MenuTutorialPointerBehaviour.CreateOnTapEntity();
    }

    private void CreateParams(BinaryCard bCard)
    {
        paramsList = new List<HeroParamBehaviour>();
        paramsList.AddRange(grid.AddParamsToSkill(bCard));
    }

    void ResetWindow()
    {
        state = CardUpgradeWindowState.Start;
    }
}
