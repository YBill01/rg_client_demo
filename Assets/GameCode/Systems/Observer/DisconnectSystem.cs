
using Legacy.Client;
using Legacy.Database;
using Legacy.Server;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;

namespace Legacy.Network
{
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class DisconnectSystem : JobComponentSystem
	{
		private BeginSimulationEntityCommandBufferSystem _barrier;

		private EntityQuery _query_observer_connect;
		private EntityQuery _query_game_send;
		private EntityQuery _query_game_connect;
		private EntityQuery _query_observer_send;

		protected override void OnCreate()
		{
			_barrier = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

			_query_observer_send = GetEntityQuery(
				ComponentType.ReadOnly<NetworkMessageRaw>(),
				ComponentType.ReadOnly<ObserverMessageTag>()
			);

			_query_observer_connect = GetEntityQuery(
				ComponentType.ReadOnly<ObserverConnectionClient>(),
				ComponentType.ReadOnly<ObserverConnectionAuthorization>(),
				ComponentType.Exclude<ObserverConnectionDisconnected>()
			);

			_query_game_send = GetEntityQuery(
				ComponentType.ReadOnly<NetworkMessageRaw>(),
				ComponentType.ReadOnly<GameMessageTag>()
			);

			_query_game_connect = GetEntityQuery(
				ComponentType.ReadOnly<ServerConnectionClient>(),
				ComponentType.ReadOnly<ServerConnectionAuthorization>(),
				ComponentType.Exclude<ServerConnectionDisconnected>()
			);
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			if(!_query_observer_send.IsEmptyIgnoreFilter)
			{
				if (!_query_observer_connect.IsEmptyIgnoreFilter)
				{
					var _connect_entity = _query_observer_connect.GetSingletonEntity();
					var _client = EntityManager.GetComponentData<ObserverConnectionClient>(_connect_entity);
					inputDeps = new SendJob
					{
						connection = _client.Connection,
						buffer = _barrier.CreateCommandBuffer(),
						driver = ObserverConnection.Instance.Driver,
						reliable = ObserverConnection.Instance.ReliablePeline

					}.ScheduleSingle(_query_observer_send, inputDeps);

					_barrier.AddJobHandleForProducer(inputDeps);
				}
			}

			if (!_query_game_send.IsEmptyIgnoreFilter)
			{
				if (!_query_game_connect.IsEmptyIgnoreFilter)
				{
					var _connect_entity = _query_game_connect.GetSingletonEntity();
					var _client = EntityManager.GetComponentData<ServerConnectionClient>(_connect_entity);
					inputDeps = new SendJob
					{
						connection = _client.connection,
						buffer = _barrier.CreateCommandBuffer(),
						driver = ServerConnection.Instance.Driver,
						reliable = ServerConnection.Instance.ReliablePipeline

					}.ScheduleSingle(_query_game_send, inputDeps);

					_barrier.AddJobHandleForProducer(inputDeps);
				}
			}

			return inputDeps;
		}

		struct SendJob : IJobForEachWithEntity<NetworkMessageRaw>
		{
			internal EntityCommandBuffer buffer;
			internal NetworkPipeline reliable;
			[ReadOnly] public NetworkConnection connection;
			internal NetworkDriver driver;

			unsafe public void Execute(Entity entity, int index, [ReadOnly] ref NetworkMessageRaw message)
			{
				message.Send(driver, reliable, connection);
				buffer.DestroyEntity(entity);
			}
		}

	}
}

