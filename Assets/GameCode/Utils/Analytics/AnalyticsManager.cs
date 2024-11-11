using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Legacy.Client
{
    public class AnalyticsManager : MonoBehaviour
    {
        public static AnalyticsManager Instance;

        private ProfileInstance _profileInstance;
        private ProfileInstance Profile
        {
            get
            {
                if(_profileInstance == null)
                {
                    _profileInstance = ClientWorld.Instance.Profile;
                }
                return _profileInstance;
            }
        }

        void Awake()
        {
                Instance = this;
          //      DontDestroyOnLoad(this);
      
        }

        [SerializeField] private List<AnalyticsSender> senders = new List<AnalyticsSender>();

        private string _battleRival = "Bot";
        private int userArenaNumber { get { return !Profile.IsBattleTutorial ? Profile.CurrentArena.number + 1 : Profile.CurrentArena.number; } }
        private int userArenaID { get { return Profile.CurrentArena.index; } }

        public void Init()
        {
            for (int i = 0; i < senders.Count; i++)
            {
                senders[i].Init();
            }

            FirstOpen();
            SessionStart();
            Login();
            CompleteRegistration();
            FirstLoadCoplete();

            //MenuTriggerSystem.MenuTutorialStepEvent.AddListener(OnMenuTuTorialStep);
        }
        private void OnMenuTuTorialStep(BinaryMenuTutorialEvent eventData)
        {
            if (eventData.analytic_event > AnalyticTutorialStep.None)
            {
                Debug.Log($"Send Tutorial Event: {eventData.analytic_event}.");
                SendTutorialStep(eventData.analytic_event);
            }
        }
        private void Send(CustomEventTypes eventType, Dictionary<string, object> eventData = null)
        {
#if !PRODUCTION && !PRE_PRODUCTION
            if (AppInitSettings.Instance.EnableAnalytics)
#endif
            {
                if (CustomEventOnce.HasCustomEventOnce(eventType, out int index))
                {
                    if (Profile.analyticEvents.HasEvent((byte)index))
                    {
                        // Don't send event...
                        return;
                    }
                    else
                    {
                        CustomEventOnce.SaveCustomEventOnce((byte)index);
                    }
                }

                Debug.Log($"Send analytics event: {eventType}.");
                if (eventData == null)
                    eventData = new Dictionary<string, object>();

                if (!eventData.ContainsKey(CustomEventAttr.TIME))
                {
                    eventData.Add(CustomEventAttr.TIME, DateTime.UtcNow.ToString());
                }
                for (int i = 0; i < senders.Count; i++)
                {
                    senders[i].SendEvent(eventType, eventData);
                }
            }
        }
        internal void SendTutorialStep(AnalyticTutorialStep step)
        {
            if (!Profile.analyticEvents.HasTutorialStep((byte)step))
			{               
                Profile.analyticEvents.AddTutorialStep((byte)step);

                var eventData = new Dictionary<string, object>();
                eventData.Add(CustomEventAttr.TUTORIAL_STEP, step);

                Send(CustomEventTypes.TutorialStep, eventData);

                Debug.Log($"OnMenuTuTorialStep event TutorialStep: {step}.");

                var message = new NetworkMessageRaw();
                message.Write((byte)ObserverPlayerMessage.UserCommand);
                message.Write((byte)UserCommandType.Analytics);
                message.Write((byte)AnalyticCommandType.TutorialStep);
                message.Write((byte)step);

                var em = ClientWorld.Instance.EntityManager;
                var messageEntity = em.CreateEntity();
                em.AddComponentData(messageEntity, message);
                em.AddComponentData(messageEntity, default(ObserverMessageTag));
            }
        }
        internal void HomeLoaded()
        {
            if (Profile.IsTutorial)
            {
                switch (Profile.HardTutorialState)
                {
                    case 1:
                        SendTutorialStep(AnalyticTutorialStep.Menu1Loaded);
                        break;
                    case 2:
                        SendTutorialStep(AnalyticTutorialStep.Menu2Loaded);
                        break;
                    case 3:
                        SendTutorialStep(AnalyticTutorialStep.Menu3Loaded);
                        break;
                    case 4:
                        SendTutorialStep(AnalyticTutorialStep.Finish);
                        break;
                    default:
                        break;
                }
            }
        }
        private void FirstOpen()
        {
            Send(CustomEventTypes.FirstOpen);
        }
        public void SessionEnd()
        {
            Send(CustomEventTypes.SessionEnd);
        }
        public void SessionStart()
        {
            Send(CustomEventTypes.SessionStart);
        }
        public void Login()
        {
            Send(CustomEventTypes.Login);
        }
        public void CompleteRegistration()
        {
            Send(CustomEventTypes.CompleteRegistration);
        }
        public void FirstLoadCoplete()
        {
            Send(CustomEventTypes.FirstLoadComplete);
        }
        private void OnDestroy()
        {
            SessionEnd();
        }
        public void LevelUp()
        {
            int level = (int)Profile.Level.level;
            var eparams = new Dictionary<string, object>();
            eparams.Add(CustomEventAttr.LEVEL_UP, level);
            Send(CustomEventTypes.LevelUp, eparams);
        }
        public void RealPayment(PlayerPayment payment)
        {
            SoundsPlayerManager.Instance.PlaySound(SoundName.Buy);
            if (payment.price > 0)
            {
                var eparams = new Dictionary<string, object>();
                eparams.Add(CustomEventAttr.REAL_PAYMENT_TRANSACTION_ID, payment.transactionID);
                eparams.Add(CustomEventAttr.REAL_PAYMENT_IN_APP_PRICE, payment.price);
                eparams.Add(CustomEventAttr.REAL_PAYMENT_IN_APP_NAME, payment.title);
                eparams.Add(CustomEventAttr.REAL_PAYMENT_IN_APP_CURRENCY_ISO_CODE, payment.ISOCurrencyCode);
                Send(CustomEventTypes.RealPayment, eparams);
            }
            else
            {
                GameDebug.LogError($"Null purchase product come to analytics sender.");
            }
        }
        public void CurrencyEarn(int amount, CurrencyType type, int sourceID, CurrencyChangeSourceType sourceType)
        {
            var eventData = new Dictionary<string, object> {
                { CustomEventAttr.CURRENCY_TYPE, type.ToString() },
                { CustomEventAttr.CURRENCY_AMOUNT, amount },
                { CustomEventAttr.CURRENCY_SOURCE_TYPE, sourceType.ToString() },
                { CustomEventAttr.CURRENCY_SOURCE_ID, sourceID },
                { CustomEventAttr.USER_ARENA, userArenaID },   
                { CustomEventAttr.USER_RATING, Profile.Rating.current }
            };
            Send(CustomEventTypes.CurrencyEarn, eventData);
        }
        public void CurrencySpend(int amount, CurrencyType type, int sourceID, CurrencyChangeSourceType sourceType)
        {
            var eparams = new Dictionary<string, object>();
            eparams.Add(CustomEventAttr.CURRENCY_AMOUNT, amount);
            eparams.Add(CustomEventAttr.CURRENCY_TYPE, type.ToString());
            eparams.Add(CustomEventAttr.CURRENCY_SOURCE_ID, sourceID);
            eparams.Add(CustomEventAttr.CURRENCY_SOURCE_TYPE, sourceType.ToString());

            Send(CustomEventTypes.CurrencySpend, eparams);
        }
        internal void RealPaymentTest()
        {
            var eparams = new Dictionary<string, object>();
            eparams.Add(CustomEventAttr.REAL_PAYMENT_TRANSACTION_ID, "product.transactionID");
            eparams.Add(CustomEventAttr.REAL_PAYMENT_IN_APP_PRICE, 7.99m);
            eparams.Add(CustomEventAttr.REAL_PAYMENT_IN_APP_NAME, "TestItem");
            eparams.Add(CustomEventAttr.REAL_PAYMENT_IN_APP_CURRENCY_ISO_CODE, "TST");
            Send(CustomEventTypes.RealPayment, eparams);
        }
        internal void FirstPayment(PlayerPayment payment)
        {
            var eparams = new Dictionary<string, object>();
            eparams.Add(CustomEventAttr.REAL_PAYMENT_TRANSACTION_ID, payment.transactionID);
            eparams.Add(CustomEventAttr.REAL_PAYMENT_IN_APP_PRICE, payment.price);
            eparams.Add(CustomEventAttr.REAL_PAYMENT_IN_APP_NAME, payment.title);
            eparams.Add(CustomEventAttr.REAL_PAYMENT_IN_APP_CURRENCY_ISO_CODE, payment.ISOCurrencyCode);
            Send(CustomEventTypes.FirstPayment, eparams);
        }
        internal void NameChosen(string name)
        {
            var eventData = new Dictionary<string, object> { { CustomEventAttr.USER_NAME, name } };
            Send(CustomEventTypes.NameChosen, eventData);
        }
        internal void ArenaOpen(int number)
        {
            number = !Profile.IsBattleTutorial ? number + 1 : number;
            var settings = Settings.Instance.Get<ArenaSettings>();
            settings.Index((byte)number, out var info);
            var arenaId = info.index;
            var eventData = new Dictionary<string, object> { 
                { CustomEventAttr.ARENA_NUMBER, number },
                { CustomEventAttr.ARENA_ID, arenaId },
                { CustomEventAttr.USER_RATING, Profile.Rating.current } 
            };
            Send(CustomEventTypes.ArenaOpen, eventData);
        }
        internal void ArenaReward(int rewardId, int rewardOrder, int rewardArenaNumber)
        {
            var rewardType = "";
            var rewardCount = 0;
            Rewards.Instance.Get((ushort)rewardId, out BinaryReward reward);
            switch (Rewards.Instance.GetRewadType((ushort)rewardId))
            {
                case Rewards.RewardType.Hard:
                    rewardType = CustomEventEnum.RewardType.HARD;
                    rewardCount = reward.hard;
                    break;
                case Rewards.RewardType.Soft:
                    rewardType = CustomEventEnum.RewardType.SOFT;
                    rewardCount = reward.soft;
                    break;
                case Rewards.RewardType.LootBox:
                    rewardType = CustomEventEnum.RewardType.LOOTBOX;
                    rewardCount = 1;
                    break;
                case Rewards.RewardType.Card:
                    rewardType = CustomEventEnum.RewardType.CARD;
                    rewardCount = reward.cards[0].count;
                    break;
            }

            rewardArenaNumber = !Profile.IsBattleTutorial ? rewardArenaNumber + 1 : rewardArenaNumber;
            var eventData = new Dictionary<string, object> {
                { CustomEventAttr.REWARD_ID, rewardId },
                { CustomEventAttr.ARENA_REWARD_TYPE, rewardType },
                { CustomEventAttr.AMOUNT, rewardCount },
                { CustomEventAttr.USER_RATING, Profile.Rating.current },      
                { CustomEventAttr.ARENA_REWARD_ORDER, rewardOrder },
                { CustomEventAttr.USER_ARENA, userArenaID },
                { CustomEventAttr.ARENA_NUMBER, rewardArenaNumber}
            };
            Send(CustomEventTypes.ArenaReward, eventData);
        }
        internal void BattlePassReward(int seasonID, int rewardID, int rewardOrder, bool isPremium)
        {
            var rewardType = "";
            var rewardCount = 0;
            Rewards.Instance.Get((ushort)rewardID, out BinaryReward reward);
            switch (Rewards.Instance.GetRewadType((ushort)rewardID))
            {
                case Rewards.RewardType.Hard:
                    rewardType = CustomEventEnum.RewardType.HARD;
                    rewardCount = reward.hard;
                    break;
                case Rewards.RewardType.Soft:
                    rewardType = CustomEventEnum.RewardType.SOFT;
                    rewardCount = reward.soft;
                    break;
                case Rewards.RewardType.LootBox:
                    rewardType = CustomEventEnum.RewardType.LOOTBOX;
                    rewardCount = 1;
                    break;
                case Rewards.RewardType.Card:
                    rewardType = CustomEventEnum.RewardType.CARD;
                    rewardCount = reward.cards[0].count;
                    break;
            }

            var bp_type = isPremium ? CustomEventEnum.BattlePassType.PREMIUM : CustomEventEnum.BattlePassType.FREE;
            var eventData = new Dictionary<string, object> {
                { CustomEventAttr.USER_ARENA, userArenaID },
                { CustomEventAttr.SEASON_ID, seasonID },
                { CustomEventAttr.REWARD_ID, rewardID },
                { CustomEventAttr.BP_REWARD_TYPE, rewardType },
                { CustomEventAttr.AMOUNT, rewardCount },
                { CustomEventAttr.BP_REWARD_ORDER, rewardOrder },
                { CustomEventAttr.BP_TYPE, bp_type }
            };

            Send(CustomEventTypes.BattlePassReward, eventData);
        }
        internal void SoftExchange(int soft, int hard)
        {
            var eventData = new Dictionary<string, object> {
                { CurrencyType.Soft.ToString(), soft },
                { CurrencyType.Hard.ToString(), hard },
                { CustomEventAttr.USER_ARENA, userArenaID },
                { CustomEventAttr.USER_RATING, Profile.Rating.current }
            };

            Send(CustomEventTypes.SoftExchange, eventData);
        }
        internal void HeroUpgrade(int heroId, int newLVL, int soft)
        {
            var eventData = new Dictionary<string, object> {
                { CustomEventAttr.HERO_ID, heroId },
                { CustomEventAttr.HERO_NEW_LEVEL, newLVL },
                { CurrencyType.Soft.ToString(), soft },
                { CustomEventAttr.USER_ARENA, userArenaID }, 
                { CustomEventAttr.USER_RATING, Profile.Rating.current }
            };

            InAppPurchase(heroId.ToString(), InAppPurchaseType.UpgradeHero.ToString(), 1, soft, CurrencyType.Soft.ToString());
            Send(CustomEventTypes.HeroUpgrade, eventData);
        } 
        internal void HeroBuy(int heroId, CurrencyType type, int currencyAmount)
        {
            var eventData = new Dictionary<string, object> {
                { CustomEventAttr.HERO_ID, heroId },
                { CustomEventAttr.CURRENCY_TYPE, type.ToString() },
                { CustomEventAttr.CURRENCY_AMOUNT, currencyAmount },
                { CustomEventAttr.USER_ARENA, userArenaID },   
                { CustomEventAttr.USER_RATING, Profile.Rating.current }
            };

            InAppPurchase(heroId.ToString(), InAppPurchaseType.BuyHero.ToString(), 1, currencyAmount, type.ToString());
            Send(CustomEventTypes.HeroBuy, eventData);
        }
        internal void CardUpgrade(BinaryCard binaryCard, ClientCardData clientCardData)
        {
            var newLVL          = clientCardData.level + 1;
            var softPrice       = clientCardData.SoftToUpgrade;
            var cardsToUpgrade  = clientCardData.CardsToUpgrade;

            var eventData = new Dictionary<string, object> {
                { CustomEventAttr.CARD_ID, binaryCard.index },
                { CustomEventAttr.CARD_TYPE, binaryCard.rarity.ToString() },
                { CustomEventAttr.CARD_NEW_LEVEL, newLVL },
                { CurrencyType.Soft.ToString(), softPrice },
                { CustomEventAttr.CARD_AMOUNT, cardsToUpgrade },
                { CustomEventAttr.USER_ARENA, userArenaID },     
                { CustomEventAttr.USER_RATING, Profile.Rating.current }
            };

            InAppPurchase(binaryCard.index.ToString(), InAppPurchaseType.UpgradeCard.ToString(), 1, (int)softPrice, CurrencyType.Soft.ToString());
            Send(CustomEventTypes.CardUpgrade, eventData);
        }
        internal void CardsReceive(PlayerUpdateLootEvent playerUpdateLootEvent) 
        {
             var sourceID = playerUpdateLootEvent.lootSourceID;
             var sorceType = playerUpdateLootEvent.lootSourceType;

            foreach (var card in playerUpdateLootEvent.cards)
            {
                if (Cards.Instance.Get(card.Key, out BinaryCard binaryCard))
                {
                    var rarity  = binaryCard.rarity;
                    var isNew   = !Profile.Inventory.HasCard(binaryCard.index);

                    CardReceive(card.Key, rarity, card.Value, sorceType, sourceID, isNew);
                }
            }
        }
        internal void CardReceive(int cardID, CardRarity rarity, int cardsCount, LootSourceType lootSource, int sourceID, bool isNew )
        {
            var eventData = new Dictionary<string, object> {
                { CustomEventAttr.CARD_ID, cardID },
                { CustomEventAttr.CARD_TYPE, rarity.ToString() },
                { CustomEventAttr.CARD_AMOUNT, cardsCount },
                { CustomEventAttr.CARD_SOURCE_TYPE, lootSource.ToString() },
                { CustomEventAttr.CARD_SOURCE_ID, sourceID },
                { CustomEventAttr.CARD_NEW_CARD, isNew ? CustomEventEnum.NewCard.YES : CustomEventEnum.NewCard.NO },
                { CustomEventAttr.USER_ARENA, userArenaID },        
                { CustomEventAttr.USER_RATING, Profile.Rating.current }
            };

            Send(CustomEventTypes.CardReceive, eventData);
        }
        internal void ChestSpeedUp(int chestID, int chestCost, int timeToOped)
        {
            var eventData = new Dictionary<string, object> {
                { CustomEventAttr.CHEST_ID, chestID },
                { CustomEventAttr.CHEST_COST, chestCost },
                { CustomEventAttr.CHEST_TIME_TO_OPEN, timeToOped },
                { CustomEventAttr.USER_ARENA, userArenaID },
                { CustomEventAttr.USER_RATING, Profile.Rating.current },
            };

            InAppPurchase(chestID.ToString(), InAppPurchaseType.SpeedUpChest.ToString(), 1, chestCost, CurrencyType.Hard.ToString());
            Send(CustomEventTypes.ChestSpeedUp, eventData);
        }
        internal void ChestStart(int chestID)
        {
            var eventData = new Dictionary<string, object> {
                { CustomEventAttr.CHEST_ID, chestID },
                { CustomEventAttr.USER_ARENA, userArenaID },
                { CustomEventAttr.USER_RATING, Profile.Rating.current },
            };

            Send(CustomEventTypes.ChestStart, eventData);
        }
        internal void ChestOpen(int chestID, LootSourceType sourceType)
        {  
            var eventData = new Dictionary<string, object> {
                { CustomEventAttr.CHEST_ID, chestID },
                { CustomEventAttr.CHEST_SOURCE_TYPE, sourceType.ToString() },
                { CustomEventAttr.USER_ARENA, userArenaID },
                { CustomEventAttr.USER_RATING, Profile.Rating.current },
            };

            Send(CustomEventTypes.ChestOpen, eventData);
        }
        internal void SkillUsed(int skillNumber)
        {
            int heroID = Profile.SelectedHero;
            int battleNumber = (int)Profile.battleStatistic.battles;
            int heroLVL = Profile.CurrentHero.level;
            int skillID = 0;

            if(Heroes.Instance.Get((ushort)heroID, out BinaryHero binaryHero))
            {
                if(Components.Instance.Get<MinionSkills>().TryGetValue(binaryHero.minion, out MinionSkills skills))
                {
                    if(skillNumber == 0)
                    {
                        skillID = skills.skill1;
                    }
                    else if (skillNumber == 1)
                    {
                        skillID = skills.skill2;
                    }
                }
            }

            var eventData = new Dictionary<string, object> {
                { CustomEventAttr.SKILL_ID, skillID },
                { CustomEventAttr.HERO_ID, heroID },
                { CustomEventAttr.HERO_LEVEL, heroLVL },
                { CustomEventAttr.BATTLE_NUMBER, battleNumber },
                { CustomEventAttr.USER_ARENA, userArenaID },
                { CustomEventAttr.USER_RATING, Profile.Rating.current },
            };

            Send(CustomEventTypes.SkillUsed, eventData);
        }
        internal void CardUsed(int cardID)
        {
            int battleNumber = (int)Profile.battleStatistic.battles;
            int cardLVL = Profile.Inventory.GetCardData((ushort)cardID).level;

            var eventData = new Dictionary<string, object> {
                { CustomEventAttr.CARD_ID, cardID },
                { CustomEventAttr.CARD_LEVEL, cardLVL },
                { CustomEventAttr.BATTLE_NUMBER, battleNumber },
                { CustomEventAttr.USER_ARENA, userArenaID },
                { CustomEventAttr.USER_RATING, Profile.Rating.current },
            };

            Send(CustomEventTypes.CardUsed, eventData);
        }
        internal void BattleEnd()
        {
            bool    isWin           = BattleDataContainer.Instance.isVictory;
            var     battleResult    = isWin ? CustomEventEnum.BattleResult.WIN : CustomEventEnum.BattleResult.DEFEAT;
            int     starsCount      = BattleDataContainer.Instance.starsWeGot;
            int     ratingDelta     = BattleDataContainer.Instance.RatingDelta;
            int     softDelta       = BattleDataContainer.Instance.Soft;
            int     lootBoxId       = BattleDataContainer.Instance.GetLootIndex();
            int     battleNumber    = (int)Profile.battleStatistic.battles;
            int     heroId          = Profile.SelectedHero;
            int     battleLasting   = BattleDataContainer.Instance.BattleLasting;

            Profile.GetPlayerHero(Profile.SelectedHero, out PlayerProfileHero playerProfileHero);
            int heroLVL = playerProfileHero.level;

            var eventData = new Dictionary<string, object> {
                { CustomEventAttr.BATTLE_RESULT, battleResult },
                { CustomEventAttr.HERO_ID, heroId },
                { CustomEventAttr.HERO_LEVEL, heroLVL },
                { CustomEventAttr.STARS_GET, starsCount },
                { CustomEventAttr.BATTLE_TIME, battleLasting },
                { CustomEventAttr.BATTLE_NUMBER, battleNumber },
                { CustomEventAttr.RATING_RESULT, ratingDelta },
                { CurrencyType.Soft.ToString(), softDelta },
                { CustomEventAttr.REWARD_ID, lootBoxId },
                { CustomEventAttr.USER_RATING, Profile.Rating.current }, 
                { CustomEventAttr.USER_ARENA, userArenaID },
                { CustomEventAttr.RIVAL, _battleRival}
            };

            Send(CustomEventTypes.BattleEnd, eventData);
        }
        internal void BattleStart(ObserverBattlePlayer enemy, int timeQue)
        {
            int battleNumber = (int)Profile.battleStatistic.battles;

            _battleRival = enemy.profile.is_bot ? CustomEventEnum.RivalType.BOT : CustomEventEnum.RivalType.REAL;
            var eventData = new Dictionary<string, object> {
                { CustomEventAttr.RIVAL, _battleRival },
                { CustomEventAttr.TIME_QUE, timeQue },
                { CustomEventAttr.BATTLE_NUMBER, battleNumber },
                { CustomEventAttr.USER_ARENA, userArenaID },
                { CustomEventAttr.USER_RATING, Profile.Rating.current },
                { CustomEventAttr.ENEMY_RATING, enemy.profile.rating.current }
            };


            Send(CustomEventTypes.BattleStart, eventData);
            Send(CustomEventTypes.RatingVsRating, eventData);
        }
        internal void InAppPurchase(string purchaseId, string purchaseType, int purchaseAmount, int purchasePrice, string purchaseCurrency)
        {
            var eventData = new Dictionary<string, object> {
                { CustomEventAttr.PURCHASE_ID,      purchaseId },
                { CustomEventAttr.PURCHASE_TYPE,    purchaseType },
                { CustomEventAttr.PURCHASE_AMOUNT,  purchaseAmount },
                { CustomEventAttr.PURCHASE_PRICE,   purchasePrice },
                { CustomEventAttr.PURCHASE_CURRENCY, purchaseCurrency }
            };

            Send(CustomEventTypes.InAppPurchase, eventData);
        }
        internal void LocalesLoaded(string lang)
        {
            var eventData = new Dictionary<string, object> {{ CustomEventAttr.LANGUAGE, lang }};
            Send(CustomEventTypes.LocalesLoaded, eventData);
        }
        internal void BinaryDomainRead(string domain)
        {
            var eventData = new Dictionary<string, object> {{ CustomEventAttr.BINARY_DOMAIN, domain }};
            Send(CustomEventTypes.BinaryDomainRead, eventData);
        }
        internal void SendTutorBattleResultEvent(bool isWin)
        {
            if (ClientWorld.Instance.Profile.IsTutorial)
            {
                switch (ClientWorld.Instance.Profile.HardTutorialState)
                {
                    case 0:
                        if (isWin)
                        {
                            SendTutorialStep(AnalyticTutorialStep.Battle1Win);
                        }
                        //else
                        //{
                        //    SendTutorialStep(AnalyticTutorialStep.Battle1Lose);
                        //}
                        break;
                    case 1:
                        if (isWin)
                        {
                            SendTutorialStep(AnalyticTutorialStep.Battle2Win);
                        }
                        //else
                        //{
                        //    SendTutorialStep(AnalyticTutorialStep.Battle2Lose);
                        //}
                        break;
                    case 2:
                        if (isWin)
                        {
                            SendTutorialStep(AnalyticTutorialStep.Battle3Win);
                        }
                        //else
                        //{
                        //    SendTutorialStep(AnalyticTutorialStep.Battle3Lose);
                        //}
                        break;
                    case 3:
                        if (isWin)
                        {
                            SendTutorialStep(AnalyticTutorialStep.Battle4Win);
                        }
                        //else
                        //{
                        //    SendTutorialStep(AnalyticTutorialStep.Battle4Lose);
                        //}
                        break;
                }
            }
        }
    }
}
