using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Client;
using Legacy.Database;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckCardBehaviour : MonoBehaviour,/* IPointerDownHandler, IPointerUpHandler,IDragHandler,*/ IPointerEnterHandler, IPointerExitHandler
{
    public DecksWindowBehaviour DeckWindow;

    [SerializeField]
    public CardViewBehaviour cardView;
    [SerializeField]
    private CardTextDataBehaviour cardText;
    
    public InDeckBehaviour InDeckBehaviour;

    [SerializeField]
    public RectTransform CardRect;

    public BinaryCard binaryCard;

    [SerializeField]
    public TextMeshProUGUI cardName;

    [SerializeField]
    private GameObject ButtonsBack;


    [SerializeField]
    private bool shouldUpdateLevel = true;

    [SerializeField]
    private GameObject cardBackImage;

    [SerializeField]
    private GameObject UseButtonContainer;
    [SerializeField]
    private GameObject DeleteCardButton;
    [SerializeField]
    public GameObject UpgradeButton;
    [SerializeField]
    public GameObject InfoButton;
    [SerializeField]
    private ButtonWithPriceViewBehaviour UpgradeButtonWithPrice;

    private ClientCardData PlayerCard;
    private ProfileInstance PlayerProfile;

    public float scale;
    public static bool isSelected = false;
    public static DeckCardBehaviour SelectedDeckCard;
    private bool isDragging = false;

    public ClientCardData GetPlayerCard()
    {
        return PlayerCard;
    }

    internal void Init(BinaryCard card)
    {
        if(TryGetComponent(out Image img))
        {
            img.enabled = false;
        }
        if(cardBackImage != null)
        {
            cardBackImage.SetActive(false);
        }
        PlayerProfile = ClientWorld.Instance.Profile;
        binaryCard = card;
        cardView.Init(binaryCard);
        InDeckBehaviour.SetBinary(binaryCard);
        UpdateButtonsPanel();
    }

    public void InitEmptySlot()
    {
        PlayerProfile = ClientWorld.Instance.Profile;
        binaryCard = default(BinaryCard);
        InDeckBehaviour.SetBinary(binaryCard);
        InDeckBehaviour.gameObject.SetActive(false);
    }

    public void UpdateAll()
    {
        if (binaryCard.index == 0) return;
        UpdateButtonsPanel();
        UpdData(PlayerProfile.Inventory.GetCardData(binaryCard.index));//отут обновляет каунты всех карт
        //cardView.Init(binaryCard);//отут обновляет индексы рарность и тд тп
        //InDeckBehaviour.SetBinary(binaryCard);
    }

    public void UpdateButtonsPanel()
    {
        binaryCard = InDeckBehaviour.card;
        PlayerCard = PlayerProfile.Inventory.GetCardData(binaryCard.index);
        cardView = InDeckBehaviour.GetComponent<CardViewBehaviour>();
        cardText = InDeckBehaviour.GetComponent<CardTextDataBehaviour>();
        UseButtonUpdate();
    }


    public void SetCardText(CardTextDataBehaviour _cardText)
    {
        cardText = _cardText;
    }
    public void SetData(ClientCardData cardData, DecksWindowBehaviour windowBehaviour, float scaleInDeck, UnityAction OpenCardWindow)
    {
        InitData(cardData);
        DeckWindow = windowBehaviour;
        scale = scaleInDeck;
        UpgradeButton.GetComponent<LegacyButton>().onClick.AddListener(OpenCardWindow);
        //  UpgradeButtonWithPrice.isCloseLook=true;
         UpgradeButtonWithPrice.IsNotEnoughtCoins(false);
         UpgradeButtonWithPrice.IsNotLockedClick(true);
        UpgradeButtonWithPrice.UpdateView();

        InfoButton.GetComponent<LegacyButton>().onClick.AddListener(OpenCardWindow);
       /* if(PlayerProfile == null)
            PlayerProfile = ClientWorld.Instance.Profile;*/
        PlayerProfile.PlayerProfileUpdated.AddListener(UpdateAll);
    }

    public bool IsInDeck()
    {
        return PlayerProfile.DecksCollection.IsCardInDeck(binaryCard.index);
    }
    private void UseButtonUpdate()
    {
        UseButtonContainer.SetActive(!IsInDeck());
        if (DeleteCardButton)
        {
            if (PlayerProfile.IsBattleTutorial)
            {
                DeleteCardButton.SetActive(false);
            }
            else
                DeleteCardButton.SetActive(IsInDeck());
        }
    }

    
    public void ShowButtons()//сделать так чтоб драг не перекрывал клики
    {
        /*
        if (PlayerProfile.IsBattleTutorial)
        {
            if (DeckWindow.CardTutorUpdate > 0 && HomeTutorialHelper.Instance.IsDescTutor())
            {
                if (binaryCard.index != DeckWindow.CardTutorUpdate)
                {
                    return;
                }
                else if (ClientWorld.Instance.Profile.HardTutorialState == 2)
                {
                    HomeTutorialHelper.Instance.HardHomeTutorStep = 23;
                    DeckWindow.CardClick(this);
                    DeckWindow.OpenCardWindow();
                    return;
                }
            }
          //  if (ClientWorld.Instance.Profile.HardTutorialState == 2/ * && HomeTutorialHelper.Instance.HardHomeTutorStep == 23* /)
          //  {
             //   return;
           // }
        }*/
        
        if (binaryCard.index == 0) return;
        if (DeckWindow == null || DeckWindow.ChosenCard != null) return;
        if (ButtonsBack.activeInHierarchy) return;
        DeckWindow.CardClick(this);

        var canvas = gameObject.GetComponent<Canvas>();
        if (canvas)
            Destroy(canvas);

         canvas = gameObject.AddComponent<Canvas>();

        var raycaster = gameObject.AddComponent<GraphicRaycaster>();
            
        if (canvas)
        {
            canvas.gameObject.SetActive(true);
            canvas.overrideSorting = true;
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = 20;
        }

        SetCardName(binaryCard.title);
        UpgradeButtonWithPrice.SetPrice(PlayerCard.SoftToUpgrade);
        UpgradeButton.SetActive(PlayerCard.CanUpgrade);
        InfoButton.SetActive(!PlayerCard.CanUpgrade);
        ButtonsBack.SetActive(true);
    }

    public void SetCardName(string name)
    {
        cardName.text = Locales.Get(name);
    }
    public void HideButtons()
    {
        Destroy(GetComponent<GraphicRaycaster>());
        Destroy(GetComponent<Canvas>());
        ButtonsBack.SetActive(false);
    }

    internal void InitData(ClientCardData cardData)
    {

        PlayerCard = cardData;
        cardText.SetLevel(cardData.level);
        if(PlayerProfile== null)
            PlayerProfile = ClientWorld.Instance.Profile;
        if (binaryCard.index == 0)
        {
            if (Cards.Instance.Get(cardData.index, out BinaryCard _card))
            {
                binaryCard = _card;
            }
            
        }
        cardText.SetManaCost(binaryCard.manaCost);
        cardText.SetCount(
            cardData.count,
            PlayerCard.CardsToUpgrade
        );
    }
    internal void UpdData(ClientCardData cardData)
    {
        PlayerCard = cardData;
        if (shouldUpdateLevel)//господи помоги
            cardText.SetLevel(cardData.level);
        cardText.SetManaCost(binaryCard.manaCost);
        cardText.SetCount(
            cardData.count,
            PlayerCard.CardsToUpgrade
        );
    }

    public void Change()//step 1  step 3
    {
        /*if (PlayerProfile.IsBattleTutorial)
        {
            if (DeckWindow.CardTutorUpdate > 0 && HomeTutorialHelper.Instance.IsDescTutor())
            {
                if (binaryCard.index != DeckWindow.CardTutorUpdate)
                    return;
            }
        }*/
        if (binaryCard.index != 0 && DeckWindow != null)
        {
            DeckWindow.ChangeCard(this);
            SelectedDeckCard = null;
        }
    }
    public void DeChoose() // убираем и освобождаем карту.
    {
        DeckWindow.DecCardToEmpty(binaryCard.index);
        isSelected = false;
        SelectedDeckCard = null;
         HideButtons();
    }
    public void Choose()//step 2
    {
        isSelected = true;
        SelectedDeckCard = this;
        if (!ClientWorld.Instance.Profile.DecksCollection.IsFullDesc())
        {
            isSelected = false;
            SelectedDeckCard = null;
            DeckWindow.CardToEmpty(binaryCard.index);
            HideButtons();
        }
        else
        {
            DeckWindow.CardChoose(this);
        }
        SoundsPlayerManager.Instance.PlaySound(SoundName.Card_use_menu);
    }


    internal float GetHeightOffset()
    {
        return ButtonsBack.GetComponent<RectTransform>().rect.height / 5.0f;
    }

    private void OnDestroy()
    {
        PlayerProfile?.PlayerProfileUpdated?.RemoveListener(UpdateAll);
    }
    public static DeckCardBehaviour hoveredCard;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SelectedDeckCard)
        {
            DeckWindow.OnHoverSetDeckCard(this);
            DeckWindow.ScaleAndOffsetPosition(this);
            hoveredCard = this;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SelectedDeckCard)
            DeckWindow.RescaleAndStartPosition(this);
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!hoveredCard) return;
            if (isSelected && SelectedDeckCard && SelectedDeckCard.InDeckBehaviour == hoveredCard.InDeckBehaviour)
            {
                DeckWindow.OnBeginDrag(SelectedDeckCard);
                isSelected = false;
                isDragging = true;
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (!isSelected && isDragging && SelectedDeckCard)
            {
                DeckWindow.OnMoveCard(SelectedDeckCard);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (!isSelected && isDragging)
            {
                DeckWindow.SetChoosenCarsOnBeginDrag(SelectedDeckCard);
                DeckWindow.OnEndDrag(SelectedDeckCard);
                SelectedDeckCard = null;
                isDragging = false;
                isSelected = true;
            }
        }
    }

    public void OnEmptySlot(bool v = true)
    {

        cardBackImage.SetActive(v);
  /*      if (TryGetComponent(out Image img))
        {
            img.enabled = v;
        }*/
    }

}
