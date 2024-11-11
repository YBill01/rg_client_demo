using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class LootBoxPopUpDataBehaviour : MonoBehaviour
    {
        //oldVersion
        //  [SerializeField] GameObject LootCurrencyPrefab;
          [SerializeField] GameObject LootMiniCardPrefab;

        [SerializeField] GameObject LootSlotPrefab;
        [SerializeField] GameObject LootSlotMiniPrefab;

        [SerializeField] LootCardPopUpBehaviour LootCardPopUp;

        [SerializeField] RectTransform CurrenciesLayout;
        [SerializeField] RectTransform MiniCardsLayout;
        [SerializeField] RectTransform MiniHardLayout;

        [SerializeField] TextMeshProUGUI ArenaNumberText;
        [SerializeField] TextMeshProUGUI BoxTitleText;

        //oldVersion
     //   List<LootCurrencyBehaviour> currenciesList = new List<LootCurrencyBehaviour>();
      //  List<LootMiniCardBehaviour> cardsList = new List<LootMiniCardBehaviour>();
        List<LootSlotBeahaviour> currenciesUpList = new List<LootSlotBeahaviour>();
        List<LootSlotBeahaviour> currenciesDownList = new List<LootSlotBeahaviour>();

        private bool isDownCars = true;
        
        BinaryLoot BinaryBox;
        byte battlefieldNumber;
        int settingsPercent;

        public void Init(BinaryLoot BinaryBox, ushort battlefield, bool isShop = false)
        {
            settingsPercent = isShop ? 0 : Settings.Instance.Get<LootSettings>().percent;
            this.BinaryBox = BinaryBox;

            battlefieldNumber = Settings.Instance.Get<ArenaSettings>().GetNumber(battlefield);
            UpdateData();
        }

        void UpdateData()
        {
            BoxTitleText.text = BinaryBox.GetTitle();
            ArenaNumberText.text = Locales.Get("locale:712") + " " + (battlefieldNumber + 1);

            UpdateUpContent();
            UpdateDownPanel();
            //old version
           // UpdateCurrencies();
           // UpdateMiniCards();
        }

        private void UpdateUpContent()
        {
            isDownCars = true;
            var percent = settingsPercent * battlefieldNumber;
            if (BinaryBox.currency.soft.min > 0)
            {
                CreateCurrencyNew(CurrencyType.Soft, percent, BinaryBox.currency.soft.min, BinaryBox.currency.soft.max, CurrenciesLayout, LootSlotPrefab, currenciesUpList);
            }
            var totalCount = BinaryBox.cards.total + Mathf.FloorToInt(BinaryBox.cards.total * 0.01f * percent);

            if(totalCount > 0)
            {
                if (BinaryBox.cards.rest.type == CardRarity.Common)
                {
                    CreateCards(totalCount, CardRarity.Common, CurrenciesLayout, LootSlotPrefab, currenciesUpList);
                }
                else
                {
                    bool rar = true;
                    BinaryBox.cards.options.ForEach((LootOption) =>
                    {
                        if(LootOption.type != BinaryBox.cards.rest.type)
                        {
                            rar = false;
                        }
                    });
                    if (rar)
                    {
                        isDownCars = false;
                        CreateCards(totalCount, BinaryBox.cards.rest.type, CurrenciesLayout, LootSlotPrefab, currenciesUpList);
                    }
                }
            }
        }
        private void UpdateDownPanel()
        {
            if (BinaryBox.currency.hard.min > 0)
            {
                CreateCurrencyNew(CurrencyType.Hard, 0, BinaryBox.currency.hard.min, BinaryBox.currency.hard.max, MiniHardLayout, LootSlotMiniPrefab, currenciesDownList);
            }
            if (isDownCars)
            {
                var percent = settingsPercent * battlefieldNumber;
                BinaryBox.cards.options.ForEach((LootOption) =>
                {
                    if (LootOption.type > CardRarity.Common)
                    {
                        ushort cardsCount = (ushort)Mathf.FloorToInt((LootOption.percent + percent) * 0.01f);
                        if (cardsCount > 0)
                        {
                            CreateCards(cardsCount, LootOption, MiniCardsLayout, LootSlotMiniPrefab, currenciesDownList);
                        }
                    }
                });
            }
            if (currenciesDownList.Count == 0)
            {
                MiniCardsLayout.parent.parent.gameObject.SetActive(false);
            }
            else
            {
                MiniCardsLayout.parent.parent.gameObject.SetActive(true);
            }
           

        }

        //oldVersion
        /*   private void UpdateMiniCards()
           {
               var percent = settingsPercent * battlefieldNumber;
               LootMiniCardBehaviour total = Instantiate(LootMiniCardPrefab, CurrenciesLayout).GetComponent<LootMiniCardBehaviour>();
               var totalCount = BinaryBox.cards.total + Mathf.FloorToInt(BinaryBox.cards.total * 0.01f * percent);
               total.Init(CardRarity.Common, (ushort)totalCount);
               cardsList.Add(total);


               BinaryBox.cards.options.ForEach((LootOption) => {
                   if (LootOption.type > CardRarity.Common)
                   {
                       ushort cardsCount = (ushort)Mathf.FloorToInt((LootOption.percent + percent) * 0.01f);
                       if (cardsCount > 0)

                       {
                          LootMiniCardBehaviour miniCard = Instantiate(LootMiniCardPrefab, MiniCardsLayout).GetComponent<LootMiniCardBehaviour>();
                          CardRarity type = LootOption.type;
                           if(LootOption.card > 0 && Cards.Instance.Get(LootOption.card, out BinaryCard _card))
                          {
                              type = _card.rarity;
                               LootCardPopUp.Init(_card, cardsCount);
                               miniCard.EnablePopUpCard(LootCardPopUp);
                           }                        
                           miniCard.Init(LootOption.type, cardsCount);

                          cardsList.Add(miniCard);
                       }
                   }
               });
           }*/
        // old variant
     /*   private void UpdateCurrencies()
        {
            var percent = settingsPercent * battlefieldNumber;
            if (BinaryBox.currency.soft.min > 0)
            {
                CreateCurrency(CurrencyType.Soft, percent, BinaryBox.currency.soft.min, BinaryBox.currency.soft.max);
            }
            if (BinaryBox.currency.hard.min > 0)
            {
                CreateCurrency(CurrencyType.Hard, 0, BinaryBox.currency.hard.min, BinaryBox.currency.hard.max);
            }
            //if (BinaryBox.currency.shard > 0)
            //{
                //CreateCurrency(CurrencyType.Shards, percent, BinaryBox.currency.shard);
            //}
        }*/

        private void CreateCurrencyNew(CurrencyType type, int percent, ushort min, ushort max = 0, RectTransform layout=default, GameObject prefab = default , List<LootSlotBeahaviour> arr =default)
        {
            LootSlotBeahaviour slot = Instantiate(prefab, layout).GetComponent<LootSlotBeahaviour>();
            min = (ushort)(min + Mathf.FloorToInt(min * percent * 0.01f));
            max = (ushort)(max + Mathf.FloorToInt(max * percent * 0.01f));
            slot.Init(type, min, max);
            arr.Add(slot);
        }
        private void CreateCards(int value, BinaryLootOption lootOption, RectTransform layout = default, GameObject prefab = default, List<LootSlotBeahaviour> arr = default)
        {
            LootSlotBeahaviour miniCard = Instantiate(prefab, layout).GetComponent<LootSlotBeahaviour>();
            miniCard.Init(lootOption.type, (ushort)value);

            CardRarity type = lootOption.type;
            if (lootOption.card > 0 && Cards.Instance.Get(lootOption.card, out BinaryCard _card))
            {
                type = _card.rarity;
                LootCardPopUp.Init(_card, (ushort)value);
                miniCard.EnablePopUpCard(LootCardPopUp);
            }
            arr.Add(miniCard);

            //old version
            //  miniCard.EnablePopUpCard(LootCardPopUp);

        }

        private void CreateCards(int value, CardRarity  cardType, RectTransform layout = default, GameObject prefab = default, List<LootSlotBeahaviour> arr = default)
        {
            LootSlotBeahaviour total = Instantiate(prefab, layout).GetComponent<LootSlotBeahaviour>();
            total.Init(cardType, (ushort)value);
            arr.Add(total);

            //old version
            //  var totalCount = BinaryBox.cards.total + Mathf.FloorToInt(BinaryBox.cards.total * 0.01f * percent);
            // LootMiniCardBehaviour miniCard = Instantiate(LootMiniCardPrefab, MiniCardsLayout).GetComponent<LootMiniCardBehaviour>();
            //   total.EnablePopUpCard(LootCardPopUp);

        }


        // old variant
       /* private void CreateCurrency(CurrencyType type, int percent, ushort min, ushort max = 0)
        {
            LootCurrencyBehaviour currency = Instantiate(LootCurrencyPrefab, CurrenciesLayout).GetComponent<LootCurrencyBehaviour>();
            min = (ushort)(min + Mathf.FloorToInt(min * percent * 0.01f));
            max = (ushort)(max + Mathf.FloorToInt(max * percent * 0.01f));
            currency.Init(type, min, max);
            currenciesList.Add(currency);
        }*/

        public void ResetData()
        {
            currenciesUpList.ForEach((LootSlotBeahaviour) => {
                DestroyImmediate(LootSlotBeahaviour.gameObject);
            });
            currenciesUpList.Clear();

            currenciesDownList.ForEach((LootSlotBeahaviour) => {
                DestroyImmediate(LootSlotBeahaviour.gameObject);
            });
            currenciesDownList.Clear();
            //old Version
          /*  currenciesList.ForEach((LootCurrencyBehaviour) => {
                DestroyImmediate(LootCurrencyBehaviour.gameObject);
            });
            cardsList.ForEach((LootCurrencyBehaviour) => {
                DestroyImmediate(LootCurrencyBehaviour.gameObject);
            });
            currenciesList.Clear();
            cardsList.Clear();*/
        }
    }
}
