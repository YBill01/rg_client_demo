using DevToDev;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class DevToDevSender : AnalyticsSender
    {
        public override void Init()
        {
            DevToDev.Analytics.UserId = ClientWorld.Instance.Profile.index.ToString();
            GameDebug.Log($"DevToDev inited! CrossID: {DevToDev.Analytics.UserId}");

            DevToDev.Analytics.CurrentLevel(ClientWorld.Instance.Profile.Level.level);
            DevToDev.Analytics.StartSession();
        }
        public override void SendEvent(CustomEventTypes eventType, Dictionary<string, object> eventData)
        {
            try
            {
                GameDebug.Log("event: DevToDevSender \n");
                switch (eventType)
                {
                    case CustomEventTypes.TutorialStep:
                        OnTutorialStep(eventData);
                        break;
                    case CustomEventTypes.SessionEnd:
                        DevToDev.Analytics.EndSession();
                        break;
                    case CustomEventTypes.LevelUp:
                        OnLevelUp(eventData);
                        break;
                    case CustomEventTypes.RealPayment:
                        OnRealPayment(eventData);
                        break;
                    case CustomEventTypes.CurrencySpend:
                        OnCurrencyChanged(eventData, DevToDev.AccrualType.Purchased);
                        break;


                    // CUSTOM EVENTS
                    case CustomEventTypes.BattleStart:
                        OnBattleStart(eventData);
                        break;
                    case CustomEventTypes.BattleEnd:
                        OnBattleEnd(eventData);
                        break;
                    case CustomEventTypes.CardUsed:
                        OnCardUsed(eventData);
                        break;
                    case CustomEventTypes.SkillUsed:
                        OnSkillUsed(eventData);
                        break;
                    case CustomEventTypes.ChestOpen:
                        OnChestOpen(eventData);
                        break;
                    case CustomEventTypes.ChestStart:
                        OnChestStart(eventData);
                        break;
                    case CustomEventTypes.ChestSpeedUp:
                        OnChestSpeedUp(eventData);
                        break;
                    case CustomEventTypes.CardReceive:
                        OnCardReceive(eventData);
                        break;
                    case CustomEventTypes.CardUpgrade:
                        OnCardUpgrade(eventData);
                        break;
                    case CustomEventTypes.CurrencyEarn:
                        OnCurrencyChanged(eventData, DevToDev.AccrualType.Earned);
                        OnCurrencyEarned(eventData);
                        break;
                    case CustomEventTypes.SoftExchange:
                        OnSoftExchanged(eventData);
                        break;
                    case CustomEventTypes.HeroBuy:
                        OnHeroBuy(eventData);
                        break;
                    case CustomEventTypes.HeroUpgrade:
                        OnHeroUpgrade(eventData);
                        break;
                    case CustomEventTypes.BattlePassReward:
                        OnPattlePassReward(eventData);
                        break;
                    case CustomEventTypes.ArenaReward:
                        OnArenaReward(eventData);
                        break;
                    case CustomEventTypes.ArenaOpen:
                        OnArenaOpen(eventData);
                        break;
                    case CustomEventTypes.NameChosen:
                        OnNameChosen(eventData);
                        break;
                    case CustomEventTypes.InAppPurchase:
                        OnInAppPurchase(eventData);
                        break;
                    case CustomEventTypes.CompleteRegistration:
                        OnCompleteRegistration();
                        break;
                    case CustomEventTypes.FirstLoadComplete:
                        OnFirstLoadComplete();
                        break;
                    case CustomEventTypes.LocalesLoaded:
                        OnLocalesLoaded(eventData);
                        break;
                    case CustomEventTypes.BinaryDomainRead:
                        OnDomainRead(eventData);
                        break;
                    case CustomEventTypes.RatingVsRating:
                        OnRatingVsRating(eventData);
                        break;
                    default:
                        break;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"DevToDev Error [{eventType.ToString()}] - {ex.Message}");
            }
        }

        /// <summary> In-app purchase with a definite ID. </summary>
        /// <param name="purchaseId"> Unique purchase ID  or name (max. 32 symbols)</param>
        /// <param name="purchaseType"> Purchase type or group (max. 96 symbols)</param>
        /// <param name="purchaseAmount"> Number of purchased goods </param>
        /// <param name="purchasePrice"> Cost of purchased goods (total cost - if several goods were purchased)</param>
        /// <param name="purchasePriceCurrency"> Currency name (max. 24 symbols)</param>
        private void OnInAppPurchase(Dictionary<string, object> eventData)
        {
            GameDebug.Log($"event: OnInAppPurchase param PURCHASE_ID,      is {Convert.ToString(GetEventParam(CustomEventAttr.PURCHASE_ID, eventData))}");
            GameDebug.Log($"event: OnInAppPurchase param PURCHASE_TYPE,    is {Convert.ToString(GetEventParam(CustomEventAttr.PURCHASE_TYPE, eventData))}");
            GameDebug.Log($"event: OnInAppPurchase param PURCHASE_AMOUNT,  is {Convert.ToInt32(GetEventParam(CustomEventAttr.PURCHASE_AMOUNT, eventData))}");
            GameDebug.Log($"event: OnInAppPurchase param PURCHASE_PRICE,   is {Convert.ToInt32(GetEventParam(CustomEventAttr.PURCHASE_PRICE, eventData))}");
            GameDebug.Log($"event: OnInAppPurchase param PURCHASE_CURRENCY is {Convert.ToString(GetEventParam(CustomEventAttr.PURCHASE_CURRENCY, eventData))}");

            string purchaseId       = Convert.ToString(GetEventParam(CustomEventAttr.PURCHASE_ID, eventData));
            string purchaseType     = Convert.ToString(GetEventParam(CustomEventAttr.PURCHASE_TYPE, eventData));
            int purchaseAmount   = Convert.ToInt32(GetEventParam(CustomEventAttr.PURCHASE_AMOUNT, eventData));
            int purchasePrice    = Convert.ToInt32(GetEventParam(CustomEventAttr.PURCHASE_PRICE, eventData));
            string purchaseCurrency = Convert.ToString(GetEventParam(CustomEventAttr.PURCHASE_CURRENCY, eventData));

            DevToDev.Analytics.InAppPurchase(purchaseId, purchaseType, purchaseAmount, purchasePrice, purchaseCurrency);
        }
        private void OnCurrencyChanged(Dictionary<string, object> eventData, AccrualType accuralType)
        {
            GameDebug.Log($"event: CurrencyAccrual!!!! param AMOUNT           is {Convert.ToInt32(GetEventParam(CustomEventAttr.CURRENCY_AMOUNT, eventData))}");
            GameDebug.Log($"event: CurrencyAccrual!!!! param CURRENCY_TYPE    is {Convert.ToString(GetEventParam(CustomEventAttr.CURRENCY_TYPE, eventData))}");
            GameDebug.Log($"event: CurrencyAccrual!!!! param accuralType      is {accuralType}");

            DevToDev.Analytics.CurrencyAccrual
            (
                Convert.ToInt32(GetEventParam(CustomEventAttr.CURRENCY_AMOUNT, eventData)),
                Convert.ToString(GetEventParam(CustomEventAttr.CURRENCY_TYPE, eventData)),
                accuralType
            );
        }
        private void OnRealPayment(Dictionary<string, object> eventData)
        {
            string title = Convert.ToString(GetEventParam(CustomEventAttr.REAL_PAYMENT_IN_APP_NAME, eventData));
            string transactionID = Convert.ToString(GetEventParam(CustomEventAttr.REAL_PAYMENT_TRANSACTION_ID, eventData));
            float revenue = float.Parse(Convert.ToString(GetEventParam(CustomEventAttr.REAL_PAYMENT_IN_APP_PRICE, eventData)));
            string iso = Convert.ToString(GetEventParam(CustomEventAttr.REAL_PAYMENT_IN_APP_CURRENCY_ISO_CODE, eventData));
            GameDebug.Log($"DevToDev RealPayment title: {title}");
            GameDebug.Log($"DevToDev RealPayment transactionID: {transactionID}");
            GameDebug.Log($"DevToDev RealPayment revenue: {revenue}");
            GameDebug.Log($"DevToDev RealPayment iso: {iso}");
            DevToDev.Analytics.RealPayment(
                transactionID,
                revenue,
                title,
                iso
            );
        }
        private Dictionary<string, int> GetUserCurrencies()
        {
            PlayerStock stock = ClientWorld.Instance.Profile.Stock;
            var resources = new Dictionary<string, int>();
            //resources.Add(CustomEventAttr.SHARDS, (int)stock.GetCount(CurrencyType.Shards));
            resources.Add(CurrencyType.Soft.ToString(), (int)stock.GetCount(CurrencyType.Soft));
            resources.Add(CurrencyType.Hard.ToString(), (int)stock.GetCount(CurrencyType.Hard));
            return resources;
        }
        private void OnLevelUp(Dictionary<string, object> eventData)
        {
            var level = (int)GetEventParam(CustomEventAttr.LEVEL_UP, eventData);

            GameDebug.Log($"event: CurrentLevel is {ClientWorld.Instance.Profile.Level.level}");
            GameDebug.Log($"event: LevelUp: level is {level}");

            Analytics.CurrentLevel(ClientWorld.Instance.Profile.Level.level);
            Analytics.LevelUp(level, GetUserCurrencies());
        }        
        void OnTutorialStep(Dictionary<string, object> eventData)
        {
            GameDebug.Log($"event: OnTutorialStep {Convert.ToInt32(GetEventParam(CustomEventAttr.TUTORIAL_STEP, eventData))}");

            int tutorial_step = Convert.ToInt32(GetEventParam(CustomEventAttr.TUTORIAL_STEP, eventData));
            int tutStepIndex;

            if (tutorial_step == (int)AnalyticTutorialStep.Start)
            {
                tutStepIndex = DevToDev.TutorialState.Start;
            }
            else if (tutorial_step == (int)AnalyticTutorialStep.Finish)
            {
                tutStepIndex = DevToDev.TutorialState.Finish;
            }
            else
            {
                tutStepIndex = tutorial_step;
            }

            DevToDev.Analytics.Tutorial(tutStepIndex);
        }
        private void OnBattleStart(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: BATTLE_START param USER_ARENA    is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,     eventData))}");
            GameDebug.Log($"event: BATTLE_START param USER_RATING   is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,    eventData))}");
            GameDebug.Log($"event: BATTLE_START param RIVAL         is {Convert.ToString(GetEventParam(CustomEventAttr.RIVAL,         eventData))}");
            GameDebug.Log($"event: BATTLE_START param TIME_QUE      is {Convert.ToInt32(GetEventParam(CustomEventAttr.TIME_QUE,       eventData))}");
            GameDebug.Log($"event: BATTLE_START param BATTLE_NUMBER is {Convert.ToInt32(GetEventParam(CustomEventAttr.BATTLE_NUMBER,  eventData))}");

            customEventParams.AddParam(CustomEventAttr.USER_ARENA,    Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,     eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_RATING,   Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,    eventData)));
            customEventParams.AddParam(CustomEventAttr.RIVAL,         Convert.ToString(GetEventParam(CustomEventAttr.RIVAL,         eventData)));
            customEventParams.AddParam(CustomEventAttr.TIME_QUE,      Convert.ToInt32(GetEventParam(CustomEventAttr.TIME_QUE,       eventData)));
            customEventParams.AddParam(CustomEventAttr.BATTLE_NUMBER, Convert.ToInt32(GetEventParam(CustomEventAttr.BATTLE_NUMBER,  eventData)));

            Analytics.CustomEvent(CustomEventName.BATTLE_START, customEventParams);
        }
        private void OnBattleEnd(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: BATTLE_END param BATTLE_RESULT is {Convert.ToString(GetEventParam(CustomEventAttr.BATTLE_RESULT,   eventData))}");
            GameDebug.Log($"event: BATTLE_END param HERO_ID       is {Convert.ToInt32(GetEventParam(CustomEventAttr.HERO_ID,          eventData))}");
            GameDebug.Log($"event: BATTLE_END param HERO_LEVEL    is {Convert.ToInt32(GetEventParam(CustomEventAttr.HERO_LEVEL,       eventData))}");
            GameDebug.Log($"event: BATTLE_END param STARS_GET     is {Convert.ToInt32(GetEventParam(CustomEventAttr.STARS_GET,        eventData))}");
            GameDebug.Log($"event: BATTLE_END param BATTLE_TIME   is {Convert.ToInt32(GetEventParam(CustomEventAttr.BATTLE_TIME,      eventData))}");
            GameDebug.Log($"event: BATTLE_END param BATTLE_NUMBER is {Convert.ToInt32(GetEventParam(CustomEventAttr.BATTLE_NUMBER,    eventData))}");
            GameDebug.Log($"event: BATTLE_END param RATING_RESULT is {Convert.ToInt32(GetEventParam(CustomEventAttr.RATING_RESULT,    eventData))}");
            GameDebug.Log($"event: BATTLE_END param USER_RATING   is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,      eventData))}");
            GameDebug.Log($"event: BATTLE_END param USER_ARENA    is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,       eventData))}");
            GameDebug.Log($"event: BATTLE_END param REWARD_ID     is {Convert.ToInt32(GetEventParam(CustomEventAttr.REWARD_ID,        eventData))}");
            GameDebug.Log($"event: BATTLE_END param SOFT          is {Convert.ToInt32(GetEventParam(CurrencyType.Soft.ToString(),     eventData))}");
            GameDebug.Log($"event: BATTLE_END param RIVAL         is {Convert.ToString(GetEventParam(CustomEventAttr.RIVAL, eventData))}");

            customEventParams.AddParam(CustomEventAttr.BATTLE_RESULT,   Convert.ToString(GetEventParam(CustomEventAttr.BATTLE_RESULT, eventData)));
            customEventParams.AddParam(CustomEventAttr.HERO_ID,         Convert.ToInt32(GetEventParam(CustomEventAttr.HERO_ID,        eventData)));
            customEventParams.AddParam(CustomEventAttr.HERO_LEVEL,      Convert.ToInt32(GetEventParam(CustomEventAttr.HERO_LEVEL,     eventData)));
            customEventParams.AddParam(CustomEventAttr.STARS_GET,       Convert.ToInt32(GetEventParam(CustomEventAttr.STARS_GET,      eventData)));
            customEventParams.AddParam(CustomEventAttr.BATTLE_TIME,     Convert.ToInt32(GetEventParam(CustomEventAttr.BATTLE_TIME,    eventData)));
            customEventParams.AddParam(CustomEventAttr.BATTLE_NUMBER,   Convert.ToInt32(GetEventParam(CustomEventAttr.BATTLE_NUMBER,  eventData)));
            customEventParams.AddParam(CustomEventAttr.RATING_RESULT,   Convert.ToInt32(GetEventParam(CustomEventAttr.RATING_RESULT,  eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_RATING,     Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,    eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_ARENA,      Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,     eventData)));
            customEventParams.AddParam(CustomEventAttr.REWARD_ID,       Convert.ToInt32(GetEventParam(CustomEventAttr.REWARD_ID,      eventData)));
            customEventParams.AddParam(CurrencyType.Soft.ToString(),    Convert.ToInt32(GetEventParam(CurrencyType.Soft.ToString(),   eventData)));
            customEventParams.AddParam(CustomEventAttr.RIVAL, Convert.ToString(GetEventParam(CustomEventAttr.RIVAL, eventData)));

            Analytics.CustomEvent(CustomEventName.BATTLE_END, customEventParams);
        }
        private void OnCardUsed(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: CARD_USED param CARD_ID       is {Convert.ToInt32(GetEventParam(CustomEventAttr.CARD_ID,          eventData))}");
            GameDebug.Log($"event: CARD_USED param CARD_LEVEL    is {Convert.ToInt32(GetEventParam(CustomEventAttr.CARD_LEVEL,       eventData))}");
            GameDebug.Log($"event: CARD_USED param USER_ARENA    is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,       eventData))}");
            GameDebug.Log($"event: CARD_USED param BATTLE_NUMBER is {Convert.ToInt32(GetEventParam(CustomEventAttr.BATTLE_NUMBER,    eventData))}");
            GameDebug.Log($"event: CARD_USED param USER_RATING   is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,      eventData))}");

            customEventParams.AddParam(CustomEventAttr.CARD_ID,         Convert.ToInt32(GetEventParam(CustomEventAttr.CARD_ID,          eventData)));
            customEventParams.AddParam(CustomEventAttr.CARD_LEVEL,      Convert.ToInt32(GetEventParam(CustomEventAttr.CARD_LEVEL,       eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_ARENA,      Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,       eventData)));
            customEventParams.AddParam(CustomEventAttr.BATTLE_NUMBER,   Convert.ToInt32(GetEventParam(CustomEventAttr.BATTLE_NUMBER,    eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_RATING,     Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,      eventData)));

            Analytics.CustomEvent(CustomEventName.CARD_USED, customEventParams);
        }
        private void OnSkillUsed(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: SKILL_USED param SKILL_ID      is {Convert.ToInt32(GetEventParam(CustomEventAttr.SKILL_ID,      eventData))}");
            GameDebug.Log($"event: SKILL_USED param HERO_ID       is {Convert.ToInt32(GetEventParam(CustomEventAttr.HERO_ID,       eventData))}");
            GameDebug.Log($"event: SKILL_USED param HERO_LEVEL    is {Convert.ToInt32(GetEventParam(CustomEventAttr.HERO_LEVEL,    eventData))}");
            GameDebug.Log($"event: SKILL_USED param USER_ARENA    is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,    eventData))}");
            GameDebug.Log($"event: SKILL_USED param BATTLE_NUMBER is {Convert.ToInt32(GetEventParam(CustomEventAttr.BATTLE_NUMBER, eventData))}");
            GameDebug.Log($"event: SKILL_USED param USER_RATING   is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,   eventData))}");

            customEventParams.AddParam(CustomEventAttr.SKILL_ID,        Convert.ToInt32(GetEventParam(CustomEventAttr.SKILL_ID,         eventData)));
            customEventParams.AddParam(CustomEventAttr.HERO_ID,         Convert.ToInt32(GetEventParam(CustomEventAttr.HERO_ID,          eventData)));
            customEventParams.AddParam(CustomEventAttr.HERO_LEVEL,         Convert.ToInt32(GetEventParam(CustomEventAttr.HERO_LEVEL,    eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_ARENA,      Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,       eventData)));
            customEventParams.AddParam(CustomEventAttr.BATTLE_NUMBER,   Convert.ToInt32(GetEventParam(CustomEventAttr.BATTLE_NUMBER,    eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_RATING,     Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,      eventData)));

            Analytics.CustomEvent(CustomEventName.SKILL_USED, customEventParams);
        }
        private void OnChestOpen(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: CHEST_OPEN param CHEST_ID          is {Convert.ToInt32(GetEventParam(CustomEventAttr.CHEST_ID,           eventData))}");
            GameDebug.Log($"event: CHEST_OPEN param USER_ARENA        is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,         eventData))}");
            GameDebug.Log($"event: CHEST_OPEN param CHEST_SOURCE_TYPE is {Convert.ToString(GetEventParam(CustomEventAttr.CHEST_SOURCE_TYPE, eventData))}");
            GameDebug.Log($"event: CHEST_OPEN param USER_RATING       is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,        eventData))}");

            customEventParams.AddParam(CustomEventAttr.CHEST_ID,            Convert.ToInt32(GetEventParam(CustomEventAttr.CHEST_ID,             eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_ARENA,          Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,           eventData)));
            customEventParams.AddParam(CustomEventAttr.CHEST_SOURCE_TYPE,   Convert.ToString(GetEventParam(CustomEventAttr.CHEST_SOURCE_TYPE,   eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_RATING,         Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,          eventData)));

            Analytics.CustomEvent(CustomEventName.CHEST_OPEN, customEventParams);
        }
        private void OnChestStart(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: CHEST_START param CHEST_ID    is {Convert.ToInt32(GetEventParam(CustomEventAttr.CHEST_ID,       eventData))}");
            GameDebug.Log($"event: CHEST_START param USER_ARENA  is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,     eventData))}");
            GameDebug.Log($"event: CHEST_START param USER_RATING is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,    eventData))}");

            customEventParams.AddParam(CustomEventAttr.CHEST_ID,    Convert.ToInt32(GetEventParam(CustomEventAttr.CHEST_ID,     eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_ARENA,  Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,   eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_RATING, Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,  eventData)));

            Analytics.CustomEvent(CustomEventName.CHEST_START, customEventParams);
        }
        private void OnChestSpeedUp(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: CHEST_SPEEDUP param CHEST_ID           is {Convert.ToInt32(GetEventParam(CustomEventAttr.CHEST_ID,             eventData))}");
            GameDebug.Log($"event: CHEST_SPEEDUP param CHEST_COST         is {Convert.ToInt32(GetEventParam(CustomEventAttr.CHEST_COST,           eventData))}");
            GameDebug.Log($"event: CHEST_SPEEDUP param USER_ARENA         is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,           eventData))}");
            GameDebug.Log($"event: CHEST_SPEEDUP param CHEST_TIME_TO_OPEN is {Convert.ToInt32(GetEventParam(CustomEventAttr.CHEST_TIME_TO_OPEN,   eventData))}");
            GameDebug.Log($"event: CHEST_SPEEDUP param USER_RATING        is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,          eventData))}");

            customEventParams.AddParam(CustomEventAttr.CHEST_ID,            Convert.ToInt32(GetEventParam(CustomEventAttr.CHEST_ID,             eventData)));
            customEventParams.AddParam(CustomEventAttr.CHEST_COST,          Convert.ToInt32(GetEventParam(CustomEventAttr.CHEST_COST,           eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_ARENA,          Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,           eventData)));
            customEventParams.AddParam(CustomEventAttr.CHEST_TIME_TO_OPEN,  Convert.ToInt32(GetEventParam(CustomEventAttr.CHEST_TIME_TO_OPEN,   eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_RATING,         Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,          eventData)));

            Analytics.CustomEvent(CustomEventName.CHEST_SPEEDUP, customEventParams);
        }
        private void OnCardReceive(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: CARD_RECEIVE param CARD_ID          is {Convert.ToInt32(GetEventParam(CustomEventAttr.CARD_ID,                eventData))}");
            GameDebug.Log($"event: CARD_RECEIVE param CARD_TYPE        is {Convert.ToString(GetEventParam(CustomEventAttr.CARD_TYPE,             eventData))}");
            GameDebug.Log($"event: CARD_RECEIVE param CARD_AMOUNT      is {Convert.ToInt32(GetEventParam(CustomEventAttr.CARD_AMOUNT,            eventData))}");
            GameDebug.Log($"event: CARD_RECEIVE param CARD_SOURCE_TYPE is {Convert.ToString(GetEventParam(CustomEventAttr.CARD_SOURCE_TYPE,      eventData))}");
            GameDebug.Log($"event: CARD_RECEIVE param CARD_SOURCE_ID   is {Convert.ToInt32(GetEventParam(CustomEventAttr.CARD_SOURCE_ID,         eventData))}");
            GameDebug.Log($"event: CARD_RECEIVE param USER_ARENA       is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,             eventData))}");
            GameDebug.Log($"event: CARD_RECEIVE param CARD_NEW_CARD    is {Convert.ToString(GetEventParam(CustomEventAttr.CARD_NEW_CARD,          eventData))}");
            GameDebug.Log($"event: CARD_RECEIVE param USER_RATING      is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,            eventData))}");

            customEventParams.AddParam(CustomEventAttr.CARD_ID,             Convert.ToInt32(GetEventParam(CustomEventAttr.CARD_ID,              eventData)));
            customEventParams.AddParam(CustomEventAttr.CARD_TYPE,           Convert.ToString(GetEventParam(CustomEventAttr.CARD_TYPE,           eventData)));
            customEventParams.AddParam(CustomEventAttr.CARD_AMOUNT,         Convert.ToInt32(GetEventParam(CustomEventAttr.CARD_AMOUNT,          eventData)));
            customEventParams.AddParam(CustomEventAttr.CARD_SOURCE_TYPE,    Convert.ToString(GetEventParam(CustomEventAttr.CARD_SOURCE_TYPE,    eventData)));
            customEventParams.AddParam(CustomEventAttr.CARD_SOURCE_ID,      Convert.ToInt32(GetEventParam(CustomEventAttr.CARD_SOURCE_ID,       eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_ARENA,          Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,           eventData)));
            customEventParams.AddParam(CustomEventAttr.CARD_NEW_CARD,       Convert.ToString(GetEventParam(CustomEventAttr.CARD_NEW_CARD,        eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_RATING,         Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,          eventData)));

            Analytics.CustomEvent(CustomEventName.CARD_RECEIVE, customEventParams);
        }
        private void OnCardUpgrade(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();


            

            GameDebug.Log($"event: CARD_UPGRADE param CARD_ID        is {Convert.ToInt32(GetEventParam(CustomEventAttr.CARD_ID,          eventData))}");
            GameDebug.Log($"event: CARD_UPGRADE param CARD_TYPE      is {Convert.ToString(GetEventParam(CustomEventAttr.CARD_TYPE,       eventData))}");
            GameDebug.Log($"event: CARD_UPGRADE param CARD_NEW_LEVEL is {Convert.ToInt32(GetEventParam(CustomEventAttr.CARD_NEW_LEVEL,   eventData))}");
            GameDebug.Log($"event: CARD_UPGRADE param SOFT           is {Convert.ToInt32(GetEventParam(CurrencyType.Soft.ToString(),     eventData))}");
            GameDebug.Log($"event: CARD_UPGRADE param CARD_AMOUNT    is {Convert.ToInt32(GetEventParam(CustomEventAttr.CARD_AMOUNT,      eventData))}");
            GameDebug.Log($"event: CARD_UPGRADE param USER_ARENA     is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,       eventData))}");
            GameDebug.Log($"event: CARD_UPGRADE param USER_RATING    is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,      eventData))}");

            customEventParams.AddParam(CustomEventAttr.CARD_ID,         Convert.ToInt32(GetEventParam(CustomEventAttr.CARD_ID,          eventData)));
            customEventParams.AddParam(CustomEventAttr.CARD_TYPE,       Convert.ToString(GetEventParam(CustomEventAttr.CARD_TYPE,       eventData)));
            customEventParams.AddParam(CustomEventAttr.CARD_NEW_LEVEL,  Convert.ToInt32(GetEventParam(CustomEventAttr.CARD_NEW_LEVEL,   eventData)));
            customEventParams.AddParam(CurrencyType.Soft.ToString(),    Convert.ToInt32(GetEventParam(CurrencyType.Soft.ToString(),     eventData)));
            customEventParams.AddParam(CustomEventAttr.CARD_AMOUNT,     Convert.ToInt32(GetEventParam(CustomEventAttr.CARD_AMOUNT,      eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_ARENA,      Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,       eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_RATING,     Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,      eventData)));

            Analytics.CustomEvent(CustomEventName.CARD_UPGRADE, customEventParams);
        }
        private void OnCurrencyEarned(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: EARNED_CURRENCY param CURRENCY_TYPE        is {Convert.ToString(GetEventParam(CustomEventAttr.CURRENCY_TYPE,           eventData))}");
            GameDebug.Log($"event: EARNED_CURRENCY param CURRENCY_AMOUNT      is {Convert.ToInt32(GetEventParam(CustomEventAttr.CURRENCY_AMOUNT,          eventData))}");
            GameDebug.Log($"event: EARNED_CURRENCY param CURRENCY_SOURCE_TYPE is {Convert.ToString(GetEventParam(CustomEventAttr.CURRENCY_SOURCE_TYPE,    eventData))}");
            GameDebug.Log($"event: EARNED_CURRENCY param CURRENCY_SOURCE_ID   is {Convert.ToString(GetEventParam(CustomEventAttr.CURRENCY_SOURCE_ID,      eventData))}");
            GameDebug.Log($"event: EARNED_CURRENCY param USER_ARENA           is {Convert.ToString(GetEventParam(CustomEventAttr.USER_ARENA,              eventData))}");
            GameDebug.Log($"event: EARNED_CURRENCY param USER_RATING          is {Convert.ToString(GetEventParam(CustomEventAttr.USER_RATING,             eventData))}");

            customEventParams.AddParam(CustomEventAttr.CURRENCY_TYPE,           Convert.ToString(GetEventParam(CustomEventAttr.CURRENCY_TYPE,           eventData)));
            customEventParams.AddParam(CustomEventAttr.CURRENCY_AMOUNT,         Convert.ToInt32(GetEventParam(CustomEventAttr.CURRENCY_AMOUNT,          eventData)));
            customEventParams.AddParam(CustomEventAttr.CURRENCY_SOURCE_TYPE,    Convert.ToString(GetEventParam(CustomEventAttr.CURRENCY_SOURCE_TYPE,    eventData)));
            customEventParams.AddParam(CustomEventAttr.CURRENCY_SOURCE_ID,      Convert.ToString(GetEventParam(CustomEventAttr.CURRENCY_SOURCE_ID,      eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_ARENA,              Convert.ToString(GetEventParam(CustomEventAttr.USER_ARENA,              eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_RATING,             Convert.ToString(GetEventParam(CustomEventAttr.USER_RATING,             eventData)));

            Analytics.CustomEvent(CustomEventName.EARNED_CURRENCY, customEventParams);
        }
        private void OnSoftExchanged(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: SOFT_EXCHANGE param SOFT        is {Convert.ToInt32(GetEventParam(CurrencyType.Soft.ToString(),   eventData))}");
            GameDebug.Log($"event: SOFT_EXCHANGE param HARD        is {Convert.ToInt32(GetEventParam(CurrencyType.Hard.ToString(),   eventData))}");
            GameDebug.Log($"event: SOFT_EXCHANGE param USER_ARENA  is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,     eventData))}");
            GameDebug.Log($"event: SOFT_EXCHANGE param USER_RATING is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,    eventData))}");

            customEventParams.AddParam(CurrencyType.Soft.ToString(),            Convert.ToInt32(GetEventParam(CurrencyType.Soft.ToString(), eventData)));
            customEventParams.AddParam(CurrencyType.Hard.ToString(),            Convert.ToInt32(GetEventParam(CurrencyType.Hard.ToString(), eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_ARENA,      Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,   eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_RATING,     Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,  eventData)));

            Analytics.CustomEvent(CustomEventName.SOFT_EXCHANGE, customEventParams);
        }
        private void OnHeroBuy(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: HERO_BUY param HERO_ID         is {Convert.ToInt32(GetEventParam(CustomEventAttr.HERO_ID,            eventData))}");
            GameDebug.Log($"event: HERO_BUY param USER_ARENA      is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,         eventData))}");
            GameDebug.Log($"event: HERO_BUY param CURRENCY_TYPE   is {Convert.ToString(GetEventParam(CustomEventAttr.CURRENCY_TYPE,     eventData))}");
            GameDebug.Log($"event: HERO_BUY param CURRENCY_AMOUNT is {Convert.ToString(GetEventParam(CustomEventAttr.CURRENCY_AMOUNT,   eventData))}");
            GameDebug.Log($"event: HERO_BUY param USER_RATING     is {Convert.ToString(GetEventParam(CustomEventAttr.USER_RATING,       eventData))}");

            customEventParams.AddParam(CustomEventAttr.HERO_ID,         Convert.ToInt32(GetEventParam(CustomEventAttr.HERO_ID,          eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_ARENA,      Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,       eventData)));
            customEventParams.AddParam(CustomEventAttr.CURRENCY_TYPE,   Convert.ToString(GetEventParam(CustomEventAttr.CURRENCY_TYPE,   eventData)));
            customEventParams.AddParam(CustomEventAttr.CURRENCY_AMOUNT, Convert.ToString(GetEventParam(CustomEventAttr.CURRENCY_AMOUNT, eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_RATING,     Convert.ToString(GetEventParam(CustomEventAttr.USER_RATING,     eventData)));


            Analytics.CustomEvent(CustomEventName.HERO_BUY, customEventParams);
        }
        private void OnHeroUpgrade(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: HERO_UPGRADE param HERO_ID        is {Convert.ToInt32(GetEventParam(CustomEventAttr.HERO_ID,         eventData))}");
            GameDebug.Log($"event: HERO_UPGRADE param USER_ARENA     is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,      eventData))}");
            GameDebug.Log($"event: HERO_UPGRADE param HERO_NEW_LEVEL is {Convert.ToInt32(GetEventParam(CustomEventAttr.HERO_NEW_LEVEL,  eventData))}");
            GameDebug.Log($"event: HERO_UPGRADE param SOFT           is {Convert.ToInt32(GetEventParam(CurrencyType.Soft.ToString(),    eventData))}");
            GameDebug.Log($"event: HERO_UPGRADE param USER_RATING    is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,     eventData))}");

            customEventParams.AddParam(CustomEventAttr.HERO_ID,         Convert.ToInt32(GetEventParam(CustomEventAttr.HERO_ID,          eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_ARENA,      Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,       eventData)));
            customEventParams.AddParam(CustomEventAttr.HERO_NEW_LEVEL,  Convert.ToInt32(GetEventParam(CustomEventAttr.HERO_NEW_LEVEL,   eventData)));
            customEventParams.AddParam(CurrencyType.Soft.ToString(),    Convert.ToInt32(GetEventParam(CurrencyType.Soft.ToString(),     eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_RATING,     Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,      eventData)));

            Analytics.CustomEvent(CustomEventName.HERO_UPGRADE, customEventParams);
        }
        private void OnPattlePassReward(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: BATTLE_PASS_REWARD param USER_ARENA      is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,       eventData))}");
            GameDebug.Log($"event: BATTLE_PASS_REWARD param SEASON_ID       is {Convert.ToInt32(GetEventParam(CustomEventAttr.SEASON_ID,        eventData))}");
            GameDebug.Log($"event: BATTLE_PASS_REWARD param REWARD_ID       is {Convert.ToInt32(GetEventParam(CustomEventAttr.REWARD_ID,        eventData))}");
            GameDebug.Log($"event: BATTLE_PASS_REWARD param BP_REWARD_TYPE  is {Convert.ToString(GetEventParam(CustomEventAttr.BP_REWARD_TYPE,  eventData))}");
            GameDebug.Log($"event: BATTLE_PASS_REWARD param AMOUNT          is {Convert.ToInt32(GetEventParam(CustomEventAttr.AMOUNT,           eventData))}");
            GameDebug.Log($"event: BATTLE_PASS_REWARD param BP_REWARD_ORDER is {Convert.ToInt32(GetEventParam(CustomEventAttr.BP_REWARD_ORDER,  eventData))}");
            GameDebug.Log($"event: BATTLE_PASS_REWARD param BP_TYPE         is {Convert.ToString(GetEventParam(CustomEventAttr.BP_TYPE,         eventData))}");

            customEventParams.AddParam(CustomEventAttr.USER_ARENA,      Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,       eventData)));
            customEventParams.AddParam(CustomEventAttr.SEASON_ID,       Convert.ToInt32(GetEventParam(CustomEventAttr.SEASON_ID,        eventData)));
            customEventParams.AddParam(CustomEventAttr.REWARD_ID,       Convert.ToInt32(GetEventParam(CustomEventAttr.REWARD_ID,        eventData)));//name of the reward
            customEventParams.AddParam(CustomEventAttr.BP_REWARD_TYPE,  Convert.ToString(GetEventParam(CustomEventAttr.BP_REWARD_TYPE,  eventData)));
            customEventParams.AddParam(CustomEventAttr.AMOUNT,          Convert.ToInt32(GetEventParam(CustomEventAttr.AMOUNT,           eventData)));
            customEventParams.AddParam(CustomEventAttr.BP_REWARD_ORDER, Convert.ToInt32(GetEventParam(CustomEventAttr.BP_REWARD_ORDER,  eventData)));
            customEventParams.AddParam(CustomEventAttr.BP_TYPE,         Convert.ToString(GetEventParam(CustomEventAttr.BP_TYPE,         eventData)));

            Analytics.CustomEvent(CustomEventName.BATTLE_PASS_REWARD, customEventParams);
        }
        private void OnArenaReward(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: ARENA_REWARD param USER_ARENA         is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,          eventData))}");
            GameDebug.Log($"event: ARENA_REWARD param REWARD_ID          is {Convert.ToInt32(GetEventParam(CustomEventAttr.REWARD_ID,           eventData))}");
            GameDebug.Log($"event: ARENA_REWARD param ARENA_REWARD_TYPE  is {Convert.ToString(GetEventParam(CustomEventAttr.ARENA_REWARD_TYPE,  eventData))}");
            GameDebug.Log($"event: ARENA_REWARD param AMOUNT             is {Convert.ToInt32(GetEventParam(CustomEventAttr.AMOUNT,              eventData))}");
            GameDebug.Log($"event: ARENA_REWARD param USER_RATING        is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,         eventData))}");
            GameDebug.Log($"event: ARENA_REWARD param ARENA_REWARD_ORDER is {Convert.ToInt32(GetEventParam(CustomEventAttr.ARENA_REWARD_ORDER,  eventData))}");
            GameDebug.Log($"event: ARENA_REWARD param ARENA_NUMBER       is {Convert.ToInt32(GetEventParam(CustomEventAttr.ARENA_NUMBER,        eventData))}");

            customEventParams.AddParam(CustomEventAttr.USER_ARENA,          Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA,           eventData)));
            customEventParams.AddParam(CustomEventAttr.REWARD_ID,           Convert.ToInt32(GetEventParam(CustomEventAttr.REWARD_ID,            eventData)));
            customEventParams.AddParam(CustomEventAttr.ARENA_REWARD_TYPE,   Convert.ToString(GetEventParam(CustomEventAttr.ARENA_REWARD_TYPE,   eventData)));
            customEventParams.AddParam(CustomEventAttr.AMOUNT,              Convert.ToInt32(GetEventParam(CustomEventAttr.AMOUNT,               eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_RATING,         Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,          eventData)));
            customEventParams.AddParam(CustomEventAttr.ARENA_REWARD_ORDER,  Convert.ToInt32(GetEventParam(CustomEventAttr.ARENA_REWARD_ORDER,   eventData)));
            customEventParams.AddParam(CustomEventAttr.ARENA_NUMBER,        Convert.ToInt32(GetEventParam(CustomEventAttr.ARENA_NUMBER,         eventData)));

            Analytics.CustomEvent(CustomEventName.ARENA_REWARD, customEventParams);
        }
        private void OnArenaOpen(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: ARENA_OPEN param ARENA_NUMBER is  {Convert.ToInt32(GetEventParam(CustomEventAttr.ARENA_NUMBER,    eventData))}");
            GameDebug.Log($"event: ARENA_OPEN param ARENA_ID is  {Convert.ToInt32(GetEventParam(CustomEventAttr.ARENA_ID,    eventData))}");
            GameDebug.Log($"event: ARENA_OPEN param USER_RATING is  {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING, eventData))}");

            customEventParams.AddParam(CustomEventAttr.ARENA_NUMBER,    Convert.ToInt32(GetEventParam(CustomEventAttr.ARENA_NUMBER,     eventData)));
            customEventParams.AddParam(CustomEventAttr.ARENA_ID,    Convert.ToInt32(GetEventParam(CustomEventAttr.ARENA_ID,     eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_RATING, Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING,  eventData)));

            Analytics.CustomEvent(CustomEventName.ARENA_OPEN, customEventParams);
        }
        private void OnNameChosen(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: NAME_CHOSEN param USER_NAME is {Convert.ToString(GetEventParam(CustomEventAttr.USER_NAME, eventData))}");

            customEventParams.AddParam(CustomEventAttr.USER_NAME, Convert.ToString(GetEventParam(CustomEventAttr.USER_NAME, eventData)));

            Analytics.CustomEvent(CustomEventName.NAME_CHOSEN, customEventParams);
        }
        private void OnCompleteRegistration()
        {
            GameDebug.Log($"event: COMPLETE_REGISTRATION sent)");
            Analytics.CustomEvent(CustomEventName.COMPLETE_REGISTRATION);    
        }
        private void OnFirstLoadComplete()
        {
            GameDebug.Log($"event: FIRST_LOAD_COMPLETE sent)");
            Analytics.CustomEvent(CustomEventName.FIRST_LOAD_COMPLETE);
        }
        private void OnLocalesLoaded(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: LOCALES_LOADED param LANGUAGE is {Convert.ToString(GetEventParam(CustomEventAttr.LANGUAGE, eventData))}");

            customEventParams.AddParam(CustomEventAttr.LANGUAGE, Convert.ToString(GetEventParam(CustomEventAttr.LANGUAGE, eventData)));

            Analytics.CustomEvent(CustomEventName.LOCALES_LOADED, customEventParams);
        }
        private void OnDomainRead(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: DOMAIN_READ param BINARY_DOMAIN is {Convert.ToString(GetEventParam(CustomEventAttr.BINARY_DOMAIN, eventData))}");

            customEventParams.AddParam(CustomEventAttr.BINARY_DOMAIN, Convert.ToString(GetEventParam(CustomEventAttr.BINARY_DOMAIN, eventData)));

            Analytics.CustomEvent(CustomEventName.BINARY_DOMAIN_READ, customEventParams);
        }

        private void OnRatingVsRating(Dictionary<string, object> eventData)
        {
            CustomEventParams customEventParams = new CustomEventParams();

            GameDebug.Log($"event: RATING_VS_RATING param USER_ARENA    is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA, eventData))}");
            GameDebug.Log($"event: RATING_VS_RATING param USER_RATING   is {Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING, eventData))}");
            GameDebug.Log($"event: RATING_VS_RATING param ENEMY_RATING  is {Convert.ToInt32(GetEventParam(CustomEventAttr.ENEMY_RATING, eventData))}");
            GameDebug.Log($"event: RATING_VS_RATING param RIVAL         is {Convert.ToString(GetEventParam(CustomEventAttr.RIVAL, eventData))}");

            customEventParams.AddParam(CustomEventAttr.USER_ARENA, Convert.ToInt32(GetEventParam(CustomEventAttr.USER_ARENA, eventData)));
            customEventParams.AddParam(CustomEventAttr.USER_RATING, Convert.ToInt32(GetEventParam(CustomEventAttr.USER_RATING, eventData)));
            customEventParams.AddParam(CustomEventAttr.ENEMY_RATING, Convert.ToInt32(GetEventParam(CustomEventAttr.ENEMY_RATING, eventData)));
            customEventParams.AddParam(CustomEventAttr.RIVAL, Convert.ToString(GetEventParam(CustomEventAttr.RIVAL, eventData)));

            Analytics.CustomEvent(CustomEventName.RATING_VS_RATING, customEventParams);
        }
    }
}
