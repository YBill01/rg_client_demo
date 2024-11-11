using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class ShopPanelBehaviour : MonoBehaviour
    {
        public ShopMenuType MenuType;

        [SerializeField] 
        protected RectTransform panelContent;
        [SerializeField] 
        protected Transform sectionsHolder;
        [SerializeField] 
        private HorizontalLayoutGroup layoutGroup;
        [SerializeField] 
        private List<BasicSectionBehaviour> sectionsInOrder;

        public bool Selected { get; set; }

        protected ProfileInstance profile;
        protected ShopWindowBehaviour parentShopWindow;

        public void Init(ProfileInstance profileInstance)
        {
            profile = profileInstance;
            InitData();
        }

        protected virtual void InitData()
        {
        }
        
        public float GetWidth()
        {
            return panelContent.rect.width;
        }

        public virtual void SetOffersOrder()
        {
        }

        public void SetSectionsPositions(float panelPosition)
        {
            var pos = panelPosition;

            for (int i = 0; i < sectionsInOrder.Count; i++)
            {
                sectionsInOrder[i].SetSectionPosition(pos);
                pos -= sectionsInOrder[i].GetSectionWidth();
                pos -= layoutGroup.spacing;
            }
        }
        
        public void ScrollToMe()
        {
            Selected = true;
        }

        public void ScrollOffMe()
        {
            Selected = false;
        }

        public virtual void ClearData()
        {
        }

        public ProfileInstance GetProfile()
        {
            return profile;
        }

        private void Start()
        {
            parentShopWindow = gameObject.GetComponentInParent<ShopWindowBehaviour>();
        }
    }
}
