using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Legacy.Client
{
    public class ArenaNewCardsBehaviour : MonoBehaviour
    {
        [SerializeField] private CardViewBehaviour CardPrefab;
        [SerializeField] private Transform FirstRow;
        [SerializeField] private Transform SecondRow;

        private List<CardViewBehaviour> newCards;

        internal void Init(List<ushort> cards)
        {
            newCards = new List<CardViewBehaviour>();
            var sortedCards = GetSortedCards(cards);
            var countInHalf = sortedCards.Count / 2;
            var firstRowCount = Mathf.FloorToInt(countInHalf);
            InstantiateInRange(0, firstRowCount, FirstRow, sortedCards);
            InstantiateInRange(firstRowCount, sortedCards.Count, SecondRow, sortedCards);
            gameObject.SetActive(true);
        }

        private List<BinaryCard> GetSortedCards(List<ushort> cards)
        {
            var binaryCards = new List<BinaryCard>();

            for (byte i = 0; i < cards.Count; i++)
            {
                if (Cards.Instance.Get(cards[i], out BinaryCard binaryCard))
                {
                    binaryCards.Add(binaryCard);
                }
            }

            var sortedCards = binaryCards.OrderBy(x => x.rarity).ToList();

            return sortedCards;
        }

        private void InstantiateInRange(int start, int end, Transform parent, List<BinaryCard> cards)
        {
            for (int i = start; i < end; i++)
            {
                var card = Instantiate(CardPrefab, parent);
                card.Init(cards[i]);
                newCards.Add(card);
            }
        }

        public void SetNewCardsState(bool isGrayedOut)
        {
            for (int i = 0; i < newCards.Count; i++)
            {
                newCards[i].MakeGray(isGrayedOut);
            }
        }
    }
}