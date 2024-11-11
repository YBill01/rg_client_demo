using Legacy.Database;
using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace Legacy.Client
{

	public struct ClientCardData
	{
		public ushort index;
		public byte level;
		public ushort count;
		public bool isNew;

		public void SetIsNew(bool value)
        {
     		isNew = value;
        }
        public uint CardsToUpgrade {
            get {
                return Levels.Instance.GetCountToUpgradeCard(index, level, UpgradeCostType.CardsCount);
            }
        }

        public uint ExpToUpgrade
        {
            get
            {
                return Levels.Instance.GetCountToUpgradeCard(index, level, UpgradeCostType.CardsAccountExp);
            }
        }

        public uint SoftToUpgrade
        {
            get
            {
                return Levels.Instance.GetCountToUpgradeCard(index, level, UpgradeCostType.CardsSoft);
            }
        }

        public bool CanUpgrade
        {
            get
            {
                return count >= CardsToUpgrade;
            }
        }
    }

	public class Inventory
	{
        private CardsSettings cardSettings;
		public Inventory()
		{
            cardSettings = Settings.Instance.Get<CardsSettings>();

			_all_cards = new ClientCardData[Cards.Instance.GetEnabledCardsByArena().Count];
            int i = 0;
            foreach (var key in Cards.Instance.GetEnabledCardsByArena().Keys)
            {
				_all_cards[i] = new ClientCardData
				{
					index = key,
					count = 1,
                    level = cardSettings.GetStartLevel(key)
                };
                i++;
            }			
		}

		public ClientCardData GetCardData(ushort index)
		{
			foreach (ClientCardData c in _all_cards)
			{
				if (c.index != index) continue;
				return c;
			}
			return default;
		}

		private UnityEvent allertUpdated = new UnityEvent();
		public UnityEvent AllertUpdated { get => allertUpdated; }
		public void CardNewFlag(ushort index)
		{
			for (int  i=0; i< _all_cards.Length; i++)
			{
				if(_all_cards[i].index == index)
                {
					_all_cards[i].SetIsNew(false);
					AllertUpdated.Invoke();
					break;
				}
			}
			
		}

		public bool UpgradeCard(ushort sid)
		{
			var cData = GetCardData(sid);
			ushort count = (ushort)Levels.Instance.GetCountToUpgradeCard(sid, cData.level, UpgradeCostType.CardsCount);

			for (int i = 0; i < _all_cards.Length; i++)
			{
				ClientCardData ccd = _all_cards[i];
				if (ccd.index != sid) continue;
				ccd.level++;
				ccd.count -= count;
				_all_cards[i] = ccd;
				return true;
			}
			return false;
		}

		public void Read(PlayerProfileInstance player)
		{
			_all_cards = new ClientCardData[player.cards.Count];
			
			var _index = 0;
			player.cards.Each((ushort index, PlayerProfileCard card) => 
			{
				_all_cards[_index++] = new ClientCardData
				{
					count = card.count,
					index = index,
					level = card.level,
					isNew = card.isNew
				};
			});
		}

		

        internal bool HasCard(ushort cardID)
        {
            foreach(ClientCardData data in _all_cards)
            {
                if(data.index == cardID && data.count >= 0 && data.level != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void SortByLevel(ref ushort[] cards)
        {
            Array.Sort(cards,
                delegate (ushort card1, ushort card2) {
                    var cardData1 = GetCardData(card1);
                    var cardData2 = GetCardData(card2);
					var result = cardData2.level - cardData1.level;
					return result != 0 ? result : card2 - card1;
                });
        }

        private ClientCardData[] _all_cards;

		public ClientCardData[] AvailableCards { get => _all_cards; }
		public ushort[] AvailableCardsIndexes { get
			{
				var res = new ushort[_all_cards.Length];
				for (int i = 0; i < res.Length; i++)
					res[i] = _all_cards[i].index;
				return res;
			}}
	}

}