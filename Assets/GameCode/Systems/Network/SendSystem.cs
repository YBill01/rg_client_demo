
using Legacy.Server;
using Legacy.Client;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;

namespace Legacy.Network
{

	public class SendMessageSystem : JobComponentSystem
	{
		private EntityQuery _query_observer_connect;
		private EntityQuery _query_game_connect;

		private ObserverConnectionSystem _observer_system;
		private ServerConnectionSystem _game_system;

		protected override void OnCreate()
		{

			_query_observer_connect = GetEntityQuery(
				ComponentType.ReadOnly<ObserverConnectionClient>(),
				ComponentType.ReadOnly<ObserverConnectionAuthorization>(),
				ComponentType.Exclude<ObserverConnectionDisconnected>()
			);

			_query_game_connect = GetEntityQuery(
				ComponentType.ReadOnly<ServerConnectionClient>(),
				ComponentType.ReadOnly<ServerConnectionAuthorization>(),
				ComponentType.Exclude<ServerConnectionDisconnected>()
			);

			_observer_system = World.GetOrCreateSystem<ObserverConnectionSystem>();
			_game_system = World.GetOrCreateSystem<ServerConnectionSystem>();
		}

		private float timeToDisconnect;
		private bool inited;
		private bool fault;
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			if (fault) return inputDeps;
			if(!inited)
			{
				if(!_query_observer_connect.IsEmptyIgnoreFilter || !_query_game_connect.IsEmptyIgnoreFilter)
				{
					inited = true;
					timeToDisconnect = ObserverConnectionSystem.DisconnectTimeout;
				}
			}
			else
			{
				if (_query_observer_connect.IsEmptyIgnoreFilter || _query_game_connect.IsEmptyIgnoreFilter)
				{
					timeToDisconnect -= Time.DeltaTime;
				}
				if (timeToDisconnect <= 0)
				{
					NativeMessage msg = new NativeMessage("Oops =(", "Server connection lost..");
					fault = true;
				}
			}
			return inputDeps;
		}

	}
}

