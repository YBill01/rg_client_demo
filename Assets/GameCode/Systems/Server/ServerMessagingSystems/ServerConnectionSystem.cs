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

    public class ServerConnectionSystem : ComponentSystem
    {
        private EntityQuery _query_connection;
        private BattleSystems _battle;

        private EntityQuery _connect_request;

        protected override void OnCreate()
        {
            _connect_request = GetEntityQuery(
                ComponentType.ReadOnly<ServerConnectionRequest>(),
                ComponentType.ReadOnly<ServerConnectionAuthorization>(),
                ComponentType.Exclude<ServerConnectionClient>()
            );

            _query_connection = GetEntityQuery(
                ComponentType.ReadWrite<ServerConnectionClient>(),
                ComponentType.ReadOnly<ServerConnectionRequest>(),
                ComponentType.ReadOnly<ServerConnectionAuthorization>(),
                ComponentType.Exclude<ServerConnectionDisconnected>()
            );

            _battle = ClientWorld.Instance.GetOrCreateSystem<BattleSystems>();
        }

        protected override void OnDestroy()
        {
            ServerConnection.Instance.Dispose();
            base.OnDestroy();
        }

        protected override void OnUpdate()
        {            
            //Нет запросов (на подключение) - нет вопросов
            if (_connect_request.IsEmptyIgnoreFilter)
                return;

            //Есть подключение - больше не надо
            if (!_query_connection.IsEmptyIgnoreFilter)
                return;

            var driver = ServerConnection.Instance.Driver;

            var _entity = _connect_request.GetSingletonEntity();
            var _request = EntityManager.GetComponentData<ServerConnectionRequest>(_entity);

            var network_point = NetworkEndPoint.Parse(_request.ip.ToString(), _request.port);

            UnityEngine.Debug.Log("Server ip: " + _request.ip);
            UnityEngine.Debug.Log("Server Port: " + network_point.Port.ToString());
            UnityEngine.Debug.Log("Time: " + _battle.CurrentTime);

            if (!network_point.IsValid)
            {
                throw new System.Exception($"Wrong network point {_request.ip} {_request.port}");
            }

            PostUpdateCommands.AddComponent(_entity, new ServerConnectionClient
            {
                connection = driver.Connect(network_point),
                status = PlayerGameStatus.Disconnected
            });
            PostUpdateCommands.AddComponent(_entity, default(ServerConnectionDisconnected));
        }
    }
}

