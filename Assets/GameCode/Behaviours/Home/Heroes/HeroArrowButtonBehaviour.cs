using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class HeroArrowButtonBehaviour : MonoBehaviour
    {
        [SerializeField] private Image Arrow;
        [SerializeField] private HeroNameColorBehaviour Name;

        internal void SetHero(string name, Color color)
        {
            Name.SetName(name, color);
            Arrow.color = color;
        }
    }
}
