using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;


//This logic is written for content panel with anchors set to left side
//All ifs and calculations are made for content position with anchored position x in range [0; -∞]
namespace Legacy.Client
{
    public class ShopScrollBehaviour : MonoBehaviour
    {
        [SerializeField] 
        public RectTransform contentTransform;
        [SerializeField] 
        public ContentSizeFitter ContentFitter;
        [SerializeField] 
        private RectTransform panelTransform;
        [SerializeField] 
        private HorizontalLayoutGroup layoutGroup;
        [SerializeField] 
        private GameObject rightArrow;
        [SerializeField] 
        private GameObject leftArrow;
        [SerializeField, Range(0f, 1f)] 
        private float snapSpeed;

        private float maxContentPosition;
        public float minContentPosition;
        public List<ShopScrollItem> panelsList = new List<ShopScrollItem>();
        private bool isScrollToPanel;
        private Vector2 targetPosition;
        private ShopScrollItem selectedItem;
        private float panelQuarterWidth;

        //private ScrollRect scrollRect;

        public void Init()
        {
            targetPosition = contentTransform.anchoredPosition;
            panelQuarterWidth = panelTransform.rect.width / 4;
        }

		public void AddPanelToScrollList(ShopPanelBehaviour panel, ShopMenuButtonBehaviour menuButton, ShopMenuType type, int number)
        {
            panelsList.Add(new ShopScrollItem()
            {
                panel = panel,
                menuButton = menuButton,
                menuType = type,
                numberInScroll = number
            });
            
            menuButton.ShopMenuButtonClick += OnShopMenuButtonClick;
        }

        public void SetupPanelsPositions()
        {
            for (int i = 0; i < panelsList.Count; i++)
            {
                panelsList[i].position = GetPanelPositionX(i);
                //panelsList[i].panel.SetSectionsPositions(panelsList[i].position);
            }
        }

        private float GetPanelPositionX(int panelNumber)
        {
            float targetPositionX = 0;

            if (panelNumber > 0)
                for (int i = 0; i < panelNumber; i++)
                {
                    targetPositionX += panelsList[i].panel.GetWidth();
                }

            targetPositionX += layoutGroup.spacing * panelNumber;

            return -targetPositionX;
        }

        public void SelectNext()
        {
            var currentNumber = selectedItem.numberInScroll;
            if (currentNumber < panelsList.Count - 1)
            {
                ScrollToPanel(currentNumber + 1);
            }
        }

        private void ScrollToPanel(int panelNumber)
        {
            ShopScrollItem item = panelsList.Find(x => x.numberInScroll.Equals(panelNumber));
            if (item != null)
            {
                ChangeSelectedItem(item);

                isScrollToPanel = true;
            }
        }

        private void ChangeSelectedItem(ShopScrollItem item, float newPosition = 0)
        {
            var targetPositionX = newPosition == 0 ? item.position : newPosition;
            targetPosition = new Vector2(targetPositionX, contentTransform.position.y);
            SetBounds();
            
            Debug.Log(targetPosition);
            if (targetPosition.x < minContentPosition)
                targetPosition = new Vector2(minContentPosition, targetPosition.y);
            Debug.Log(targetPosition);
            UnselectCurrentItem();
            SelectNewItem(item);
            SetArrowsStates();
        }

        private void SetBounds()
        {
            var viewportWidth = panelTransform.rect.width;
            var contentWidth = contentTransform.rect.width;
            var viewportsInContent = Mathf.FloorToInt(contentWidth / viewportWidth);
            var viewportsInContentLength = panelTransform.rect.width * viewportsInContent;
            var outOfViewportContent = contentWidth - viewportsInContentLength;
            minContentPosition = -(viewportsInContentLength - (viewportWidth - outOfViewportContent));

            if (minContentPosition > 0)
                minContentPosition = 0;
        }

        private void UnselectCurrentItem()
        {
            if (selectedItem != null)
            {
                selectedItem.menuButton.Deselect();
                selectedItem.panel.ScrollOffMe();
            }
        }

        private void SelectNewItem(ShopScrollItem item)
        {
            selectedItem = item;
            selectedItem.panel.ScrollToMe();
            selectedItem.menuButton.Select();
        }

        private void SetArrowsStates()
        {
            rightArrow.SetActive(selectedItem.numberInScroll < panelsList.Count - 1);
            leftArrow.SetActive(selectedItem.numberInScroll > 0);
        }

        public void SelectPrevious()
        {
            var currentNumber = selectedItem.numberInScroll;
            if (currentNumber > 0)
            {
                ScrollToPanel(currentNumber - 1);
            }
        }

        private void OnShopMenuButtonClick(ShopMenuType type)
        {
            ShopScrollItem item = panelsList.Find(x => x.menuType.Equals(type));
            if (item != null)
            {
                ScrollToPanel(item.numberInScroll);
            }
        }

        public void SelectFirst()
        {
            ScrollToPanel(0);
        }

        public void SelectSection(ShopMenuType type, float sectionPos)
        {
            ShopScrollItem item = panelsList.Find(x => x.menuType.Equals(type));
            if (item != null)
            {
                ChangeSelectedItem(item, sectionPos);

                isScrollToPanel = true;
            }

            //OnShopMenuButtonClick(type);

            //ScrollToPanel((int)ShopMenuType.Coins);
            //ChangeSelectedItem(item, sectionPos);

            /*ShopScrollItem item = panelsList.Find(x => x.menuType.Equals(type));
            if (item != null)
            {
                ChangeSelectedItem(item, sectionPos);

                isScrollToPanel = true;
            }*/
        }

        public void ClearData()
        {
            panelsList.Clear();
            UnselectCurrentItem();
            selectedItem = null;
        }

        private void Update()
        {
            if (isScrollToPanel)
            {
                ChangeContentPanelPosition();
            }
            else
            {
                CheckForPanelInView();
            }
        }

        private void ChangeContentPanelPosition()
        {
			if (Input.GetMouseButton(0))
			{
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };
                List<RaycastResult> raycastResultList = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

				if (Convert.ToBoolean(raycastResultList. Any(x => x.gameObject == panelTransform.gameObject)))
				{
                    isScrollToPanel = false;
                    return;
                }
			}

			/*if (EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject != null)
			{
                Debug.Log(EventSystem.current.currentSelectedGameObject.ToString());
			}*/

			/*if (Input.touchCount > 0 || Input.anyKeyDown)
            {
                isScrollToPanel = false;
                return;
            }*/

			if (Vector2.SqrMagnitude(contentTransform.anchoredPosition - targetPosition) >= 3)
            {
                contentTransform.anchoredPosition =
                    Vector2.Lerp(contentTransform.anchoredPosition, targetPosition, snapSpeed);
            }
            else
            {
                contentTransform.anchoredPosition = targetPosition;
                isScrollToPanel = false;
            }
        }

        private void CheckForPanelInView()
        {
            if (IsContentAtTargetPosition()) return;
            
            if (IsContentScrollsToRight())
            {
                if (IsSelectedPanelFirstInList()) return;
                
                for (int i = selectedItem.numberInScroll - 1; i >= 0; i--)
                {
                    if (IsPanelsPositionInFirstQuarter(panelsList[i]) || IsPanelsEndInLastQuarter(panelsList[i]))
                    {
                        ChangeSelectedItem(panelsList[i]);
                        return;
                    }
                }
            }
            else if (IsContentScrollsToLeft())
            {
                if (IsSelectedPanelLastInList()) return;
                
                for (int i = selectedItem.numberInScroll + 1; i < panelsList.Count; i++)
                {
                    if (IsPanelsPositionInFirstQuarter(panelsList[i]))
                    {
                        ChangeSelectedItem(panelsList[i]);
                        return;
                    }
                }
            }
        }

        private bool IsContentAtTargetPosition()
        {
            return Vector2.SqrMagnitude(contentTransform.anchoredPosition - targetPosition) <= 0.00001;
        }

        private bool IsContentScrollsToRight()
        {
            return contentTransform.anchoredPosition.x > targetPosition.x;
        }

        private bool IsSelectedPanelFirstInList()
        {
            return selectedItem == null || selectedItem.numberInScroll.Equals(0);
        }

        private bool IsPanelsPositionInFirstQuarter(ShopScrollItem item)
        {
            float start = contentTransform.anchoredPosition.x;
            float end = contentTransform.anchoredPosition.x - panelQuarterWidth;
            return item.position <= start && item.position > end;
        }

        private bool IsContentScrollsToLeft()
        {
            return contentTransform.anchoredPosition.x < targetPosition.x;
        }

        private bool IsSelectedPanelLastInList()
        {
            return selectedItem == null || selectedItem.numberInScroll.Equals(panelsList.Count - 1);
        }

        private bool IsPanelsEndInLastQuarter(ShopScrollItem item)
        {
            float panelEnd = item.position - item.panel.GetWidth();
            float start = contentTransform.anchoredPosition.x - panelQuarterWidth * 3;
            float end = contentTransform.anchoredPosition.x - panelQuarterWidth * 4;
            return panelEnd < start && panelEnd >= end;
        }

        private void OnDestroy()
        {
            for (int i = 0; i < panelsList.Count; i++)
            {
                panelsList[i].menuButton.ShopMenuButtonClick += OnShopMenuButtonClick;
            }
        }
    }
}
