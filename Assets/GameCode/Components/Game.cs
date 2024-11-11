using Legacy.Database;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;

namespace Legacy.Client
{

	public struct GameConnectRequest : IComponentData
	{
		public FixedString64 ip;
		public ushort port;
	}

	public struct GameMessageTag : IComponentData
	{

	}

	public struct PlayerGameClient : IComponentData
	{
		public NetworkConnection connection;
		public PlayerGameStatus status;
		public long offset;
		public Entity player;
		public float alive;
	}

	public struct PlayerGameDisconnected : IComponentData
	{

	}

	public struct BattlePlayerOwner : IComponentData
	{

	}

	public struct MinionIsSpawned : IComponentData
	{

	}

}
