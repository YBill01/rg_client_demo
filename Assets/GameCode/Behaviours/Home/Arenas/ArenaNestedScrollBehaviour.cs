using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class ArenaNestedScrollBehaviour : ScrollRect
    {
        ScrollRect MainScrollRect;
        public bool _draggingParent = false;

        private RectTransform SliderLayoutRect;

        public void SetSliderRect(RectTransform slidersRect)
        {
            SliderLayoutRect = slidersRect;
        }

        protected override void Awake()
        {
            base.Awake();
            MainScrollRect = GetMainScrollRect(transform);
        }

        private ScrollRect GetMainScrollRect(Transform t)
        {
            if (t.parent != null)
            {
                ScrollRect scroll = t.parent.GetComponent<ScrollRect>();
                if (scroll != null) return scroll;
                else return GetMainScrollRect(t.parent);
            }

            return null;
        }

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            base.OnInitializePotentialDrag(eventData);
            MainScrollRect?.OnInitializePotentialDrag(eventData);
        }

        bool IsScrollable()
        {
            return (transform as RectTransform).rect.width < content.rect.width;
        }

        bool IsPotentialParentDrag(Vector2 delta)
        {
            if (MainScrollRect != null)
            {
                if (normalizedPosition.x > 0.999f && delta.x < 0.0f && !last)
                {
                    return true;
                }

                if (normalizedPosition.x < 0.001f && delta.x > 0.0f && !first)
                {
                    return true;
                }
            }

            return false;
        }

        private bool autoScrolling = true;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            autoScrolling = false;
            if (!IsScrollable() || IsPotentialParentDrag(eventData.delta))
            {
                MainScrollRect.OnBeginDrag(eventData);
                _draggingParent = true;
            }
            else
            {
                base.OnBeginDrag(eventData);
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (_draggingParent)
            {
                if (!IsScrollable() || IsPotentialParentDrag(eventData.delta))
                {
                    MainScrollRect.OnDrag(eventData);
                }
                else
                {
                    _draggingParent = false;
                    base.OnBeginDrag(eventData);
                }
            }
            else
            {
                if (!IsScrollable() || IsPotentialParentDrag(eventData.delta))
                {
                    MainScrollRect.OnBeginDrag(eventData);
                    _draggingParent = true;
                }
                else
                {
                    base.OnDrag(eventData);
                }
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            if (MainScrollRect != null && _draggingParent)
            {
                _draggingParent = false;
                MainScrollRect.OnEndDrag(eventData);
            }
        }

        private Vector2 currentNormalizedPos = Vector2.zero;

        public void SetScrollPosition(float myRatingXPos)
        {
            autoScrolling = true;
            float ScrollPosition;
            float halfViewPortWidth = viewport.rect.width / 2;
            if (myRatingXPos < halfViewPortWidth)
            {
                ScrollPosition = 0.0f;
            }
            else
            {
                if (SliderLayoutRect.rect.width - myRatingXPos < halfViewPortWidth)
                {
                    ScrollPosition = 1.0f;
                }
                else
                {
                    ScrollPosition = myRatingXPos / SliderLayoutRect.rect.width;
                }
            }

            currentNormalizedPos.x = Mathf.Clamp01(ScrollPosition);
        }

        public void EnableAutoScroll()
        {
            autoScrolling = true;
        }

        private bool first = false;
        private bool last = false;

        internal void SetFirstLast(bool first, bool last)
        {
            this.first = first;
            this.last = last;
            movementType = MovementType.Elastic;
        }

        void Update()
        {
            if (autoScrolling)
            {
                normalizedPosition = Vector2.Lerp(normalizedPosition, currentNormalizedPos, 0.2f);
            }
        }
    }
}