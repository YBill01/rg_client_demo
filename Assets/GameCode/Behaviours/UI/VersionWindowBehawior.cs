using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Legacy.Database;

namespace Legacy.Client
{
    public class VersionWindowBehawior : MonoBehaviour
    {
        [SerializeField] private TMP_Text rewardCountText;
        public void RedirectToDownloadNewVersion()
        {
#if UNITY_ANDROID
            Application.OpenURL($"market://details?id={Application.identifier}");
            // or  "https://play.google.com/store/apps/details?id=" + id;
            Application.Quit();
#if !UNITY_EDITOR
        Application.Quit();
#endif
#elif UNITY_IOS
          //Application.OpenURL($"itms-apps://itunes.apple.com/app/id1508979017");
          //Application.Quit();
#endif
        }
    }
}