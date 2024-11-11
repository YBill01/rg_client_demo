using Legacy.Database;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Legacy.Client
{

	public class PlayerStock
	{
		private List<StockItem> _items;
		public UnityEvent ChangeEvent { get; private set; }
		public UnityEvent PoolFillEvent { get; private set; }
		public UnityEvent PoolReleaseEvent { get; private set; }

		public PlayerStock()
		{
			ChangeEvent = new UnityEvent();
			PoolFillEvent = new UnityEvent();
			_items = new List<StockItem>();

			/**
			 * Glory
			 */
			_items.Add(new StockItem(CurrencyType.Rating, 0));

			/**
			 * Currencies
			 */
			_items.Add(new StockItem(CurrencyType.Soft, 0));
			_items.Add(new StockItem(CurrencyType.Hard, 0));
			//_items.Add(new StockItem(CurrencyType.Shards, 0));

			ChangeEvent.Invoke();
		}

		public StockItem getItem(CurrencyType type)
		{
			foreach(StockItem s in _items)
			{
				if (s.type == type) 
				    return s;
			}
			return null;
		}

		public uint GetCount(CurrencyType type)
		{
			StockItem s = getItem(type);
			if (s == null) return 0;
			return s.Count;
		}
	
		public void SetItem(CurrencyType type, uint count)
		{
			foreach (StockItem s in _items)
			{
				if (s.type != type) continue;
				s.Count = count;
				return;
			}
		}

		public bool take(CurrencyType type, uint count)
		{
			if (!CanTake(type, count))
				return false;
			SetItem(type, GetCount(type) - count);
			ChangeEvent.Invoke();
			return true;
		}

		public bool CanTake(CurrencyType type, uint count)
		{
            if (type > 0)
            {
                if (count == 0) return true;
                StockItem s = getItem(type);
                if (s == null) return false;
                return s.Count >= count;
            }
            else
            {
                return true;
            }
		}

	}
}
