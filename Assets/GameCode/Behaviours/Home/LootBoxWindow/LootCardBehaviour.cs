using Legacy.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Legacy.Client.LootBoxWindowBehaviour;

namespace Legacy.Client {
    public class LootCardBehaviour : MonoBehaviour
    {

        private uint count = 0;
        public LootCardType type;

        public byte indexInGrid = 0;

        public bool appeared = false;
        public bool appearedInGrid = false;

        public BinaryCard BinaryCard => view.binaryCard;

        ClientCardData PlayerCard;

        [SerializeField] CardViewBehaviour view;
        [SerializeField] CardTextDataBehaviour data;
        [SerializeField] TextMeshProUGUI AddCountText;
        [SerializeField] TextMeshProUGUI CardNameText;
        [SerializeField] TextMeshProUGUI RarityText;
        [SerializeField] GameObject UpgradeObject;
        [SerializeField] GameObject LevelObject;
        [SerializeField] Image FillerImage;
        [SerializeField] ParticleSystem OpenedEffect;
        private uint currencyValue;

        private CardRarity cardRarity = CardRarity.Common;
        private CardsSettings cardSettings;
        public void Init(ushort index, uint count, LootCardType type, byte indexInGrid)
        {
            this.indexInGrid = indexInGrid;
            this.type = type;
            this.count = count;
            AddCountText.text = "+" + count.ToString();
            if (Cards.Instance.Get(index, out BinaryCard binaryCard))
            {
                view.Init(binaryCard);
                LevelObject.SetActive(true);
                CardNameText.text = binaryCard.GetTitle();
                RarityText.text = binaryCard.GetRarityString();
                cardRarity = binaryCard.rarity;
                RarityText.color = VisualContent.Instance.GetRarityColor(binaryCard.rarity);
                PlayerCard = ClientWorld.Instance.Profile.Inventory.GetCardData(binaryCard.index);
                if (PlayerCard.level == 0 && PlayerCard.count == 0)
                {
                    cardSettings = Database.Settings.Instance.Get<CardsSettings>();
                    byte StartCardLevel = cardSettings.GetStartLevel(binaryCard.index);
                    data.SetLevel(StartCardLevel);
                    var CardsToUpgrade = Levels.Instance.GetCountToUpgradeCard(binaryCard.index, StartCardLevel, UpgradeCostType.CardsCount);
                    data.SetCount(0, CardsToUpgrade);
                }
                else { 
                    data.SetLevel(PlayerCard.level);
                    data.SetCount(PlayerCard.count, PlayerCard.CardsToUpgrade);
                }
                view.ProgressBar.ProgressValue.FullBarEvent.AddListener(() => UpgradeObject.SetActive(true));

            }
            else
            {
                if(type != LootCardType.Cards)
                {
                    currencyValue = 0;
                    RarityText.text = Locales.Get("locale:1087");
                    switch (type)
                    {
                        case LootCardType.Hard:
                            CardNameText.text = Locales.Get("locale:832");
                            RarityText.color = VisualContent.Instance.GetRarityColor(CardRarity.Epic);
                            view.AddCurrencyBehaviour(CurrencyType.Hard, count);
                            currencyValue = ClientWorld.Instance.Profile.Stock.GetCount(CurrencyType.Hard);
                            break;
                        case LootCardType.Soft:
                            CardNameText.text = Locales.Get("locale:835");
                            RarityText.color = VisualContent.Instance.GetRarityColor(CardRarity.Common);
                            currencyValue = ClientWorld.Instance.Profile.Stock.GetCount(CurrencyType.Soft);
                            view.AddCurrencyBehaviour(CurrencyType.Soft, count);
                            break;
                        //case LootCardType.Shards:
                            //CardNameText.text = Locales.Get("locale:1297");
                            //RarityText.color = VisualContent.Instance.GetRarityColor(CardRarity.Rare);
                            //currencyValue = ClientWorld.Instance.Profile.Stock.GetCount(CurrencyType.Shards);
                            //view.AddCurrencyBehaviour(CurrencyType.Shards, count);
                            //break;
                    }
                    view.ProgressBar.ProgressValue.Set(currencyValue);
                    // may be needed to set bar color 
                }
            }
        }

        public void PlayAppearSound()
        {
            GetComponent<AudioSource>().Play();
        }
        public void PlayAppearEffect()
        {
            OpenedEffect.gameObject.SetActive(true);
        }

        internal void ShowLoot()
        {
            if(type == LootCardType.Cards)
            {
                view.GetComponent<CardViewBehaviour>().SetLabelNew(true);
            }

            gameObject.SetActive(true);
        }

        //Called from LootCardAnimator when Appear animation clip ends
        public void AppearAnimationFinished()
        {
            if (type == LootCardType.Cards)
            {
                data.AddCards(count);
            }
            else
            {
                view.ProgressBar.ProgressValue.ChangeValue(currencyValue + count);
            }
        }

        internal void DropParticles()
        {
            if(type == LootCardType.Cards)
            {
                RewardParticlesBehaviour.Instance.Drop(
                        Vector3.zero,
                        (byte)count,
                        type,
                        cardRarity
                    );
            }
            else
            {
                RewardParticlesBehaviour.Instance.Drop(
                        Vector3.zero,
                        (byte)count,
                        type
                    );
            }
        }

        internal CardRarity GetRarity()
        {
            if(type == LootCardType.Cards)
            {
                return BinaryCard.rarity;
            }
            else
            {
                return CardRarity.Common;
            }
        }
    }
}
