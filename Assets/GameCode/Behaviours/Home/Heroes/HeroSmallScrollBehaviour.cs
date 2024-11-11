using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class HeroSmallScrollBehaviour : ScrollRect
    {
        public ScrollRect MainScroll { get; set; }

        public bool isScrolling = false;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            isScrolling = true;
            base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            isScrolling = false;
            base.OnEndDrag(eventData);
        }
    }
}