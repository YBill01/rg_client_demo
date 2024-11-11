using System;
using System.Collections;
using Legacy.Database;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
using UnityEngine.UI;
using DG.Tweening;
using static Legacy.Client.CardProgressBarBehaviour;

namespace Legacy.Client
{
    public class CardViewBehaviour : MonoBehaviour
    {

        public ushort db_index { get { return index; } }

        public Image IconContent { get => Icon; }
        public GameObject ManaCostText { get => ManaCost; }

        private ushort index;

        [SerializeField] private GameObject ManaCost;

        [SerializeField] private GameObject LootParticles;

        [SerializeField] private Image touchedBackground;

        [SerializeField] public CardProgressBarBehaviour ProgressBar;


        [SerializeField] private RaritySprites RarityFrameSprites;

        [SerializeField] public NewSubstracteBehaviour LabelNew;

        [SerializeField] public GameObject OpenContent;

        [SerializeField] [Range(0, 30)] private float sinAmplitude;

        [SerializeField] public Image Icon;
        [SerializeField] public Image RarityIcon;

        [SerializeField] private CardGlowState glowState;
        [SerializeField] public CardGlowState previousState = CardGlowState.Update;

        //[Foldout("Frames", true)]
        [SerializeField] public Image   cardIconMask;
        [SerializeField] public Sprite  simpleMask;
        [SerializeField] public Sprite  legendMask;
        [Space]
        [SerializeField] public Image   upgradeGlowFrame;
        [SerializeField] public Sprite  simpleGlowFrame;
        [SerializeField] public Sprite  legendGlowFrame;
        [Space]
        [SerializeField] public Image   outerFrame;
        [SerializeField] public Sprite  simpleOuterFrame;
        [SerializeField] public Sprite  legendOuterFrame;
        [Space]
        [SerializeField] public Image   innerFrame;
        [SerializeField] public Sprite  simpleInnerFrame;
        [SerializeField] public Sprite  legendInnerFrame;
        [Space]
        [SerializeField] private Sprite outerFrameSpriteCommon;
        [SerializeField] private Sprite outerFrameSpriteRare;
        [SerializeField] private Sprite outerFrameSpriteEpic;
        [SerializeField] private Sprite outerFrameSpriteLegendary;
        
        private CardTextDataBehaviour textData;

        public bool CanUpdate => textData? textData.CanUpdate:false;

        private bool isGray = false;
        private CardRarity rarity;
        public BinaryCard binaryCard;

        private ProfileInstance _profileInstance;
        private ProfileInstance ProfileInst
        {
            get
            {
                if (_profileInstance == null)
                    _profileInstance = ClientWorld.Instance.Profile;
                return _profileInstance;
            }
        }

        internal void Init(BinaryCard binaryCard)
        {
            textData = GetComponent<CardTextDataBehaviour>();
            this.binaryCard = binaryCard;
            SetRarity(binaryCard.rarity);
            rarity = binaryCard.rarity;
            index = binaryCard.index;            

            SetIconSprite(VisualContent.Instance.CardIconsAtlas.GetSprite(binaryCard.icon));

            ProgressBar.ProgressValue.FullBarEvent.AddListener(() =>
            {
                SetStateCanUpdate(CanUpdate, (CardGlowState)Convert.ToInt32(CanUpdate));
            });
            //ProgressBar.gameObject.SetActive(true);
            if ( WindowManager.Instance.CurrentWindow is ShopWindowBehaviour)
            {
                OpenTextInShop();
            }
        }

        internal void AddCurrencyBehaviour(CurrencyType type, uint count)
        {
            var currency = gameObject.AddComponent<CurrencyCardBehaviour>();
            currency.view = this;
            currency.Init(type, count);
        }

        public void OpenTextInShop()
        {
            ProfileInstance Profile = ClientWorld.Instance.Profile;
            bool find = false;
           foreach(ushort cardID in Profile.DecksCollection.NotFound)
            {
                if(cardID == index)
                {
                    find = true;
                    break;
                }
            }
            if (!find)
            {
                foreach (ushort cardID in Profile.DecksCollection.Unavailable)
                {
                    if (cardID == index)
                    {
                        find = true;
                        break;
                    }
                }
            }

			if (ClientWorld.Instance.Profile.Inventory.GetCardData(index).level > 0 && ClientWorld.Instance.Profile.Inventory.GetCardData(index).count > 0)
			{
                find = false;
            }

            if (find)
            {
                OpenContent.SetActive(true);
                SetLabelNew(true);
                ProgressBar.gameObject.SetActive(false);
			}
			else
			{
                OpenContent.SetActive(false);
                SetLabelNew(false);
                ProgressBar.gameObject.SetActive(true);
            }
            
        }
        public void SetIconSprite(Sprite sprite)
        {
            Icon.sprite = sprite;
        }

        public void Init(BinaryCardRandomRarity binaryRandomRarity)
        {
            rarity = binaryRandomRarity.rarity;
            index = 0;            

            SetRarityIconSprite(VisualContent.Instance.CardIconsAtlas.GetSprite("back_card_" + rarity.ToString().ToLower()));
            MakeGray(false);
        }

        public void SetUpLegendaryCardFrames()
        {
            upgradeGlowFrame.type   = Image.Type.Simple;
            outerFrame.type         = Image.Type.Simple;
            innerFrame.type         = Image.Type.Simple;
            cardIconMask.type       = Image.Type.Simple;

            var rect    = upgradeGlowFrame.GetComponent<RectTransform>().rect;
            //rect.width  = legendGlowFrame.bounds.size.x;
            rect.height = legendGlowFrame.bounds.size.y;
            upgradeGlowFrame.sprite = legendGlowFrame;
            upgradeGlowFrame.SetNativeSize();

            rect = outerFrame.GetComponent<RectTransform>().rect;
            //rect.width = legendOuterFrame.bounds.size.x;
            rect.height = legendOuterFrame.bounds.size.y;
            outerFrame.sprite = legendOuterFrame;
            outerFrame.SetNativeSize();

            rect = innerFrame.GetComponent<RectTransform>().rect;
            //rect.width = legendInnerFrame.bounds.size.x;
            rect.height = legendInnerFrame.bounds.size.y;
            innerFrame.sprite = legendInnerFrame;
            innerFrame.SetNativeSize();

            rect = cardIconMask.GetComponent<RectTransform>().rect;
            rect.height = legendMask.bounds.size.y;
            //rect.width = legendMask.bounds.size.x;
            cardIconMask.sprite = legendMask;
            cardIconMask.SetNativeSize();
        }

        public void SetUpRegularCardFrames()
        {
            upgradeGlowFrame.type   = Image.Type.Sliced;
            outerFrame.type         = Image.Type.Sliced;
            innerFrame.type         = Image.Type.Sliced;
            cardIconMask.type       = Image.Type.Sliced;

            var rect = upgradeGlowFrame.GetComponent<RectTransform>().rect;
            rect.height = simpleGlowFrame.bounds.size.y;
            upgradeGlowFrame.sprite = simpleGlowFrame;

            rect = outerFrame.GetComponent<RectTransform>().rect;
            rect.height = simpleOuterFrame.bounds.size.y;
            outerFrame.sprite = simpleOuterFrame;

            rect = innerFrame.GetComponent<RectTransform>().rect;
            rect.height = simpleInnerFrame.bounds.size.y;
            innerFrame.sprite = simpleInnerFrame;

            rect = cardIconMask.GetComponent<RectTransform>().rect;
            rect.height = simpleMask.bounds.size.y;
            cardIconMask.sprite = simpleMask;
        }

        public void SetRarityIconSprite(Sprite sprite)
        {
            RarityIcon.gameObject.SetActive(true);
            RarityIcon.sprite = sprite;
        }

        public void SetLabelNew(bool flag = true)
        {
            LabelNew.UpdateLableNew(binaryCard.index, flag);
        }

        void SetRarity(CardRarity rarity)
        {
            ProgressBar.SetRarity(rarity);
            switch (rarity)
            {
                case CardRarity.Common:
                    outerFrame.sprite = outerFrameSpriteCommon;            
                    break;
                case CardRarity.Rare:
                    outerFrame.sprite = outerFrameSpriteRare;
                    break;
                case CardRarity.Epic:
                    outerFrame.sprite = outerFrameSpriteEpic;
                    break;
                case CardRarity.Legendary:
                    SetUpLegendaryCardFrames();
                    break;
            }

            //if (rarity != CardRarity.Legendary)
            //{
            //    var alterColor = color;
            //    alterColor.r = (byte)(color.r * 1.2f > byte.MaxValue ? byte.MaxValue : color.r * 1.2f);
            //    alterColor.g = (byte)(color.g * 1.2f > byte.MaxValue ? byte.MaxValue : color.g * 1.2f);
            //    alterColor.b = (byte)(color.b * 1.2f > byte.MaxValue ? byte.MaxValue : color.b * 1.2f);
            //    var s = DOTween.Sequence();
            //    s.Append(GlowImage.DOColor(alterColor, 0.75f));
            //    s.Append(GlowImage.DOColor(color, 0.75f));
            //    s.SetLoops(-1);
            //}
            //else
            //{
            //    var alterColor = new Color32(255, 112, 245, 255);
            //    var alterColor2 = Color.white;

            //    var s = DOTween.Sequence();
            //    s.Append(GlowImage.DOColor(alterColor2, 0.75f));
            //    s.Append(GlowImage.DOColor(alterColor, 0.75f));
            //    s.Append(GlowImage.DOColor(color, 0.75f));
            //    s.SetLoops(-1);
            //}
        }

        public CardGlowState GetState()
        {
            return glowState;
        }

        internal void Glow(bool glow)
        {
            //upgradeGlowFrame.gameObject.SetActive(glow);
        }
        internal void SetShadow(bool glow)
        {
            //Shadow.gameObject.SetActive(glow);
        }
        internal void SetGlow(bool glow)
        {
            //upgradeGlowFrame.enabled = (glow);
        }
        internal void SetFrame(bool glow)
        {
            //Frame.gameObject.SetActive(glow);
        }
        internal void SetStroke(bool enabled)
        {
            //Stroke.enabled = (enabled);
        }
        internal void StrokeGO(bool enabled)
        {
            //Stroke.gameObject.SetActive(enabled);
        }

        private void SetStateColor(CardGlowState glowState)
        {
            if (this.glowState != CardGlowState.Tremor)
                previousState = this.glowState;
            this.glowState = glowState;

            StopAllCoroutines();
            SetStroke(true);
            Color32 color = VisualContent.Instance.GetRarityState(glowState, rarity);
            if (rarity == CardRarity.Common && glowState!= CardGlowState.Update)
            {
                color.a = 0;
            }
            upgradeGlowFrame.color = color;

            if (glowState == CardGlowState.Tremor)
            {
                SetStroke(false);
                StartCoroutine(TremorColorGlowAnim(upgradeGlowFrame, color, new Color32(254, 255, 204, 255)));
            }
        }

        private IEnumerator TremorColorGlowAnim(Graphic target, Color32 startColor, Color32 endColor)
        {
            float ElapsedTime = 0f;
            while (glowState == CardGlowState.Tremor)
            {
                var percentageComplete = (Mathf.Sin(ElapsedTime * sinAmplitude) + 1) / 2;
                target.color = Color.Lerp(startColor, endColor, percentageComplete);
                ElapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        void SetRarityColor(Color32 color)
        {
            //var main = LootParticles.GetComponent<ParticleSystem>().main;
            //main.startColor = new ParticleSystem.MinMaxGradient(color);  
        }

        internal void MakeGray(bool toggle)
        {
            isGray = toggle;
            Icon.material = toggle ? VisualContent.Instance.GrayScaleMaterial : null;
            RarityIcon.material = toggle ? VisualContent.Instance.GrayScaleMaterial : null;
            outerFrame.material = toggle ? VisualContent.Instance.GrayScaleMaterial : null;
            upgradeGlowFrame.enabled = !toggle;
            if (ManaCost.activeInHierarchy)
            {
                ManaCost.GetComponent<Image>().material = toggle ? VisualContent.Instance.GrayScaleMaterial : null;
            }
        }

        internal void EnableManaImage(bool enable)
        {
            ManaCost.SetActive(enable);
        }

        internal void SetStateCanUpdate(bool? can_update, CardGlowState glowState)
        {
            SetStateColor(glowState);

            Glow(true);
            ProgressBar.SetCanUpdate(can_update, IsEnoughMoneyToUpgradeCard(binaryCard));
        }

        internal void SetRaycastTarget(bool toggle)
        {
            Icon.raycastTarget = toggle;
            RarityIcon.raycastTarget = toggle;
        }

        public void BecomeRaycasted(bool flag)
        {
            upgradeGlowFrame.GetComponent<Image>().raycastTarget = flag;
            Icon.GetComponent<Image>().raycastTarget = flag;
            RarityIcon.GetComponent<Image>().raycastTarget = flag;
        }

        private bool IsEnoughMoneyToUpgradeCard(BinaryCard card)
        {
            var cardData        = ProfileInst.Inventory.GetCardData(card.index);
            var softToUpgrade   = cardData.SoftToUpgrade;
            var totalSoft       = ProfileInst.Stock.getItem(CurrencyType.Soft).Count;
            return totalSoft    >= softToUpgrade;
        }
    }
}
