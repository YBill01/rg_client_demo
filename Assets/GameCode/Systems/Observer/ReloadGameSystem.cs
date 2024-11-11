using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using System.Diagnostics;
using Legacy.Database;
using UnityEngine;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]

    public class ReloadGameSystem : ComponentSystem
    {
        private EntityQuery _query_connection;

        private NetworkReachability InternetReachability;


        protected override void OnCreate()
        {


            InternetReachability = Application.internetReachability;

            _query_connection = GetEntityQuery(
                ComponentType.ReadOnly<ObserverConnectionAuthorization>(),
                ComponentType.ReadWrite<ObserverConnectionClient>()
            );



            RequireForUpdate(_query_connection);
        }
        protected override void OnUpdate()
        {
            if (Application.internetReachability!= InternetReachability)
            {
                if (!_query_connection.IsEmptyIgnoreFilter)
                {
                    var _connection = _query_connection.GetSingletonEntity();
                    PostUpdateCommands.AddComponent(_connection, new ObserverDisconnectRequest());
                }
            }
          
            InternetReachability = Application.internetReachability;
        }

    }
}