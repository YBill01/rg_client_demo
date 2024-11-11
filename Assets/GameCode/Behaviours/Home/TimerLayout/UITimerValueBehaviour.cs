using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class UITimerValueBehaviour : MonoBehaviour
    {
        public enum TimerValueType
        {
            d,
            h,
            m,
            s
        }

        [SerializeField, Range(0.0f, 1.0f)] float LerpSpeed;

        [SerializeField] RectTransform ValueRect;
        [SerializeField] RectTransform ValueTypeRect;

        [SerializeField] TextMeshProUGUI ValueText;
        [SerializeField] TextMeshProUGUI ValueTypeText;
        TimerValueType ValueType;

        float currentValue;
        byte newValue;

        [SerializeField] RectTransform MainRect;

        void Update()
        {           
            if ((byte)currentValue != newValue)
            {
                currentValue = Mathf.Lerp(currentValue, newValue, LerpSpeed);
                ValueText.text = Mathf.RoundToInt(currentValue).ToString();
                LayoutRebuilder.ForceRebuildLayoutImmediate(MainRect);
            }
        }

        public void SetValue(byte value)
        {
            newValue = value;            
        }

        internal void Init(float valueFontSize, float valueTypeFontSize, TimerValueType type)
        {
            ValueType = type;
            ValueTypeText.fontSize = valueTypeFontSize;
            ValueText.fontSize = valueFontSize;
            ValueTypeText.text = GetTypeString();
            ValueTypeRect.offsetMax = new Vector2(
                ValueTypeRect.offsetMax.x,
                valueTypeFontSize + ((valueFontSize - valueTypeFontSize) / 2) - ValueTypeRect.offsetMax.y
            );
        }

        string GetTypeString()
        {
            string text;
            switch (ValueType)
            {
                case TimerValueType.d:
                    text = Locales.Get("locale:769");
                    break;
                case TimerValueType.h:
                    text = Locales.Get("locale:772");
                    break;
                case TimerValueType.m:
                    text = Locales.Get("locale:1471");
                    break;
                case TimerValueType.s:
                    text = Locales.Get("locale:820");
                    break;
                default:
                    text = Locales.Get("locale:769");
                    break;
            }
            return text;
        }
    }
}
