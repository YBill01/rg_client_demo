using Legacy.Database;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Legacy.Client.ArenaRatingEventBehaviour;
using static Legacy.Client.LootBoxWindowBehaviour;

namespace Legacy.Client
{
    public class ArenaContentBehaviour : MonoBehaviour
    {
        [HideInInspector]
        public BinaryBattlefields BinaryData;
        [HideInInspector]
        public bool HasRating;
        [HideInInspector]
        public ushort Rating;
        [HideInInspector]
        public ushort MaxRating;
        [HideInInspector]
        public ushort newHeroesCount = 0;

        [SerializeField]
        private GameObject separatorObject;
        [SerializeField]
        private ArenaNewCardsBehaviour newCards;
        [SerializeField]
        public ArenaNewHeroesViewBehaviour newHeroes;
        [SerializeField]
        private HorizontalLayoutGroup sliderLayout;
        [SerializeField]
        private VerticalLayoutGroup newCardsLayout;

        [SerializeField]
        private ArenaRewardBehaviour startEndRatingRewardPrefab;
        [SerializeField]
        private ArenaRewardBehaviour rewardPrefab;
        [SerializeField]
        private GameObject ratingEventPrefab;

        [SerializeField]
        private RectTransform rewardContainer;
        [SerializeField]
        private RectTransform ratingEventsContainer;

        [SerializeField]
        private ArenaTitleContentBehaviour titleContent;
        [SerializeField]
        private ArenaEventsPositionsBehaviour eventsPositions;
        [SerializeField]
        private ArenaSliderBehaviour slider;

        [SerializeField]
        private RectTransform startSliderRect;
        [SerializeField]
        private RectTransform newCardsRect;
        [SerializeField]
        private RectTransform mainPanelRect;

        private ArenaRatingEventBehaviour myRatingEvent = null;


        private float updateTimer;
        private List<ArenaRewardBehaviour> rewards;
        private ArenaWindowBehaviour arenaWindow;
        private float currentScrollPosition = 0.0f;
        private float timer = 0.0f;
        private float updateTime = 0.35f;
        private float heroUpdateTime = 1.35f;
        private bool builded = false;
        private bool ratingSetted = false;
        private bool scrolled = false;
        private ProfileInstance profile;
        private byte number = 0;
        private ushort index = 0;
        private ushort StartRating = 0;

        private float panelWidth;
        private float panelStartPosition;
        private float panelEndPosition;
        private float parentPanelWidth;
        private float currentPosition;

        private GameObject MyRating;

        public bool IsLocked { get; set; }
        public bool IsTutorialArena { get; set; }

        /// <summary>
        /// Создает контент арены.
        /// </summary>
        /// <param name="id">ID из базы данных арен. Админка.</param>
        /// <param name="index">Номер по порядку в очереди арен</param>
        public void InitData(ArenaWindowBehaviour window, byte number, ushort startRating, BinaryBattlefields arena, bool first = false, bool last = false)
        {
            ushort tutorialArena = Settings.Instance.Get<ArenaSettings>().tutorial;
            if (tutorialArena > 0)
            {
                IsTutorialArena = tutorialArena == arena.index;
            }

            profile = ClientWorld.Instance.Profile;
            arenaWindow = window;
            rewards = new List<ArenaRewardBehaviour>();
            separatorObject.SetActive(!first);
            index = arena.index;
            this.number = number;
            StartRating = startRating;
            BinaryData = arena;

            InitTitleContent();
            InitNewHeroes();
            InitNewCards();
            InitRewards();

            // Блокировка арен...
            if (IsLocked)
            {
                titleContent.Lock(true);
            }
        }

        private void InitTitleContent()
        {
            if (IsTutorialArena)
            {
                titleContent.SetTutorialType(BinaryData);
            }
            else
            {
                titleContent.Init(BinaryData, number, StartRating);
            }

            if (StartRating <= profile.Rating.max)
            {
                titleContent.TurnOffRating();
            }
        }

        private void InitNewHeroes()
        {

            if (BinaryData.heroes.Count > 0)
            {
                newHeroesCount = CountNewHeroes();
                newHeroes.Init(BinaryData.heroes);
                newHeroes.SetHeroesState(StartRating > profile.Rating.max);
            }
        }

        private ushort CountNewHeroes()
        {
            ushort counter = 0;
            for (int i = 0; i < BinaryData.heroes.Count; i++)
            {
                if (!ClientWorld.Instance.Profile.ViewedHeroes.Contains(BinaryData.heroes[i]))
                    counter++;
            }

            return counter;
        }
        private void InitNewCards()
        {
            if (BinaryData.cards.Count > 0)
            {
                newCards.Init(BinaryData.cards);
                newCards.SetNewCardsState(StartRating > profile.Rating.max);
            }
        }

        private void InitRewards()
        {
            if (BinaryData.rewards.Count > 0)
            {
                AddRewards();
            }
        }

        private void AddRewards()
        {
            CreateStartReward();
            CreateRewards();
            CreateEndReward();
        }

        private void CreateStartReward()
        {
            var _start = InstantiateReward(startEndRatingRewardPrefab, 0);
            _start.SetStartEndEventRating(StartRating);
        }

        private ArenaRewardBehaviour InstantiateReward(ArenaRewardBehaviour prefab, ushort rating)
        {
            var reward = Instantiate(prefab, rewardContainer);
            eventsPositions.AddReward(rating, reward.GetRect());
            return reward;
        }
        private UpPanelBehaviour upPanel;
        private void CreateRewards()
        {
           // ushort min = 0;
          //  byte index = 0;
          //  bool isNewRewward = false;
            for (byte i = 0; i < BinaryData.rewards.Count; i++)
            {
                var _reward = InstantiateReward(rewardPrefab, BinaryData.rewards[i].rating);
                _reward.Init(
                    BinaryData.rewards[i].reward,
                    (ushort)(StartRating + BinaryData.rewards[i].rating),
                    BinaryData.index,
                    i,
                    this,
                    profile
                );
               
             //   if((ushort)(StartRating + BinaryData.rewards[i].rating) > profile.Rating.max)
             //   {
                   // if (min == 0 || (ushort)(StartRating + BinaryData.rewards[i].rating) < min)
                  //  {
                 //       min = (ushort)(StartRating + BinaryData.rewards[i].rating);
                  //      index = i;
                  //      isNewRewward = true;
                 //   }
              //  }
                rewards.Add(_reward);
            }
       /*     if (!profile.IsBattleTutorial)
            {
                if (upPanel == null)
                    upPanel = WindowManager.Instance.GetUpPanel();
                if (!isNewRewward && ((upPanel.minNewRewardRating > 0 && upPanel.minNewRewardRating < BinaryData.rating) || upPanel.minNewRewardRating == 0))
                {
                    if (profile.Rating.max < (ushort)(StartRating + BinaryData.rating))
                    {
                        upPanel.minNewRewardRating = (ushort)(StartRating + BinaryData.rating);

                        upPanel.NewReward(BinaryData.index, true);
                    }
                }
                if (isNewRewward && upPanel && (upPanel.minNewRewardRating == 0 || min < upPanel.minNewRewardRating))
                {
                    upPanel.minNewRewardRating = min;
                    upPanel.NewReward(BinaryData.rewards[index].reward);

                }
            }*/
        }

        private void CreateEndReward()
        {
            var _end = InstantiateReward(startEndRatingRewardPrefab, BinaryData.rating);
            _end.SetStartEndEventRating((ushort)(StartRating + BinaryData.rating));
        }

        public void SetClickedReward(ArenaRewardBehaviour reward)
        {
            arenaWindow.SetClickedReward(reward);
        }

        public void TakeArenaReward(ushort rewardNumber)
        {
            var _home = ClientWorld.Instance.GetOrCreateSystem<HomeSystems>();
            _home.UserProfile.TakeArenaReward(index, (byte)rewardNumber);

            var dbId = BinaryData.rewards[rewardNumber].reward;

            AnalyticsManager.Instance.ArenaReward(dbId, rewardNumber, number);
        }

        public void ClickReward(ushort rewardNumber, Vector3 pos)
        {
            var reward = rewards.Find(x => x.GetNumberInArena().Equals(rewardNumber));

            if (reward != null)
            {
                reward.SetComplete();
            }

            if (reward.Type.Equals(ArenaRewardBehaviour.RewardType.LootBox))
            {
                if (reward.GetRewardObject() is ArenaRewardLootBehaviour lootReward)
                {
                    arenaWindow.OpenLootBox(lootReward);
                }
            }
            else
            {
                if (reward.Type.Equals(ArenaRewardBehaviour.RewardType.Cards))
                {
                    ArenaRewardCardsBehaviour rewardCards = (ArenaRewardCardsBehaviour)reward.GetRewardObject();
                    BinaryCardsReward binaryCardsReward = rewardCards.GetBinaryCardsReward();

                    CardRarity card_rarity = CardRarity.Common;
                    ushort card_count = 0;
                    if (binaryCardsReward.count <= 0)
                    {
                        card_rarity = binaryCardsReward.rarity_card.rarity;
                        card_count = binaryCardsReward.rarity_card.rarity_count;
                    }
                    else if (Cards.Instance.Get(binaryCardsReward.card, out BinaryCard binaryCard))
                    {
                        card_rarity = binaryCard.rarity;
                        card_count = binaryCardsReward.count;
                    }

                    CollectRewardParticles(reward.Type, pos, (byte)card_count, card_rarity);
                }
                else
                {
                    CollectRewardParticles(reward.Type, pos);
                }
            }
        }

        private void CollectRewardParticles(ArenaRewardBehaviour.RewardType type, Vector3 pos, byte count = 10, CardRarity rarity = CardRarity.Common)
        {
            switch (type)
            {
                case ArenaRewardBehaviour.RewardType.Hard:
                    RewardParticlesBehaviour.Instance.Drop(pos, 10, LootCardType.Hard);
                    break;
                case ArenaRewardBehaviour.RewardType.Soft:
                    RewardParticlesBehaviour.Instance.Drop(pos, 10, LootCardType.Soft);
                    break;
                case ArenaRewardBehaviour.RewardType.Shards:
                    RewardParticlesBehaviour.Instance.Drop(pos, 10, LootCardType.Shards);
                    break;
                case ArenaRewardBehaviour.RewardType.Cards:
                    RewardParticlesBehaviour.Instance.Drop(pos, count, LootCardType.Cards, rarity);
                    break;
                default:
                    break;
            }
        }

        public float GetMyRatingPosition()
        {
            ushort rating = (ushort)(myRatingEvent.rating - StartRating);
            float ratingXPos;
            ratingXPos = eventsPositions.GetPosition(IsTutorialArena ? (ushort)0 : rating);
            return panelStartPosition - (startSliderRect.rect.width + newCardsRect.rect.width + sliderLayout.spacing * 2
                + newCardsLayout.padding.left + sliderLayout.padding.left + ratingXPos);
        }

        public void SetRating(EventArenaData rating)
        {
            if (IsTutorialArena || rating.number == number)
            {
                Rating = rating.rating;
                builded = false;
                HasRating = true;
                AddMyRating();
            }
            else
            {
                if (rating.number < number)
                {
                    slider.SetMinimum();
                }
                else
                {
                    slider.SetMaximum();
                }
            }
        }

        public void SetMaxRating(EventArenaData maxRating)
        {
            if (maxRating.number == number)
            {
                MaxRating = maxRating.rating;
            }
            else
            {
                if (maxRating.number < number)
                {
                    slider.SetMinimumMaxRating();
                }
                else
                {
                    slider.SetMaximumMaxRating();
                }
            }
        }

        private void AddMyRating()
        {
            MyRating = Instantiate(ratingEventPrefab, ratingEventsContainer);

            myRatingEvent = MyRating.GetComponent<ArenaRatingEventBehaviour>();
            ushort rating = (ushort)(StartRating + Rating);
            myRatingEvent.AddPlayer(new RatingEventData()
            {
                rating = rating,
                name = "You"
            }
            );

			if (IsTutorialArena || rating == 0)
			{
                myRatingEvent.Show(false);
            }
		}

        public void ShowMyRating(bool value)
        {
            myRatingEvent.Show(value);
        }

        public void ShowNewHeroesEffect()
        {
            newHeroes.ShowHeroesEffect();
        }

        public void SetSliderMovingState(bool isMovingOn)
        {
            slider.SetIsMovingOn(isMovingOn);
        }

        public void MoveSliderToEnd()
        {
            slider.MoveToEnd();
        }

        public void UpdateWidth()
        {
            panelWidth = mainPanelRect.rect.width;
        }

        public float GetWidth()
        {
            return panelWidth;
        }

        public void SetPosition(float position)
        {
            panelStartPosition = position;
            panelEndPosition = position - panelWidth;
            var rewardPanelPosition = panelStartPosition -
                                      (startSliderRect.rect.width + newCardsRect.rect.width + sliderLayout.spacing * 2
                                       - newCardsLayout.padding.left);

            for (int i = 0; i < rewards.Count; i++)
            {
                rewards[i].SetPosition(rewardPanelPosition);
            }
        }

        public float GetStartPosition()
        {
            return panelStartPosition;
        }

        public float GetEndPosition()
        {
            return panelEndPosition;
        }

        public void SetMainPanelWidth(float width)
        {
            parentPanelWidth = width;
        }

        public void SetPanelPositionForArenaInView(float position)
        {
            SetTitleToPanelPosition(position);
            StartAnimationInVisibleRewards(position);
        }

        private float beforePrev = 0;
        private float prev = 0;

        private void SetTitleToPanelPosition(float position)
        {
            if (position > panelStartPosition)
                position = panelStartPosition;

            var visibleEndPosition = position - parentPanelWidth;

            if (visibleEndPosition < panelEndPosition)
            {
                visibleEndPosition = panelEndPosition;
                position = panelEndPosition + parentPanelWidth;
            }

            var visibleMiddle = position + (visibleEndPosition - position) / 2;
            var total = panelEndPosition - panelStartPosition;
            var delta = visibleMiddle - panelStartPosition;
            var percent = delta / total;
            var titlePosition = panelWidth * percent;
            titleContent.SetPosition(titlePosition);
        }

        private void StartAnimationInVisibleRewards(float position)
        {
            var visibleEndPosition = position - parentPanelWidth;
            var border = parentPanelWidth / 10;

            for (int i = 0; i < rewards.Count; i++)
            {
                var rewardItem = rewards[i].GetRewardObject();

                if (rewardItem.HadFirstAppearance()) continue;

                var rewardPos = rewardItem.GetPosition();

                if (rewardPos < position && rewardPos > visibleEndPosition + border)
                {
                    rewardItem.PlayAnimationOnFirstAppearance();
                }
                else if (rewardPos > position + border)
                {
                    rewardItem.PlayAnimationOnFirstAppearance();
                }
            }
        }

        void Update()
        {
            if (builded) return;

            BuildRatingBar();
        }

        private void BuildRatingBar()
        {
            timer += Time.deltaTime;

            if (!(timer > updateTime)) return;

            builded = true;


            SetMaxRatingPosition();

            if (!HasRating) return;

            if (ratingSetted) return;

            SetMyRatingPosition();
        }

        private void SetMaxRatingPosition()
        {
            if (MaxRating <= 0 || IsTutorialArena) return;

            float MaxRatingXPos = eventsPositions.GetPosition(MaxRating);
            slider.SetMaxFill(MaxRatingXPos/* + ratingEventPrefab.GetComponent<RectTransform>().rect.width / 2*/);
        }

        private void SetMyRatingPosition()
        {
            if (MaxRating <= 0 || IsTutorialArena) return;

            float MyRatingXPos = eventsPositions.GetPosition((ushort)(myRatingEvent.rating - StartRating));
            myRatingEvent.SetPosition(MyRatingXPos);
            slider.SetFill(MyRatingXPos);
            ratingSetted = true;
        }
    }

}