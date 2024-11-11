using DG.Tweening;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client {
    public class CardProgressBarBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Transform Arrow;
        [SerializeField]
        private GameObject SlideLighter;
        [SerializeField]
        private GameObject StaticLighter;
        [SerializeField]
        public ProgressBarChangeValueBehaviour ProgressValue;

        [SerializeField]
        private TextMeshProUGUI LevelText;

        [SerializeField]
        private Image ProgressBarFill;

        [SerializeField]
        private GameObject UpArrowImage;

        [SerializeField]
        private Image levelImage;

        [Space]
        [SerializeField] private Sprite progressBarFillerSpriteUpgrade;
        [SerializeField] private Sprite progressBarFillerSpriteCommon;
        [SerializeField] private Sprite progressBarFillerSpriteRare;
        [SerializeField] private Sprite progressBarFillerSpriteEpic;
        [SerializeField] private Sprite progressBarFillerSpriteLegendary;
        [Space]
        [SerializeField] private Sprite levelImageSpriteCommon;
        [SerializeField] private Sprite levelImageSpriteRare;
        [SerializeField] private Sprite levelImageSpriteEpic;
        [SerializeField] private Sprite levelImageSpriteLegendary;

        public CardRarity _rarity;

        [System.Serializable]
        public struct RaritySprites
        {
            public Color32 Common;
            public Color32 Rare;
            public Color32 Epic;
            public Color32 Legendary;
        }

        [SerializeField]
        private RaritySprites RarityFrameSprites;

        internal void SetLevel(string v)
        {
            LevelText.text = v;
        }
        public void SetRarity(CardRarity rarity)
        {
            _rarity = rarity;
            switch (rarity)
            {
                case CardRarity.Common:
                    ProgressBarFill.sprite = progressBarFillerSpriteCommon;
                    if (levelImage != null)
                    {
                        levelImage.sprite = levelImageSpriteCommon;
                    }
                    break;
                case CardRarity.Rare:
                    ProgressBarFill.sprite = progressBarFillerSpriteRare;
                    if (levelImage != null)
                    {
                        levelImage.sprite = levelImageSpriteRare;
                    }
                    break;
                case CardRarity.Epic:
                    ProgressBarFill.sprite = progressBarFillerSpriteEpic;
                    if (levelImage != null)
                    {
                        levelImage.sprite = levelImageSpriteEpic;
                    }
                    break;
                case CardRarity.Legendary:
                    ProgressBarFill.sprite = progressBarFillerSpriteLegendary;
                    if (levelImage != null)
                    {
                        levelImage.sprite = levelImageSpriteLegendary;
                    }
                    break;
            }
        }

        public void UpgateProgressBarSlider()
        {
            SetRarity(_rarity);
        }


        internal void SetCanUpdate(bool? can_update, bool enoughMoney)
        {
            if (can_update.HasValue)
            {
                UpArrowImage.SetActive(can_update.Value);
                SlideLighter.SetActive(can_update.Value);
                ArrowJumping(can_update.Value && enoughMoney);

                if (can_update.Value == true)
                {
                    ProgressBarFill.sprite = progressBarFillerSpriteUpgrade;
                }
            }
        }
        
        internal void HideStaticLighter()
        {
            StaticLighter.SetActive(false);
        }

        internal void SetSlider(uint have, uint need)
        {
            ProgressValue.Set(have, true, need);
        }


        private Sequence sequence;
        private void ArrowJumping(bool runOrStop)
        {
            if (runOrStop)
            {
                if (sequence == null)
                {
                    sequence = DOTween.Sequence();
                    sequence.Append(Arrow.DOPunchPosition(Arrow.position.AddCoords(0, 7.5f, 0), 0.4f, 1, 1)).SetLoops(-1).SetEase(Ease.InQuad);
                    sequence.Append(Arrow.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 0.4f, 1, 1)).SetLoops(-1).SetEase(Ease.InQuad);
                }
                
            }
            else
            {
                if (sequence != null)
                {
                    sequence.Kill();
                }
            }
        }
    }
}
