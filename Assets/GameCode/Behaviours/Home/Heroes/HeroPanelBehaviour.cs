using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class HeroPanelBehaviour : MonoBehaviour
    {
        public static int SiblingExists = 0;
        public static int SiblingOpened = 0;
        public static int SiblingClosed = 0;

        public enum HeroState
        {
            Exists,
            Opened,
            Closed
        }

        [SerializeField] private Material GrayScaleMaterial;
        [SerializeField] private GameObject CheckMart;
        [SerializeField] private ParticleSystem BuyVFX;

        [SerializeField] private GameObject Lock;

        [SerializeField] private GameObject HeroNamePanel;
        [SerializeField] private GameObject InfoPanel;
        [SerializeField] private GameObject Upgrade;
        [SerializeField] private GameObject Arrow;
        [SerializeField] private GameObject NewLabel;
        [SerializeField] private TextMeshProUGUI Arena;

        [SerializeField] private GameObject LevelObject;
        [SerializeField] private TextMeshProUGUI LevelText;

        [SerializeField] private Image Glow;
        [SerializeField] private GameObject SelectedShadow;

        [SerializeField] private HeroNameColorBehaviour MainName;
        [SerializeField] private HeroNameColorBehaviour SecondName;

        [SerializeField] private HeroNameColorBehaviour MainNameOnInfoLabel;
        [SerializeField] private ButtonWithPriceViewBehaviour BuyButton;
        [SerializeField] public LegacyButton HeroButton;

        [SerializeField] private Image HeroIcon;

        private BinaryHero binaryHero;
        private PlayerProfileHero playerHero;
        private HeroesScrollBehaviour scroll;
        private HeroesWindowBehaviour HeroesListWindow;
        private bool canUpdate;

        public bool Selected { get; private set; }
        public bool CanUpdate => canUpdate;

        private ProfileInstance profile;
        private Vector3 currentScale = Vector3.one;

        [SerializeField] private RectTransform ScaleContainer;
        [SerializeField, Range(0, 1)] private float ScaleLerpSpeed;
        [SerializeField, Range(0.0f, 1.0f)] private float UnactiveScale;

        public HeroState state;
        [SerializeField, Range(-1.0f, 1.0f)] private float TextGradientUpLighter;
        [SerializeField, Range(-1.0f, 1.0f)] private float TextGradientDownLighter;
        [SerializeField, Range(-1.0f, 1.0f)] private float GlowLighterMultiplier;
        public byte NumberInScroll;

        public BinaryHero BinaryHero => binaryHero;
        public PlayerProfileHero PlayerHero => playerHero;

        internal void UpdateSelected()
        {
            if (profile.GetPlayerHero(binaryHero.index, out PlayerProfileHero _hero))
            {
                playerHero = _hero;

                HeroNamePanel.SetActive(true);
                MakeGray(false);
            }

            Selected = profile.SelectedHero == binaryHero.index;
            CheckMart.SetActive(Selected);
            if (Selected)
            {
                Select();
                scroll.SetCurrentSnap(NumberInScroll);
            }
            else
            {
                Deselect();
            }
        }

        public void OpenThis()
        {
            if (HeroesListWindow != null)
            {
                HeroesListWindow.OpenHero(binaryHero.index);
            }
            else if(WindowManager.Instance.CurrentWindow is DecksWindowBehaviour)
            {
                WindowManager.Instance.MainWindow.Hero();
            }
        }
        public void ScrollToMe()
        {
            if (Selected)
            {
              //  HeroesListWindow.OpenHero(binaryHero.index);
            }
            else
            {
                scroll.SetCurrentSnap(NumberInScroll);
            }
        }

        internal byte SetNumber()
        {
            NumberInScroll = (byte)transform.GetSiblingIndex();
            return NumberInScroll;
        }

        void Update()
        {
            ScaleContainer.localScale = Vector3.Lerp(ScaleContainer.localScale, currentScale, ScaleLerpSpeed);
        }

        internal void Deselect()
        {
            Selected = false;
            currentScale = new Vector3(UnactiveScale, UnactiveScale, UnactiveScale);
            Glow.gameObject.SetActive(false);
            SelectedShadow.SetActive(false);
        }

        internal void Select()
        {
            currentScale = Vector3.one;
            Selected = true;
            Glow.gameObject.SetActive(true);
            SelectedShadow.SetActive(true);
        }

        internal void InitFromDeck(ProfileInstance profile)
        {
            if (Heroes.Instance.Get(profile.SelectedHero, out BinaryHero _hero))
            {
                if (profile.GetPlayerHero(_hero.index, out PlayerProfileHero _playerHero))
                {
                    binaryHero = _hero;
                    playerHero = _playerHero;
                    HeroIcon.sprite = VisualContent.Instance.GetHeroVisualData(binaryHero.index).HeroListIcon;
                    Glow.gameObject.SetActive(Selected);
                    SelectedShadow.SetActive(Selected);
                    SetNames();
                    Exists();
                }
            }
        }
        public BinaryHero GetHeroOfPanel()
        {
            return this.binaryHero;
        }
        internal void Init(BinaryHero binaryHero, HeroesWindowBehaviour heroesWindow)
        {

            profile = ClientWorld.Instance.Profile;
            currentScale = new Vector3(UnactiveScale, UnactiveScale, UnactiveScale);
            this.binaryHero = binaryHero;
            playerHero = null;
            
            scroll = heroesWindow.GetComponent<HeroesScrollBehaviour>();
            HeroesListWindow = heroesWindow;
            UpdateSelected();
            HeroIcon.sprite = VisualContent.Instance.GetHeroVisualData(binaryHero.index).HeroListIcon;
            SetNames();
            var isNew = false;
            if (playerHero != null)
            {
                GetComponent<RectTransform>().SetSiblingIndex(SiblingExists);
                SiblingExists++;
                Exists();

                canUpdate = playerHero.level < ClientWorld.Instance.Profile.Level.level /*&& playerHero.UpdatePrice <= profile.Stock.GetCount(CurrencyType.Soft)*/;

                if (canUpdate)
                {
                    InfoPanel.SetActive(true);
                    Upgrade.SetActive(true);
                    Animator anim = Arrow.GetComponent<Animator>();
                    if (playerHero.UpdatePrice <= profile.Stock.GetCount(CurrencyType.Soft))
                    {
                         anim.enabled = true;
                    }else
                         anim.enabled = false;
                }
                else
                {
                    InfoPanel.SetActive(false);
                    Upgrade.SetActive(false);
                }
            }
            else
            {
                InfoPanel.SetActive(true);

                if (binaryHero.GetLockedByArena(out BinaryBattlefields binaryArena))
                {
                    byte number = Settings.Instance.Get<ArenaSettings>().GetNumber(binaryArena.index);
                    if (profile.CurrentArena.number >= number && !profile.IsBattleTutorial)
                    {
                        GetComponent<RectTransform>().SetSiblingIndex(SiblingExists + SiblingOpened);
                        SiblingOpened++;
                        NeedBuy();

                        isNew = !profile.ViewedHeroes.Contains(binaryHero.index);
                    }
                    else
                    {
                        GetComponent<RectTransform>().SetSiblingIndex(SiblingExists + SiblingOpened + SiblingClosed);
                        SiblingClosed++;
                        NeedOpen(number);
                    }
            ComingSoon(number);
                }
                else
                {
            ComingSoon(Settings.Instance.Get<ArenaSettings>().GetNumber(binaryArena.index));
                    NeedBuy();
                }
            }
            NewLabel.SetActive(isNew);
        }

      

        private void SetNames()
        {
            Color TextColor = Color.white;
            if (ColorUtility.TryParseHtmlString(binaryHero.color, out Color color))
            {
                Glow.color = color * (1.0f + GlowLighterMultiplier);
                LevelObject.GetComponent<Image>().color = color;
                MainName.SetLighters(TextGradientDownLighter, TextGradientUpLighter);
                SecondName.SetLighters(TextGradientDownLighter, TextGradientUpLighter);
                TextColor = color;
            }

            MainName.SetName(binaryHero.title, TextColor);
            SecondName.SetName(binaryHero.second_name, TextColor);
            MainNameOnInfoLabel.SetName(binaryHero.title, TextColor);
        }

        private void NeedBuy()
        {
            state = HeroState.Opened;
            LevelObject.SetActive(false);
            MakeGray();
            InfoPanel.SetActive(true);
            HeroNamePanel.SetActive(false);
            BuyButton.SetHeroPrice(binaryHero.price);
            BuyButton.gameObject.SetActive(true);
            MainNameOnInfoLabel.gameObject.SetActive(true);
            HeroButton.interactable = true;
            HeroButton.isLocked = false;
        }
        private void ComingSoon(byte ArenaNumber)
        {
            if (ArenaNumber+1 > ArenaTemporarySettings.Instance.RealArenasCount)
            {
                InfoPanel.SetActive(false);
                Upgrade.SetActive(false);
                LevelObject.SetActive(false);
                InfoPanel.SetActive(true);
                MakeGray();
                Arena.gameObject.SetActive(true);
                BuyButton.gameObject.SetActive(false);
                MainNameOnInfoLabel.gameObject.SetActive(false);
                HeroButton.interactable = false;
                HeroButton.isLocked = true;
                Arena.text = Locales.Get("locale:1291");
                HeroButton.localeAlert = Locales.Get("locale:1291");
            }
        }

        private void NeedOpen(byte ArenaNumber)
        {
            ArenaNumber++;
            state = HeroState.Closed;
            LevelObject.SetActive(false);
            InfoPanel.SetActive(true);
            Lock.SetActive(true);
            MakeGray();
            Arena.text = Locales.Get("locale:712") + " " + ArenaNumber.ToString();
            Arena.gameObject.SetActive(true);
            BuyButton.gameObject.SetActive(false);
            MainNameOnInfoLabel.gameObject.SetActive(false);
            HeroButton.interactable = false;
            HeroButton.isLocked = true;
            HeroButton.localeAlert = Locales.Get("locale:1843", ArenaNumber);
        }

        private void MakeGray(bool toggle = true)
        {
            HeroIcon.material = toggle ? GrayScaleMaterial : null;
        }

        private void Exists()
        {
            state = HeroState.Exists;
            LevelObject.SetActive(true);
            InfoPanel.SetActive(false);
            LevelText.text = playerHero.level.ToString();
            BuyButton.gameObject.SetActive(false);
            MainNameOnInfoLabel.gameObject.SetActive(false);
            HeroButton.interactable = true;
            HeroButton.isLocked = false;
        }

        public void BuyHero()
        {
            BuyVFX.Play();
            BuyButton.gameObject.SetActive(false);
            StartCoroutine(BuyThings());
            if (BinaryHero.price.isReal)
            {
                //AnalyticsManager.Instance.HeroBuy(BinaryHero.index, CurrencyType.Real, BinaryHero.price); //DO NOT KNOW THE PRICE
            }
            else if (BinaryHero.price.isSoft)
            {
                AnalyticsManager.Instance.HeroBuy(BinaryHero.index, CurrencyType.Soft, (int)BinaryHero.price.soft);
            }
            else if (BinaryHero.price.isHard)
            {
                AnalyticsManager.Instance.HeroBuy(BinaryHero.index, CurrencyType.Hard, BinaryHero.price.hard);
            }
        }

        IEnumerator BuyThings()
        {
            yield return new WaitForSeconds(TimeToUpdateView);
            profile.CreateHero(BinaryHero.index);
            MakeGray(false);
        }

        [Header("BuyEffect")]
        [SerializeField, Range(0.0f, 1.0f)] float TimeToUpdateView = 0.2f;

    }
}