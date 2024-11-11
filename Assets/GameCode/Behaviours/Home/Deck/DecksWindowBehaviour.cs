using DG.Tweening;
using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Legacy.Client.LootBoxWindowBehaviour;

namespace Legacy.Client
{
    public class DecksWindowBehaviour : WindowBehaviour //, IPointerClickHandler
    {
        private ProfileInstance Profile;
        [SerializeField] private RectTransform MainCanvas;
        [SerializeField] private TextMeshProUGUI DeckNumber;
        [SerializeField] private TextMeshProUGUI AverageMana;
        [SerializeField] private TextMeshProUGUI FoundedCards;
        [SerializeField] private TextMeshProUGUI TotalCards;

        internal DeckCardBehaviour GetClickedCard()
        {
            return ClickedCard;
        }

        internal void ClickedCardReset()
        {
            if (ClickedCard != null)
            {
                ClickedCard.HideButtons();
                if (ClickedCard.cardView != null)
                     ClickedCard.cardView.SetLabelNew(false);
            }

            ClickedCard = null;
        }

        internal void ChosenCardForDragReset()
        {
            ChosenCardForDrag = null;
        }
       /* public Ease typeEase;  //для настройки анимации карт рантайм
        [SerializeField, Range(0.0f, 5.0f)] public float speedTab;
        [SerializeField, Range(0.0f, 5.0f)] public float speedChange;*/

        [SerializeField] private TextMeshProUGUI SortText;

        [SerializeField] private ScrollRect scrollRect;

        [SerializeField] private RectTransform ContentRect; //меняется хейт

        [SerializeField] private RectTransform ViewPortRect;

        [SerializeField] private HeroPanelBehaviour HeroPanel;

        [SerializeField] private GameObject Fader;
        [SerializeField] private GameObject ChooseFader;

        [SerializeField] private GameObject CardPrefab;
        [SerializeField] private RectTransform PoolCardsRect;
        private Dictionary<ushort, GameObject> pool = new Dictionary<ushort, GameObject>();

        [SerializeField] private GameObject MissClickObject;
        [SerializeField] private CanvasGroup ListCanvasGroup;
        [SerializeField] private CanvasGroup NotFoundedListCanvasGroup;
        [SerializeField, Range(0.0f, 1.0f)] private float scaleInList;

        [SerializeField] private GameObject NotFoundedCardPrefab;
        [SerializeField] private RectTransform NotFoundedGrid;

        [SerializeField] private float scrollSpeed = 0.4f;
        [SerializeField] private AnimationCurve curve;


        [SerializeField] float standartCardScale = 0.6f;
        [SerializeField] float enlargedCardScale = 0.65f;

        private DeckCardBehaviour deckCard;
        private InDeckBehaviour rememberedLastTouchedCard;
        private bool isDragging = false;
        private bool isChoosenCard = false;
        private Vector3 moveVector;
        private Vector3 offsetVector;


       private List<DeckCardBehaviour> DeckCardsObjects = new List<DeckCardBehaviour>();
        private List<DeckCardBehaviour> DeckCardNotFounded = new List<DeckCardBehaviour>();

        private float aspectAdditionalX;
        private float aspectAdditionalY;
        private float partOfContainerToMove = 12f;
        private float elapsedTime = 0;
        private bool startAnimation = false;
        private bool upgradeCardOpen;


        //new 
        private InDeckPanel inDeckPanel;
        private InFoundDeckPanel inFoundDeckPanel;
        private DeckTabPanel deckTabPanel;
        private ushort isEndDrag = 0;
        private bool inTabDeck = false;
        private bool UpScroll = false;
        public override void Init(Action callback)
        {
            Profile = ClientWorld.Instance.Profile;

            InitData();
            Profile.HeroSelectEvent.AddListener(delegate { InitHero(); });
            Profile.DecksCollection.DeckChangeEvent.AddListener(delegate { SynchronizeWithProfile(); });

            callback();
            if (Profile.DecksCollection.In_collection.Length > 0)
                CheckedFullSets();

        }
        private void CheckedFullSets()
        {
            foreach (CardSet sets in Profile.DecksCollection.CardSets)
            {
                if (sets.Cards.Length < 8)
                {
                    NetworkMessageHelper.ChangeCardInDeck(World.DefaultGameObjectInjectionWorld.EntityManager, 0, 0);
                    break;
                }
            }
        }

        protected override void SelfClose()
        {
            //CardTutorUpdate = 0;
            MissClick();
            if (ChosenCard)
            {
             //   ChosenCard.GetComponent<RectTransform>().SetParent(PreviousParent.GetComponent<RectTransform>());
            //    ChosenCard.SetPosition(Vector3.zero);
             //   ChosenCard.transform.localPosition = Vector3.zero;
                ClearChoosenDragCard();
            }
            gameObject.SetActive(false);
            isTutorScrollBlock = false;
        //    scrollRect.onValueChanged.RemoveListener(OnScrollValue);
        }

        protected override void SelfOpen()
        {
            gameObject.SetActive(true);
            DeckCardBehaviour.SelectedDeckCard = null;
            ClickedCardReset();
            ChosenCardForDragReset();
            Fader.SetActive(false);

            scrollRect.normalizedPosition = new Vector2(0, 1);

        //    if (Profile.HardTutorialState == 3)  -в третем шаге карты не открываем.
         //       HomeTutorialHelper.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.UpgradeCard);
            deckTabPanel.CloseInTutor();

            isEmptyTocard = false;
            isEmptyTocard2 = false;
            isEndDrag = 0;
            inTabDeck = false;

            OnShow();

            SoftTutorialManager.Instance.CheckTutorialsForCurrentWindowFast();
        }
        public void WindowUpgradeCardOpen()
        {
            WindowManager.Instance.ClosePopUp();
            WindowManager.Instance.NotWait();
            WindowManager.Instance.OpenWindow(childs_windows[1]);
            /*if (ClientWorld.Instance.Profile.HardTutorialState == 2 && HomeTutorialHelper.Instance.HardHomeTutorStep == 23)
            {
                HomeTutorialHelper.Instance.HardHomeTutorStep = 23;
                DeChooseThings();
            }
            else*/

            upgradeCardOpen = true;
        }

        public void WindowUpgradeCardClose()
        {
            WindowManager.Instance.Back();
            RewardParticlesBehaviour.Instance.Drop(
                       rememberedLastTouchedCard.transform.position,
                       10,
                       LootCardType.Exp
                   );
            upgradeCardOpen = false;
            /*  var rememberedCard = DeckCardsObjects.Where(x => x.InDeckBehaviour == rememberedLastTouchedCard).FirstOrDefault();

              ChangeCard(rememberedCard);
              rememberedCard.ShowButtons();
              ClickedCard = rememberedCard;
              RewardParticlesBehaviour.Instance.Drop(
                          rememberedLastTouchedCard.transform.position,
                          10,
                          LootCardType.Exp
                      );
              upgradeCardOpen = false;
              */
        }

        public void ScrollAllDeckIfTutorial(Action onFinish)
        {
            StartCoroutine(ScrollAllDeckIfTutorialCor(onFinish));
        }

        private bool isTutorScrollBlock = false;
        private IEnumerator ScrollAllDeckIfTutorialCor(Action onFinish)
        {
            scrollRect.normalizedPosition = new Vector2(0, 0);
            elapsedTime = 0;
            startAnimation = true;
            isTutorScrollBlock = true;

            yield return new WaitForSeconds(1f);

            while (scrollRect.normalizedPosition.y <= 0.99f)
            {
                var percentageComplete = curve.Evaluate(elapsedTime * scrollSpeed * Mathf.PI / 2);
                scrollRect.normalizedPosition = new Vector2(0, percentageComplete);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            startAnimation = false;
            isTutorScrollBlock = false;
            onFinish?.Invoke();
        }

        public void OpenCardWindow()
        {
            uint expCad = ClickedCard.GetPlayerCard().ExpToUpgrade;
            uint need = Levels.Instance.GetToUpgradeCount(Profile.Level.level, UpgradeCostType.ExpAccount);

            Fader.SetActive(false);
            ClickedCard.HideButtons();
            WindowManager.Instance.NotWait();
            WindowManager.Instance.OpenWindow(childs_windows[0]);
        }


        public GameObject GetCardPool(ushort idCard)
        {
            GameObject go = default;
            if (pool.ContainsKey(idCard))
            {
                go = pool[idCard];
                pool.Remove(idCard);
            }
            return go;
        }
        public void SetCardPool(ushort _index, GameObject _card, bool inPosition)
        {
            InDeckBehaviour inDeckCard = _card.GetComponent<InDeckBehaviour>();
            if (inPosition)
            {
                inDeckCard.GetComponent<RectTransform>().SetParent(PoolCardsRect);
                inDeckCard.SetPoolPosition();
                inDeckCard.typeAnim = true;
            }else
                inDeckCard.typeAnim = false;

            if (!pool.ContainsKey(_index))
                pool.Add(_index, _card);
        }
        private void OnShow()
        {
            inDeckPanel.Show();
            inFoundDeckPanel.Show();
        }
        private void SynchronizeWithProfile()
        {
            
            if (isEndDrag>0)
            {
                CardRepeat();
                return;
            }
            if(isCardToEmpty || isCardToEmpty2)
            {
                if (isCardToEmpty)
                {
                    DeckCardToEmptyReparent();
                    return;
                }
                isCardToEmpty2 = false;
                FaderClick();
                return;
            }
            if (isEmptyTocard || isEmptyTocard2)
            {
                if (isEmptyTocard)
                {
                    CardToEmptyReparent();
                    return;
               }
                isEmptyTocard2 = false;
                FaderClick();
                if(ClientWorld.Instance.Profile.DecksCollection.IsFullDesc()) 
                {
                    UpScroll = true;
                }
                return;
            }
           
            if (inTabDeck)
            {
                inTabDeck = false;
                InitTabs();
                InitHero();
                DeckNumber.text = Locales.Get("locale:940", Profile.DecksCollection.Active_set_id + 1);
                return;
            }

         
            

            inDeckPanel.Hide();
            inFoundDeckPanel.Hide();
            CheckNewCard();
            OnShow();
        }
     
        private void CheckNewCard()
        {
            ushort count =(ushort)( Profile.DecksCollection.In_deck.Length + Profile.DecksCollection.In_collection.Length);
            bool isFind = false;
            if (count > pool.Count)
            {
                foreach (byte cardID in Profile.DecksCollection.In_collection)
                {
                    if (cardID > 0 && !pool.ContainsKey(cardID))
                    {
                        creadCard(cardID);
                        HideInNotFounded(cardID);
                        isFind = true;
                    }
                }
                 foreach (byte idCard in Profile.DecksCollection.In_deck)
                 {
                        if (idCard > 0 && !pool.ContainsKey(idCard))
                        {
                            creadCard(idCard);
                            HideInNotFounded(idCard);
                            isFind = true;
                         }
                 }

                if (isFind)
                {
                    InitTexts();
                   // WindowManager.Instance.MainWindow.isNewCard = true;
                }
            }
        }

        private void HideInNotFounded(byte cardID)
        {
           foreach(DeckCardBehaviour deckCard in DeckCardNotFounded)
            {
                if(deckCard.binaryCard.index == cardID)
                {
                    deckCard.gameObject.SetActive(false);
                    break;
                }
            }
        }

        private void CardRepeat()
        {
            inFoundDeckPanel.GetCardToReparent(ChosenCard.card.index,true);
            inDeckPanel.GetCardToReparent(isEndDrag,true);
            inFoundDeckPanel.GetCardToReparent(isEndDrag, false);
            inDeckPanel.GetCardToReparent(ChosenCard.card.index, false);
            isEndDrag = 0;
            DeChooseThings();
        }

        bool isEmptyTocard = false;
        bool isEmptyTocard2 = false;
        public void CardToEmpty(ushort index)
        {
            isEmptyTocard = true;
            isEmptyTocard2 = true;
            Profile.DecksCollection.ActiveSet.ReplaceCards(0, index);
        }
        bool isCardToEmpty = false;
        bool isCardToEmpty2 = false;
        public void DecCardToEmpty(ushort index)
        {
            isCardToEmpty = true;
            isCardToEmpty2 = true;
            Profile.DecksCollection.ActiveSet.ReplaceCards(index, 0);
        }
        public void CardToEmptyReparent()
        {
            isEmptyTocard = false;
            ushort id = ClickedCard.binaryCard.index;
            inFoundDeckPanel.GetCardToEmpty(id);
            inDeckPanel.GetCardToEmpty(id);
        }
        public void DeckCardToEmptyReparent()
        {
            isCardToEmpty = false;
            ushort id = ClickedCard.binaryCard.index;
            inDeckPanel.CardToEmpty(id);
            inFoundDeckPanel.EmptyToCard(id);
            //  inFoundDeckPanel.GetCardToEmpty(id);
            // inDeckPanel.GetCardToEmpty(id);
        }
        private void InitTexts()
        {
            DeckNumber.text = Locales.Get("locale:940", Profile.DecksCollection.Active_set_id + 1);
            AverageMana.text = LegacyHelpers.GetNiceValue(Profile.DecksCollection.ActiveSet.AverageMana, 1).ToString();
            byte _count = 0;
            foreach (ushort cardID in Profile.DecksCollection.In_deck)
            {
                if (cardID > 0)
                    _count++;
            }
               FoundedCards.text = (Profile.DecksCollection.In_collection.Length + _count /*Profile.DecksCollection.In_deck.Length*/).ToString();
            TotalCards.text = " / " + Cards.Instance.GetEnabledCardsByArena().Count.ToString();
            SortText.text = Profile.DecksCollection.CurrentSortName;
        }

        private void Update()
        {
            if (!startAnimation)
            {
                if (isTutorScrollBlock)
                {
                    ContentRect.anchoredPosition = Vector2.Lerp(ContentRect.anchoredPosition, contentPosition, ScrollToCardSpeed);
                    if (ContentRect.anchoredPosition == contentPosition)
                        isTutorScrollBlock = false;
                }
                if(UpScroll)
                {
                    contentPosition.y = 0.0f;
                    ContentRect.anchoredPosition = Vector2.Lerp(ContentRect.anchoredPosition, contentPosition, ScrollToCardSpeed);
                    if (ContentRect.anchoredPosition ==  contentPosition)
                        UpScroll = false;
                }
                if (ChosenCard != null)
                {
                    contentPosition.y = 0.0f;
                    ContentRect.anchoredPosition =  Vector2.Lerp(ContentRect.anchoredPosition, contentPosition, ScrollToCardSpeed);
                }
                else
                {
                    if (ClickedCard != null)
                    {
                        ContentRect.anchoredPosition = Vector2.Lerp(ContentRect.anchoredPosition, contentPosition, ScrollToCardSpeed);
                    }
                }
            }
        }

        private void InitData()
        {
            InitHero();
            InitCards();
            InitTabs();
            InitTexts();
        }

        private void InitCards()
        {
            if (inDeckPanel == null)
            {
                inDeckPanel = gameObject.GetComponent<InDeckPanel>();
                inDeckPanel.Init(this);
            }
            if(inFoundDeckPanel == null)
            {
                inFoundDeckPanel = gameObject.GetComponent<InFoundDeckPanel>();
                inFoundDeckPanel.Init(this);
            }
            foreach (byte inDeck in  Profile.DecksCollection.In_deck) // 
            {
                if (inDeck > 0)
                {
                    creadCard(inDeck);
                }
            }
            foreach (byte cardID in Profile.DecksCollection.In_collection)
            {
                creadCard(cardID);
            }

            //old cards
             foreach (ushort cardID in Profile.DecksCollection.NotFound)
             {
                 DeckCardBehaviour notFoundedObject = InitCard(cardID, NotFoundedCardPrefab, NotFoundedGrid);
                 Cards.Instance.Get(cardID, out BinaryCard _card);
                 notFoundedObject.GetComponent<NotFoundedCardBehaviour>().Opened(Cards.Instance.GetArenaNumber(cardID), _card);
                 notFoundedObject.cardView.SetLabelNew(false);
                 notFoundedObject.InDeckBehaviour.SetScale(scaleInList);
                 DeckCardNotFounded.Add(notFoundedObject);
             }

             foreach (ushort cardID in Profile.DecksCollection.Unavailable)
             {
                 DeckCardBehaviour notFoundedObject = InitCard(cardID, NotFoundedCardPrefab, NotFoundedGrid);
                 Cards.Instance.Get(cardID, out BinaryCard _card);
                 notFoundedObject.GetComponent<NotFoundedCardBehaviour>().Locked(Cards.Instance.GetArenaNumber(cardID), _card);
                 notFoundedObject.cardView.SetLabelNew(false);
                 notFoundedObject.InDeckBehaviour.SetScale(scaleInList);
                  DeckCardNotFounded.Add(notFoundedObject);
             }

        }

        private void creadCard(byte inDeck)
        {
            if (Cards.Instance.Get(inDeck, out BinaryCard _card))
            {
                GameObject CardObject = Instantiate(CardPrefab, PoolCardsRect);
                CardViewBehaviour cardView = CardObject.GetComponent<CardViewBehaviour>();
                cardView.Init(_card);
                CardObject.GetComponent<InDeckBehaviour>().SetBinary(_card);
                 pool.Add(_card.index, CardObject);
            }
        }


        private void InitTabs()
        {
            if (deckTabPanel == null)
            {
                deckTabPanel = gameObject.GetComponent<DeckTabPanel>();
            }
            deckTabPanel.Init();
        }

        private void InitHero()
        {
            HeroPanel.InitFromDeck(Profile);
        }

        public void MissClick()
        {
            UpScroll = false;
            if(isDragging || isChoosenCard)
            {
                return;
            }
            DeChoose();

        }
        public void OnScrolls()
        {
            UpScroll = false;
        }

        public void NextHero()
        {
            Profile.SelectNextHero();
        }

        public void PreviousHero()
        {
            Profile.SelectPreviousHero();
        }

        //private float oldTab = 0;
        public void ChangeDeck(int deck)
        {
           /* if (oldTab == 0)
            {
                oldTab = Time.time;
            } else if (oldTab + 3f > Time.time)
            {
                return;
            }
            oldTab = Time.time;*/
            inTabDeck = true;
            DeChooseThings();
            Profile.DecksCollection.ChooseSet((byte)deck);
        }

        void ScrollToCard(DeckCardBehaviour card)
        {
            var rect = card.GetComponent<RectTransform>();
            Vector2 centeredPos =
                (Vector2)scrollRect.transform.InverseTransformPoint(ContentRect.position)
                - (Vector2)scrollRect.transform.InverseTransformPoint(rect.position);

            contentPosition.x = ContentRect.position.x;
            float y = centeredPos.y - ViewPortRect.rect.height / 2 + card.GetHeightOffset();
            contentPosition.y = Mathf.Clamp(y, 0.0f, ContentRect.rect.height - ViewPortRect.rect.height);
        }

        public void ChangeCard(DeckCardBehaviour deckCardTarget)
        {
            if (ChosenCard != null)
            {
                SoundsPlayerManager.Instance.PlaySound(SoundName.Card_change_menu);
                isEndDrag = deckCardTarget.InDeckBehaviour.card.index;
                Profile.DecksCollection.ActiveSet.ReplaceCards(deckCardTarget.InDeckBehaviour.card.index,
                    ChosenCard.card.index);
            }

            DeChooseThings();
        }

        public void OnMoveCard(DeckCardBehaviour deck_card)
        {
            Vector2 lp = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(ContentRect, Input.mousePosition, Camera.main,  out lp);
            deck_card.InDeckBehaviour.transform.localPosition = lp;
        }

        public void OnBeginDrag(DeckCardBehaviour deck_card)
        {
            if (ChosenCard != deck_card.InDeckBehaviour) return;
            if (!isChoosenCard) return;

            isDragging = true;
            SetParentAndPosition(deck_card);
            deck_card.InDeckBehaviour.GetComponent<CardViewBehaviour>().BecomeRaycasted(false);
            ChooseThings();
        }

        public void SetChoosenCarsOnBeginDrag(DeckCardBehaviour deckCardTarget)
        {
            if (!deckCardTarget || !ChosenCard || ChosenCard != deckCardTarget.InDeckBehaviour) return;
            SetDeckChoosenCardParamtrs(deckCardTarget);
            Destroy(deckCardTarget.InDeckBehaviour.gameObject.GetComponent<LayoutElement>());
        }

        public void OnDrag(DeckCardBehaviour deckCardTarget, PointerEventData eventData)
        {
            if (ChosenCard != deckCardTarget.InDeckBehaviour) return;
            if (!isChoosenCard) return;
            moveVector.x = eventData.delta.x * aspectAdditionalX;
            moveVector.y = eventData.delta.y * aspectAdditionalY;
            moveVector.z = 0;
            deckCardTarget.InDeckBehaviour.transform.localPosition += moveVector;
        }

        public void OnHoverSetDeckCard(DeckCardBehaviour deckCardTarget)
        {
            deckCard = deckCardTarget;
        }

        public void OnEndDrag(DeckCardBehaviour deckCardTarget)
        {
            isDragging = false;
            if (!isChoosenCard || ChosenCard.card.index == deckCard.InDeckBehaviour.card.index)
            {  // если карту не поместили на другую на доске. возвращаем назад.
                ClearChoosenDragCard();
                return;
            }
            isEndDrag = deckCard.InDeckBehaviour.card.index;
            Profile.DecksCollection.ActiveSet.ReplaceCards(deckCard.InDeckBehaviour.card.index, deckCardTarget.InDeckBehaviour.card.index);
            DeckChosenCard.InDeckBehaviour.GetComponent<CardViewBehaviour>().BecomeRaycasted(true);
           // DeChooseThings();
        }

        private void ClearChoosenDragCard()
        {
            DeChoose();
            DeckChosenCard.InDeckBehaviour.GetComponent<CardViewBehaviour>().BecomeRaycasted(true);
        }

        private void SetParentAndPosition(DeckCardBehaviour deck_card)
        {
            var cardRect = deck_card.InDeckBehaviour.GetComponent<RectTransform>();
            cardRect.SetParent(ContentRect);
            RescaleAndStartPosition(deck_card);
            deck_card.InDeckBehaviour.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
        }

        public void RescaleAndStartPosition(DeckCardBehaviour deckCardTarget)
        {
            if (isDragging)
            {
                deckCardTarget.InDeckBehaviour.SetScale(standartCardScale);
                deckCardTarget.InDeckBehaviour.SetPosition(Vector3.zero);
            }
        }

        public void ScaleAndOffsetPosition(DeckCardBehaviour deckCardTarget)
        {
            if (isDragging)
            {
                deckCardTarget.InDeckBehaviour.SetScale(enlargedCardScale);
                deckCardTarget.InDeckBehaviour.SetPosition(offsetVector);
            }
        }

        private void SetDeckChoosenCardParamtrs(DeckCardBehaviour deckCardTarget)
        {
            DeckChosenCard.InDeckBehaviour = deckCardTarget.InDeckBehaviour;
            deckCardTarget.InDeckBehaviour.transform.position = deckCardTarget.transform.position;
        }

        private void RecountCardPositionWhileHover()
        {
            offsetVector.x = 0;
            offsetVector.z = 0;
            offsetVector.y = DeckCardsObjects[0].InDeckBehaviour.transform.GetComponent<RectTransform>().rect.height /
                             partOfContainerToMove;
        }

        private void AspectRatioCount()
        {
            aspectAdditionalX = MainCanvas.rect.height / Camera.main.pixelRect.height;
            aspectAdditionalY = MainCanvas.rect.width / Camera.main.pixelRect.width;
        }

        private void Tremor(bool enable)
        {
            ListCanvasGroup.alpha = enable ? 0 : 1;
            NotFoundedListCanvasGroup.alpha = enable ? 0 : 1;
            for (byte i = 0; i < DeckCardsObjects.Count; i++)
            {
                DeckCardsObjects[i].GetComponent<Animator>().SetTrigger(enable ? "Tremor" : "Normal");
                SetTremorStateOrPrevious(enable, i);
            }
        }

        private void SetTremorStateOrPrevious(bool enable, byte i)
        {
            if (enable)
                DeckCardsObjects[i].cardView.SetStateCanUpdate(null, CardGlowState.Tremor);
            if (!enable)
                DeckCardsObjects[i].cardView
                    .SetStateCanUpdate(null, DeckCardsObjects[i].cardView.previousState); //стоп тремор
        }

        private void TremorOne(bool enable, DeckCardBehaviour deckCard)
        {
            deckCard.GetComponent<Animator>().SetTrigger(enable ? "Tremor" : "Normal");
        }


        public InDeckBehaviour ChosenCard = null;
        public InDeckBehaviour ChosenCardForDrag = null;
        public DeckCardBehaviour PreviousParent = null;
        public DeckCardBehaviour FirstParent = null;
        public DeckCardBehaviour ClickedCard = null;
        [SerializeField, Range(0.0f, 1.0f)] private float ScrollToCardSpeed;
        private Vector2 contentPosition = Vector2.zero;

        [SerializeField] private DeckCardBehaviour DeckChosenCard;

        [SerializeField] private GameObject ChosenCardContainer;
        [SerializeField] private GameObject HeroButtonObject;
        [SerializeField] private GameObject HeroArrowsObject;


        public bool isFaderClick = true;
        public void FaderClick()
        {
            if (!isFaderClick) return;
            ClickedCardReset();
            ChosenCardForDragReset();
            Fader.SetActive(false);
        }

        public void CardClick(DeckCardBehaviour card)
        {

            if (ChosenCard == null)
            {
                ClickedCard = card;
                bool isNew = ClientWorld.Instance.Profile.Inventory.GetCardData(ClickedCard.InDeckBehaviour.card.index).isNew;
                if (isNew)
                    Profile.ViewCard(ClickedCard.InDeckBehaviour.card.index);
                isNew = false;
                ClientWorld.Instance.Profile.Inventory.CardNewFlag(ClickedCard.InDeckBehaviour.card.index);
                ClickedCard.cardView.SetLabelNew(false);
               
                ChosenCardForDrag = card.InDeckBehaviour;
                rememberedLastTouchedCard = card.InDeckBehaviour;
                Fader.SetActive(true);
                ScrollToCard(ClickedCard);
            }
            else
            {
                DeChooseThings();
            }
        }

        private void DeChooseThings()
        {
            if (ChosenCard != null)
            {
                Tremor(false);
                ChosenCard.GetComponent<CardViewBehaviour>().Glow(false);
                ChosenCard.SetPosition(Vector3.zero);
                ChosenCardContainer.SetActive(false);
                HeroButtonObject.SetActive(true);
                HeroArrowsObject.SetActive(true);
                MissClickObject.SetActive(false);
                ChooseFader.SetActive(false);
                PreviousParent = null;
                ChosenCard = null;
                isChoosenCard = false;
                ClickedCard = null;
            }
        }

        private void ChooseThings()
        {
            MissClickObject.SetActive(true);
            ChosenCardContainer.SetActive(true);
            HeroButtonObject.SetActive(false);
            HeroArrowsObject.SetActive(false);
            ChooseFader.SetActive(true);
            ClickedCard = DeckChosenCard;
            Tremor(true);
            TremorOne(true, ClickedCard);
        }

        private void DeChoose()
        {
            if (ChosenCard != null && PreviousParent != null)
            {
                isChoosenCard = false;
                ChosenCard.GetComponent<RectTransform>().SetParent(PreviousParent.GetComponent<RectTransform>());
                ChosenCard.SetPosition(Vector3.zero);
                ChosenCardForDrag = null;
                ChosenCard.SetScale(PreviousParent.scale);
                DeChooseThings();
            }
        }

        public void CardChoose(DeckCardBehaviour deck_card)
        {
            var card = deck_card.InDeckBehaviour;
            isChoosenCard = true;
            ChosenCard = card;
            FaderClick();
            PreviousParent = deck_card;
            FirstParent = deck_card;
            card.GetComponent<RectTransform>().SetParent(DeckChosenCard.GetComponent<RectTransform>());
            card.GetComponent<CardViewBehaviour>().Glow(true);
            DeckChosenCard.InDeckBehaviour = card;
            DeckChosenCard.InDeckBehaviour.SetPosition(Vector3.zero);
            DeckChosenCard.InDeckBehaviour.SetScale(DeckChosenCard.scale);
            ChosenCard.SetPosition(Vector3.zero);
            DeckChosenCard.cardName.text = PreviousParent.cardName.text;

            ChooseThings();
        }

        private DeckCardBehaviour InitCard(ushort id, GameObject prefab, RectTransform parent)
        {
            DeckCardBehaviour deckCardObject = null;
            if (Cards.Instance.Get(id, out BinaryCard _card))
            {
                var CardObject = Instantiate(prefab, parent);
                deckCardObject = CardObject.GetComponent<DeckCardBehaviour>();
                deckCardObject.Init(_card);
            }
            return deckCardObject;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (UpScroll) UpScroll = false;
            scrollRect.normalizedPosition = new Vector2(0, 0);
        }
    }
}