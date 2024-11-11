using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class LootMiniCardBehaviour : MonoBehaviour
    {
        [SerializeField] Image Icon;
        [SerializeField] TextMeshProUGUI CountText;
        [SerializeField] LootCardPopUpBehaviour PopUpCard;

        private LootCardPopUpBehaviour LootPopUp;

        public void OnEnable()
        {
            LootPopUp = null;
        }
        public void Init(CardRarity rarity, ushort count)
        {
            Icon.sprite = VisualContent.Instance.GetMiniCardIconRarity(rarity);
            CountText.text = count.ToString();
        }

        internal void EnablePopUpCard(LootCardPopUpBehaviour loot_card)
        {
            LootPopUp = loot_card;
            LootPopUp.GetComponent<RectTransform>().position = Icon.GetComponent<RectTransform>().position;
        }

        private void OnMouseOver()
        {
            if (LootPopUp != null)
            {
                LootPopUp.On();
            }
        }

        private void OnMouseExit()
        {
            if (LootPopUp != null)
            {
                LootPopUp.Off();
            }
        }
    }
}
