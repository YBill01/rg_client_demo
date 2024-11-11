using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardWindowRarityPanelBehaviour : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI RarityText;

    internal void SetRarity(CardRarity rarity)
    {
        RarityText.text = rarity.ToString();
    }
}
