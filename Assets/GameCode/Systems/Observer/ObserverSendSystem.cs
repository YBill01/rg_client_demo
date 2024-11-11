using Legacy.Database;
using Unity.Entities;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
	public class ObserverSendSystem : ComponentSystem
	{
        private EntityQuery _query_send;

		protected override void OnCreate()
		{
            _query_send = GetEntityQuery(
                ComponentType.ReadOnly<NetworkMessageRaw>(),
                ComponentType.ReadOnly<ObserverMessageTag>()
            );

            RequireForUpdate(_query_send);
            RequireSingletonForUpdate<ObserverConnectionClient>();
		}

		protected override void OnUpdate()
		{
            var _client = GetSingleton<ObserverConnectionClient>();
            if (_client.Status > ObserverPlayerStatus.LoseConnect)
            {
                Entities
                    .ForEach((Entity entity, ref NetworkMessageRaw message) =>
                    {
                        GameDebug.Log($"ObserverSendSystem --> Send Entity({entity.Index}). Message size ({message.size}).");
                        GameDebug.Log($"ObserverSendSystem --> Try send message.");

                        message.Send(ObserverConnection.Instance.Driver, ObserverConnection.Instance.ReliablePeline, _client.Connection);
                        PostUpdateCommands.DestroyEntity(entity);
                    });
            }
        }
    }
}

