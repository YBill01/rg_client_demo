using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;

namespace Legacy.Client
{
	public enum ObserverPlayerStatus
	{
		Disconnect,
        LoseConnect,
		Connected,
		Authorized,
		Matchmaking,
		BattleReady
	}

    public struct ObserverConnectionRequest : IComponentData
    {
    }
	
	public struct ObserverDisconnectRequest : IComponentData
	{
	}

	public struct RestartClient : IComponentData
    {
    }

    public struct ObserverConnectionClient : IComponentData
	{
		public NetworkConnection Connection;
		public ObserverPlayerStatus Status;
		public long timer;
	}

    public struct ObserverConnectionAuthorization : IComponentData
    {
		public uint player_id;
		public FixedString64 name;
		public FixedString64 device_id;
		public FixedString64 device_model;
		public FixedString64 operating_system;
		public int memory_size;
	}

    public struct ObserverConnectionDisconnected : IComponentData
	{

	}

	public struct ObserverMessageTag : IComponentData
	{

	}
}
