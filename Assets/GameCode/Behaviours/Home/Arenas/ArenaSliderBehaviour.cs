using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class ArenaSliderBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private RectTransform SlidersRect;
        [SerializeField] 
        private RectTransform Fill;
        [SerializeField] 
        private RectTransform MaxFill;
        [SerializeField, Range(0.0f, 1.0f)] 
        private float SlideSpeed = 0.1f;

        private Vector2 CurrentMaxFillMaxAnchor = Vector2.zero;
        private Vector2 CurrentFillMaxAnchor = Vector2.zero;
        private bool isMovingOn = true;
        private bool startFromEnd = false;

        private const float ADDED_POSITION = 21f;

        public void SetMaxFill(float xPos)
        {
            xPos += ADDED_POSITION;
            CurrentMaxFillMaxAnchor.x = xPos / SlidersRect.rect.width;
        }

        public void SetFill(float xPos)
        {
            xPos += ADDED_POSITION;
            CurrentFillMaxAnchor.x = xPos / SlidersRect.rect.width;
        }

        public void SetMaximum()
        {
            CurrentFillMaxAnchor.x = 1;
        }

        public void SetMinimum()
        {
            CurrentFillMaxAnchor.x = 0;
        }

        public void SetMinimumMaxRating()
        {
            CurrentMaxFillMaxAnchor.x = 0;
        }

        public void SetMaximumMaxRating()
        {
            CurrentMaxFillMaxAnchor.x = 1;
        }

        public void SetIsMovingOn(bool value)
        {
            isMovingOn = value;
        }

        public void MoveToEnd()
        {
            SetMaximum();
            SetMaximumMaxRating();
            startFromEnd = true;
        }

        void Start()
        {
            Fill.anchorMax = Vector2.zero;
            MaxFill.anchorMax = Vector2.zero;
        }

        void Update()
        {
            if (isMovingOn)
            {
                Fill.anchorMax = Vector2.Lerp(Fill.anchorMax, CurrentFillMaxAnchor, SlideSpeed);
                MaxFill.anchorMax = Vector2.Lerp(MaxFill.anchorMax, CurrentMaxFillMaxAnchor, SlideSpeed * 2);
            }
            else if(startFromEnd)
            {
                startFromEnd = false;
                Fill.anchorMax = CurrentFillMaxAnchor;
                MaxFill.anchorMax = CurrentMaxFillMaxAnchor;
            }
        }
    }
}