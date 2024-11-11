using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class LevelUpWindowContentBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text levelIndex;
        [SerializeField] 
        private TMP_Text subtitleText;
        [SerializeField] 
        private Transform title;
        [SerializeField] 
        private Transform subtitle;
        [SerializeField] 
        private Transform level;
        [SerializeField] 
        private TMP_Text alphaText;
        [SerializeField] 
        private Image alphaImage;
        [SerializeField]
        private AudioSource audioSource;

        public void Init(ProfileInstance profile)
        {
            levelIndex.text = profile.Level.level.ToString();
            subtitleText.text = Locales.Get("locale:1411", $"<#E7CA00><size=140%>{profile.Level.level}</size></color>");
        }

        public void SetItemsBeforeAnimation()
        {
            title.localScale = Vector3.zero;
            subtitle.localScale = Vector3.zero;
            level.localScale = Vector3.zero;
            alphaText.color = new Color(alphaText.color.r, alphaText.color.g, alphaText.color.b, 0);
            alphaImage.color = new Color(alphaImage.color.r, alphaImage.color.g, alphaImage.color.b, 0);
            SoundManager.Instance.FadeOutMenuMusic(audioSource.clip.length);
        }
    }
}