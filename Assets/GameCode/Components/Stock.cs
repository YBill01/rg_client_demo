using Legacy.Database;

namespace Legacy.Client
{
    public enum StockItemType
	{
		Card,
		Simple,
		Hero,
		Equipment
	}

	public struct StockItemData
	{
		public StockItemType type;
		public ushort sid;
		public long count;

		public void Serialize(ref NetworkMessageRaw data)
		{
			data.Write((byte)type);
			data.Write(sid);
			data.Write(count);
		}

		public void Deserialize(ref NetworkMessageRaw data)
		{
			type = (StockItemType)data.ReadByte();
			sid = data.ReadUShort();
			count = data.ReadLong();
		}
	}

	public struct StuffItem
	{
		public ushort sid;
		public ushort uniqueID;
		public byte level;

		public void Serialize(ref NetworkMessageRaw data)
		{
			data.Write(sid);
			data.Write(uniqueID);
			data.Write(level);
		}

		public void Deserialize(ref NetworkMessageRaw data)
		{
			sid = data.ReadUShort();
			uniqueID = data.ReadUShort();
			level = data.ReadByte();
		}
	}

}