using DG.Tweening;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Legacy.Client.LootBoxWindowBehaviour;

namespace Legacy.Client {
    public class ArenaRewardBehaviour : MonoBehaviour
    {

        [SerializeField]
        private GameObject ReadyEffect;
        [SerializeField]
        private GameObject GetEffect;
        [SerializeField]
        private GameObject CardOpenEffect;
        [SerializeField]
		private Image CardOpenEffectImage;
        [SerializeField]
        private RectTransform RewardTransform;
        [SerializeField]
        private List<TextMeshProUGUI> RewardRating;
        [SerializeField]
        private GameObject CompleteCheck;
        [SerializeField]
        private GameObject ActiveCheck;

        [SerializeField]
        private float activeWidth;
        [SerializeField]
        private float regularWidth;
        [SerializeField]
        private Sprite activeSprite;
        [SerializeField]
        private Sprite regularSprite;

        [SerializeField]
        private GameObject LockedCheck;
        [SerializeField] 
        private LegacyButton rewardButton;
        [SerializeField]
        private ArenaRewardMaterialBehaviour CoinsPrefab;
        [SerializeField]
        private ArenaRewardMaterialBehaviour GemsPrefab;
        [SerializeField]
        private ArenaRewardMaterialBehaviour ShardsPrefab;
        [SerializeField]
        private ArenaRewardCardsBehaviour CardsPrefab;
        [SerializeField]
        private ArenaRewardLootBehaviour LootPrefab;
        
        public RewardType Type;
        

        private ArenaContentBehaviour arenaContent;
        private ProfileInstance Profile;
        private ArenaBasicRewardBehaviour rewardObject;
        private ushort RewardRatingValue;
        private ushort index;
        private BinaryReward _binaryReward;
        private byte NumberInArena;
        public RewardState state;

        public enum RewardType
        {
            Empty = 0,
            Hard = 1,
            Soft = 2,
            Shards = 3,
            Cards = 4,
            LootBox = 5,
            RandomRarityCards = 6
        }

        public enum RewardState
        {
            Locked = 0,
            Active = 1,
            Completed = 2
        }
        
        public void InitNewReward(ushort index, ProfileInstance profile)
        {
            if (Rewards.Instance.Get(index, out BinaryReward binaryReward))
            {
                if (binaryReward.cards.Count > 0)
                {
                    rewardObject = Instantiate(CardsPrefab, transform);
                    ArenaRewardCardsBehaviour arenaRewardCardsBehaviour = rewardObject.GetComponent<ArenaRewardCardsBehaviour>();
                    arenaRewardCardsBehaviour.Init(binaryReward.cards[0], Profile, index);
                    Type = arenaRewardCardsBehaviour.GetCardsType();

                    rewardObject.SetInitialScale();
                }
                else if (binaryReward.soft > 0)
                {
                    Type = RewardType.Soft;
                    rewardObject = Instantiate(CoinsPrefab, transform);
                    rewardObject.GetComponent<ArenaRewardMaterialBehaviour>().Init(binaryReward.soft);
                    rewardObject.SetInitialScale();
                }
                else if (binaryReward.hard > 0)
                {
                    Type = RewardType.Hard;
                    rewardObject = Instantiate(GemsPrefab, transform);
                    rewardObject.GetComponent<ArenaRewardMaterialBehaviour>().Init(binaryReward.hard);
                    rewardObject.SetInitialScale();
                }
                else if (binaryReward.shard > 0)
                {
                    Type = RewardType.Shards;
                    rewardObject = Instantiate(ShardsPrefab, transform);
                    rewardObject.GetComponent<ArenaRewardMaterialBehaviour>().Init(binaryReward.shard);
                    rewardObject.SetInitialScale();
                }
                else if (binaryReward.lootbox > 0)
                {
                    Type = RewardType.LootBox;
                    rewardObject = Instantiate(LootPrefab, transform);
                    rewardObject.GetComponent<ArenaRewardLootBehaviour>().Init(binaryReward.lootbox);
                    rewardObject.SetInitialScale();
                }
            }
        }
        public void Init(ushort index, ushort rating, ushort ArenaIndex, byte RewardNumberInArena, ArenaContentBehaviour ArenaContent, ProfileInstance profile)
        {
            arenaContent = ArenaContent;
            RewardRatingValue = rating;
            NumberInArena = RewardNumberInArena;
            Profile = profile;
            
            for (int i = 0; i < RewardRating.Count; i++)
            {
                RewardRating[i].text = rating.ToString();
            }
            
            rewardObject = null;
            
            if (Rewards.Instance.Get(index, out BinaryReward binaryReward))
            {
                _binaryReward = binaryReward;

                if (binaryReward.cards.Count > 0)
                {
                    rewardObject = Instantiate(CardsPrefab, transform);
                    ArenaRewardCardsBehaviour arenaRewardCardsBehaviour = rewardObject.GetComponent<ArenaRewardCardsBehaviour>();
                    arenaRewardCardsBehaviour.Init(binaryReward.cards[0], Profile, index);
                    Type = arenaRewardCardsBehaviour.GetCardsType();

                    rewardObject.SetInitialScale();
                }
                else if (binaryReward.soft > 0)
                {
                    Type = RewardType.Soft;
                    rewardObject = Instantiate(CoinsPrefab, transform);
                    rewardObject.GetComponent<ArenaRewardMaterialBehaviour>().Init(binaryReward.soft);
                    rewardObject.SetInitialScale();
                } 
                else if (binaryReward.hard > 0)
                {
                    Type = RewardType.Hard;
                    rewardObject = Instantiate(GemsPrefab, transform);
                    rewardObject.GetComponent<ArenaRewardMaterialBehaviour>().Init(binaryReward.hard);
                    rewardObject.SetInitialScale();
                } 
                else if (binaryReward.shard > 0)
                {
                    Type = RewardType.Shards;
                    rewardObject = Instantiate(ShardsPrefab, transform);
                    rewardObject.GetComponent<ArenaRewardMaterialBehaviour>().Init(binaryReward.shard);
                    rewardObject.SetInitialScale();
                } 
                else if (binaryReward.lootbox > 0)
                {
                    Type = RewardType.LootBox;
                    rewardObject = Instantiate(LootPrefab, transform);
                    rewardObject.GetComponent<ArenaRewardLootBehaviour>().Init(binaryReward.lootbox);
                    rewardObject.SetInitialScale();
                }
                
                if (rewardObject != null)
                {
                    rewardObject.GetComponent<RectTransform>().SetAsFirstSibling();
                }
            }

            if (Profile.Rating.HasReward(ArenaIndex, NumberInArena))
                Completed();
            else
                UpdateState();
        }
        
        private void Completed()
        {
            ReadyEffect.SetActive(false);
            GetEffect?.SetActive(false);
            state = RewardState.Completed;
            CompleteCheck.SetActive(true);
            LockedCheck.SetActive(false);
            rewardButton.interactable = false;
            
            if (rewardObject != null)
                rewardObject.SetCompleteState();
        }

        private void UpdateState(bool isStartEnd = false)
        {
            if (arenaContent != null && arenaContent.IsLocked)
			{
                Locked(isStartEnd);

                return;
			}

            if(RewardRatingValue > Profile.Rating.max)
            {
                Locked(isStartEnd);
            }
            else
            { 
                Active(isStartEnd);
            }
        }

        private void Locked(bool isStartEnd = false)
        {
            state = RewardState.Locked;
            rewardButton.interactable = false;
			var rect = ActiveCheck.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(regularWidth, rect.sizeDelta.y);
            ActiveCheck.GetComponent<Image>().sprite = regularSprite;
            rewardButton.isLocked = true;

            if (arenaContent != null && arenaContent.IsLocked)
			{
                rewardButton.localeAlert = Locales.Get("locale:1291", RewardRatingValue);
            }
			else
			{
                rewardButton.localeAlert = Locales.Get("locale:751", RewardRatingValue);
            }

            if (rewardObject != null)
                rewardObject.SetLockedState();
        }

        private void Active(bool isStartEnd = false)
        {
            state = RewardState.Active;
            ActiveCheck.SetActive(true);
            if (!isStartEnd)
            {
                ActiveCheck.GetComponent<Image>().sprite = activeSprite;
                var rect = ActiveCheck.GetComponent<RectTransform>();
                rect.DOScale(1.1f, 0f);
                rect.sizeDelta = new Vector2(activeWidth, rect.sizeDelta.y);
                var blink = ActiveCheck.GetComponent<BlickControl>();
                if (blink)
                {
                    blink.Enable();
                }
                RewardRating[0].text = Locales.Get("locale:1363");
                rewardButton.interactable = true;
                ReadyEffect.SetActive(true);
            }
            else
            {
                rewardButton.interactable = false;
                var rect = ActiveCheck.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(regularWidth, rect.sizeDelta.y);
                ActiveCheck.GetComponent<Image>().sprite = regularSprite;
            }

            if (rewardObject != null)
                rewardObject.SetActiveState();
        }

        public void Click()
        {
            if (Type == RewardType.RandomRarityCards)
			{
                arenaContent.SetClickedReward(this);
                arenaContent.TakeArenaReward(NumberInArena);
			}
			else
			{
                arenaContent.TakeArenaReward(NumberInArena);
                arenaContent.ClickReward(NumberInArena, RewardTransform.position);

                if (state == RewardState.Completed)
                {
                    GetEffect.SetActive(true);
                }
            }
		}

        public void ClickReward(PlayerUpdateLootEvent loot)
        {
            StopAllCoroutines();

            //Cards.Instance.Get(loot.cards[0].Key, out BinaryCard binaryCard);

            ushort cardId = 0;
            foreach (var pair in loot.cards)
            {
                cardId = pair.Key;
            }

            if (Cards.Instance.Get(cardId, out BinaryCard binaryCard))
            {
                //VisualContent.Instance.CardIconsAtlas.GetSprite(binaryCard.icon);

                ArenaRewardCardsBehaviour arenaRewardCardsBehaviour = rewardObject.GetComponent<ArenaRewardCardsBehaviour>();
                arenaRewardCardsBehaviour.SetRarityIconSprite(VisualContent.Instance.CardIconsAtlas.GetSprite(binaryCard.icon));

                //CardOpenEffectImage.sprite = VisualContent.Instance.CardIconsAtlas.GetSprite(binaryCard.icon);
                
            }

            rewardObject.gameObject.SetActive(false);
            CardOpenEffect.SetActive(true);

            StartCoroutine(ClickWaitReward1());
        }

        private IEnumerator ClickWaitReward1()
        {
            yield return new WaitForSeconds(1.4f);

            rewardObject.gameObject.SetActive(true);

            if (state == RewardState.Completed)
            {
                GetEffect.SetActive(true);
            }

            StartCoroutine(ClickWaitReward2());
        }
        private IEnumerator ClickWaitReward2()
        {
            yield return new WaitForSeconds(1.8f);

            CardOpenEffect.SetActive(false);

			arenaContent.ClickReward(NumberInArena, RewardTransform.position);
		}

        public ushort GetNumberInArena()
        {
            return NumberInArena;
        }

        public RectTransform GetRect()
        {
            return RewardTransform;
        }

        public void SetComplete()
        {
            Completed();
        }
        
        public ArenaBasicRewardBehaviour GetRewardObject()
        {
            return rewardObject;
        }

        public void SetStartEndEventRating(ushort rating)
        {
            RewardRatingValue = rating;
            Profile = ClientWorld.Instance.Profile;
            UpdateState(true);
            
            for (int i = 0; i < RewardRating.Count; i++)
            {
                RewardRating[i].text = rating.ToString();
            }
        }

        public void SetPosition(float panelPosition)
        {
            rewardObject.SetInitialScale();
            rewardObject.SetPosition(panelPosition - RewardTransform.anchoredPosition.x);
        }
    }
}