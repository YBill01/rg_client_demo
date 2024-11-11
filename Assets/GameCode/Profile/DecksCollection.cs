using Legacy.Database;
using Legacy.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace Legacy.Client
{    
    public class DecksCollection
    {
        private CardSet[] _cardSets;
        private CardSet _activeSet;
        private int _active_set_id;
        private Inventory inventory;
        private ushort[] _in_collection;
        private ushort[] _unavailable;
        private ushort[] _not_found;
        private ushort[] _in_deck;
        private CardSortType _currentSort;

        public Inventory Inventory { get => inventory; }
		public CardSet ActiveSet { get => _activeSet; }
		public int Active_set_id { get => _active_set_id; }
		public ushort[] In_deck { get => _in_deck; }
		public ushort[] In_collection { get => _in_collection; }
		public ushort[] Unavailable { get => _unavailable; }
		public ushort[] NotFound { get => _not_found; }
		public CardSet[] CardSets { get => _cardSets; }
		public UnityEvent DeckChangeEvent { get => deckChangeEvent; }
		public UnityEvent SortChangeEvent { get => sortChangeEvent; }
        public List<ushort> AvailableCards { get; private set; }
        public CardSortType CurrentSort { get => _currentSort; }

        public bool IsFullDesc()
        {
            bool isZero = true;
            for (byte k = 0; k < In_deck.Length; k++)
            {
                if (In_deck[k] == 0)
                {
                    isZero = false;
                    break;
                }
            }
            return isZero;
        }
        public string CurrentSortName 
        {
            get 
            {
				switch (CurrentSort)
				{
					case CardSortType.Level:
                        return Locales.Get("locale:955");
					case CardSortType.Arena:
                        return Locales.Get("locale:958");
                    case CardSortType.EtherUp:
                        return Locales.Get("locale:961");
                    case CardSortType.EtherDown:
                        return Locales.Get("locale:1486");
                    case CardSortType.RarityUp:
                        return Locales.Get("locale:964");
                    case CardSortType.RarityDown:
                        return Locales.Get("locale:967");
                    default:
                        return Locales.Get("locale:955");
                };
			}
		}

		private UnityEvent deckChangeEvent;
        private UnityEvent sortChangeEvent;
		public DecksCollection(Inventory inventory)
        {

            uint SetsCount = 5;
			this.deckChangeEvent = new UnityEvent();
			this.sortChangeEvent = new UnityEvent();
			_cardSets = new CardSet[SetsCount];
			this.inventory = inventory;
            for (int i = 0; i < SetsCount; i++)
                _cardSets[i] = new CardSet(inventory);


        }

        public void ChooseSet(byte ID)
        {
            if (ID != _active_set_id)
                NetworkMessageHelper.ChangeDeck(ID);

            _active_set_id = ID;
            UpdateSet();			
        }
		private void OnDeckModify()
		{
			RebuildCollection();
		}

        public void UpdateSet()
        {
            _activeSet = _cardSets[_active_set_id];

            _activeSet.ModifyEvent.AddListener(OnDeckModify);

            RebuildCollection();
        }

        internal void RebuildCollection()
        {
            UpdateCollectionList();            
        }

        public void SortByCurrentMethod(ref ushort[] cards)
        {            
            switch (_currentSort)
            {
                case CardSortType.EtherUp: SortByEther(ref cards, true); break;
                case CardSortType.EtherDown: SortByEther(ref cards, false); break;
                case CardSortType.Level: inventory.SortByLevel(ref cards); break;
                case CardSortType.Arena: SortByArena(ref cards); break;
                case CardSortType.RarityDown: SortByRarity(ref cards, false); break;
                case CardSortType.RarityUp: SortByRarity(ref cards, true); break;
            }
        }

        private void SortByArena(ref ushort[] cards)
        {
            Array.Sort(cards,
                delegate (ushort card1, ushort card2) {
                    Cards.Instance.GetEnabledCardsByArena().TryGetValue(card1, out byte arena1);
                    Cards.Instance.GetEnabledCardsByArena().TryGetValue(card2, out byte arena2);
                    var result = arena1 - arena2;
                    return result != 0 ? result : card2 - card1;
                });
        }

        private void SortByRarity(ref ushort[] cards, bool asc)
        {
            Array.Sort(cards,
                delegate (ushort card1, ushort card2) {
                    Cards.Instance.Get(card1, out BinaryCard bc1);
                    Cards.Instance.Get(card2, out BinaryCard bc2);
                    var result = (byte)bc1.rarity - (byte)bc2.rarity;
                    return (asc ? 1 : -1) * (result != 0 ? result : card2 - card1);
                });
        }

        private void SortByEther(ref ushort[] cards, bool asc)
        {
            Array.Sort(cards,
                delegate (ushort card1, ushort card2) {
                    Cards.Instance.Get(card1, out BinaryCard bcard1);
                    Cards.Instance.Get(card2, out BinaryCard bcard2);
                    var result = bcard1.manaCost - bcard2.manaCost;
                    return (asc ? 1 : -1) * (result != 0 ? result : card2 - card1);
                });
        }

        public void NextSort()
		{
            var values = Enum.GetValues(typeof(CardSortType));
            int preID = (int)_currentSort;
            int finalID;
            finalID = (preID + 1) % (values.Length);
            NetworkMessageHelper.NextSort((byte)finalID);
            SetSort((byte)finalID);
            SortByCurrentMethod(ref _in_collection);
            sortChangeEvent.Invoke();
        }

        public void UpdateAvailable(byte number)
        {
            AvailableCards = Legacy.Database.Settings.Instance.Get<ArenaSettings>().GetAvailableCards(number);
        }

        private void UpdateCollectionList()
		{
            List<ushort> indeck = new List<ushort>();
            List<ushort> inlist = new List<ushort>();
			List<ushort> not_found = new List<ushort>();
			List<ushort> unavailable = new List<ushort>();

            foreach (var pair in Cards.Instance.GetEnabledCardsByArena().OrderBy(key => key.Value))
			{
                ushort cardID = pair.Key;			

                if (IsOpenedCard(cardID))
                {
                    if (Inventory.HasCard(cardID))
                    {
                        if (Array.IndexOf(_activeSet.Cards, cardID) == -1)
                        {
                            inlist.Add(cardID);
                            continue;
                        }
                    }
                    else
                    {
                        not_found.Add(cardID);
                    }
                }
                else
                {
                    unavailable.Add(cardID);
                }
			}
            _in_collection = inlist.ToArray();
			_unavailable = unavailable.ToArray();
			_not_found = not_found.ToArray();
            _in_deck = _activeSet.Cards;
            SortByCurrentMethod(ref _in_collection);
            deckChangeEvent.Invoke();
        }

        private bool IsOpenedCard(ushort cardID)
        {
#if UNITY_EDITOR
            return true;  // доступны все карты из инветаря.
#endif
            return AvailableCards.Contains(cardID);
        }

        internal bool IsCardInDeck(ushort index, byte deckIndex = 0)
        {
            if (deckIndex == 0)
            {
                deckIndex = (byte)Active_set_id;
            }
            return _cardSets[deckIndex].HasCard(index);
        }

        public void ResetDecks(DatabaseList<PlayerProfileDeck> playerProfileDecks)
		{
            int decksCount = playerProfileDecks.Count;
			_cardSets = new CardSet[decksCount];
			for (int i = 0; i < decksCount; i++)
				_cardSets[i] = new CardSet(playerProfileDecks[i].list.ToArray(), playerProfileDecks[i].heroID);
		}

        internal void SetSort(byte sort)
        {
            _currentSort = (CardSortType)sort;
        }
	}
}