using Legacy.Client;
using UnityEngine;
using Legacy.Database;

public class LoadingBehaviour : MonoBehaviour
{
    void Start()
    {
		UnityEngine.Debug.Log("LoadingBehaviour");
        
        var device_info = new PlayerProfileDevice
        {
            device_id = SystemInfo.deviceUniqueIdentifier,
            device_model = SystemInfo.deviceModel,
            operating_system = SystemInfo.operatingSystem,
            memory_size = SystemInfo.systemMemorySize
        };
        UnityEngine.Debug.LogError("ObserverConnect add LoadingBehaviour");
        ClientWorld.Instance.ObserverConnect(device_info, SystemInfo.deviceName);        
    }
}
