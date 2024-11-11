using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    
    public class MenuArenaBehaviour : MonoBehaviour
    {

        [Header("Background Settings")]
        [SerializeField] BGSettings BackgroundSettings;

        internal void Enable(bool toggle)
        {
            if (toggle)
            {
                MainBGBehaviour.Instance.SwitchSetting(BackgroundSettings);
            }
            gameObject.SetActive(toggle);
        }

        
    }
}
