
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Legacy.Client
{
    public class RotateHeroBehaviour : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField, Range(-5.0f, 5.0f)] private float rotateIntensity = 1.0f;

        public void OnBeginDrag(PointerEventData eventData)
        {
            MenuHeroesBehaviour.Instance.RotateHeroStart();
        }

        public void OnDrag(PointerEventData eventData)
        {
            MenuHeroesBehaviour.Instance.RotateHero(eventData.delta.x * rotateIntensity);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            MenuHeroesBehaviour.Instance.RotateHeroEnd();
        }
    }
}