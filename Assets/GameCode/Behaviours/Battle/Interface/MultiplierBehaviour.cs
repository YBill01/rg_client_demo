using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Legacy.Game
{
    public class MultiplierBehaviour : MonoBehaviour
    {
        public Color32 multipliedColor;
        public Color32 regularColor;

        private int multiplier = 100;
        void Bump()
        {
            GetComponent<Animator>().Play("Bump");
        }

        private Color32 GetColor()
        {
            return multiplier > 100 ? multipliedColor : regularColor;            
        }

        private string GetTextValue()
        {
            string value = "x1";
            if(multiplier > 100 && multiplier < 106)
            {
                value = "x1.05";
            }
            if(multiplier > 105)
            {
                value = "x1.1";
            }
            return value;
        }       

        internal void Set(bool isMy)
        {
            multiplier += isMy ? 5 : -5;
            if(multiplier < 100)
            {
                multiplier = 100;
            }
            GetComponent<TextMeshProUGUI>().color = GetColor();
            GetComponent<TextMeshProUGUI>().text = GetTextValue();
            Bump();
        }
    }
}
