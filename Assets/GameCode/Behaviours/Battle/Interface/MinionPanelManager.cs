using Legacy.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class MinionPanelManager : MinionHealthBar
    {
        [SerializeField, Range(80, 150)] byte BaseBarWidth = 110;
        public HealthSliderBehaviour slider;
        public GameObject LevelObject;

        public TextMeshProUGUI levelText;
        public Image levelImage;
        public GameObject PointIsInAgroRadius;

        public Sprite redSpriteLevel;


        public override void SetValue(float value, bool shouldView = true, GameObject heroGameObject = null, MinionLayerType _layer = MinionLayerType.Ground)
        {
            //Debug.Log("CurrentHP: " + value);
            //Debug.Log("MaxHP: " + panel.maxHP);
            //Debug.Log("CurrentHPNew: " + value);
            if (!slider.IsActive)
                slider.SetValue(1.0f);
            if (value == slider.CurrentValue) return;
            if (value == 0f)
            {
                if (slider.IsActive)
                {
                    DeActivateHealthBar();
                }
                return;
            }

            if (!slider.IsActive)
            {
                ActivateHealthBar(true);
            }
            if (shouldView)
            {
                //UnityEngine.Debug.LogError("shouldView " + shouldView);
                Flash(value);
            }
            slider.SetValue(value, false, shouldView, _layer);

        }

        private void Flash(float value)
        {
            if (value < slider.CurrentValue)
            {
                minionPanel.GetComponent<DamageEffect>().Punch();
            }
        }

        public override void ActivateHealthBar(bool flag)
        {
            float width = BaseBarWidth;
            if(colliderSize > 0)
            {
                width *= colliderSize;
            }
            slider.SetWidth(flag ? width : 0);

            slider.SetValue(1.0f);
            
            LevelObject.SetActive(minionPanel.IsEnemy || flag);

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }

        public override void DeActivateHealthBar()
        {
            slider.SetWidth(0);
        }

        public void SetExclamationPointIfIsInAgro(bool show)
        {
            if (PointIsInAgroRadius != null)
                PointIsInAgroRadius.SetActive(show);
        }

        public override void SetLevel(byte level)
        {
            levelText.text = level.ToString();
            if (minionPanel.IsEnemy)
            {
                LevelObject.SetActive(true);
                levelImage.sprite = redSpriteLevel;
                slider.SetRedSprites();
            }
        }

    }
}
