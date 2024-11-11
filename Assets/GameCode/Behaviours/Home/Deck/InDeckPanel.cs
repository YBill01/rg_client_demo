using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class InDeckPanel : MonoBehaviour
    {
        private const int cardsCountInDeck = 8;
        [SerializeField] private GameObject DeckCardPrefab;
        [SerializeField] private RectTransform DeckGrid;
        [SerializeField, Range(0.0f, 1.0f)] private float scaleInDeck;
        private List<DeckCardBehaviour> DeckCardsObjects = new List<DeckCardBehaviour>();
        private ProfileInstance Profile;
        private DecksWindowBehaviour decksWindow;
       // private GameObject CardObject;
       // private bool typeShow = false;
        public void Init(DecksWindowBehaviour _decksWindow) // создаем 8 карт пустых.
       {
            decksWindow = _decksWindow;
            Profile = ClientWorld.Instance.Profile;
            for (int i = 0; i < cardsCountInDeck; i++)
            {
                GameObject CardObject = Instantiate(DeckCardPrefab, DeckGrid);
                var deckCardObject = CardObject.GetComponent<DeckCardBehaviour>();
                //deckCardObject.OnEmptySlot();
                deckCardObject.DeckWindow = _decksWindow;
                DeckCardsObjects.Add(deckCardObject);
            }
        }
       
        public void Show()
        {
            byte index = 0;
            foreach (ushort cardID in Profile.DecksCollection.In_deck)
            {
                if (cardID > 0)
                {
                    ShowCard(index, cardID);
                    
                }
                index++;
            }

        }
        public void ShowCard(byte index, ushort cardID)
        {
            var card = decksWindow.GetCardPool(cardID);
            if (card)
            {
                InDeckBehaviour inDeckCard = card.GetComponent<InDeckBehaviour>();
                DeckCardsObjects[index].InDeckBehaviour = inDeckCard;
                DeckCardsObjects[index].GetComponent<DeckCardBehaviour>().OnEmptySlot(false);
                DeckCardsObjects[index].GetComponent<DeckCardBehaviour>().SetCardText(card.GetComponent<CardTextDataBehaviour>());
                if (DeckCardsObjects[index].CardRect == null)
                    DeckCardsObjects[index].CardRect = inDeckCard.GetComponent<RectTransform>();
                if (DeckCardsObjects[index].cardView == null)
                    DeckCardsObjects[index].cardView = inDeckCard.GetComponent<CardViewBehaviour>();
                DeckCardsObjects[index].cardView.SetLabelNew();
                DeckCardsObjects[index].GetComponent<DeckCardBehaviour>().SetData(Profile.Inventory.GetCardData(cardID), decksWindow, scaleInDeck, decksWindow.OpenCardWindow);
                DeckCardsObjects[index].UpdateButtonsPanel();

                inDeckCard.GetComponent<RectTransform>().SetParent(DeckCardsObjects[index].GetComponent<RectTransform>());
                DeckCardsObjects[index].InDeckBehaviour.SetPosition(Vector3.zero);
                DeckCardsObjects[index].InDeckBehaviour.SetScale(scaleInDeck);
            }
        }
        public void CardToEmpty(ushort idCard) //освободить слот.
        {
            for (byte i = 0; i < DeckCardsObjects.Count; i++)
            {
                if (DeckCardsObjects[i].InDeckBehaviour != null && DeckCardsObjects[i].binaryCard.index == idCard)
                {
                    Profile.DecksCollection.In_deck[i] = 0;
                   CardToPool(i,false);
                    break;
                }
            }
        }
        public void GetCardToEmpty(ushort idCard)  // занять пустой слот.
        {
            for (byte i = 0; i < DeckCardsObjects.Count; i++)
            {
                if (DeckCardsObjects[i].InDeckBehaviour == null && DeckCardsObjects[i].binaryCard.index == 0)
                {
                    ShowCard(i, idCard);
                    break;
                }
            }
        }
        public void Hide()// очищаем доску.
        {

            for (byte i=0;i< DeckCardsObjects.Count;i++)
            {
                if (DeckCardsObjects[i].InDeckBehaviour == null) continue;
                CardToPool(i);
            }
        }
        private byte indexReparent=0;
        public void GetCardToReparent(ushort idCard, bool inPool) // карта на обмен.
        {
            if (inPool)
            {
                for (byte i = 0; i < DeckCardsObjects.Count; i++)
                {
                    if (DeckCardsObjects[i].InDeckBehaviour != null && DeckCardsObjects[i].binaryCard.index == idCard)
                    {
                        CardToPool(i,false);
                        indexReparent = i;
                        break;
                    }
                }
            }
            else
            {
                ShowCard(indexReparent, idCard);
            }
        }

        private void CardToPool(byte index, bool pos = true)
        {
           var  card = DeckCardsObjects[index];
            GameObject cardGO = card.InDeckBehaviour.gameObject;
            decksWindow.SetCardPool(card.binaryCard.index, cardGO , pos);
            card.OnEmptySlot();
            card.cardView = null;
            card.CardRect = null;
            card.binaryCard = default(BinaryCard);
            card.InDeckBehaviour = null;
        }
    }
}