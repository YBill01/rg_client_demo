using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class BasicSectionBehaviour : MonoBehaviour
    {
        
        [SerializeField] 
        private Transform offersHolder;
        [SerializeField] 
        private Transform offersHolder2;
        [SerializeField] 
        private RectTransform offersHolderRec;

        private float sectionPosition;

        public Transform GetOffersHolder()
        {
            return offersHolder;
        }
        
        public Transform GetOffersHolder2()
        {
            return offersHolder2;
        }

        public void SetSectionPosition(float pos)
        {
            sectionPosition = pos;
        }

        public float GetSectionWidth()
        {
            return offersHolderRec.rect.width;
        }

        public float GetSectionPosition()
        {
            return sectionPosition;
        }

        public RectTransform GetOffersRectTransform()
        {
            return offersHolderRec;
        }
    }
}