using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class HeroLvlBehaviour : MonoBehaviour
    {
        [SerializeField] private RectTransform Filler;
        [SerializeField] private GameObject BarObject;
        [SerializeField] private TextMeshProUGUI LvlText;
        [SerializeField] private TextMeshProUGUI ExpText;
        [SerializeField] private GameObject UpgradeArrow;
        [SerializeField] private GameObject effect;
        private Vector2 FillerAnchor = Vector2.zero;
        private Vector2 anchorCurrent;
        private float timer = 0f;
        private float allTime = 0f;
        private float particleoffset;
        private const int barPart = 15;

        private byte level;
        private byte maxLevel;
        public bool canUpgradeByLevel => level < maxLevel;
        public byte Level => level;

        private void Start()
        {
            particleoffset = 0;// (Filler.anchorMax.x / barPart);
        }
        internal void SetData(byte level)
        {
            this.level = level;
            maxLevel = ClientWorld.Instance.Profile.Level.level;
            LvlText.text = "<size=50%>" + Locales.Get("locale:775") + "</size> " + level.ToString()/* + "/" + maxLevel.ToString()*/;
            /*
            var nextExp = Settings.Instance.Get<HeroSettings>().GetExp(level);
            if (currentExp < nextExp)
            {
                ExpText.text = currentExp + "/" + nextExp;
            }
            else
            {
                ExpText.text = "";
                UpgradeArrow.SetActive(true);
            }

            var progress = (float)(currentExp) / (float)(nextExp);
            if (progress > 1) progress = 1;
            Vector2 NeedFillerAnchor = new Vector2(progress, Filler.anchorMax.y);
            this.FillerAnchor = NeedFillerAnchor;
            anchorCurrent = Filler.anchorMax;
            //   timer = Mathf.Abs(this.FillerAnchor.x - anchorCurrent.x) * 1.3f;
            timer = 0;
            allTime = timer;
            Filler.anchorMax = FillerAnchor;
            */
        }

        internal void ToggleBar(bool val)
        {
            BarObject.SetActive(val);
        }

        internal void ResetExp()
        {
            UpgradeArrow.SetActive(false);
        }

        public void AnimationToZero(float time)
        {
            FillerAnchor = new Vector2(0, Filler.anchorMax.y);
            timer = time;
            allTime = timer;
            anchorCurrent = Filler.anchorMax;

        }
        private void Update()
        {
            if (timer > 0)
            {
                Filler.anchorMax = Vector2.Lerp(anchorCurrent, FillerAnchor, 1 - GetAnchorPersentage(timer));
                timer -= Time.deltaTime;
                effect.SetActive(true);
                //   effect.transform.position = new Vector3(Filler.transform.position.x + Filler.anchorMax.x / 2 + particleoffset, Filler.transform.position.y, effect.transform.position.z);

            }
            else
            {
                effect.SetActive(false);
            }
        }
        private float GetAnchorPersentage(float remainTime)
        {
            var t = remainTime / allTime;
            return t;
        }
    }
}
