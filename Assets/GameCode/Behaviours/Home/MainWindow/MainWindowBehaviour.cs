using Legacy.Database;
using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace Legacy.Client
{
    public class MainWindowBehaviour : WindowBehaviour
    {
        [SerializeField]
        private LootBoxesBehaviour loots;
        [SerializeField]
        private CurrenciesBehaviour currencies;
        [SerializeField]
        private AccountBehaviour Account;
        [SerializeField]
        private RectTransform BattleButton;
        [SerializeField]
        private BattlePassButtonBehaviour BattlePassButton;
        [SerializeField]
        private HomeTutorPanelBehaviour HomeTutorPanel;
        [SerializeField]
        private GameObject LeftContainer;
        [SerializeField]
        private GameObject updateVersionRewardWindow;

        public MenuTutorialPointerBehaviour menuTutorialPointer;

        public UnityEvent LoadedProfile;

        private Vector3 battleButtonPosition = Vector3.zero;

        public LootBoxBehaviour BoxToOpen;
        private bool loaded;

        private ProfileInstance profile => ClientWorld.Instance.Profile;

        public LootBoxesBehaviour GetLoots
        {
            get => loots;
        }

        public RectTransform GetBattleButton
        {
            get => BattleButton;
        }

        public BattlePassButtonBehaviour GetBattlePassButton
        {
            get => BattlePassButton;
        }

        public GameObject GetLeftContainer
        {
            get => LeftContainer;
        }

        private void AfterProfileLoaded()
        {
            loaded = true;

            BattleDataContainer.Instance.CheckRewardParticles(BattleButton.position);
        }

        public void Settings()
        {
            if (profile.HardTutorialState < 4)
            {
                PopupAlertBehaviour.ShowHomePopupAlert(Input.mousePosition, Locales.Get("locale:1483"));
                return;
            }
            WindowManager.Instance.OpenWindow(childs_windows[10]);
        }

        public void Arena()
        {
            if (profile.HardTutorialState < 4)
            {
                PopupAlertBehaviour.ShowHomePopupAlert(Input.mousePosition, Locales.Get("locale:1483"));
                return;
            }
            WindowManager.Instance.OpenWindow(childs_windows[0]);
        }

        public void Hero()
        {
            WindowManager.Instance.OpenWindow(childs_windows[4]);
        }

        public void Decks()
        {
            WindowManager.Instance.OpenWindow(childs_windows[3]);
        }

        public void Heroes()
        {
            WindowManager.Instance.OpenWindow(childs_windows[2]);
        }

        public void Shop()
        {
            WindowManager.Instance.OpenWindow(childs_windows[7]);
        }

        public void BattlePass()
        {
            if (Database.Shop.Instance.BattlePass.GetCurrent() != null)
            {
                WindowManager.Instance.OpenWindow(childs_windows[9]);
            }
            else
            {
                BattlePassButton.UpdateView();
            }
        }

        public void OpenShopWithSection(RedirectMenuSection section)
        {
            Shop();

            if (childs_windows[7] is ShopWindowBehaviour shopWindow)
            {
                shopWindow.RedirectToSection(section);
            }
        }

        public void ChangeName()
        {
            WindowManager.Instance.OpenWindow(childs_windows[8]);
        }

        private IEnumerator BotsBattleCoroutine()
        {
            for (int i = 0; i < 10; i++)
            {
                Debug.Log("Go to BotxBot");
                NetworkMessageHelper.BattleBotxBot();
                yield return new WaitForSeconds(3f);
            }
        }
        public void Battle()
        {
            menuTutorialPointer.popupMessage.Hide();

#if !PRODUCTION
            if (Input.GetKey(KeyCode.S))
            {
                WindowManager.Instance.OpenWindow(childs_windows[1]);
                Debug.Log("Go to Sandbox. It's a world of miracles and rainbows.");
                //TutorialMessageBehaviour.InitBattleTriggerSystem();
                NetworkMessageHelper.Sandbox(ClientWorld.Instance.EntityManager);
            }

            else
            if (Input.GetKey(KeyCode.B))
            {
                StartCoroutine(BotsBattleCoroutine());
            }
            else
#endif
            if (profile.HardTutorialState < Tutorial.Instance.TotalCount())
            {
                if (profile.HardTutorialState > 2 && !IsPlayerReadyToRatingBattle())
                {
                    PopupAlertBehaviour.ShowHomePopupAlert(Input.mousePosition, Locales.Get("locale:1204"));
                    SoftTutorialManager.Instance.ClickedOnBattleWith6Cards = true;
                    SoftTutorialManager.Instance.CheckTutorialsForCurrentWindowFast();
                    //ThirdMenuBehaviourExtention.CreateStartBattleWith6CardsEvent();
                    return;
                }

                WindowManager.Instance.OpenWindow(childs_windows[1]);
                TutorialMessageBehaviour.InitBattleTriggerSystem();
                NetworkMessageHelper.Tutorial(
                    ClientWorld.Instance.EntityManager,
                    (ushort)(ClientWorld.Instance.Profile.HardTutorialState + 1)
                );
                menuTutorialPointer.HideStrongPointer();

                #region SendAnalytic
                switch (ClientWorld.Instance.Profile.HardTutorialState)
                {
                    case 1:
                        AnalyticsManager.Instance.SendTutorialStep(AnalyticTutorialStep.Menu1ClickBattle);
                        break;
                    case 2:
                        AnalyticsManager.Instance.SendTutorialStep(AnalyticTutorialStep.Menu2ClickBattle);
                        break;
                    case 3:
                        AnalyticsManager.Instance.SendTutorialStep(AnalyticTutorialStep.Menu3ClickBattle);
                        break;
                    default:
                        break;
                }

                #endregion
            }
            else
            {
                if (IsPlayerReadyToRatingBattle() && ClientWorld.Instance.Profile.DecksCollection.IsFullDesc())
                {
                    WindowManager.Instance.OpenWindow(childs_windows[1]);
                    NetworkMessageHelper.Battle1x1();
                    menuTutorialPointer.HideStrongPointer();
                }
                else
                {
                    PopupAlertBehaviour.ShowHomePopupAlert(Input.mousePosition, Locales.Get("locale:1204"));
                }
            }
        }

        internal Vector3 GetBattleButtonPosition()
        {
            return battleButtonPosition;
        }

        private bool IsPlayerReadyToRatingBattle()
        {
            return profile.DecksCollection.ActiveSet.Cards.Length > 7;
        }

        internal bool GetClickedBox(out LootBoxBehaviour clickedBox)
        {
            clickedBox = BoxToOpen;
            return clickedBox != null;
        }

        internal void DropClickedBox()
        {
            BoxToOpen = null;
        }

        protected override void SelfClose()
        {
            loots.IfBoxArived();
            gameObject.SetActive(false);
            BattlePassButton.DisableShiftEffect();
            MenuArenasBehaviour.Instance.HideArena();
        }

        public void StartTutor()
        {
            menuTutorialPointer.gameObject.SetActive(true);
            //HomeTutorialHelper.Instance.OnTutorialStart(); // запуск тутора
        }
        public void StopTutor()
        {
            menuTutorialPointer.gameObject.SetActive(false);
        }

        protected override void SelfOpen()
        {

#if !UNITY_EDITOR
            var clientLastVersion   = ClientWorld.Instance.Profile.customRewards.ClientAppVersion;
            var currentVersion      = Application.version;
            GameDebug.Log("-=- clientLastSavedVersion is " + clientLastVersion);
            GameDebug.Log("-=- currentAppVersion is " + currentVersion);
            if (!clientLastVersion.Equals(currentVersion))
            {
                ClientWorld.Instance.Profile.customRewards.ClientAppVersion = currentVersion;
                Instantiate(updateVersionRewardWindow, transform);
            }
#endif
            menuTutorialPointer.HideHeroMessage();
            LoadProfile();
            MenuArenasBehaviour.Instance.Enable();

            gameObject.SetActive(true);
            //  HomeTutorPanel.OnStart();
            if (!profile.IsBattleTutorial && profile.battleStatistic.battles > 0)
            {
                MainMenuArenaChangeBehaviour.Instance.Enable();
            }
            else
            {
                HomeTutorPanel.OnStart();
            }
            StartTutor();

            if (!profile.IsBattleTutorial)
            {
                BattlePassButton.UpdateView();
            }

            WindowManager.Instance.SetUpPanelNextLevel();

            /*if (isNewCard)
            {
                menuTutorialPointer.popupMessage.ShowTextAtLeftFrom(Locales.Get("locale:2119"), menuTutorialPointer.OpenDeckPoint);
                isNewCard = false;
            }*/
            //else 
            menuTutorialPointer.popupMessage.Hide();
        }

        //public bool isNewCard = false;
        public void GetLootBox(LootBoxBehaviour boxToOpen)
        {
            BoxToOpen = boxToOpen;
            WindowManager.Instance.OpenWindow(childs_windows[5]);
        }

        void LoadProfile()
        {
            if (!loaded)
            {
                SetProfileData();
                //TODO: Подписаться когда надо! Сейчас на старте валюты сетятся 500 раз
                profile.PlayerProfileUpdated.AddListener(SetProfileData);
                profile.Stock.ChangeEvent.AddListener(SetCurrencies);
                loaded = true;
            }
        }

        private void SetProfileData()
        {
            if (!profile.IsBattleTutorial)
            {
                BattlePassButton.Init();
            }

            loots.InitBoxes(this);
            SetCurrencies();
            LoadedProfile.Invoke();

            // Отправка push-notification
            //PushNotifications.Instance.DailyLocalNotificationStart();
        }

        void SetCurrencies()
        {
            Account.ArenaButton.SetRating((ushort)profile.Rating.current);
            currencies.SetHard((int)profile.Stock.getItem(CurrencyType.Hard).Count);
            currencies.SetSoft((int)profile.Stock.getItem(CurrencyType.Soft).Count);
            //currencies.SetShards((int)profile.Stock.getItem(CurrencyType.Shards).Count);
        }

        public override void Init(Action callback)
        {
            LoadedProfile.AddListener(AfterProfileLoaded);
            HomeTutorPanel.Init();
            if (childs_windows[9] is BattlePassWindowBehaviour battlePassWindow)
                battlePassWindow.RewardTimerUp += OnRewardTimerUp;

            MobileInputManager.Instance.BackButtonDown += OnBackButtonDown;
            callback();
        }

        private void OnRewardTimerUp()
        {
            BattlePassButton.UpdateView();
        }

        private void OnBackButtonDown()
        {
            // Кастомная обработка Esc(back в телефонах) в разных открытых окнах
            if (WindowManager.Instance.CurrentWindow is LootBoxWindowBehaviour)
            {
                (WindowManager.Instance.CurrentWindow as LootBoxWindowBehaviour).Tap();
                return;
            }
            else if (WindowManager.Instance.CurrentWindow is CardUpgradeWindowBehavior)
            {
                (WindowManager.Instance.CurrentWindow as CardUpgradeWindowBehavior).TapToClose();
                (WindowManager.Instance.CurrentWindow as CardUpgradeWindowBehavior).GetComponent<LevelUpCardBehaviour>().SkipStep();
                return;
            }
            else if (WindowManager.Instance.CurrentWindow is HeroWindowBehaviour)
            {
                (WindowManager.Instance.CurrentWindow as HeroWindowBehaviour).TapToClose();
                return;
            }

            if (WindowManager.Instance.CurrentWindow == this)
                WindowManager.Instance.OpenWindow(childs_windows[11]);
            else
                WindowManager.Instance.Back();
        }

        internal void OpenLootPopUp(LootBoxBehaviour lootBoxBehaviour)
        {
            BoxToOpen = lootBoxBehaviour;
            WindowManager.Instance.OpenWindow(childs_windows[6]);
        }

        private void OnDestroy()
        {
            if (childs_windows[9] is BattlePassWindowBehaviour battlePassWindow)
                battlePassWindow.RewardTimerUp -= OnRewardTimerUp;

            MobileInputManager.Instance.BackButtonDown -= OnBackButtonDown;
        }
    }
}