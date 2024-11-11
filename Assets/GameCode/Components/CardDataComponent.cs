using Legacy.Database;
using Unity.Entities;

namespace Legacy.Client
{
	public enum CardStatus
	{
		Active,
        ActiveSelected,
        NotEnoughMana,
        NotEnoughManaSelected,
        Next,
        Played,
        Selected,
        Tremor
    }

	public struct EntityDestroyTag : IComponentData
	{
		public float StartDie;
	}

	public struct CardActionComponent : IComponentData
	{
		public BattlePlayer player;
		public uint CardID;
	}

	public struct PropertyOfSomeoneElse : IComponentData { };
	public struct PropertyOfMe : IComponentData { };
	public struct CardDataComponent : IComponentData, IDatabase
	{
		public BattlePlayerSide side;
		public uint OrderID;
		public ushort DBID;
		public CardStatus Status;

		public void Deserialize(ref NetworkMessageRaw content)
		{
			side = (BattlePlayerSide)content.ReadByte();
			OrderID = (uint)content.ReadUInt();
			DBID = (ushort)content.ReadUShort();
			Status = (CardStatus)content.ReadByte();
		}

		public void Interpolate(IComponentData first, IComponentData second, float alpha)
		{
			CardDataComponent ft = (CardDataComponent)first;
			side = ft.side;
			OrderID = ft.OrderID;
			DBID = ft.DBID;
			Status = ft.Status;
		}

		public void Serialize(ref NetworkMessageRaw content)
		{
			content.Write((byte)side);
			content.Write((uint)OrderID);
			content.Write((ushort)DBID);
			content.Write((byte)Status);
		}
	}
}
