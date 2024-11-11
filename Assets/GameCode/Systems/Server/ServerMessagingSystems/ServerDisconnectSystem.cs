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

    public class ServerDisconnectSystem : ComponentSystem
    {
        private EntityQuery _query_disconnect_request;

        protected override void OnCreate()
        {
            _query_disconnect_request = GetEntityQuery(
                ComponentType.ReadWrite<ServerConnectionClient>(),
                ComponentType.ReadOnly<ServerConnectionRequest>(),
                ComponentType.ReadOnly<ServerConnectionAuthorization>(),
                ComponentType.ReadOnly<ServerDisconnectRequest>()
            );
        }

        protected override void OnUpdate()
        {
            if (_query_disconnect_request.IsEmptyIgnoreFilter)
                return;

            var driver = ServerConnection.Instance.Driver;

            var _connection = _query_disconnect_request.GetSingletonEntity();
            var _client = EntityManager.GetComponentData<ServerConnectionClient>(_connection);
            var discontcContext = EntityManager.GetComponentData<ServerDisconnectRequest>(_connection);

            driver.Disconnect(_client.connection);
            PostUpdateCommands.DestroyEntity(_connection);

            ServerReceiveSystem.StateMachineMessage(new NetworkMessageHelper
            {
                protocol = discontcContext.protocol,
                data = discontcContext.data
            });;
        }
    }
}

