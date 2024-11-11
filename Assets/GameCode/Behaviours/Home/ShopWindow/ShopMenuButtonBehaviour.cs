using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Client;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class ShopMenuButtonBehaviour : MonoBehaviour
    {
        public event Action<ShopMenuType> ShopMenuButtonClick; 
        
        public ShopMenuType MenuType;

        public bool Selected { get; private set; }

        [SerializeField] 
        private RectTransform ScaleContainer;
        [SerializeField] 
        private GameObject VisibilityContainer;
        [SerializeField] 
        private LegacyButton LegacyButtonComponent;
        [SerializeField, Range(0, 1)] 
        private float ScaleLerpSpeed;
        [SerializeField, Range(0.0f, 1.0f)] 
        private float UnselectedScale;

        private Vector3 currentScale;
        private bool changeScale;

        public void Init(ShopPanelBehaviour panel)
        {
            LegacyButtonComponent.onClick.AddListener(OnLegacyButtonClick);
            currentScale = new Vector3(UnselectedScale, UnselectedScale, UnselectedScale);
            Selected = panel.Selected;

            if (Selected)
                Select();
        }

        private void OnLegacyButtonClick()
        {
            ShopMenuButtonClick?.Invoke(MenuType);
        }

        public void Select()
        {
            currentScale = Vector3.one;
            Selected = true;
            VisibilityContainer.SetActive(true);
        }

        public void Deselect()
        {
            Selected = false;
            currentScale = new Vector3(UnselectedScale, UnselectedScale, UnselectedScale);
            VisibilityContainer.SetActive(false);
        }

        private void Update()
        {
            if (!ScaleContainer.localScale.Equals(currentScale))
            {
                ScaleContainer.localScale = Vector3.Lerp(ScaleContainer.localScale, currentScale, ScaleLerpSpeed);
            }
        }

        private void OnDestroy()
        {
            LegacyButtonComponent.onClick.RemoveListener(OnLegacyButtonClick);
        }
    }
}
