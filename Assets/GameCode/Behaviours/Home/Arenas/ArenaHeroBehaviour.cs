using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class ArenaHeroBehaviour : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text name;
        [SerializeField]
        private GameObject openHeroEffect;
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private List<Image> imagesForGrayOut;
        [SerializeField]
        private Image titleBackground;
        [SerializeField]
        private Color32 titleBackgroundGrayColor;

        private Color32 titleBackgroundActiveColor;
        private ushort heroIndex;
        private float waitTimeBeforePlay = 1.35f;
        private ushort indexer = System.UInt16.MaxValue;

        public void SetName(string nameString)
        {
            name.text = nameString;
        }

        public void SetColor(Color32 heroColor)
        {
            titleBackgroundActiveColor = heroColor;
        }
        private void OnEnable()
        {
            if (openHeroEffect) openHeroEffect.SetActive(false);
        }

        public void ShowEffect()
        {
            StartCoroutine(SelfEnable());
        }

        private IEnumerator SelfEnable()
        {
            yield return new WaitForSeconds(indexer * waitTimeBeforePlay);

            if (openHeroEffect)  openHeroEffect.SetActive(true);

            yield return new WaitForSeconds(1.6f);

            MakeGray(false);
        }

        public void SetDataToPlayEffect(ushort index)
        {
            if (Heroes.Instance.Get(index, out BinaryHero hero))
                heroIndex = hero.index;
        }

        public void SetIndexer(ushort value)
        {
            indexer = value;
        }

        public bool IsNewHero(List<int> indexes)
        {
            return indexes.Contains(heroIndex);
        }

        public float GetWidth()
        {
            return rectTransform.rect.width;
        }

        public void MakeGray(bool toggle)
        {
            if (imagesForGrayOut == null) return;

            for (int i = 0; i < imagesForGrayOut.Count; i++)
            {
                imagesForGrayOut[i].material = toggle ? VisualContent.Instance.GrayScaleMaterial : null;
            }

            titleBackground.color = toggle ? titleBackgroundGrayColor : titleBackgroundActiveColor;
        }
    }
}