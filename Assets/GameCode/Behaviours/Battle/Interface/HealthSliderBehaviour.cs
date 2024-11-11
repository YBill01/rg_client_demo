using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthSliderBehaviour : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField]
    private RectTransform MainRect;
    [SerializeField]
    private bool HeroEnemy = false;
    [SerializeField]
    public RectTransform lerpImage;
    [SerializeField]
    private RectTransform fillRect;

    [SerializeField]
    private Image Filler;

    [SerializeField]
    private Image Back;

    public Sprite redSpriteBar;
    public Sprite redSpriteBarBack;

    [SerializeField, Range(0f, 1f)]
    private float WhiteLerpIntencity = 0.1f;

    private Vector2 CurrentAnchor;

    public float CurrentValue { get; private set; }
    public bool IsActive => MainRect.sizeDelta.x > 0;
    private const float sliderOffsetMax = 0.15f;
    private const float sliderOffsetMin = 0f;

    public void SetRedSprites()
    {
        Filler.sprite = redSpriteBar;
        Back.sprite = redSpriteBarBack;
    }

    void Start()
    {
        CurrentValue = 0.0f;
        CurrentAnchor = HeroEnemy ? fillRect.anchorMin : fillRect.anchorMax;
    }

    public void SetHeroEnemy(bool flag)
    {
        HeroEnemy = flag;
    }
    internal void SetValue(float value,bool isHero = true, bool shouldViewLarge = false,MinionLayerType _layer = MinionLayerType.Ground)
    {
        if (CurrentValue != value)
        {
            if (CurrentValue > 0)
            {
                if (animator != null)
                {
                    string animationName;

                    if (_layer == MinionLayerType.Building)
                    {
                        if (shouldViewLarge)
                        {
                            animationName = "RedFlash";
                        }
                        else
                        {
                            animationName = "Empty";
                        }
                    }
                    else
                    {
                        if (shouldViewLarge)
                        {
                            if (value > CurrentValue)
                            {
                                animationName = "GreenFlash";
                            }
                            else
                            {
                                animationName = "RedFlash";
                            }
                        }
                        else animationName = "SmallDamageFlash";
                    }


                    animator.Play(animationName);
                }
            }
        }

        CurrentValue = value;
            if(!isHero)
            CurrentAnchor.x = HeroEnemy ? (1.0f - value + GetSliderOffsetByPersentage(1.0f - value*100)) : value + GetSliderOffsetByPersentage(value * 100);
            else
                CurrentAnchor.x = HeroEnemy ? (1.0f - value) : value;
    }

    internal void SetWidth(float width)
    {
        MainRect.sizeDelta = new Vector2(width, MainRect.sizeDelta.y);
    }

    private float GetSliderOffsetByPersentage(float percentages)
    {
        return sliderOffsetMin + ((sliderOffsetMax- sliderOffsetMin) * (100 - percentages) / 100);
    }

    void Update()
    {
        if (HeroEnemy)
        {
            fillRect.anchorMin = CurrentAnchor;
        }
        else
        {
            fillRect.anchorMax = CurrentAnchor;
        }

        lerpImage.anchorMax = Vector2.Lerp(lerpImage.anchorMax, fillRect.anchorMax, WhiteLerpIntencity);
        lerpImage.anchorMin = Vector2.Lerp(lerpImage.anchorMin, fillRect.anchorMin, WhiteLerpIntencity);
    }
}
