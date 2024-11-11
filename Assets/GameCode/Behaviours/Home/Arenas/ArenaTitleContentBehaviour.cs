using System;
using Legacy.Client;
using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class ArenaTitleContentBehaviour : MonoBehaviour
    {
        public RectTransform Container;
        public RectTransform UpElementsContainer;
        [SerializeField] 
        private Image background;
        [SerializeField] 
        private TextMeshProUGUI arenaNumberText;
        [SerializeField] 
        private TextMeshProUGUI arenaTitleText;
        [SerializeField] 
        private ChangeValueBehaviour rating;
        [SerializeField] 
        private RectTransform arenaTitleContainer;
        [SerializeField] 
        private Image arenaTitleImage;
        [SerializeField]
        private Image lockImage;
        [SerializeField]
        private GameObject infoButton;

        private ArenaWindowBehaviour arenaWindow;

        private BinaryBattlefields binaryArena;

        private bool isGray = false;

        public void Init(BinaryBattlefields binaryArena, byte number, int startRating)
        {
            this.binaryArena = binaryArena;

            arenaNumberText.text = Locales.Get("locale:1360", (number + 1).ToString());
            SetCardName(Locales.Get(binaryArena.title));
            rating.SetValue(startRating);
            
            if (ColorUtility.TryParseHtmlString(binaryArena.background_color, out Color color))
            {
                background.color = color;
            }

            arenaTitleImage.sprite = GetSprite(binaryArena.index);
        }

        public void SetCardName(string name)
        {
            arenaTitleText.text = Locales.Get(name);
        }

        private Sprite GetSprite(ushort index)
        {
            return VisualContent.Instance.GetArenaVisualData(index).TitleImage;
        }

        public void TurnOffRating()
        {
            rating.gameObject.SetActive(false);
        }

        public void InfoClick()
        {
            arenaWindow.OpenArenaInfo();
        }

        public void SetPosition(float posX)
        {
            arenaTitleContainer.anchoredPosition = new Vector2(posX, arenaTitleContainer.anchoredPosition.y);
        }

        internal void Lock(bool value)
        {
            lockImage.gameObject.SetActive(value);
            if (value)
			{
                SetCardName(Locales.Get("locale:1291"));
            }
			else
			{
                SetCardName(Locales.Get(binaryArena.title));
            }
            arenaTitleImage.gameObject.SetActive(!value);
            infoButton.SetActive(!value);
            MakeGray(value);
        }

        internal void SetTutorialType(BinaryBattlefields binaryData)
        {
            arenaNumberText.text = Locales.Get("locale:2377");
            //arenaNumberText.gameObject.SetActive(false);
            SetCardName(Locales.Get(binaryData.title));
            infoButton.SetActive(false);
        }

        internal void MakeGray(bool toggle)
        {
            isGray = toggle;

            if (toggle)
			{
                background.color = new Color(0.5F, 0.5F, 0.5F, 1.0F); ;
			}
			else
			{
                if (ColorUtility.TryParseHtmlString(binaryArena.background_color, out Color color))
                {
                    background.color = color;
                }
            }

            background.material = toggle ? VisualContent.Instance.GrayScaleMaterial : null;

            /*Icon.material = toggle ? VisualContent.Instance.GrayScaleMaterial : null;
            RarityIcon.material = toggle ? VisualContent.Instance.GrayScaleMaterial : null;
            Frame.material = toggle ? VisualContent.Instance.GrayScaleMaterial : null;
            Frame.enabled = (!toggle);
            GlowImage.enabled = !toggle;
            if (ManaCost.activeInHierarchy)
            {
                ManaCost.GetComponent<Image>().material = toggle ? VisualContent.Instance.GrayScaleMaterial : null;
            }*/
        }
    }
}
