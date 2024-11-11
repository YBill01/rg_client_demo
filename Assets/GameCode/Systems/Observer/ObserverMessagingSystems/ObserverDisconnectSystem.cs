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
    [UpdateAfter(typeof(ObserverReceiveSystem))]

    public class ObserverDisconnectSystem : ComponentSystem
    {
        public static bool HardDisconect = false;


        protected override void OnCreate()
        {
            RequireSingletonForUpdate<ObserverConnectionClient>();
            RequireSingletonForUpdate<ObserverDisconnectRequest>();
        }

        protected override void OnDestroy()
        {
            ObserverConnection.Instance.Dispose();
        }

        protected override void OnUpdate()
        {
            if (WindowManager.Instance != null && WindowManager.Instance.MainWindow != null)
            {
                var parent = WindowManager.Instance.MainWindow.GetComponent<Transform>().parent;
                foreach (Transform chid in parent)
                {
                    chid.gameObject.SetActive(false);
                }
            }

            var _connection = GetSingletonEntity<ObserverConnectionClient>();
            var _client = EntityManager.GetComponentData<ObserverConnectionClient>(_connection);

            ObserverConnection.Instance.Driver.Disconnect(_client.Connection);
            EntityManager.DestroyEntity(_connection);
            ClientWorld.Instance.GetExistingSystem<StateMachineSystem>().ReloadGame();
        }
    }
}

