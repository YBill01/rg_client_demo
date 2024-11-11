using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Client;
using Legacy.Database;
using UnityEngine;
using UnityEngine.UI;
using static Legacy.Client.HeroPanelBehaviour;

namespace Legacy.Client
{
    public class SmallHeroButtonBehaviour : MonoBehaviour
    {
        private static int SiblingExists = 0;
        private static int SiblingOpened = 0;
        private static int SiblingClosed = 0;

        public GameObject separator;

        [SerializeField] private GameObject CheckMart;
        [SerializeField] private GameObject Lock;
        [SerializeField] private GameObject Shard;

        [SerializeField] private GameObject UpgradeShadow;
        [SerializeField] private GameObject UpgradeArrow;

        [SerializeField] private Image Glow;
        [SerializeField] private GameObject SelectedShadow;

        [SerializeField] private Image HeroIcon;

        [SerializeField] private RectTransform ScaleContainer;

        [SerializeField, Range(0, 1)] private float ScaleLerpSpeed;
        [SerializeField, Range(0.0f, 1.0f)] private float UnactiveScale;

        [SerializeField] private Material GrayScaleMaterial;

        public bool Selected { get; private set; }

        private Vector3 currentScale;
        private BinaryHero binaryHero;
        private ProfileInstance profile;

        void Update()
        {
            ScaleContainer.localScale = Vector3.Lerp(ScaleContainer.localScale, currentScale, ScaleLerpSpeed);
        }

        internal void Init(BinaryHero binary_hero, HeroPanelBehaviour panel)
        {
            profile = ClientWorld.Instance.Profile;
            GetComponent<LegacyButton>().onClick.AddListener(delegate { panel.ScrollToMe(); });
            currentScale = new Vector3(UnactiveScale, UnactiveScale, UnactiveScale);
            binaryHero = binary_hero;
            UpdateSelected();
            HeroIcon.sprite = VisualContent.Instance.GetHeroVisualData(binaryHero.index).HeroListIcon;
            SetState(panel.state);
            UpgradeArrow.SetActive(panel.CanUpdate);
        }

        internal void UpdateSelected()
        {
            Selected = profile.SelectedHero == binaryHero.index;
            CheckMart.SetActive(Selected);

            if (Selected)
            {
                Select();
            }
            else
            {
                Deselect();
            }
        }

        public void Select()
        {
            currentScale = Vector3.one;
            Glow.gameObject.SetActive(true);
            SelectedShadow.SetActive(true);
        }

        internal void Deselect()
        {
            Selected = false;
            currentScale = new Vector3(UnactiveScale, UnactiveScale, UnactiveScale);
            Glow.gameObject.SetActive(false);
            SelectedShadow.SetActive(false);
        }

        void SetState(HeroState state)
        {
            switch (state)
            {
                case HeroState.Exists:
                    GetComponent<RectTransform>().SetSiblingIndex(SiblingExists++);
                     if(separator) separator.transform.SetSiblingIndex(SiblingExists++);
                    Shard.SetActive(false);
                    Lock.SetActive(false);
                    break;
                case HeroState.Opened:
                    GetComponent<RectTransform>().SetSiblingIndex(SiblingExists + SiblingOpened++);
                    if (separator) separator.transform.SetSiblingIndex(SiblingExists + SiblingOpened++);
                    Shard.SetActive(true);
                    Lock.SetActive(false);
                    break;
                case HeroState.Closed:
                    GetComponent<RectTransform>().SetSiblingIndex(SiblingExists + SiblingOpened + SiblingClosed++);
                    if (separator) separator.transform.SetSiblingIndex(SiblingExists + SiblingOpened + SiblingClosed++);
                    Shard.SetActive(false);
                    Lock.SetActive(true);
                    HeroIcon.material = GrayScaleMaterial;
                    break;
                default:
                    break;
            }
        }
    }
}