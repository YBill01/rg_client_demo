using System.Collections;
using System.Collections.Generic;
using Legacy.Database;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

namespace Legacy.Client
{
    public class BattlePassCardRewardItemBehaviour : BattlePassRewardItemBasicBehaviour
    {
        [SerializeField]
        private CardViewBehaviour viewBehaviour;
        [SerializeField]
        private Transform cardTransform;
        [SerializeField] 
        private TMP_Text count;

        private Vector3 regularScale;
        private Vector3 currentScale = new Vector3(.42f, .42f, .42f);

        public void Init(BinaryCardsReward cardData)
        {
            if (Cards.Instance.Get(cardData.card, out var currentCard))
            {
                viewBehaviour.Init(currentCard);
                count.text = cardData.count.ToString();
                viewBehaviour.MakeGray(false);
            }
        }

        public override void ScaleToCurrentState()
        {
            regularScale = cardTransform.localScale;
            cardTransform.localScale = currentScale;
        }

        public override void ScaleToRegularState()
        {
            cardTransform.localScale = regularScale;
        }
    }
}