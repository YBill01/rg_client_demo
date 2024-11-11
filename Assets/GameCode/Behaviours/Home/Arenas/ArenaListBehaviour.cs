using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class ArenaListBehaviour : MonoBehaviour
    {
        [Serializable]
        public struct ArenaRating
        {
            public ushort current;
            public ushort max;
        }

        public bool isStopScrolling;
        [SerializeField]
        private RectTransform mainRect;
        [SerializeField]
        private ScrollRect mainScrollRect;
        [SerializeField]
        private GameObject arenaPrefab;
        [SerializeField]
        private ArenaWindowBehaviour arenaWindow;
        [SerializeField]
        private RectTransform parentScrollContainer;
        [SerializeField]
        private HorizontalLayoutGroup parentScrollLayout;
        [SerializeField]
        [Range(0f, 1f)]
        private float snapSpeed;
        [SerializeField]
        [Range(0f, 1f)]
        private float snapSpeedTutorial;
        private float snapSpeedTemp;
        [SerializeField]
        private GameObject fader;

        public EventArenaData ratingData;

        private Dictionary<byte, ArenaContentBehaviour> arenasContentByNumber =
            new Dictionary<byte, ArenaContentBehaviour>();

        //private Dictionary<byte, ushort> arenasByNumber = new Dictionary<byte, ushort>();

        private byte arenasCount = 0;

        private ArenaRating rating;

        private bool isScrollingToMyRating;
        //private bool isOpened;
        private Vector2 targetPosition;
        private byte arenaIndexInView;
        private Vector2 currentPosition;
        private float panelQuarterWidth;

        private ushort tutorialArena;

        internal void Init()
        {
            //isOpened = false;
            fader.GetComponent<Canvas>().sortingLayerID = SortingLayer.GetLayerValueFromName("Fader");
            SetClickBlockerEnabled(false);

            if (!BattleDataContainer.Instance.CheckForNewArena())
            {
                this.rating = new ArenaRating
                {
                    current = (ushort)ClientWorld.Instance.Profile.Rating.current,
                    max = (ushort)ClientWorld.Instance.Profile.Rating.max
                };
            }
            else
            {
                this.rating = new ArenaRating
                {
                    current = (ushort)BattleDataContainer.Instance.PreviousRating,
                    max = (ushort)BattleDataContainer.Instance.PreviousMaxRating
                };
            }

            BinaryList arenasT = Settings.Instance.Get<ArenaSettings>().queue;
            tutorialArena = Settings.Instance.Get<ArenaSettings>().tutorial;
            arenasCount = (byte)arenasT.length;

            BinaryList arenas;
            if (tutorialArena > 0)
			{
                arenas = new BinaryList();
                arenas.Add(tutorialArena);
                for (int i = 0; i < arenasCount; i++)
				{
                    arenas.Add(arenasT[(byte)i]);
                }
                arenasCount++;
            }
			else
			{
                arenas = arenasT;
            }

            ushort rating = 0;

            for (byte i = 0; i < arenas.length; i++)
            {
                if (i >= 7)
                {
                    break;// временное решение...
                }

                GameObject arena = Instantiate(arenaPrefab, parentScrollContainer);

                //ushort arenaID = arenas[i];
                
                //arenasByNumber.Add(i, arenaID);
                var content = arena.GetComponent<ArenaContentBehaviour>();
                arenasContentByNumber.Add(i, content);

				if (i >= 6)
				{
					content.IsLocked = true; // Блокировка арен с 4 и далее...
				}
			}

            foreach (var pair in arenasContentByNumber)
            {
                if (Battlefields.Instance.Get(arenas[pair.Key], out BinaryBattlefields binaryArena))
                {
                    byte arenaKey = pair.Key;
                    if (tutorialArena > 0)
					{
                        if (arenaKey > 0)
						{
                            arenaKey--;
						}
					}

                    pair.Value.InitData(arenaWindow, arenaKey, rating, binaryArena, pair.Key == 0, pair.Key == arenasCount - 1);
                    rating += pair.Value.BinaryData.rating;
                }
            }

            snapSpeedTemp = snapSpeed;

            arenaIndexInView = 0;
            SetRating();
        }

        public void SetRating()
        {
            SetNewRating(rating);
        }

        public void SetNewRating(ArenaRating rating)
        {
            ratingData = Settings.Instance.Get<ArenaSettings>().GetArenaData(rating.current);
            EventArenaData maxRatingData = Settings.Instance.Get<ArenaSettings>().GetArenaData(rating.max);

            for (byte i = 0; i < arenasContentByNumber.Count; i++)
            {
                if (arenasContentByNumber.TryGetValue(i, out ArenaContentBehaviour contentDone))
                {
					contentDone.SetRating(ratingData);
                    contentDone.SetMaxRating(maxRatingData);
                }
            }
        }

        public void ScrollToMyRating()
        {
            snapSpeedTemp = snapSpeed;

            SetArenasPositions();

            if (arenasContentByNumber.TryGetValue((byte)(ratingData.number + (tutorialArena > 0 ? 1 : 0)), out ArenaContentBehaviour content))
            {
                parentScrollContainer.anchoredPosition = new Vector2(content.GetStartPosition(), parentScrollContainer.anchoredPosition.y);
                currentPosition = new Vector2(parentScrollContainer.anchoredPosition.x + 20, parentScrollContainer.anchoredPosition.y);
                targetPosition = new Vector2(content.GetMyRatingPosition() + panelQuarterWidth * 2, parentScrollContainer.anchoredPosition.y);
                isScrollingToMyRating = true;
                arenaIndexInView = ratingData.number;
                content.SetSliderMovingState(true);
                content.ShowMyRating(true);
            }
        }
        public void AfterOpenLootBox()
        {
            currentPosition = new Vector2(parentScrollContainer.anchoredPosition.x + .1f, parentScrollContainer.anchoredPosition.y);
        }
        public void ScrollToNextArena()
        {
            byte previousArena = ratingData.number;
            if (tutorialArena == 0)
			{
                previousArena--;
            }
			if (arenasContentByNumber.TryGetValue(previousArena, out ArenaContentBehaviour previousContent))
			{
				previousContent.ShowMyRating(false);
			}
			
            if (arenasContentByNumber.TryGetValue((byte)(ratingData.number + (tutorialArena > 0 ? 1 : 0)), out ArenaContentBehaviour nextContent)){
                targetPosition = new Vector2(nextContent.GetStartPosition() + 20, parentScrollContainer.anchoredPosition.y);
                isScrollingToMyRating = true;
                arenaIndexInView = ratingData.number;
                nextContent.ShowMyRating(false);
                nextContent.ShowNewHeroesEffect();
                nextContent.SetSliderMovingState(false);
            }
        }

        public void OpenWithoutScrolling()
        {
            SetArenasPositions();
            if (arenasContentByNumber.TryGetValue((byte)(ratingData.number + ((tutorialArena > 0 && ratingData.rating > 0) ? 1 : 0)), out ArenaContentBehaviour content))
            {
                float parentPositionX;
                if (content.IsTutorialArena)
				{
                    parentPositionX = 0.0f;// -280.0f; // TODO: before new window ...
                    snapSpeedTemp = snapSpeedTutorial;
                }
				else
                {
                    parentPositionX = content.GetMyRatingPosition() + panelQuarterWidth * 2;
                    snapSpeedTemp = snapSpeedTutorial;
                }

                parentScrollContainer.anchoredPosition = new Vector2(parentPositionX, parentScrollContainer.anchoredPosition.y);
                currentPosition = new Vector2(parentScrollContainer.anchoredPosition.x + 20, parentScrollContainer.anchoredPosition.y);
                targetPosition = new Vector2(parentScrollContainer.anchoredPosition.x + 20, parentScrollContainer.anchoredPosition.y);
                isScrollingToMyRating = false;
                arenaIndexInView = ratingData.number;

                content.SetSliderMovingState(false);
                content.MoveSliderToEnd();
            }
        }

        public void StartScrolling()
        {
            isScrollingToMyRating = true;
        }
        public void StopScrolling()
        {
            isScrollingToMyRating = false;
        }

        public void SetOnHeroes()
        {
            if (arenasContentByNumber.TryGetValue((byte)(ratingData.number + (tutorialArena > 0 ? 1 : 0)), out ArenaContentBehaviour content))
            {
                parentScrollContainer.anchoredPosition = new Vector2(content.GetStartPosition(), parentScrollContainer.anchoredPosition.y);
                currentPosition = new Vector2(parentScrollContainer.anchoredPosition.x + 20, parentScrollContainer.anchoredPosition.y);
                targetPosition = new Vector2(parentScrollContainer.anchoredPosition.x + 20, parentScrollContainer.anchoredPosition.y);

            }
        }
        public float GetNewHerows()
        {
            if (arenasContentByNumber.TryGetValue((byte)(ratingData.number + (tutorialArena > 0 ? 1 : 0)), out ArenaContentBehaviour content))
                return content.newHeroesCount * 1.5f;
            return 0;
        }

        public void SetArenasPositions()
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentScrollContainer);
            panelQuarterWidth = mainRect.rect.width / 4;

            for (byte i = 0; i < arenasContentByNumber.Count; i++)
            {
                arenasContentByNumber[i].UpdateWidth();
                arenasContentByNumber[i].SetMainPanelWidth(mainRect.rect.width);
                var panelPosition = GetPanelPositionX(i);
                arenasContentByNumber[i].SetPosition(panelPosition);
                arenasContentByNumber[i].SetPanelPositionForArenaInView(panelPosition);
            }
        }

        private float GetPanelPositionX(int panelNumber)
        {
            float targetPositionX = 0;

            if (panelNumber > 0)
                for (byte i = 0; i < panelNumber; i++)
                {
                    targetPositionX += arenasContentByNumber[i].GetWidth();
                }

            targetPositionX += parentScrollLayout.spacing * panelNumber;

            return -targetPositionX;
        }

        void Update()
        {
            CheckArenaInView();

            if (!isScrollingToMyRating) return;

            ScrollContentPanelToMyRating();
        }

        private void CheckArenaInView()
        {
            if (PanelChangedPosition())
            {
                if(arenasContentByNumber.TryGetValue(arenaIndexInView, out var arena))
                {
                    arena.SetPanelPositionForArenaInView(parentScrollContainer.anchoredPosition.x);
                }

                if (IsContentScrollsToRight())
                {
                    if (!IsArenaInViewFirstInList())
                    {
                        for (int i = arenaIndexInView - 1; i >= 0; i--)
                        {
                            if (arenasContentByNumber.TryGetValue((byte)i, out arena))
                            {
                                if (IsArenasEndInView(arena))
                                {
                                    arena.SetPanelPositionForArenaInView(parentScrollContainer.anchoredPosition.x);
                                    arenaIndexInView = (byte)i;
                                }
                            }
                        }
                    }
                }
                else if (IsContentScrollsToLeft())
                {
                    if (!IsArenaInViewLastInList())
                    {
                        for (int i = arenaIndexInView + 1; i < arenasContentByNumber.Count; i++)
                        {

                            if (arenasContentByNumber.TryGetValue((byte)i, out arena))
                            {
                                if (IsArenasStartInView(arena))
                                {
                                    arena.SetPanelPositionForArenaInView(parentScrollContainer.anchoredPosition.x);
                                    arenaIndexInView = (byte)i;
                                }
                            }
                        }
                    }
                }
                currentPosition = parentScrollContainer.anchoredPosition;
            }
        }

        private bool PanelChangedPosition()
        {
            return Vector2.SqrMagnitude(parentScrollContainer.anchoredPosition - currentPosition) >= 0.00001;
        }

        private bool IsContentScrollsToRight()
        {
            return parentScrollContainer.anchoredPosition.x > currentPosition.x;
        }

        private bool IsArenaInViewFirstInList()
        {
            return arenaIndexInView.Equals((byte)0);
        }

        private bool IsArenasEndInView(ArenaContentBehaviour arena)
        {
            float start = parentScrollContainer.anchoredPosition.x;
            float end = parentScrollContainer.anchoredPosition.x - panelQuarterWidth * 4;
            return arena.GetEndPosition() < start && arena.GetEndPosition() >= end;
        }

        private bool IsContentScrollsToLeft()
        {
            return parentScrollContainer.anchoredPosition.x < currentPosition.x;
        }

        private bool IsArenaInViewLastInList()
        {
            return arenaIndexInView.Equals((byte)(arenasContentByNumber.Count - 1));
        }

        private bool IsArenasStartInView(ArenaContentBehaviour arena)
        {
            float start = parentScrollContainer.anchoredPosition.x;
            float end = parentScrollContainer.anchoredPosition.x - panelQuarterWidth * 3.7f;
            return arena.GetStartPosition() <= start && arena.GetStartPosition() > end;
        }

        private void ScrollContentPanelToMyRating()
        {
            if (Input.touchCount > 0 || Input.anyKeyDown)
            {
                isScrollingToMyRating = false;
                return;
            }

            if (Vector2.SqrMagnitude(parentScrollContainer.anchoredPosition - targetPosition) >= 3)
            {
                parentScrollContainer.anchoredPosition =
                    Vector2.Lerp(parentScrollContainer.anchoredPosition, targetPosition, snapSpeedTemp);
            }
            else
            {
                parentScrollContainer.anchoredPosition = targetPosition;
                isScrollingToMyRating = false;
                arenasContentByNumber[arenaIndexInView]
                    .SetPanelPositionForArenaInView(parentScrollContainer.anchoredPosition.x);
            }
        }

        public void SetClickBlockerEnabled(bool enabled)
        {
            fader.SetActive(enabled);
            mainScrollRect.horizontal = !enabled;
        }

    }
}
