using Legacy.Database;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static LootBoxBehaviour;

namespace Legacy.Client
{
    public class LootBoxPopUpWindowBehaviour : WindowBehaviour
    {
        [SerializeField] Animator WindowAnimator;

        [SerializeField] LootBoxPopUpDataBehaviour LootDataBehaviour;

        [SerializeField] RectTransform BoxContainer;
        [SerializeField] LegacyButton OpenButton;
        [SerializeField] Image OpenButtonImage;
        [SerializeField] Sprite OpenButtonActiveSprite;
        [SerializeField] Sprite OpenButtonInactiveSprite;
        [SerializeField] LegacyButton SkipWaitingButton;
        [SerializeField] ButtonWithPriceViewBehaviour SkipWaitingButtonPriceBehaviour;
        [SerializeField] GameObject CommentText;

        [SerializeField] UITimerBehaviour Timer;
        [SerializeField] TextMeshProUGUI OpenButtonText;
        [SerializeField] TextMeshProUGUI RagePassText; 
        public LootBoxBehaviour BoxToOpen;

        [SerializeField] GameObject BattlePass;
        [SerializeField] GameObject BattonCont;
        public LootBoxViewBehaviour LootBoxView { get; private set; }

        [SerializeField, Range(0.0f, 1.0f)] float ScaleMultiPlier = 0.6f;

        private ProfileInstance Player;
        private RectTransform OldParent;

		private void Start()
		{
            Player.PlayerProfileUpdated.AddListener(UpdateButtons);
        }

		private void OnDestroy()
		{
            Player.PlayerProfileUpdated.RemoveListener(UpdateButtons);
        }

		public override void Init(Action callback)
        {
            Player = ClientWorld.Instance.Profile;
            RagePassText.text = Locales.Get("locale:1915", $"<color=orange><size=110%>{Locales.Get("locale:868")}</size></color>");

            BattlePass.SetActive(!Player.IsBattleTutorial);
            BattonCont.SetActive(true);
            //if (Player.HardTutorialState < 4 )
                //isMissClick = false;
            callback();
        }

        protected override void SelfClose()
        {
            if(LootBoxView)
                DestroyImmediate(LootBoxView.gameObject);
            WindowAnimator.Play("Close");
        }
        private bool isMissClick = true;
        public void StartOpening()
        {
            var lootbox = Player.loot.boxes[BoxToOpen.indexInLoots];
            if (lootbox.started && lootbox.secondsToOpen == 0)
            {
                WindowManager.Instance.ClosePopUp();
                BoxToOpen.ClickOpenedBox();
            }
            else if (Player.loot.CanOpenBox || (Player.battlePass.isPremiumBought && Player.loot.CanOpenNextBox))
            {
                AnalyticsManager.Instance.ChestStart(lootbox.index);
                var em = ClientWorld.Instance.EntityManager;

                var message = new NetworkMessageRaw();
                message.Write((byte)ObserverPlayerMessage.UserCommand);
                message.Write((byte)UserCommandType.LootUpdate);
                message.Write((byte)LootCommandType.Open);
                message.Write(BoxToOpen.number);

                var messageEntity = em.CreateEntity();
                em.AddComponentData(messageEntity, message);
                em.AddComponentData(messageEntity, default(ObserverMessageTag));
                isMissClick = true;
                //ChangeState(BoxState.Opening);            
            }
            MissClick();
        }

        public void SkipLoot()
        {
            int id = BoxToOpen.BinaryBox.index;
            int price = 0;
            int timeToOpen = 0;
            if (Player.loot.index == BoxToOpen.number)
            {
                price = Loots.PriceToSkip(BoxToOpen.PlayerBox.secondsToOpen, BoxToOpen.settings);
                timeToOpen = (int)BoxToOpen.PlayerBox.secondsToOpen / 60;
            }
            else
            {
                uint time;
                if (Player.arenaBoosterTime.IsActive)
                {
					time = Player.arenaBoosterTime.GetSecondsToOpen(BoxToOpen.BinaryBox.time);
                }
                else
                {
                    time = BoxToOpen.BinaryBox.time;
                }

				price = Loots.PriceToSkip(time, BoxToOpen.settings);
                timeToOpen = (int)BoxToOpen.BinaryBox.time / 60;
            }
            AnalyticsManager.Instance.ChestSpeedUp(id, price, timeToOpen);

            var em = ClientWorld.Instance.EntityManager;

            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.LootUpdate);
            message.Write((byte)LootCommandType.Skip);
            message.Write(BoxToOpen.number);

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));

            WindowManager.Instance.ClosePopUp();
            BoxToOpen.ClickOpenedBox();
            MissClick();
        }

        protected override void SelfOpen()
        {
            BattlePass.SetActive(!Player.IsBattleTutorial);
            BattonCont.SetActive(true);
            ReParentClickedBox();
            if (BoxToOpen != null)
            {
                LootDataBehaviour.ResetData();
                LootDataBehaviour.Init(BoxToOpen.BinaryBox, BoxToOpen.PlayerBox.battlefield);
                UpdateButtons();
            }
            gameObject.SetActive(true);

        }

        private void UpdateButtons()
        {
            var lootbox = Player.loot.boxes[BoxToOpen.indexInLoots];

            if (lootbox != null && lootbox.started && lootbox.secondsToOpen == 0)
            {
                SetCanOpenButtons(true);
                return;
            }

            if (Player.loot.CanOpenBox)
            {
                SetCanOpenButtons(false);
                return;
            }

            if (Player.loot.index == BoxToOpen.number)
            {
                SetCanSkipButtons();
                return;
            }
            
            if (Player.battlePass.isPremiumBought)
            {
                if (Player.loot.CanOpenNextBox)
                {
                    SetCanOpenButtons(false);
                    return;
                }
            }

            SetCanSkipButtons();
        }

        private void SetCanSkipButtons()
        {
            SkipWaitingButton.gameObject.SetActive(true);
            OpenButton.gameObject.SetActive(false);
            OpenButton.interactable = true;

            if (Player.loot.index == BoxToOpen.number)
            {
                  Timer.SetFinishedTime(BoxToOpen.FinishTime);
                // var price = Loots.PriceToSkip(Timer.GetSecondsToFinish(), BoxToOpen.settings);
                var price = Loots.PriceToSkip(BoxToOpen.PlayerBox.secondsToOpen, BoxToOpen.settings);
                SkipWaitingButtonPriceBehaviour.SetPrice(price);
            }
            else
            {
                Timer.gameObject.SetActive(false);
                uint time;
                if (Player.arenaBoosterTime.IsActive)
				{
                    time = Player.arenaBoosterTime.GetSecondsToOpen(BoxToOpen.BinaryBox.time);
				}
				else
				{
                    time = BoxToOpen.BinaryBox.time;
                }
                var price = Loots.PriceToSkip(time, BoxToOpen.settings);

                SkipWaitingButtonPriceBehaviour.SetPrice(price);
            }


            var lootOpeningOrNext = Player.loot.index == BoxToOpen.number || Player.loot.nextIndex == BoxToOpen.number;
            CommentText.SetActive(!lootOpeningOrNext);
        }

        private void SetCanOpenButtons(bool opened)
        {
            OpenButton.gameObject.SetActive(true);
            OpenButtonImage.sprite = OpenButtonActiveSprite;
            SkipWaitingButton.gameObject.SetActive(false);
            OpenButton.interactable = true;
            CommentText.SetActive(false);
            Timer.gameObject.SetActive(false);

            OpenButtonText.text = Locales.Get(opened ? "locale:1030" : "locale:1036");
        }

        private void ReParentClickedBox()
        {
            switch (parent)
            {
                case MainWindowBehaviour _:
                    ReParentToMainWindow();
                    break;
               /* case ShopWindowBehaviour _:
                    ReParentToShopWindow();
                    break;
                case ArenaWindowBehaviour _:
                    ReParentToArenasWindow();
                    break;*/
                case BattlePassWindowBehaviour _:
                    ReParentToBattlePassWindow();
                    break;
                default:
                    Debug.LogError("Try to open LotBox Window with unhandled behaviour script");
                    break;
            }
            
        }

        private void ReParentToBattlePassWindow()
        {
            if ((parent as BattlePassWindowBehaviour).GetClickedShowBox(out LootBoxViewBehaviour clickedBox))
            {
                BoxToOpen = null;
                LootBoxView = clickedBox;

                LootBoxView = Instantiate(LootBoxView, BoxContainer).GetComponent<LootBoxViewBehaviour>(); //???
                LootBoxView.Init(BoxState.Opening, clickedBox.BinaryData);

                LootBoxView.SetScaleMultiplier(ScaleMultiPlier);
                LootBoxView.SetPopUpLayer(true);

                ushort numbArena = Player.CurrentArena.number;
                numbArena+=1;
                LootDataBehaviour.ResetData();
                LootDataBehaviour.Init(clickedBox.BinaryData, numbArena);
                BattlePass.SetActive(false);
                BattonCont.SetActive(false);
            }
        }

        private void ReParentToMainWindow()
        {
            if (WindowManager.Instance.MainWindow.GetClickedBox(out LootBoxBehaviour ClickedBox))
            {
                BoxToOpen = ClickedBox;
                LootBoxView = Instantiate(BoxToOpen.BoxView.gameObject, BoxContainer).GetComponent<LootBoxViewBehaviour>();
                LootBoxView.Init(BoxState.Opening, BoxToOpen.BinaryBox);
                LootBoxView.SetScaleMultiplier(ScaleMultiPlier);
                LootBoxView.SetPopUpLayer(true);
            }
        }
        
        public void OnBattlePass()
        {
            WindowManager.Instance.ClosePopUp();
            WindowManager.Instance.MainWindow.BattlePass();
        }

        public void MissClick()
        {
            if (!isMissClick) return;
            WindowManager.Instance.ClosePopUp();
        }
    }
}
