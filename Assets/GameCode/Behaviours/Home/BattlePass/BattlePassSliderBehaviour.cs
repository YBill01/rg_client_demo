using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class BattlePassSliderBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private RectTransform SlidersRect;
        [SerializeField] 
        private RectTransform Fill;
        [SerializeField] 
        private RectTransform ShadowFill;
        [SerializeField, Range(0.0f, 1.0f)] 
        private float SlideSpeed = 0.1f;

        private Vector2 currentFillMaxAnchor = Vector2.zero;

        public void SetFill(int starsInLevel, int level, int maxLevel)
        {
            float oneLevel = 1f / (float) maxLevel;
            float currentLevel = oneLevel * (level + 1);
            float oneStar = oneLevel / PlayerProfileBattlePass.STARS_IN_LEVEL;
            float currentStar = starsInLevel * oneStar;
            
            currentFillMaxAnchor.x = Mathf.Min(currentLevel + currentStar, 1.0f);
        }

        void Start()
        {
            Fill.anchorMax = Vector2.zero;
        }

        void Update()
        {
            Fill.anchorMax = Vector2.Lerp(Fill.anchorMax, currentFillMaxAnchor, SlideSpeed);
            ShadowFill.anchorMin = Vector2.Lerp(ShadowFill.anchorMin, currentFillMaxAnchor, SlideSpeed);
        }
    }
}
