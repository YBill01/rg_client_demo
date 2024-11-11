using Legacy.Database;
using Legacy.Server;
using Unity.Entities;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattleSimulation))]

	public class SendMessageSystem : SystemBase
	{
        private EndSimulationEntityCommandBufferSystem _barrier;
        private EntityQuery _query_game_connect;
        protected override void OnCreate()
		{
            _barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            _query_game_connect = GetEntityQuery(
                ComponentType.ReadOnly<ServerConnectionClient>(),
                ComponentType.ReadOnly<ServerConnectionAuthorization>(),
                ComponentType.Exclude<ServerConnectionDisconnected>()
            );
            RequireForUpdate(_query_game_connect);
		}

		protected override void OnUpdate()
		{
            var _client = _query_game_connect.GetSingleton<ServerConnectionClient>();
            var connection = _client.connection;
            var driver = ServerConnection.Instance.Driver;
            var reliable = ServerConnection.Instance.ReliablePipeline;
            var buffer = _barrier.CreateCommandBuffer();

            Entities
                .WithAll<ServerMessageTag>()
                .ForEach(
                (
                    Entity entity,  
                    in NetworkMessageRaw message
                )
                => 
                {
                    message.Send(driver, reliable, connection);
                    buffer.DestroyEntity(entity);
                }).WithoutBurst().Run();
        }
	}
}

