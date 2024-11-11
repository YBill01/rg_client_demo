using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Database;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class BattlePassScrollContentBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private BattlePassLevelBehaviour levelPrefab;
        [SerializeField] 
        private BattlePassSliderBehaviour sliderBehaviour;
        [SerializeField] 
        private RectTransform contentHolder;
        [SerializeField] 
        private RectTransform contentParent;
        [SerializeField] [Range(0f, 1f)] 
        private float snapSpeed;

        private List<BattlePassLevelBehaviour> currentLevels;
        private float updateTime = 0.35f;
        private float timer;
        private bool ratingSetted;
        private bool isScrollingToMyRating;
        private Vector2 targetPosition;
        private int starsInCurrentLevel;
        private int allStars;
        private int currentLevel;
        private int treasuresCount;
        private ProfileInstance playerProfile;
        private Vector2 currentPosition;

        public void Init(BinaryBattlePass binaryBattlePass, BattlePassWindowBehaviour battlePassWindow)
        {
            playerProfile = ClientWorld.Instance.Profile;


            currentLevels = new List<BattlePassLevelBehaviour>();
            allStars = playerProfile.battlePass.stars;
            this.starsInCurrentLevel = playerProfile.battlePass.StarsInCurrentLevel;
            this.currentLevel = Mathf.Min(playerProfile.battlePass.CurrentLevel, binaryBattlePass.tresures.Count);
            treasuresCount = binaryBattlePass.tresures.Count;

            foreach (var treasure in binaryBattlePass.tresures)
            {
                var level = Instantiate(levelPrefab, contentHolder);
                level.Init(playerProfile, treasure.Value, treasure.Key, currentLevel, binaryBattlePass.timeStart, battlePassWindow, binaryBattlePass);
                currentLevels.Add(level);
            }

            playerProfile.PlayerProfileUpdated.AddListener(OnPlayerProfileUpdated);
        }

        private void OnPlayerProfileUpdated()
        {
            CheckStars();
            CheckPremium();
        }

        private void CheckStars()
        {
            if (allStars < playerProfile.battlePass.stars)
            {
                var newCurrentLevel = (int) Mathf.Floor(playerProfile.battlePass.stars / PlayerProfileBattlePass.STARS_IN_LEVEL);
                if (newCurrentLevel > currentLevel && newCurrentLevel < currentLevels.Count)
                {
                    currentLevels[currentLevel].SetPreviousState();
                    currentLevels[newCurrentLevel].SetCurrentState();
                    currentLevel = newCurrentLevel;
                    
                    if (currentLevel >= treasuresCount)
                    {
                        currentLevel = treasuresCount - 1;
                        starsInCurrentLevel = PlayerProfileBattlePass.STARS_IN_LEVEL;
                    }
                }

                allStars = playerProfile.battlePass.stars;
                starsInCurrentLevel = allStars - currentLevel * PlayerProfileBattlePass.STARS_IN_LEVEL;
                sliderBehaviour.SetFill(starsInCurrentLevel,currentLevel,treasuresCount);
            }
        }

        private void CheckPremium()
        {
            if (!playerProfile.battlePass.isPremiumBought) return;
            
            for (int i = 0; i < currentLevels.Count; i++)
            {
                currentLevels[i].ActivatePremiumReward();
                
                if(i == currentLevel)
                    currentLevels[i].EnablePremiumReward();
            }
        }

        public void OpenWindow()
        {
            SetUpLevels();
            SetInitialScales();
            ScrollToMyRating();
            CheckVisibleRewards();
        }

        private void SetUpLevels()
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentHolder);
            
            float oneLevel = contentHolder.rect.width * (1f / (float) currentLevels.Count);

            for (int i = 0; i < currentLevels.Count; i++)
            {
                currentLevels[i].SetPosition(- oneLevel * i);
            }
        }

        public void SetInitialScales()
        {
            for (int i = 0; i < currentLevels.Count; i++)
            {
                if (this.currentLevel + 2 > i)
                {
                    currentLevels[i].SetInitialNoAnimation();
                }
                else
                {
                    currentLevels[i].SetInitialScale();
                }
            }
        }

        private void ScrollToMyRating()
        {
            var level = 0;
            for (int i = 0; i < currentLevels.Count; i++)
            {
                if (currentLevels[i].IsActiveToCollected)
                {
                    level = i;

                    break;
                }
            }

            float oneLevel = 1f / (float) currentLevels.Count;
            float currentLevel = oneLevel * level;// oneLevel * (this.currentLevel + 1);
            float oneStar = oneLevel / PlayerProfileBattlePass.STARS_IN_LEVEL;
            float currentStar = starsInCurrentLevel * oneStar;
            float visibleCenter = contentParent.rect.width / 2;
            float targetX = contentHolder.rect.width * (currentLevel + currentStar) - visibleCenter;

            targetPosition = visibleCenter - targetX > visibleCenter * 0.75
                ? new Vector2(0, contentHolder.anchoredPosition.y)
                : new Vector2(-targetX, contentHolder.anchoredPosition.y);
            
            isScrollingToMyRating = true;
        }

        void Update()
        {
            if (!ratingSetted)
                BuildRatingBar();
            
            if(isScrollingToMyRating)
                ScrollContentPanelToMyRating();
            
            if(PanelChangedPosition())
                CheckVisibleRewards();
        }

        private void BuildRatingBar()
        {
            timer += Time.deltaTime;

            if (!(timer > updateTime)) return;
            
            ratingSetted = true;
            
            SetMyRatingPosition();
        }

        private void SetMyRatingPosition()
        {
            sliderBehaviour.SetFill(starsInCurrentLevel, currentLevel, treasuresCount);
            ratingSetted = true;
        }

        private void ScrollContentPanelToMyRating()
        {
            if (Input.touchCount > 0 || Input.anyKeyDown)
            {
                isScrollingToMyRating = false;
                return;
            }
            
            if (Vector2.SqrMagnitude(contentHolder.anchoredPosition - targetPosition) >= 3)
            {
                contentHolder.anchoredPosition = 
                    Vector2.Lerp(contentHolder.anchoredPosition, targetPosition, snapSpeed);
            }
            else
            {
                contentHolder.anchoredPosition = targetPosition;
                isScrollingToMyRating = false;
            }
        }

        private bool PanelChangedPosition()
        {
            return Vector2.SqrMagnitude(contentHolder.anchoredPosition - currentPosition) >= 0.00001;
        }

        private void CheckVisibleRewards()
        {
            var visibleEndPosition = contentHolder.anchoredPosition.x - contentParent.rect.width;
            
            for (int i = 0; i < currentLevels.Count; i++)
            {
                if(currentLevels[i].HadFirstAppearance()) continue;

                var rewardPos = currentLevels[i].GetPosition();
                
                if(rewardPos <= contentHolder.anchoredPosition.x && rewardPos >= visibleEndPosition)
                {
                    currentLevels[i].PlayAnimationOnFirstAppearance();
                }
                   
            }

            currentPosition = contentHolder.anchoredPosition;
        }

        private void OnDestroy()
        {
            playerProfile.PlayerProfileUpdated.RemoveListener(OnPlayerProfileUpdated);
        }
    }
}