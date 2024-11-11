using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroIconBehaviour : MonoBehaviour
{
    private RectTransform ContainerRect;

    [SerializeField]
    private RectTransform IconRect;

    [SerializeField]
    private Image IconImage;

    [SerializeField]
    private bool isEnemy;

    [SerializeField]
    private BattleStartWindowBehaviour BattleStartWindow;

    private bool offsetted;
    [SerializeField, Range(0.0f, 1.0f)]
    public float EffectAmount = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)]
    public float Opacity = 1.0f;

    void Update()
    {
        if (IconImage.material != null)
        {
            IconImage.material.SetFloat("_EffectAmount", EffectAmount);
            IconImage.material.SetFloat("_Opacity", Opacity);
        }
    }
    internal void Set(Sprite icon_sprite)
    {
        IconImage.sprite = icon_sprite;
      //  IconImage.SetNativeSize();
    }

    internal void RemoveMaterial()
    {
        IconImage.material = null;
    }
}
