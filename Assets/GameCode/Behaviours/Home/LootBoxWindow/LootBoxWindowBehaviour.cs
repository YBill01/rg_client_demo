using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static LootBoxBehaviour;

namespace Legacy.Client
{
    public class LootBoxWindowBehaviour : WindowBehaviour
    {
        public UnityEvent OpenedLootWindow;
        public static LootBoxWindowBehaviour Instance;
        [SerializeField] LootGridBehaviour grid;

        [SerializeField] DelayedRewardTargetBehaviour HardTarget;
        [SerializeField] DelayedRewardTargetBehaviour SoftTarget;
        //[SerializeField] DelayedRewardTargetBehaviour ShardsTarget;

        [SerializeField] GameObject LootCountObject;
        [SerializeField] TMP_Text LootCountText;
        [SerializeField] GameObject LootObject;
        [SerializeField] GameObject FinalObject;
        [SerializeField] RectTransform BoxParent;
        [SerializeField] RectTransform LootCardsGridRect;
        [SerializeField] Animator LootContainerAnimator;
        [SerializeField] RectTransform LootCardContainer;
        [SerializeField] RectTransform LootCardParent;
        [SerializeField] Animator GridAnimator;
        [SerializeField] GameObject TapToContinue;
        [SerializeField] AudioClip tapLootBox;
        [SerializeField] AudioClip explosion;

        [SerializeField, Range(1.0f, 3.0f)] float ScaleMultiPlier = 1.0f;

        private List<LootCardBehaviour> LootsList = new List<LootCardBehaviour>();
        private byte LootCardCursor = 0;
        private LootBoxBehaviour BoxToOpen = null;
        private LootBoxViewBehaviour BoxViewToOpen = null;

        private LootCardBehaviour previousCard { get { return LootsList[LootCardCursor - 1]; } }
        private LootCardBehaviour currentCard { get { return LootsList[LootCardCursor]; } }

        private LootSourceType _kostilLootSourceType = LootSourceType.Default;

        public enum LootBoxWindowState : byte
        {
            Start = 0,
            WaitingResult = 1,
            GettingLoot = 2,
            Finished = 3
        }

        public LootBoxWindowState state;
        private bool exploded;
        private bool lastCardScenarioStarted;

        public bool GettingLootComplete { get; private set; }

        public override void Init(Action callback)
        {
            Instance = this;
            state = LootBoxWindowState.Start;
            callback();
        }

        protected override void SelfClose()
        {
            gameObject.SetActive(false);
            ResetWindow();
        }

        private void UnHoldTargets()
        {
            HoldPotentialTargets(0);
        }

        void SwitchToNextState()
        {
            if (state == LootBoxWindowState.Finished)
            {
                UnHoldTargets();
                WindowManager.Instance.Back();
            }
            else if (state == LootBoxWindowState.WaitingResult)
            {
                state = LootBoxWindowState.GettingLoot;
                StartCoroutine(StartLoot());
                LootCardCursor = 0;
            }
            else
            {
                state++;
            }
        }

        IEnumerator StartLoot()
        {
            yield return new WaitForSeconds(0.25f);
            OpenNextLootCard();
            LootCountObject.SetActive(true);
            LootCountText.text = LootLeft.ToString();
            PlayClip(tapLootBox);
            yield return new WaitForSeconds(0.5f);
            LootContainerAnimator.Play("FirstCard");
            TapToContinue.SetActive(true);
        }

        protected override void SelfOpen()
        {
            ReParentClickedBox();
            HoldPotentialTargets();
            SendOpenLootRequest();
            SwitchToNextState();
            gameObject.SetActive(true);
            OpenedLootWindow.Invoke();
        }

        private void HoldPotentialTargets(uint holdValue = 1)
        {
            SoftTarget.SetDelayed(holdValue);
            //ShardsTarget.SetDelayed(holdValue);
            HardTarget.SetDelayed(holdValue);
        }

        private void SendOpenLootRequest()
        {
            if (BoxToOpen != null)
            {
                var em = ClientWorld.Instance.EntityManager;

                var message = new NetworkMessageRaw();
                message.Write((byte)ObserverPlayerMessage.UserCommand);
                message.Write((byte)UserCommandType.LootUpdate);
                message.Write((byte)LootCommandType.Reward);
                message.Write(BoxToOpen.number);

                var messageEntity = em.CreateEntity();
                em.AddComponentData(messageEntity, message);
                em.AddComponentData(messageEntity, default(ObserverMessageTag));
            }
            else if (BoxViewToOpen == null)
            {
                state = LootBoxWindowState.Finished;
            }

            //PushNotifications.Instance.ChestOpeningLocalNotificationCancel();
        }

        private void ReParentClickedBox()
        {
            switch (parent)
            {
                case MainWindowBehaviour _:
                    _kostilLootSourceType = LootSourceType.Battle;
                    ReParentToMainWindow();
                    break;
                case ShopWindowBehaviour _:
                    _kostilLootSourceType = LootSourceType.Shop;
                    ReParentToShopWindow();
                    (parent as ShopWindowBehaviour).SendAnalytic();
                    break;
                case ArenaWindowBehaviour _:
                    _kostilLootSourceType = LootSourceType.ArenaReward;
                    ReParentToArenasWindow();
                    break;
                case BattlePassWindowBehaviour _:
                    _kostilLootSourceType = LootSourceType.BattlePass;
                    ReParentToBattlePassWindow();
                    break;
                default:
                    Debug.LogError("Try to open LotBox Window with unhandled behaviour script");
                    break;
            }
        }

        private void ReParentToMainWindow()
        {
            if ((parent as MainWindowBehaviour).GetClickedBox(out LootBoxBehaviour ClickedBox))
            {
                BoxToOpen = ClickedBox;
                BoxViewToOpen = BoxToOpen.BoxView;
                BoxViewToOpen.GetComponent<RectTransform>().SetParent(BoxParent);
                BoxViewToOpen.SetScaleMultiplier(ScaleMultiPlier);
            }
        }

        private void ReParentToShopWindow()
        {
            if ((parent as ShopWindowBehaviour).GetClickedBox(out LootBoxViewBehaviour clickedBox))
            {
                BoxToOpen = null;
                BoxViewToOpen = clickedBox;
                BoxViewToOpen.SetScaleMultiplier(1.0f);
                BoxViewToOpen.GetComponent<RectTransform>().SetParent(BoxParent);
                BoxViewToOpen.SetScaleMultiplier(ScaleMultiPlier);
            }
        }

        private void ReParentToArenasWindow()
        {
            if ((parent as ArenaWindowBehaviour).GetClickedBox(out LootBoxViewBehaviour clickedBox))
            {
                BoxToOpen = null;
                BoxViewToOpen = clickedBox;
                BoxViewToOpen.GetComponent<RectTransform>().SetParent(BoxParent);
                BoxViewToOpen.SetScaleMultiplier(ScaleMultiPlier);
            }
        }

        private void ReParentToBattlePassWindow()
        {
            if ((parent as BattlePassWindowBehaviour).GetClickedBox(out LootBoxViewBehaviour clickedBox))
            {
                BoxToOpen = null;
                BoxViewToOpen = clickedBox;
                BoxViewToOpen.GetComponent<RectTransform>().SetParent(BoxParent);
                var reset = BoxViewToOpen.gameObject.GetComponent<RectScaleToBehaviour>();
                if (reset)
                {
                    reset.Reset();
                    Debug.Log("Reset RectScaleToBehaviour Scale");
                }
                BoxViewToOpen.SetScaleMultiplier(ScaleMultiPlier);
            }
        }

        public void Tap()
        {
            /*BoxViewToOpen.GetOneLootEffect(currentCard, () =>
            {
                
            });
            return;*/

            switch (state)
            {
                case LootBoxWindowState.GettingLoot:
                    if (LootsList.Count < 1)
                    {
                        SwitchToNextState();
                        return;
                    }

                    if (!currentCard.appeared)
                    {
						if (currentCard.GetRarity() != CardRarity.Legendary)
						{
                            CurrentCardSkipAnimation();
                            return;
                        }
                    }

                    if (LootCardCursor == LootsList.Count - 1)
                    {
                        LastCardScenario();
                        SwitchToNextState();
                    }
                    else
                    {
                        CurrentCardBackToGrid();
                        LootCardCursor++;
                        OpenNextLootCard();
                        PlayClip(tapLootBox);
                        UpdateLootCountText();
                    }

                    break;
                case LootBoxWindowState.Finished:
                    if (exploded && AllLootShown())
                    {
                        StartLootParticles();
                        WindowManager.Instance.Back();
					}
					else
					{
                        GridCardSkipAnimation();
                    }
                    break;
                default:
                    break;
            }
        }

        private void CurrentCardSkipAnimation()
		{
            Animator animator = currentCard.GetComponent<Animator>();
            if (animator != null)
            {
                animator.speed = 3.0f;
            }
			if (!BoxViewToOpen.IsApperEffectStart)
			{
                currentCard.PlayAppearEffect();
            }
            BoxViewToOpen.StopAppear();
        }
        private void GridCardSkipAnimation()
		{
            BoxViewToOpen.SkipExplosion();
            if (!exploded)
			{
                StopCurrentClip();
                PlayClip(explosion, 0.8f);
            }

            exploded = true;
        }

        private bool AllLootShown()
        {
            for (int i = 0; i < LootsList.Count; i++)
            {
                if (!LootsList[i].appearedInGrid)
                    return false;
            }
            return true;
        }

        private void LastCardScenario()
        {
            lastCardScenarioStarted = true;
            Animator animator = currentCard.GetComponent<Animator>();
            if (animator != null)
            {
                animator.speed = 1.0f;
                animator.SetBool("Viewed", true);
                StartCoroutine(ReParentLastCardToGrid());
            }
        }

        IEnumerator ReParentLastCardToGrid()
        {
            LootContainerAnimator.Play("LastCard");
            yield return new WaitForSeconds(0.2f);
            grid.BackToGrid(currentCard);
            ShowFinalLoot();
        }

        public void CurrentCardBackToGrid()
        {
            Animator animator = currentCard.GetComponent<Animator>();
            if (animator != null)
            {
                animator.speed = 1.0f;
                animator.SetBool("Viewed", true);
                StartCoroutine(ReParentLootCardToGrid());
            }
        }

        IEnumerator ReParentLootCardToGrid()
        {
            yield return new WaitForSeconds(0.2f);
            grid.BackToGrid(previousCard);
        }

        void ResetWindow()
        {
            PostEffectsBehaviour.Instance.LootBoxWindowFinish(false);
            exploded = false;
            LootObject.SetActive(true);
            FinalObject.SetActive(false);
            for (int i = 1; i < BoxParent.childCount; i++)
            {
                Destroy(BoxParent.GetChild(i).gameObject);
            }
			foreach (LootCardBehaviour loot in LootsList)
			{
                Destroy(loot.gameObject);
            }
            state = LootBoxWindowState.Start;
            LootsList.Clear();
            LootCardCursor = 0;
            grid.ClearGrid();
            LootCardContainer.gameObject.SetActive(false);
        }

        private void ShowFinalLoot()
        {
            Explosion();
        }

        private void OpenNextLootCard()
        {
            if (!LootCardContainer.gameObject.activeSelf)
            {
                LootCardContainer.gameObject.SetActive(true);

            }

            RectTransform CardObject = currentCard.gameObject.GetComponent<RectTransform>();
            CardObject.SetParent(LootCardParent);


            BoxViewToOpen.GetOneLootEffect(currentCard, () =>
            {
                currentCard.ShowLoot();
            });
        }

        public void StartLootParticles()
        {
            foreach (LootCardBehaviour loot in LootsList)
            {
                loot.DropParticles();
            }
        }

        public enum LootCardType
        {
            Soft,
            Hard,
            Shards,
            Cards,
            Exp,
            HeroExp,
            Rating,
            Star
        }

        public void ShowGrid()
        {
            exploded = true;

            StartCoroutine(GridAppears());
        }

        internal void Bang(PlayerUpdateLootEvent loot)
        {
            AnalyticsManager.Instance.ChestOpen(loot.lootSourceID, _kostilLootSourceType);

            if (state == LootBoxWindowState.WaitingResult)
            {
                if (loot.currency.soft > 0)
                {
                    CreateCard(loot.currency.soft, 0, LootCardType.Soft);
                    SoftTarget.SetDelayed(loot.currency.soft);
                }

                //if (loot.currency.shard > 0)
                //{
                    //CreateCard(loot.currency.shard, 0, LootCardType.Shards);
                    //ShardsTarget.SetDelayed(loot.currency.shard);
                //}

                if (loot.currency.hard > 0)
                {
                    CreateCard(loot.currency.hard, 0, LootCardType.Hard);
                    HardTarget.SetDelayed(loot.currency.hard);
                }

                foreach (var pair in loot.cards)
                {
                    CreateCard(pair.Value, pair.Key);
                }
                SwitchToNextState();
                UpdateLootCountText();
            }
        }

        private byte LootLeft => (byte)(LootsList.Count - (LootCardCursor + 1));

        private void UpdateLootCountText()
        {
            if(LootLeft > 0)
            {
                LootCountText.text = LootLeft.ToString();
                LootCountObject.SetActive(true);
            }
            else
            {
                LootCountObject.SetActive(false);
            }
        }

        private void Explosion()
        {
            BoxViewToOpen.ChangeState(BoxState.Explosion, ShowGrid);
            StartCoroutine(PlaySoundWithDelay());
        }

        private IEnumerator PlaySoundWithDelay()
        {
            yield return new WaitForSeconds(0.5f);
			if (!exploded)
			{
                PlayClip(explosion);
            }
        }

        IEnumerator GridAppears()
        {
            PostEffectsBehaviour.Instance.LootBoxWindowFinish(true);
            FinalObject.SetActive(true);

            ResetAnimationInGrid();

            foreach (LootCardBehaviour LootCard in LootsList)
            {
                LootCard.GetComponent<Animator>().SetBool("PlayGridAnim", true);

                var isUpRow = LootCard.indexInGrid % 2 > 0;
                yield return new WaitForSeconds(0.1f * Convert.ToInt32(isUpRow));
            }
            yield return new WaitForSeconds(1.2f);
            LootObject.SetActive(false);
        }

        private void ResetAnimationInGrid()
        {
            foreach (LootCardBehaviour LootCard in LootsList)
            {
                LootCard.GetComponent<Animator>().SetBool("Moved", true);
                LootCard.GetComponent<Animator>().Play("AppearInGrid");
            }
        }

        private void CreateCard(uint count, ushort index, LootCardType type = LootCardType.Cards)
        {
            LootsList.Add(grid.AddLoot(count, index, type));
        }
        private void PlayClip(AudioClip clip, float time = 0.0f)
        {
            SoundManager.Instance.FadeOutMenuMusic(clip.length);
            GetComponent<AudioSource>().clip = clip;
            GetComponent<AudioSource>().time = time;
            GetComponent<AudioSource>().Play();
        }
        private void StopCurrentClip()
        {
            GetComponent<AudioSource>().Stop();
        }
    }
}
