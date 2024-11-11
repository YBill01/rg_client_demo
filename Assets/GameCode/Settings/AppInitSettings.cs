using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "GameLegacy/AppInitSettings", fileName = "AppInitSettings")]
public class AppInitSettings : ScriptableObject
{
	static AppInitSettings _instance = null;
	public static AppInitSettings Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = (AppInitSettings)Resources.Load("Settings/AppInitSettings");
				if(!_instance.EnableLogs)
				{
					Debug.unityLogger.logEnabled = false;
					Debug.developerConsoleVisible = false;
					Debug.unityLogger.filterLogType = LogType.Warning;
				}
			}
			return _instance;
		}
	}

	public void OnValidate()
	{
		
	}

	[Serializable]
	public struct ServerData
	{
		public ConnectionID connectionID;
		public string IPv4;
		public string ForceUsername;
		public string BinaryDomain;

        public override string ToString()
        {
            return $"ConnectionID: {connectionID}, ip: {IPv4}, domain: {BinaryDomain}.";
        }
    }

	public List<ServerData> observerIPs;

	public enum ConnectionID
	{
		Prod,
		Test,
		Dev,
		Local,
		Custom
	}

	public ConnectionID ObserverConnection;

	public string GetForcedUsername()
	{
#if UNITY_EDITOR
		foreach (ServerData ipdata in observerIPs)
		{
			if (ipdata.connectionID != ObserverConnection) continue;
			return ipdata.ForceUsername;
		}
#endif
		return observerIPs[0].ForceUsername;
	}

	private ServerData GetServerData()
    {
		ServerData serverData = default;
#if PRODUCTION
        EnableLogs = false;
		EnableAnalytics = true;
		ObserverConnection = ConnectionID.Prod;
#elif PRE_PRODUCTION
        ObserverConnection = ConnectionID.Test;
#endif
		bool found = false;
		foreach (ServerData ipdata in observerIPs)
		{
			if (ipdata.connectionID != ObserverConnection) continue;
			serverData = ipdata;
			found = true;
		}
		if (!found)
		{
			serverData = observerIPs[0];
		}

        GameDebug.Log($"ObserverData: {serverData}");
		return serverData;
	}
    public string LocalesDomain => GetServerData().BinaryDomain;
    public string GetObserverIP()
	{
		return GetServerData().IPv4;
	}

    public string GetBinaryDatabaseHost()
    {
		return GetServerData().BinaryDomain;
	}

	public uint snapshotExpireTime = 300u;
	public uint snapshotDiffTime = 150;


	public bool UseLocalBinary;

	public bool EnableLogs;
	public bool EnableAnalytics;

    public bool EnableColliders;

    public string UserName;
    public string UserIndex;

	[Serializable]
	public class CampaignDebug
	{
		public ushort index;
		public ushort mission;
	}

	public CampaignDebug campaign;

}
