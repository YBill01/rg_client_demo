using Unity.Entities;
using Unity.Networking.Transport;

namespace Legacy.Client
{
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]

    public class ObserverConnectionSystem : ComponentSystem
    {
        public const int DisconnectTimeout = 10;

        private EntityQuery _connect_request;

        protected override void OnCreate()
        {
            _connect_request = GetEntityQuery(
                ComponentType.ReadWrite<ObserverConnectionAuthorization>(),
                ComponentType.ReadOnly<ObserverConnectionRequest>(),
                ComponentType.Exclude<ObserverConnectionClient>()
            );
        }

        protected override void OnDestroy()
        {
            ObserverConnection.Instance.Dispose();
        }

        protected override void OnUpdate()
        {
            if (_connect_request.IsEmptyIgnoreFilter)
                return;

            ushort port = 6668;
            var ip = AppInitSettings.Instance.GetObserverIP();

            var network_point = NetworkEndPoint.Parse(ip, port);

            if (network_point.IsValid)
            {
                GameDebug.Log("NetworkPoint is valid!");
                GameDebug.Log("Create ObserverConnectionClient, add ObserverConnectionDisconnect.");
                var entity = _connect_request.GetSingletonEntity();
                PostUpdateCommands.AddComponent(entity, new ObserverConnectionClient
                {
                    Connection = ObserverConnection.Instance.Driver.Connect(network_point),
                    Status = ObserverPlayerStatus.Disconnect
                });
                PostUpdateCommands.AddComponent(entity, default(ObserverConnectionDisconnected));
            }
        }
    }
}

