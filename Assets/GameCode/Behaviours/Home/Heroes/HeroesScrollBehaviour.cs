using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class HeroesScrollBehaviour : MonoBehaviour
    {
        public class ScrollItem
        {
            public SmallHeroButtonBehaviour smallPanel;
            public HeroPanelBehaviour panel;
            public float position;
            public ushort hero_index;

            public bool isSelected
            {
                get { return panel.Selected; }
            }
        }

        [SerializeField] private GameObject RightArrow;
        [SerializeField] private GameObject LeftArrow;

        [SerializeField] private ScrollRect scroll;
        [SerializeField] private HeroSmallScrollBehaviour smallScroll;
        [SerializeField] private VerticalLayoutGroup MainLayout;
        [SerializeField] private RectTransform ContentRect;
        [SerializeField] private RectTransform ScrollViewRect;
        [SerializeField] private RectTransform SmallContentRect;
        [SerializeField] private RectTransform SmallScrollViewPort;
        [SerializeField] private HorizontalLayoutGroup ContentLayout;

        private float maxContentPosition;
        private float minContentPosition;

        [SerializeField] private RectTransform MainWindowRect;

        private float PanelWidth;

        public float GetScrollPosition()
        {
            return scroll.horizontalNormalizedPosition;
        }

        internal void SetPanelWidth(float width)
        {
            PanelWidth = width;
        }

        private List<ScrollItem> panelsList = new List<ScrollItem>();
        private SortedDictionary<byte, ScrollItem> panels = new SortedDictionary<byte, ScrollItem>();

        private float currentPositionX;
        private Vector2 currentPosition;

        [SerializeField, Range(0.0f, 1.1f)] private float SnapSpeed;

        [SerializeField, Range(500.0f, 5000.0f)]
        private float StopScrollSpeed;

        [SerializeField, Range(2000.0f, 10000.0f)]
        private float SelectStopScrollSpeed;

        [SerializeField, Range(0.0f, 1.0f)] private float SnapSensitivity;

        private bool inited;
        private float startOffset;

        private bool NeedScrolling => maxContentPosition - minContentPosition > MainLayout.padding.left;

        internal void AddPanelToScrollList(HeroPanelBehaviour panel, SmallHeroButtonBehaviour smallPanel, ushort index)
        {
            if (PanelWidth == 0)
            {
                PanelWidth = panel.GetComponent<RectTransform>().rect.width;
            }

            panelsList.Add(new ScrollItem()
            {
                panel = panel,
                smallPanel = smallPanel,
                hero_index = index
            });
        }
        public void ClearPanelList()
        {
            panelsList.Clear();
        }

        float PanelTotalWidth
        {
            get { return PanelWidth + ContentLayout.spacing; }
        }

        internal void SetBounds()
        {
            if (maxContentPosition == minContentPosition)
            {
                maxContentPosition = MainLayout.padding.left;
                minContentPosition = maxContentPosition - (PanelTotalWidth * panelsList.Count) +
                                     ScrollViewRect.rect.width;

                //scroll.enabled = NeedScrolling;
            }
        }

        internal void BuildScrollSnaping()
        {
            startOffset = MainLayout.padding.left;
            panels.Clear();
            for (byte i = 0; i < panelsList.Count; i++)
            {
                byte number = panelsList[i].panel.SetNumber();
                panelsList[i].position = startOffset - number * PanelTotalWidth +
                                         (MainWindowRect.rect.width - PanelTotalWidth) / 2;
                panels.Add(number, panelsList[i]);
                if (panelsList[i].isSelected)
                {
                    SetCurrentSnap(number);
                }
            }

            inited = true;
        }

        public ScrollItem SelectedItem;

        public void SelectNext()
        {
            var currentNumber = SelectedItem.panel.NumberInScroll;
            if (currentNumber < panels.Count - 1)
            {
                SetCurrentSnap((byte) (currentNumber + 1));
            }
        }

        public void SelectPrevious()
        {
            var currentNumber = SelectedItem.panel.NumberInScroll;
            if (currentNumber > 0)
            {
                SetCurrentSnap((byte) (currentNumber - 1));
            }
        }

        internal void SetCurrentSnap(byte number)
        {
            RightArrow.SetActive(number < panels.Count - 1);
            LeftArrow.SetActive(number > 0);

            if (panels.TryGetValue(number, out ScrollItem item))
            {
                currentPositionX = item.position;
                if (SelectedItem != null)
                {
                    SelectedItem.panel.Deselect();
                    SelectedItem.smallPanel.Deselect();
                }

                item.panel.Select();
                item.smallPanel.Select();
                SelectedItem = item;
            }
        }

        void SearchSnap()
        {
			currentPosition.x = ContentRect.anchoredPosition.x;
            
			int itemPosition = (int)((ContentRect.rect.width - (ScrollViewRect.rect.width - MainLayout.padding.left- MainLayout.padding.right)) / panels.Count);

			int itemIndex = (int)(Mathf.Abs(currentPosition.x - MainLayout.padding.left) / itemPosition);

            if (currentPosition.x > MainLayout.padding.left)
            {
                itemIndex = 0;
			}
			else if (itemIndex >= panels.Count)
			{
                itemIndex = panels.Count - 1;
            }

            SetCurrentSnap((byte)itemIndex);

			/*if (SelectedItem != null)
			{
				if (SelectedItem != panels[(byte)itemIndex])
				{
					SetCurrentSnap((byte)itemIndex);
				}
			}
			else
			{
				SetCurrentSnap((byte)itemIndex);
			}*/


			//ScrollViewRect


			/*currentPosition.x = ContentRect.anchoredPosition.x;
			float maxBound = maxContentPosition + ScrollViewRect.rect.width / 8;
			float minBound = minContentPosition - ScrollViewRect.rect.width / 8;
			if (currentPosition.x > maxBound || currentPosition.x < minBound)
			{
				currentPosition.x = Mathf.Clamp(currentPosition.x, minBound, maxBound);
				ContentRect.anchoredPosition = currentPosition;
				scroll.velocity = Vector2.zero;
			}

			if (scroll.velocity.x < 0 || smallScroll.velocity.x < 0)
			{
				for (int i = panels.Count - 1; i > -1; i--)
				{
					if (panels.TryGetValue((byte)i, out ScrollItem item))
					{
						if (item.position + (PanelTotalWidth * SnapSensitivity) > currentPosition.x)
						{
							SetCurrentSnap((byte)i);
							break;
						}
					}
				}
			}
			else if (scroll.velocity.x > 0 || smallScroll.velocity.x > 0)
			{
				for (int i = 0; i < panels.Count; i++)
				{
					if (panels.TryGetValue((byte)i, out ScrollItem item))
					{
						if (item.position - (PanelTotalWidth * SnapSensitivity) < currentPosition.x)
						{
							SetCurrentSnap((byte)i);
							break;
						}
					}
				}
			}*/
		}

        void Update()
        {
            //if (NeedScrolling)
            //{
                var velocity = Mathf.Max(Mathf.Abs(scroll.velocity.x), Mathf.Abs(smallScroll.velocity.x));
                if (smallScroll.isScrolling && SmallContentRect.rect.width > SmallScrollViewPort.rect.width)
                {
                    scroll.horizontalNormalizedPosition = smallScroll.horizontalNormalizedPosition;
                }
                else
                {
                    smallScroll.horizontalNormalizedPosition = scroll.horizontalNormalizedPosition;
                }

                if (inited)
                {
                    if (Input.GetMouseButton(0))
                    {
					    if (velocity != 0.0f)
					    {
                            SearchSnap();
                        }
                    }
                    else if (velocity < StopScrollSpeed)
                    {
                        SetBounds();
                        currentPositionX = Mathf.Clamp(currentPositionX, minContentPosition, maxContentPosition);
                        currentPosition.x = Mathf.SmoothStep(currentPosition.x, currentPositionX, SnapSpeed);
                        ContentRect.anchoredPosition = currentPosition;
                    }
                    else if (velocity < SelectStopScrollSpeed)
                    {
                        SearchSnap();
                    }
                }
            //}
        }
    }
}
