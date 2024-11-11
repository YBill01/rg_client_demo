using Legacy.Database;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace Legacy.Client
{
    public class DeckChangeEvent : UnityEvent<byte> { }
    public class DeckModifyEvent : UnityEvent<ushort, ushort> { }
    public class HeroSelectEvent : UnityEvent<ushort> { }
    public class HeroUpgradeEvent : UnityEvent<ushort> { }
    public class StockItemChangeEvent : UnityEvent<ushort, long> { };
    public class CardUpgradeEvent : UnityEvent<ushort, ushort> { };
    public class ProfileInstance
    {
        public uint index { get; private set; }

        public UnityEvent NameUpdateEvent = new UnityEvent();

        private Inventory _inventory;
        private PlayerCampaigns _campaigns;
        private PlayerStock _stock;
        private DecksCollection _decksCollection;
        private PlayerTutorial _tutorial;
        private PlayerProfileBattleStatistic _battleSatistic;
        private List<ushort> viewedHeroes; // id героев на которых мы уже смотрели

        public bool payer;
        internal bool GetPlayerHero(ushort hero_index, out PlayerProfileHero hero)
        {
            hero = default;
            if (heroes.GetByIndex(hero_index, out PlayerProfileHero _hero))
            {
                hero = _hero;
                return true;
            }
            return false;
        }

        private HeroSelectEvent heroSelectEvent = new HeroSelectEvent();
        private CardUpgradeEvent cardUpgradeEvent = new CardUpgradeEvent();
        private DeckModifyEvent deckModifyEvent = new DeckModifyEvent();
        private UnityEvent playerProfileUpdated = new UnityEvent();

        public PlayerProfileHero CurrentHero => heroes[SelectedHero];
        public ushort SelectedHero { get { return DecksCollection.ActiveSet.HeroID; } }
        public DatabaseDictionary<PlayerProfileHero> heroes { get; private set; }

        public string name;
        public PlayerProfileLoots loot { get; private set; }
        public PlayerProfileArenaBoosterTime arenaBoosterTime { get; private set; }
        public PlayerProfileLevel Level { get; private set; }

        internal void SelectNextHero()
        {
            var nextHero = GetNextExistingHero();
            SelectHero(nextHero);
        }

        internal void SelectHero(BinaryHero hero)
        {
            SelectHero(hero.index);
        }

        private ushort GetNextExistingHero()
        {
            ushort nextIndex = 0;
            byte i = 0;
            foreach (var pair in heroes)
            {
                if (SelectedHero.ToString() == pair.Key)
                {
                    if (i == heroes.Count - 1)
                    {
                        nextIndex = 0;
                    }
                    else
                    {
                        nextIndex = (ushort)(i + 1);
                    }
                    break;
                }
                i++;
            }
            i = 0;
            foreach (var pair in heroes)
            {
                if (i == nextIndex)
                {
                    return (ushort)int.Parse(pair.Key);
                }
                i++;
            }
            return 0;
        }

        internal void SelectPreviousHero()
        {
            var previousHero = GetPreviousExistingHero();
            SelectHero(previousHero);
        }

        private ushort GetPreviousExistingHero()
        {
            ushort previousIndex = 0;
            byte i = 0;
            foreach (var pair in heroes)
            {
                if (SelectedHero.ToString() == pair.Key)
                {
                    if (i == 0)
                    {
                        previousIndex = (ushort)(heroes.Count - 1);
                    }
                    else
                    {
                        previousIndex = (ushort)(i - 1);
                    }
                    break;
                }
                i++;
            }
            i = 0;
            foreach (var pair in heroes)
            {
                if (i == previousIndex)
                {
                    return (ushort)int.Parse(pair.Key);
                }
                i++;
            }
            return 0;
        }

        //private LootBoxList _lootBoxList;

        public static ushort CurrentCampaignID;

        private EntityManager entityManager;
        public PlayerProfileRating Rating { get; private set; }

        public PlayerProfileDaylics daylics;
        public PlayerProfileActions actions;
        public PlayerProfileDailyDeals dailyDeals;
        public PlayerProfileBattlePass battlePass;
        public PlayerProfileSettings playerSettings;
        public PlayerProfileAnalyticEvents analyticEvents;
        public PlayerProfileBattleStatistic battleStatistic;
        public PlayerProfileCustomRewardsInfo customRewards;

        public ProfileInstance()
        {
            var _settings = Settings.Instance.Get<LootSettings>();

            entityManager = ClientWorld.Instance.EntityManager;

            _inventory = new Inventory();
            _stock = new PlayerStock();
            _campaigns = new PlayerCampaigns();
            _tutorial = new PlayerTutorial();
            _battleSatistic = new PlayerProfileBattleStatistic();
            loot = new PlayerProfileLoots
            {
                boxes = new DatabaseList<PlayerProfileLootBox>(_settings.slots)
            };

            //_lootBoxList = new LootBoxList(loot);
            _decksCollection = new DecksCollection(_inventory);
        }

        public void UpdateName(string newName)
        {
            if (name != ValidateName(newName))
            {
                FixedString64 _name = new FixedString64(newName);
                var message = new NetworkMessageRaw();
                message.Write((byte)ObserverPlayerMessage.UserCommand);
                message.Write((byte)UserCommandType.AccountUpdate);
                message.Write((byte)AccountCommandType.UpdateName);
                message.Write(_name);

                var em = ClientWorld.Instance.EntityManager;

                var messageEntity = em.CreateEntity();
                em.AddComponentData(messageEntity, message);
                em.AddComponentData(messageEntity, default(ObserverMessageTag));
                name = newName;
                NameUpdateEvent.Invoke();
            }
        }
        public void UpdateLanguage(Language newlanguage)
        {
            //if (name != ValidateName(newName))
            //  {
            //    FixedString64 _name = new FixedString64(newName);
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.AccountUpdate);
            message.Write((byte)AccountCommandType.UpdateLanguage);
            message.Write((byte)newlanguage);

            var em = ClientWorld.Instance.EntityManager;

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
            //  name = newName;
            //  NameUpdateEvent.Invoke();
            // }
        }

        string ValidateName(string newName)
        {
            string _name = newName;
            if (
                newName.Length < 1
            )
            {
                _name = SystemInfo.deviceName;
            }
            return _name;
        }

        public void UpgradeCard(ushort sid)
        {
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.DeckUpdate);
            message.Write((byte)DeckCommandType.Card);
            message.Write(sid);

            var em = ClientWorld.Instance.EntityManager;

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }

        public void UpdatePlayerAppVersion()
        {
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.UpdateAppVersion);
            var em = ClientWorld.Instance.EntityManager;
            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }

        public void ViewCard(ushort sid)
        {
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.DeckUpdate);
            message.Write((byte)DeckCommandType.View);
            message.Write(sid);

            var em = ClientWorld.Instance.EntityManager;

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }

        public void ViewHero(ushort sid)
        {
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.HeroUpdate);
            message.Write((byte)HeroCommandType.View);
            message.Write(sid);

            var em = ClientWorld.Instance.EntityManager;

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }

        public void UpgradeHero(ushort sid)
        {
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.HeroUpdate);
            message.Write((byte)HeroCommandType.Upgrade);
            message.Write(sid);

            var em = ClientWorld.Instance.EntityManager;

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }

        public void CreateHero(ushort hero_index, FixedString4096 receipt = default)
        {
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.HeroUpdate);
            message.Write((byte)HeroCommandType.Buy);
            message.Write(hero_index);
            if(receipt.Length > 0)
            {
                message.Write(receipt);
            }

            var em = ClientWorld.Instance.EntityManager;

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }

        public void UpgradeHeroSkill(ushort hero_index, ushort skill_index)
        {
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.HeroUpdate);
            message.Write((byte)HeroCommandType.Skill);
            message.Write(hero_index);
            message.Write(skill_index);

            var em = ClientWorld.Instance.EntityManager;

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }

        public void TakeArenaReward(ushort arean_index, byte reward_index)
        {
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.ArenaUpdate);
            message.Write((byte)ArenaCommandType.Reward);
            message.Write(arean_index);
            message.Write(reward_index);

            var em = ClientWorld.Instance.EntityManager;

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }

        public void SelectHero(ushort ID)
        {
            DecksCollection.ActiveSet.SetHero(ID);
            heroSelectEvent.Invoke(ID);

            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.DeckUpdate);
            message.Write((byte)DeckCommandType.Hero);
            message.Write(ID);

            var messageEntity = entityManager.CreateEntity();
            entityManager.AddComponentData(messageEntity, message);
            entityManager.AddComponentData(messageEntity, default(ObserverMessageTag));
        }

        public void BuildFromObserverInstance(PlayerProfileInstance player, bool StartSession = false)
        {
            if (!StartSession)
            {
                //TODO check changes and invoke only changes
                if (player.playerSettings.language != playerSettings.language)
                {
                    playerSettings.language = player.playerSettings.language;

                    ObserverReachableSystem.HardDisconect = true;                   
                }
            }

            _inventory.Read(player);
            _campaigns.Read(player);
            _tutorial.Read(player);

            Rating = player.rating;

            _stock.SetItem(CurrencyType.Rating, player.rating.current);
            _stock.SetItem(CurrencyType.Soft, player.currency.soft);
            _stock.SetItem(CurrencyType.Hard, player.currency.hard);
            heroes = player.heroes;
            loot = player.loots;
            arenaBoosterTime = player.arenaBoosterTime;
            email = player.email;
            name = player.name;
            Level = player.level;
            index = player._id;
            daylics = player.daylics;
            actions = player.actions;
            dailyDeals = player.dailyDeals;
            battlePass = player.battlePass;
            playerSettings = player.playerSettings;
            payer = player.payer;
            analyticEvents = player.analyticEvents;
            battleStatistic = player.battleStatistic;
            battleStatistic = player.battleStatistic;
            customRewards = player.customRewards;

            _decksCollection.SetSort(player.config.sort);
            _decksCollection.UpdateAvailable(CurrentArena.number);
            _decksCollection.ResetDecks(player.sets);
            _decksCollection.ChooseSet(player.config.deck);

            viewedHeroes = new List<ushort>(player.viewedHeroes);

            PlayerProfileUpdated.Invoke();
        }

        public void BuyFromBank(ushort sid, FixedString4096 receipt = default)
        {
            Debug.Log($"Receipt: {receipt}");
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.ShopUpdate);
            message.Write((byte)ShopCommandType.Bank);
            message.Write(sid);
            if(receipt.Length > 0)
            {
                message.Write(receipt);
                Debug.Log($"Receipt writed.");
            }

            var em = ClientWorld.Instance.EntityManager;

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }

        public void BuyNotEnoughCoins(int count)
        {
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.ShopUpdate);
            message.Write((byte)ShopCommandType.NotEnoughCoins);
            message.Write(count);

            var em = ClientWorld.Instance.EntityManager;

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }    

        public void BuyLootBox(ushort sid)
        {
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.ShopUpdate);
            message.Write((byte)ShopCommandType.LootBox);
            message.Write(sid);

            var em = ClientWorld.Instance.EntityManager;

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }

        public void BuyDailyDeal(ushort sid)
        {
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.ShopUpdate);
            message.Write((byte)ShopCommandType.Daily);
            message.Write(sid);

            var em = ClientWorld.Instance.EntityManager;

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }

        public void BuyAction(ushort sid, FixedString4096 receipt = default)
        {
            Debug.Log($"Receipt: {receipt}");
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.ShopUpdate);
            message.Write((byte)ShopCommandType.Action);
            message.Write(sid);
            if (receipt.Length > 0)
            {
                message.Write(receipt);
                Debug.Log($"Receipt writed.");
            }

            var em = ClientWorld.Instance.EntityManager;

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }
        public void BuyActionLootBox(ushort sid, FixedString4096 receipt = default)
        {
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.ShopUpdate);
            message.Write((byte)ShopCommandType.ActionLootBox);
            message.Write(sid);
            if (receipt.Length > 0)
            {
                message.Write(receipt);
                Debug.Log($"Receipt writed.");
            }

            var em = ClientWorld.Instance.EntityManager;

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }

        public void BuyBattlePass(FixedString4096 receipt)
        {
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.ShopUpdate);
            message.Write((byte)ShopCommandType.BattlePass);
            if (receipt.Length > 0)
            {
                message.Write(receipt);
                Debug.Log($"Receipt writed.");
            }

            var em = ClientWorld.Instance.EntityManager;

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }

        public void ClaimBattlePassReward(bool free, int level)
        {
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.BattlePass);
            message.Write((byte)BattlePassCommandType.Reward);
            message.Write(free);
            message.Write((ushort) level);

            var em = ClientWorld.Instance.EntityManager;

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }

        public Inventory Inventory { get => _inventory; }
        public PlayerStock Stock { get => _stock; }
        //public HeroesInventory Heroes { get => _heroes; }
        public PlayerCampaigns Campaigns { get => _campaigns; }
        public HeroSelectEvent HeroSelectEvent { get => heroSelectEvent; }
        public CardUpgradeEvent CardUpgradeEvent { get => cardUpgradeEvent; }
        public UnityEvent PlayerProfileUpdated { get => playerProfileUpdated; }
        public DeckModifyEvent DeckModifyEvent { get => deckModifyEvent; }
        //public LootBoxList LootBoxList { get => _lootBoxList; }
        public DecksCollection DecksCollection { get => _decksCollection; }

        public List<ushort> ViewedHeroes { get => viewedHeroes; }

        public string email;
        public EventArenaData CurrentArena
        {
            get
            {
                var settings = Settings.Instance.Get<ArenaSettings>();
                return settings.GetArenaData((ushort)Rating.current);
            }
        }

        public ushort HardTutorialState
        {
            get => _tutorial.hard_tutorial_state;
        }

        public int MenuTutorialState
        {
            get { return _tutorial.menu_tutorial_state; }
            set { _tutorial.menu_tutorial_state = value; }

        }

        public DatabaseDictionary<ushort> TutorialsSteps
        {
            get => _tutorial.tutorials_steps;
        }

        //public bool IsMenuTutorial => (MenuTutorialState & (ushort)SoftTutorial.SoftTutorialState.AfterBattle4) == 0;

        public bool IsBattleTutorial => HardTutorialState < Tutorial.Instance.TotalCount();

        public bool IsTutorial => IsBattleTutorial/* || IsMenuTutorial*/;

        public bool HasSoftTutorialState(ushort state)
        {
            return (MenuTutorialState & state) > 0;
        }

        public bool HasSoftTutorialState(SoftTutorial.SoftTutorialState state)
        {
            return HasSoftTutorialState((ushort)state);
        }
    }
}