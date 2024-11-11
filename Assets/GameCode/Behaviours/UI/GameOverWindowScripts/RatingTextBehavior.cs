using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RatingTextBehavior : MonoBehaviour
{
    public void PaintText(int value)
    {
        Color32 greenColor = new Color32(87, 222, 60,255);
        Color32 redColor = new Color32(221, 80, 41,255);
        var color = value > 0 ? greenColor : redColor;
        var additionalText = value > 0 ? "+" : "";
        this.GetComponentInChildren<TextMeshProUGUI>().color = color;
        this.GetComponentInChildren<TextMeshProUGUI>().text = additionalText + value;
    }
}
