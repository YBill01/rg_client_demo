using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    [Serializable]
    public struct BGSettings
    {
        public Color BGMainColor;
        public Color LogoTextureColor;
        public BGLightSettings BGLight1;
        public BGLightSettings BGLight2;
        public BGLightSettings BGLight3;
    }

    [Serializable]
    public struct BGLightSettings
    {
        public Vector3 scale;
        public Color color;
    }

    
    public class MainBGBehaviour : MonoBehaviour
    {
        public static MainBGBehaviour Instance;
        [SerializeField] BGSettings DefaultSettings;
        [SerializeField, Range(0.0f, 3.0f)] float changeTime;

        [SerializeField] RectTransform Light1Rect;
        [SerializeField] RectTransform Light2Rect;
        [SerializeField] RectTransform Light3Rect;
        [SerializeField] Image Light1Image;
        [SerializeField] Image Light2Image;
        [SerializeField] Image Light3Image;

        [SerializeField] Image MainBGImage;
        [SerializeField] Image LogoTextureImage;

        void Start()
        {
            SwitchSetting(DefaultSettings);
            Instance = this;
        }

        void DoImageColor(Image image, Color32 color)
        {
            image.DOColor(color, changeTime);
            //image.DOFade(color.a, changeTime);
        }

        void DoScale(RectTransform rect, Vector3 scale)
        {
            if(scale != Vector3.zero)
            {
                rect.DOScale(scale, changeTime);
            }
        }

        private float changeTimeTemp;
        internal void SetChangeTime(float value)
        {
            changeTimeTemp = changeTime;
            changeTime = value;
        }
        internal void ResetChangeTime()
        {
            changeTime = changeTimeTemp;
        }

        internal void SwitchSetting(BGSettings settings)
        {
            DoScale(Light1Rect, settings.BGLight1.scale);
            DoScale(Light2Rect, settings.BGLight2.scale);
            DoScale(Light3Rect, settings.BGLight3.scale);
            DoImageColor(Light1Image, settings.BGLight1.color);
            DoImageColor(Light2Image, settings.BGLight2.color);
            DoImageColor(Light3Image, settings.BGLight3.color);
            DoImageColor(MainBGImage, settings.BGMainColor);
            DoImageColor(LogoTextureImage, settings.LogoTextureColor);
        }

        internal void ResetToDefault()
        {
            SwitchSetting(DefaultSettings);
        }
    }

    
}
