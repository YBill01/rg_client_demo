using Legacy.Database;

namespace Legacy.Client
{

	public class StockItem
	{
		public CurrencyType type;
		public StockItem(CurrencyType type, uint count = 0)
		{
			this.type = type;
			Count = count;
		}

		public uint Count;
	}
}
