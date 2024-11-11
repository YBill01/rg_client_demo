using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client {
    public class ArenaRewardCardsBehaviour : ArenaBasicRewardBehaviour
    {
        [SerializeField]
        private CardViewBehaviour CardView;

        private BinaryCardsReward _binaryCardsReward;

        private ArenaRewardBehaviour.RewardType _cardsType;
        private ushort rewardIndex;

        internal void Init(BinaryCardsReward binaryCardsReward, ProfileInstance profile, ushort index)
        {
            _binaryCardsReward = binaryCardsReward;

            rewardIndex = index;

            ushort cardId = 0;
            ushort cardCount = 0;

            if (binaryCardsReward.count <= 0)
            {
                _cardsType = ArenaRewardBehaviour.RewardType.RandomRarityCards;

                if (profile.Rating.collectedRewards.ContainsKey(rewardIndex.ToString()))
                {
                    cardId = profile.Rating.collectedRewards[rewardIndex];
				}
				else
				{
                    CardView.Init(binaryCardsReward.rarity_card);
                }

                cardCount = binaryCardsReward.rarity_card.rarity_count;
            }
			else
			{
                _cardsType = ArenaRewardBehaviour.RewardType.Cards;

                cardId = binaryCardsReward.card;
                cardCount = binaryCardsReward.count;
            }

            if (cardId > 0)
            {
                if (Cards.Instance.Get(cardId, out BinaryCard binaryCard))
                {
                    _cardsType = ArenaRewardBehaviour.RewardType.Cards;
                    CardView.Init(binaryCard);
                    SetAmount(cardCount.ToString());
                }
            }
            else
            {
                SetAmount(cardCount.ToString());
            }
        }

        public void SetIconSprite(Sprite sprite)
        {
            CardView.SetIconSprite(sprite);
        }
        public void SetRarityIconSprite(Sprite sprite)
        {
            CardView.SetRarityIconSprite(sprite);
        }

        public ArenaRewardBehaviour.RewardType GetCardsType()
        {
            return _cardsType;
        }

        public BinaryCardsReward GetBinaryCardsReward()
		{
            return _binaryCardsReward;
        }

        public override void SetActiveState()
        {
            base.SetActiveState();
            
            CardView.MakeGray(false);
        }

        public override void SetCompleteState()
        {
            base.SetCompleteState();
            
            CardView.MakeGray(false);
        }

        public override void SetLockedState()
        {
            base.SetLockedState();
            
            //CardView.MakeGray(true);
        }
    }
}
