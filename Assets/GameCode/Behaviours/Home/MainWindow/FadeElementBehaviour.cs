using System;
using DG.Tweening;
using UnityEngine;

namespace Legacy.Client
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeElementBehaviour : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
        [SerializeField]
        UpPanelElementBehaviour upPanelElement;

        [SerializeField, Range(0.0f, 1.0f)] float FadeDuration = 0.15f;

        void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        internal void Enable(bool v)
        {
            canvasGroup.DOFade(v ? 1 : 0, FadeDuration);
            canvasGroup.blocksRaycasts = v;
        }

        internal void TryOff()
        {
            if (upPanelElement != null && !upPanelElement.IsActive())
            {
                Enable(false);
            }
        }
    }
}
