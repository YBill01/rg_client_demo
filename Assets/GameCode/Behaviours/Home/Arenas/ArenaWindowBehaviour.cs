using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Legacy.Client.UpPanelBehaviour;

namespace Legacy.Client
{
    public class ArenaWindowBehaviour : WindowBehaviour
    {
        [SerializeField]
        private ArenaListBehaviour arenasList;

        [SerializeField]
        [Range(0f, 5f)]
        private float durationNewHeroShow;

        public LootBoxViewBehaviour BoxToOpen;

        private bool openingLootBox;
        private ArenaRewardLootBehaviour lootBoxReward;

        private const float WaitBeforeScrolling = 1.0f;
        private bool isOpenArenaAtFirst = true;

        private ArenaRewardBehaviour clickedReward = null;

        public override void Init(Action callback)
        {
            arenasList.Init();            
            callback();
        }

        public void OpenLootBox(ArenaRewardLootBehaviour reward)
        {
            lootBoxReward = reward;
            BoxToOpen = reward.GetBox();
            openingLootBox = true;
            WindowManager.Instance.OpenWindow(childs_windows[0]);
        }

        public bool GetClickedBox(out LootBoxViewBehaviour clickedBox)
        {
            clickedBox = BoxToOpen;
            return clickedBox != null;
        }

        protected override void SelfClose()
        {
            gameObject.SetActive(false);
        }

        protected override void SelfOpen()
        {
            gameObject.SetActive(true);

            arenasList.SetArenasPositions();

            if (openingLootBox)
            {
                lootBoxReward.ResetAfterOpening();
                openingLootBox = false;
                arenasList.AfterOpenLootBox();
            }
            else
            {
                if (ClientWorld.Instance.Profile.HasSoftTutorialState(SoftTutorial.SoftTutorialState.OpenArena))
                    StartScroll();
            }

            //MenuTutorialPointerBehaviour.CreateOnOpenArenaWIndowEntity();

            if(upPanel==null)
                upPanel = WindowManager.Instance.GetUpPanel();
            upPanel.OnArenaWindow();
        }
        private UpPanelBehaviour upPanel;
        public void SetClickedReward(ArenaRewardBehaviour reward)
        {
            clickedReward = reward;
        }
        public void ClearClickedReward()
        {
            clickedReward = null;
        }
        public void SendClickedReward(PlayerUpdateLootEvent loot)
        {
            if (IsClickedReward())
			{
                clickedReward.ClickReward(loot);
            }
        }
        public bool IsClickedReward()
        {
            return clickedReward != null;
        }

        public void StartScroll()
		{
            StartCoroutine(Scroll());
        }

        public IEnumerator Scroll()
        {
            if (!ClientWorld.Instance.Profile.HasSoftTutorialState(SoftTutorial.SoftTutorialState.OpenArena))
            {                
                SoftTutorialManager.Instance.CompliteTutorial(SoftTutorial.SoftTutorialState.OpenArena);
                WindowManager.Instance.MainWindow.GetLeftContainer.SetActive(true);
            }

            //if we should play new heroes effects
            if (BattleDataContainer.Instance.CheckForNewArena())
            {
                BattleDataContainer.Instance.NewArenaShown();

                var arenaRating = new ArenaListBehaviour.ArenaRating
                {
                    current = (ushort)ClientWorld.Instance.Profile.Rating.current,
                    max = (ushort)ClientWorld.Instance.Profile.Rating.max
                };
                arenasList.SetClickBlockerEnabled(true);

                arenasList.OpenWithoutScrolling();
                if (arenasList.ratingData.rating == 0)
				{
                    yield return new WaitForSeconds(WaitBeforeScrolling);
                }
                arenasList.SetNewRating(arenaRating);
                arenasList.ScrollToNextArena();

                arenasList.SetArenasPositions();

                if (Settings.Instance.Get<ArenaSettings>().RatingBattlefield((ushort)ClientWorld.Instance.Profile.Rating.current, out BinaryBattlefields binaryArena))
                {
                    yield return new WaitForSeconds(binaryArena.heroes.Count * durationNewHeroShow);
                }
            }
            arenasList.ScrollToMyRating();
            arenasList.SetClickBlockerEnabled(false);
            WindowManager.Instance.ShowBack(true);
        }

        internal void OpenArenaInfo()
        {
            throw new NotImplementedException();
        }

    }
}