using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client {
    [RequireComponent(typeof(Image))]
    public class BlickControl : MonoBehaviour {
        [SerializeField] private        float       step = 0.1f;
        [SerializeField] private        float       min;
        [SerializeField] private        float       max;
        bool isUp = true;
        private        string      alphaName = "_Progress";
        private float currentAlpha = 0;
        [SerializeField] Material mat;
        public bool active = false;

        [SerializeField] bool isBump;

        public void Disable()
        {
            GetComponent<Image>().material = null;
            active = false;
        }
        public void Enable() {
            GetComponent<Image>().material = mat;
            if (isBump)
            {
                DOTween.Sequence()
                    .Append(transform.DOScale(1.0f, 0.15f))
                    .Append(transform.DOScale(1.1f, 0.3f))
                    .AppendInterval(1.0f)
                    .SetLoops(-1);
            }
            currentAlpha = min;
            active = true;
        }

        void Update() {
            if (active)
            {
                currentAlpha += step;
                if (currentAlpha > max)
                {
                    currentAlpha = min;
                }
                
                mat.SetFloat(alphaName, currentAlpha);
            }
        }
    }
}