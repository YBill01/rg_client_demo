using Legacy.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class LootSlotBeahaviour : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI ValueText; 
        [SerializeField] Image Icon;
        [SerializeField] LootCardPopUpBehaviour PopUpCard;

        private LootCardPopUpBehaviour LootPopUp;

        public void Init(CurrencyType type, int valueMin, uint valueMax = 0)
        {
            Icon.sprite = VisualContent.Instance.GetCurrencyIcon(type);
            string text = valueMin.ToString();
            if (valueMax > valueMin)
            {
                text += $" - {valueMax}";
            }
            ValueText.text = text;
        }

        public void Init(CardRarity rarity, ushort count)
        {
            Icon.sprite = VisualContent.Instance.GetMiniCardIconRarity(rarity);
            ValueText.text = "<size=50%>x </size>"+ LegacyHelpers.FormatByDigits(count.ToString());
        }

        public void OnEnable()
        {
            LootPopUp = null;
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