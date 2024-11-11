using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using UnityEngine;
using static Legacy.Client.StateMachineSystem;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(StateMachineSystem))]
    [UpdateAfter(typeof(ObserverConnectionSystem))]
    public class RestartSystem : ComponentSystem
    {
        private StateMachineSystem _state_machine_system;
        private const string loadingSceneName = "Gameplay/Loading.unity";
        private NetworkDriver _driver;
        private Stopwatch _timer;
        protected override void OnCreate()
        {
            _state_machine_system = World.GetOrCreateSystem<StateMachineSystem>();
            RequireSingletonForUpdate<RestartClient>();
            _timer = new Stopwatch();
            _timer.Stop();
        }

        protected override void OnUpdate()
        {
            UnityEngine.Debug.LogError("Need reloading and destroy connection entity " + _timer.ElapsedMilliseconds);

            //вот оно где


          //  ClientWorld.Instance.GetExistingSystem<StateMachineSystem>().ReloadGame();

            var singltone = GetSingletonEntity<RestartClient>();
            EntityManager.DestroyEntity(singltone);

        }

    }
}