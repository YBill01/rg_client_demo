using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class InFoundDeckPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform FoundedGrid;
        [SerializeField] private GameObject FoundedCardPrefab;
        [SerializeField, Range(0.0f, 1.0f)] private float scaleInDeck;
        [SerializeField] private TextMeshProUGUI SortText;

        private List<DeckCardBehaviour> DeckCardsObjects = new List<DeckCardBehaviour>();
        private DecksWindowBehaviour decksWindow;
        private ProfileInstance Profile;

        public void Init(DecksWindowBehaviour _decksWindow) // создаем 8 карт пустых.
        {
            if (Profile == null)
            {
                Profile = ClientWorld.Instance.Profile;
            }
            Profile.DecksCollection.SortChangeEvent.AddListener(delegate { ChangeSort(); });
            decksWindow = _decksWindow;

        }

        public void SortClick()
        {
            Profile.DecksCollection.NextSort();
        }

        public void Show()
        {
            ushort index = 0;
            foreach (ushort cardID in Profile.DecksCollection.In_collection)
            {
                if(index < DeckCardsObjects.Count)
                {
                    DeckCardsObjects[index].gameObject.SetActive(true);
                    CreateCard(cardID, index);
                }
                else
                {
                    CreateEmpty();
                    if (index < DeckCardsObjects.Count)
                        CreateCard(cardID, index);
                }

                index++;
            }
        }

        private void CreateEmpty()
        {
            GameObject CardObject = Instantiate(FoundedCardPrefab, FoundedGrid);
            var deckCardObject = CardObject.GetComponent<DeckCardBehaviour>();
          //  deckCardObject.OnEmptySlot();
            deckCardObject.DeckWindow = decksWindow;
            DeckCardsObjects.Add(deckCardObject);
        }

        private void CreateCard(ushort cardID, ushort index)
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
      /*  public void ShowCard(byte index, ushort cardID)
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
        }*/
        public void Hide() //  очищаем перед сменой кард.
        {
            for(byte i = 0;i < DeckCardsObjects.Count;i++)
            {
                if (DeckCardsObjects[i].InDeckBehaviour != null)
                {
                    CardToPool(i, true, true);
                }
                else
                {
                    DeckCardsObjects[i].gameObject.SetActive(false);
                }
            }
        }
        private void CardToPool(byte index, bool pos = true, bool isHide = false)
        {
            var card = DeckCardsObjects[index];
            GameObject cardGO = card.InDeckBehaviour.gameObject;
            decksWindow.SetCardPool(card.binaryCard.index, cardGO, pos);
            card.OnEmptySlot();
            card.cardView = null;
            card.CardRect = null;
            card.binaryCard = default(BinaryCard);
            card.InDeckBehaviour = null;
            if(isHide)
                DeckCardsObjects[index].gameObject.SetActive(false);
        }
        private ushort indexReparent = 0;
        public void GetCardToReparent(ushort idCard,bool inPool) // карта на обмен.
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
                CreateCard(idCard, indexReparent);
                DeckCardsObjects[indexReparent].gameObject.SetActive(true);
            }
        }
        public void GetCardToEmpty(ushort idCard)  //текущую карду поместить в пустой слот на доску
        {
            for (byte i = 0; i < DeckCardsObjects.Count; i++)
            {
                if (DeckCardsObjects[i].InDeckBehaviour != null && DeckCardsObjects[i].binaryCard.index == idCard)
                {
                    CardToPool(i,false,true);
                    break;
                }
            }
        }
        public void EmptyToCard(ushort idCard)  //текущую карду поместить в пустой слот на доску
        {
            bool isFind = false;
            for (byte i = 0; i < DeckCardsObjects.Count; i++)
            {
                if (DeckCardsObjects[i].InDeckBehaviour == null )
                {
                    DeckCardsObjects[i].gameObject.SetActive(true);
                    CreateCard(idCard, i);
                    isFind = true;
                    break;
                }
            }
            if (!isFind)
            {
                CreateEmpty();
                CreateCard(idCard, (ushort)(DeckCardsObjects.Count-1));
            }
        }
        private void ChangeSort()
        {
            byte i = 0;
            for ( i = 0; i < DeckCardsObjects.Count; i++)
            {
                if (DeckCardsObjects[i].InDeckBehaviour != null && DeckCardsObjects[i].binaryCard.index >0)
                {
                    CardToPool(i, false);
                }
            }
             i = 0;
              foreach (ushort cardID in Profile.DecksCollection.In_collection)
              {
                CreateCard(cardID, i);
                i++;
              }
     
            SortText.text = Profile.DecksCollection.CurrentSortName;
        }

    }

}