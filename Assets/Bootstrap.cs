using Legacy.Client;
using Unity.Entities;
using UnityEngine;
using System.Collections;
public class Bootstrap : MonoBehaviour, ICustomBootstrap
{
    IEnumerator Start()
    {
        yield return null; // to render starting image before init starts
        if (World.DefaultGameObjectInjectionWorld == null || !World.DefaultGameObjectInjectionWorld.IsCreated)
        {
            Debug.Log("Bootstrap: Init().");
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.runInBackground = true;
            Application.targetFrameRate = 60;
            DefaultWorldInitialization.Initialize("Default World", false);
#if UNITY_ANDROID
            GooglePlay.Instance.Init();
#endif
        }
    }
    public bool Initialize(string defaultWorldName)
    {
        GameDebug.Log("Bootstrap: Initialize().");

        ConfigVar.Init();

        var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);

        var _client_world = new ClientWorld("LegacyClient");
        World.DefaultGameObjectInjectionWorld = _client_world;
        _client_world.AddSystems(systems);

        //var _server_world = new ServerWorld("LegacyServer");
        //_server_world.AddSystems(systems, _client_world);

        ScriptBehaviourUpdateOrder.AddWorldToCurrentPlayerLoop(_client_world);
        return true;
    }
}
