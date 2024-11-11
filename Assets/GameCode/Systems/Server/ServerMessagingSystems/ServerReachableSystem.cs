using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using Legacy.Database;
using Legacy.Client;
using UnityEngine;
using Legacy.Server;

namespace Legacy.Client
{
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(BattleSimulation))]
    [UpdateAfter(typeof(ServerReceiveSystem))]

    public class ServerReachableSystem : ComponentSystem
    {
        private EntityQuery _query_connection;
        private BattleSystems _battle;

        private EntityQuery _connect_request;

        protected override void OnCreate()
        {
            _query_connection = GetEntityQuery(
                ComponentType.ReadWrite<ServerConnectionClient>(),
                ComponentType.ReadOnly<ServerConnectionRequest>(),
                ComponentType.ReadOnly<ServerConnectionAuthorization>(),
                ComponentType.Exclude<ServerDisconnectRequest>(),
                ComponentType.Exclude<ServerConnectionDisconnected>()
            );

            _battle = ClientWorld.Instance.GetOrCreateSystem<BattleSystems>();
        }


        protected override void OnUpdate()
        {
            if (_query_connection.IsEmptyIgnoreFilter)
                return;

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                var _entity = _query_connection.GetSingletonEntity();
                PostUpdateCommands.AddComponent(_entity, new ServerDisconnectRequest()
                {
                    protocol = (byte)PlayerGameMessage.Disconnect
                });
            }
        }
    }
}

