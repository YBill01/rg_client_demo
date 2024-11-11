using System.Collections;
using System.Collections.Generic;
using Legacy.Database;
using UnityEngine;
using System;

namespace Legacy.Client
{

    public enum InAppPurchaseType
    {
        BuyChest,
        BuyCards,
        UpgradeCard,
        UpgradeHero,
        BuyHero,
        SpeedUpChest,
        BuyOffer,
        BuySoft,
    }

    public enum CustomEventTypes // ADDING NEW ELEMENTS MUST BE DONE STRICTLY TO THE END OF THE LIST
    {
        TutorialStep,
        SessionStart,
        SessionEnd,
        LevelUp,
        RealPayment,

        FirstPayment,
        CurrencyEarn,
        CurrencySpend,
        ArenaReached,
        FirstOpen,

        Login,
        CompleteRegistration,
        FirstLoadComplete,
        BattleStart,
        BattleEnd,

        CardUsed,
        SkillUsed,
        ChestOpen,
        ChestStart,
        ChestSpeedUp,

        CardReceive,
        CardUpgrade,
        EarnedCurrency,
        SoftExchange,
        HeroBuy,

        HeroUpgrade,
        BattlePassReward,
        ArenaReward,
        ArenaOpen,
        NameChosen,
        InAppPurchase,

        LocalesLoaded,
        BinaryDomainRead,
        RatingVsRating
    }

    public static class CustomEventOnce
	{
        public static readonly CustomEventTypes[] UniqueEventsList = {
            CustomEventTypes.FirstOpen,
            CustomEventTypes.CompleteRegistration,
            CustomEventTypes.FirstLoadComplete
        };

        public static bool HasCustomEventOnce(CustomEventTypes customEventType, out Int32 index)
        {
            index = Array.IndexOf(CustomEventOnce.UniqueEventsList, customEventType);

            return index != -1;
        }

        public static void SaveCustomEventOnce(byte index)
        {
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.Analytics);
            message.Write((byte)AnalyticCommandType.EventOnce);
            message.Write(index);

            var em = ClientWorld.Instance.EntityManager;
            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }
    }

    public static class CustomEventAttr
    {
        public const string UID = "user_id";
        public const string TIME = "time";
        public const string TUTORIAL_STEP = "tutorial_step";
        public const string LEVEL_UP = "level_up";
        public const string SHARDS = "shards";
        public const string IN_APP_PURCHASE_ID = "in_app_purchase_id";
        public const string IN_APP_PURCHASE_TYPE = "in_app_purchase_type";
        public const string IN_APP_PURCHASE_AMOUNT = "in_app_purchase_amount";
        public const string IN_APP_PURCHASE_PRICE = "in_app_purchase_price";
        public const string IN_APP_PURCHASE_CURRENCY = "in_app_purchase_currency";
        public const string REAL_PAYMENT_TRANSACTION_ID = "real_payment_id";
        public const string REAL_PAYMENT_IN_APP_PRICE = "real_payment_in_app_price";
        public const string REAL_PAYMENT_IN_APP_NAME = "real_payment_in_app_name";
        public const string REAL_PAYMENT_IN_APP_CURRENCY_ISO_CODE = "real_payment_in_app_currency";
        public const string AMOUNT                  = "amount";
        public const string TIME_QUE                = "time_que";
        public const string BATTLE_TIME             = "battle_time";
        public const string SKILL_ID                = "skill_id";
        public const string REWARD_ID               = "reward_id";
        public const string SEASON_ID               = "season_id";
        public const string RATING_RESULT           = "rating_result";
        public const string STARS_GET               = "stars_get";
        public const string RIVAL                   = "rival";
        public const string ARENA_ID                = "arena_id";
        public const string ARENA_NUMBER            = "arena_number";
        public const string ARENA_REWARD_ORDER      = "reward_order"; 
        public const string ARENA_REWARD_TYPE       = "reward_type"; 
        public const string USER_ARENA              = "user_arena";
        public const string USER_NAME               = "user_name";
        public const string USER_RATING             = "user_rating";
        public const string BATTLE_NUMBER           = "battle_number";
        public const string BATTLE_RESULT           = "battle_result";
        public const string HERO_ID                 = "hero_id";
        public const string HERO_LEVEL              = "hero_level";
        public const string HERO_NEW_LEVEL          = "hero_new_level";
        public const string CHEST_ID                = "chest_id";
        public const string CHEST_SOURCE_ID         = "source_id";
        public const string CHEST_SOURCE_TYPE       = "source_type";
        public const string CHEST_COST              = "cost";
        public const string CHEST_TIME_TO_OPEN      = "time"; // "время которое оставалось до открытия и которое ускорили"
        public const string CARD_ID                 = "card_id";
        public const string CARD_LEVEL              = "card_level";
        public const string CARD_TYPE               = "card_type";
        public const string CARD_AMOUNT             = "card_amount";
        public const string CARD_SOURCE_TYPE        = "source_type";
        public const string CARD_SOURCE_ID          = "source_id";
        public const string CARD_NEW_CARD           = "new_card";
        public const string CARD_NEW_LEVEL          = "card_new_level"; //just after upgrade
        public const string CURRENCY_SOURCE_ID      = "source_id"; 
        public const string CURRENCY_SOURCE_TYPE    = "source_type"; 
        public const string CURRENCY_AMOUNT         = "currency_amount"; 
        public const string CURRENCY_TYPE           = "currency_type";
        public const string BP_REWARD_ORDER         = "reward_order"; 
        public const string BP_REWARD_TYPE          = "reward_type"; 
        public const string BP_TYPE                 = "type"; 
        public const string PURCHASE_ID             = "purchaseId"; 
        public const string PURCHASE_TYPE           = "purchaseType"; 
        public const string PURCHASE_AMOUNT         = "purchaseAmount"; 
        public const string PURCHASE_PRICE          = "purchasePrice"; 
        public const string PURCHASE_CURRENCY       = "purchaseCurrency"; 
        public const string LANGUAGE                = "language"; 
        public const string BINARY_DOMAIN           = "binary_domain"; 
        public const string ENEMY_RATING            = "enemy_rating"; 

    }

    public static class CustomEventName
    {
        public const string BATTLE_START        = "Battle_start";
        public const string BATTLE_END          = "Battle_end";
        public const string CARD_USED           = "Card_used";
        public const string SKILL_USED          = "Skill_used";
        public const string CHEST_OPEN          = "Chest_open";

        public const string CHEST_START         = "Chest_start";
        public const string CHEST_SPEEDUP       = "Chest_speedup";
        public const string CARD_RECEIVE        = "Card_receive";
        public const string CARD_UPGRADE        = "Сard_upgrade";
        public const string HERO_BUY            = "Hero_buy";

        public const string HERO_UPGRADE        = "Hero_upgrade";
        public const string EARNED_CURRENCY     = "Earned_currency";
        public const string SOFT_EXCHANGE       = "Soft_exchange";
        public const string BATTLE_PASS_REWARD  = "Battle_Pass_Reward";
        public const string ARENA_REWARD        = "Arena_Reward";

        public const string ARENA_OPEN          = "Arena_Open";
        public const string NAME_CHOSEN         = "Name_chosen";

        public const string COMPLETE_REGISTRATION   = "Complete_registration";
        public const string FIRST_LOAD_COMPLETE     = "First_load_complete";
        public const string LOCALES_LOADED          = "Locales_loaded";
        public const string BINARY_DOMAIN_READ      = "Binary_domain_read";
        public const string RATING_VS_RATING        = "Rating_vs_rating";
    }

    public static class CustomEventEnum
    {
        public class RivalType
        {
            public const string BOT  = "Bot";
            public const string REAL = "Real";
        }
        public class BattleResult 
        {
            public const string DEFEAT = "Defeat";
            public const string WIN    = "Win";
        }
        public class NewCard 
        {
            public const string NO  = "No";
            public const string YES = "Yes";
        }
        public class BattlePassType
        {
            public const string FREE    = "Free";
            public const string PREMIUM = "Premium";
        }
        public class ChestSourceType
        {
            public const string BATTLE_WIN  = "Battle_win";
            public const string BATTLE_PASS = "Battle_pass";
            public const string BOUGHT      = "Bought";
            public const string ARENA       = "Arena";
        }
        public class CardSourceType
        {
            public const string CHEST = "Chest";
        }
        public class CardRarityType
        {
            public const string COMMON = "Common";
            public const string RARE   = "Rare";
            public const string EPIC   = "Epic";
            public const string LEGEND = "Legend";
        }
        public class CurrencySourceType
        {
            public const string OPEN_CHEST          = "Open_chest";
            public const string DAILY_TASK          = "Daily_task";
            public const string BATTLE_PASS_FREE    = "Battle_pass_free";
            public const string BATTLE_PASS_PREMIUM = "Battle_pass_premium";
            public const string BATTLE_REWAR        = "Battle_reward";
            public const string BOUGHT_IN_SHOP      = "Bought_in_Shop";
            public const string DAILY_REWARD        = "Daily_reward";
            public const string COMPETITION         = "Competition ";  //"сезонный реейтинг"
        }
        public class RewardType
        {
            public const string HARD    = "Hard";
            public const string SOFT    = "Soft";
            public const string LOOTBOX = "LootBox";
            public const string CARD    = "Card";
        }
    }

    abstract public class AnalyticsSender : MonoBehaviour
    {
        public abstract void SendEvent(CustomEventTypes eventType, Dictionary<string, object> eventData);
        public abstract void Init();

        protected object GetEventParam(string key, Dictionary<string, object> eventData)
        {
            object param = 0;
            if (eventData.TryGetValue(key, out object value))
            {
                param = value;
            }
            else
            {
                throw new Exception($"No needed key in EventData: {key}");
            }
            return param;
        }
    }  
}
