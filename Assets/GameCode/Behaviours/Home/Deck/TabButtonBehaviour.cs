using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabButtonBehaviour : MonoBehaviour
{
    [SerializeField]
    private Image buttonImage;

    [SerializeField]
    private Color ActiveColor;
    [SerializeField]
    private Color RegularColor;

    [SerializeField]
    private byte number;


    internal void Init(byte deckNumber)
    {
        if (deckNumber == number)
        {
            buttonImage.color = ActiveColor;
        }
        else
        {
            buttonImage.color = RegularColor;
        } 
    }
}
