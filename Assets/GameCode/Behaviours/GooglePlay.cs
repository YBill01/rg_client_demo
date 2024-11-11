#if UNITY_ANDROID

using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GooglePlay
{
    private static GooglePlay _instance;
    public static GooglePlay Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GooglePlay();
            }
            return _instance;
        }
    }
    public static PlayGamesPlatform Platform = null;
    public static string id;

    public bool GPG_Init = false;
    public void Init()
    {
        if (!GPG_Init)
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                //.RequestIdToken()
                .Build();

            PlayGamesPlatform.InitializeInstance(config);

            PlayGamesPlatform.Activate();
            //PlayGamesPlatform.DebugLogEnabled = true;

            Debug.Log("PlayGamesPlatform.Activate()");
            Debug.Log($"Active Social - {Social.Active.ToString()}");
            GPG_Init = true;
        }

        if (!Social.localUser.authenticated)
        {
            Debug.Log("[LogIn_GooglePlayGames] not autentificated");

            Social.localUser.Authenticate((success, error) => {
                if (!string.IsNullOrEmpty(error))
                    Debug.Log($"Authenticated Error string: {error}");

                if (success)
                {
                    id = Social.localUser.id;
                    Debug.Log("[LogIn_GooglePlayGames] social success");
                }
                else
                {
                    Debug.Log("[LogIn_GooglePlayGames] failed");
                }
            });
        }
        else
        {
            Debug.Log("[LogIn_GooglePlayGames] autentificated");
        }
    }

    
}
#endif
