using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Legacy.Client {
    public class LootCardPopUpBehaviour : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] RectTransform BubbleRect;
        [SerializeField] GameObject CardPrefab;
        [SerializeField] TMP_Text Count;

        private GameObject card;

        public void Init(BinaryCard binaryCard, ushort cardsCount)
        {
            if (card != null)
            {
                DestroyImmediate(card);
            }
            card = Instantiate(CardPrefab, BubbleRect);
            card.GetComponent<CardViewBehaviour>().Init(binaryCard);
            Count.text = cardsCount.ToString();
        }

        public void On()
        {
            gameObject.SetActive(true);
        }

        public void Off()
        {
            animator.Play("CloseBubble");
        }

        public void BumpAnimationClosed()
        {
            gameObject.SetActive(false);
        }
    }
}
