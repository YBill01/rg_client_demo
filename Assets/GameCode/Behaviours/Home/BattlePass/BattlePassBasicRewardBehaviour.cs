using System;
using System.Collections.Generic;
using Legacy.Database;
using UnityEngine;
using static Legacy.Client.LootBoxWindowBehaviour;

namespace Legacy.Client
{
    public class BattlePassBasicRewardBehaviour : MonoBehaviour
    {
        public event Action TimerUp;

        public enum BattlePassRewardType
        {
            Empty,
            Soft,
            Hard,
            Shards,
            Cards,
            LootBox
        }

        [SerializeField] 
        protected GameObject lockedState;
        [SerializeField] 
        protected GameObject currentState;
        [SerializeField] 
        protected GameObject collectedState;
        [SerializeField]
        protected GameObject CollectEffect;
        [SerializeField] 
        protected LegacyButton button;
        [SerializeField] 
        private GameObject checkImage;
        [SerializeField] 
        protected UITimerBehaviour timer;
        [SerializeField] 
        private Transform rewardHolder;
        [SerializeField] 
        private Transform rewardForScale;
        [SerializeField] 
        private Animator rewardAnim;
        [SerializeField] 
        private BattlePassCardRewardItemBehaviour cardRewardPrefab;
        [SerializeField] 
        private BattlePassLootBoxItemBehaviour lootBoxRewardPrefab;
        [SerializeField] 
        private BattlePassCurrencyRewardItemBehaviour coinsRewardPrefab;
        [SerializeField] 
        private BattlePassCurrencyRewardItemBehaviour gemsRewardPrefab;
        [SerializeField] 
        private BattlePassCurrencyRewardItemBehaviour shardsRewardPrefab;

        private BattlePassRewardItemBasicBehaviour currentItem;
        public BattlePassRewardState rewardState;
        private DateTime levelTime = DateTime.Now;
        private Vector3 defaultScale;
        private bool isScaling;
        private BattlePassRewardType _type = BattlePassRewardType.Empty;
        private BattlePassLootBoxItemBehaviour lootBoxItemBehaviour;

        private BinaryReward binaryReward = default;
        private bool isPremiumReward;
        private ProfileInstance profile;
        private BattlePassWindowBehaviour battlePassWindow;
        protected BattlePassLevelBehaviour battlePassLevel;

        internal void SetCollected()
        {
            rewardState = BattlePassRewardState.Collected;
            SwitchState();
        }

        protected void UpdateSwitchState()
        {
            SwitchState();
        }

        private void OnEnable()
        {
            SwitchState();
        }
        public void Init(ushort index, BattlePassRewardState state, DateTime timeForLevel, bool premiumReward, BattlePassWindowBehaviour bpw , BattlePassLevelBehaviour bpsl)
        {
            battlePassWindow = bpw;
            battlePassLevel = bpsl;
            profile = ClientWorld.Instance.Profile;

            rewardState = state;
            levelTime = timeForLevel;
            isPremiumReward = premiumReward;

            if (Rewards.Instance.Get(index, out BinaryReward reward))
            {
                binaryReward = reward;
                if (reward.soft > 0)
                {
                    _type = BattlePassRewardType.Soft;
                    InstantiateCurrencyItem(coinsRewardPrefab, reward.soft);
                }
                else if (reward.hard > 0)
                {
                    _type = BattlePassRewardType.Hard;
                    InstantiateCurrencyItem(gemsRewardPrefab, reward.hard);
                }
                else if (reward.shard > 0)
                {
                    _type = BattlePassRewardType.Shards;
                    InstantiateCurrencyItem(shardsRewardPrefab, reward.shard);
                }
                else if (reward.lootbox > 0)
                {
                    _type = BattlePassRewardType.LootBox;
                    InstantiateLootBoxItem(reward.lootbox);
                }
                else if (reward.cards != null)
                {
                    _type = BattlePassRewardType.Cards;
                    InstantiateCardItem(reward.cards);
                }
            }
            
            SwitchState();
        }


        protected virtual void ShowLockedLootBox()
        {
            battlePassLevel.onFreeShow();
        }
        private void SwitchState()
        {
            CollectEffect.SetActive(false);
 
            void Locked()
            {
                button.interactable = false;
                SetLockedState();
                timer.gameObject.SetActive(false);
                EnableLockedAlert();
            }

            void EnableLockedAlert()
            {
                button.isLocked = true;
                if (_type == BattlePassRewardType.LootBox)
                {
                    button.LoockedOnClick = ShowLockedLootBox;
                }
                if (isPremiumReward && !profile.battlePass.isPremiumBought)
                {
                    if (battlePassWindow.isTutor)
                    {
                        button.localeAlert = Locales.Get("locale:1483");
                    }
                    else
                    {
                        button.localeAlert = Locales.Get("locale:883");
                    }
                }
                else
                {
                    if (battlePassWindow.isTutor)
                    {
                        button.localeAlert = Locales.Get("locale:1483");
                    }
                    else
                    {
                        button.localeAlert = Locales.Get("locale:880");
                    }
                }
            }

            switch (rewardState)
            {
                case BattlePassRewardState.Locked:
                    Locked();
                    break;
                case BattlePassRewardState.Collected:
                    button.interactable = false;
                    EnableBoughtCheck();
                    SetCollectedState();
                    timer.gameObject.SetActive(false);
                    currentItem.ScaleToCollectedState();
                    break;
                case BattlePassRewardState.CurrentCollected:
                    button.interactable = false;
                    EnableBoughtCheck();
                    timer.gameObject.SetActive(false);
                    currentItem.ScaleToCurrentState();
                    SetCurrentState();
                    break;
                case BattlePassRewardState.Active:
                    button.interactable = true;
                    button.isLocked = false;
                    timer.gameObject.SetActive(false);
                    CollectEffect.SetActive(true);
                    rewardAnim.SetBool("Collectable", true);
                    break;
                case BattlePassRewardState.LockedPremium:
                    Locked();
                    if (isPremiumReward && ClientWorld.Instance.Profile.battlePass.isPremiumBought)
                    {
                        rewardState = BattlePassRewardState.Active;
                        if (lootBoxItemBehaviour != null) lootBoxItemBehaviour.SetAvailableState();
                        SwitchState();
                    }
                    break;
                case BattlePassRewardState.ActiveTimer:
                    EnableLockedAlert();
                    timer.gameObject.SetActive(true);
                    button.interactable = false;    
                    StartTimer(levelTime);
                    break;
                case BattlePassRewardState.CurrentTimer:
                    EnableLockedAlert();
                    timer.gameObject.SetActive(true);
                    button.interactable = false;
                    StartTimer(levelTime);
                    currentItem.ScaleToCurrentState();
                    SetCurrentState();
                    break;
                case BattlePassRewardState.Current:
                    timer.gameObject.SetActive(false);
                    currentItem.ScaleToCurrentState();
                    SetCurrentState();                    
                    break;
            }
        }

        internal void DropRewardParticles()
        {
            if(_type != BattlePassRewardType.Empty)
            {
                switch (_type)  
                {
                    case BattlePassRewardType.Soft:
                        RewardParticlesBehaviour.Instance.Drop(rewardForScale.position, (byte)(binaryReward.soft / 50), LootCardType.Soft);
                        break;
                    case BattlePassRewardType.Hard:
                        RewardParticlesBehaviour.Instance.Drop(rewardForScale.position, (byte)(binaryReward.hard / 10), LootCardType.Hard);
                        break;
                    case BattlePassRewardType.Shards:
                        RewardParticlesBehaviour.Instance.Drop(rewardForScale.position, (byte)(binaryReward.shard / 5), LootCardType.Shards);
                        break;
                    case BattlePassRewardType.Cards:
                        RewardParticlesBehaviour.Instance.Drop(rewardForScale.position, 5, LootCardType.Cards);
                        break;
                    default:
                        break;
                }
            }
        }

        private void InstantiateCurrencyItem(BattlePassCurrencyRewardItemBehaviour prefab, ushort amount)
        {
            var item = Instantiate(prefab, rewardHolder);
            item.Init(amount);
            currentItem = item;
        }

        private void InstantiateLootBoxItem(ushort index)
        {
            lootBoxItemBehaviour = Instantiate(lootBoxRewardPrefab, rewardHolder);
            var item = lootBoxItemBehaviour;
            item.Init(index);
            currentItem = item;

            switch (rewardState)
            {
                case BattlePassRewardState.Locked:
                    item.SetClosedState();
                    break;
                case BattlePassRewardState.Collected:
                    item.SetCollectedState();
                    break;
                case BattlePassRewardState.CurrentCollected:
                    item.SetCollectedState();
                    break;
                case BattlePassRewardState.Active:
                    item.SetAvailableState();
                    break;
                case BattlePassRewardState.ActiveTimer:
                    item.SetClosedState();
                    break;
                case BattlePassRewardState.CurrentTimer:
                    item.SetClosedState();
                    break;
                case BattlePassRewardState.Current:
                    item.SetAvailableState();
                    break;
            }
        }

        private void InstantiateCardItem(List<BinaryCardsReward> cards)
        {
            var item = Instantiate(cardRewardPrefab, rewardHolder);
            item.Init(cards[0]);
            currentItem = item;
        }

        private void StartTimer(DateTime endTime)
        {
            timer.SetFinishedTime(endTime);
            timer.TimerFinished.AddListener(OnTimerFinished);
        }

        private void OnTimerFinished()
        {
            timer.TimerFinished.RemoveListener(OnTimerFinished);
            TimerUp?.Invoke();
            
            switch (rewardState)
            {
                case BattlePassRewardState.ActiveTimer:
                    rewardState = BattlePassRewardState.Active;
                    break;
                case BattlePassRewardState.CurrentTimer:
                    rewardState = BattlePassRewardState.Current;
                    break;
            }
            
            button.interactable = true;
        }

        private void EnableBoughtCheck()
        {
            button.interactable = false;
            checkImage.SetActive(true);
            rewardAnim.SetBool("Collectable", false);
        }

        protected virtual void SetCollectedState()
        {
            //lockedState.SetActive(false);
            currentState.SetActive(false);
            collectedState.SetActive(false);
            rewardAnim.SetBool("Collectable", false);
            CollectEffect.SetActive(false);
        }

        protected virtual void SetLockedState()
        {
            lockedState.SetActive(true);
            currentState.SetActive(false);
            collectedState.SetActive(false);
            rewardAnim.SetBool("Collectable", false);
            CollectEffect.SetActive(false);
        }

        protected virtual void SetCurrentState()
        {
            lockedState.SetActive(false);
            currentState.SetActive(true);
            collectedState.SetActive(false);
        }

        public BattlePassRewardItemBasicBehaviour GetItem()
        {
            return currentItem;
        }

        public void ChangeToPreviousState()
        {
            if (rewardState == BattlePassRewardState.CurrentCollected)
                rewardState = BattlePassRewardState.Collected;
            else
                rewardState = BattlePassRewardState.Active;
            
            SwitchState();
        }

        public void ChangeToPreviousStateWithTimer(DateTime timerTime)
        {
            rewardState = BattlePassRewardState.ActiveTimer;
            levelTime = timerTime;
            SwitchState();
        }

        public void ChangeToCurrentState()
        {
            rewardState = BattlePassRewardState.Current;
            SwitchState();
        }

        public void ChangeToCurrentStateWithTimer(DateTime timerTime)
        {
            rewardState = BattlePassRewardState.CurrentTimer;
            levelTime = timerTime;
            SwitchState();
        }

        public void SetInitialScale()
        {
            rewardAnim.enabled = false;
            rewardForScale.localScale = Vector3.zero;
        }

        public void SetInitialNoAnimation()
        {
            rewardAnim.enabled = false;
            rewardForScale.localScale = Vector3.one;
            //rewardAnim.SetBool("Idle", false);
        }

        public void PlayAnimationOnFirstAppearance()
        {
            rewardAnim.enabled = true;
        }

        private void OnDestroy()
        {
            timer.TimerFinished.RemoveListener(OnTimerFinished);
            SwitchState();
        }
    }
}