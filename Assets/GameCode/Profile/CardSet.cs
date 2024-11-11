using Legacy.Database;
using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace Legacy.Client
{

	public class CardSet
	{
		private ushort _heroID = 1;

		private UnityEvent modifyEvent;
		private UnityEvent changeHeroEvent;

		public float AverageMana {
			get
			{
				float sum = 0;
				foreach(var c in _cards)
				{
					Legacy.Database.Cards.Instance.Get(c, out BinaryCard binaryCard);
					sum += binaryCard.manaCost;
				}
				return (float)(sum/_cards.Length);
			}
		}

		public CardSet(ushort[] cards, ushort heroID)
		{
			this._heroID = heroID;
			modifyEvent = new UnityEvent();
			changeHeroEvent = new UnityEvent();
			_cards = cards;
		}

		public CardSet(Inventory inventory, byte setSize = 8)
		{
			setSize = (byte)Mathf.Min(setSize,inventory.AvailableCards.Length);
			modifyEvent = new UnityEvent();
			changeHeroEvent = new UnityEvent();
			_cards = new ushort[setSize];
			int count = 0;
			while (count < _cards.Length)
			{
				int index = UnityEngine.Random.Range(0, inventory.AvailableCards.Length);
				if (inventory.AvailableCards[index].count == 0) continue;
				if (Array.IndexOf(_cards, inventory.AvailableCards[index].index) != -1) continue;
				_cards[count] = inventory.AvailableCards[index].index;
				count++;
			}

			Array.Sort(_cards);
		}

		public void ReplaceCards(ushort ID1, ushort ID2)
		{
			if (!correctReplaceOrder(ID1, ID2, out ushort id1, out ushort id2)) return;
            NetworkMessageHelper.ChangeCardInDeck(World.DefaultGameObjectInjectionWorld.EntityManager, ID1, ID2);
            int DECK_LEN = Cards.Length;
			uint i;
			for (i = 0; i < DECK_LEN; i++)
			{
				ushort currentID = Cards[i];
				if (currentID == id2)
				{
					Cards[i] = id1;
					modifyEvent.Invoke();
					return;
				}
			}
		}

        public bool HasCard(ushort index)
        {
            return Array.IndexOf(Cards, index) != -1;
        }

		public void SetHero(ushort heroID)
		{
			if (this._heroID == heroID) return;
			this._heroID = heroID;
			changeHeroEvent.Invoke();
		}

		private bool correctReplaceOrder(ushort ID1, ushort ID2, out ushort id1, out ushort id2)
		{
			int DECK_LEN = Cards.Length;
			uint i;
			id1 = 0;
			id2 = 0;
			for (i = 0; i < DECK_LEN; i++)
			{
				uint currentID = Cards[i];
				if (currentID == ID1)
				{
					id1 = ID2;
					id2 = ID1;
					return true;
				}
				if (currentID == ID2)
				{
					id1 = ID1;
					id2 = ID2;
					return true;
				}
			}
			return false;
		}

		private ushort[] _cards;

		public ushort[] Cards { get => _cards;}
		public ushort HeroID { get => _heroID; }
		public UnityEvent ModifyEvent { get => modifyEvent; }
		public UnityEvent ChangeHeroEvent { get => changeHeroEvent; }

		public string stringPresetation()
		{
			var result = "";
			foreach (uint c in _cards)
			{
				result += c.ToString() + ",";
			}
			return result;
		}
	}

}