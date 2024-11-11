using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

namespace Legacy.Client
{
    public class SpeedUpSkilArrowController : MonoBehaviour
    {
        [SerializeField] private float timeToShowUp;
        [SerializeField] private Sprite Arrow_1;
        [SerializeField] private Sprite Arrow_2;

        private Image _image;
        private RectTransform _rect;
        private Vector3 _startScale;
        private Vector3 _punchScale;

        private void Start()
        {
            _rect = GetComponent<RectTransform>();
            _image = GetComponent<Image>();
            _startScale = _rect.localScale;
            _punchScale = _startScale.AddCoords(0.35f, 0.35f, 0);
            _rect.localScale = Vector3.zero;
            if (_image != null)
            {
                _image.enabled = false;
            }
        }
        public void ShowArrow(int level)
        {
            if (_image == null)
                return;

            if (_image.enabled)
            {
                HireArrow(() => ShowArrow(level));
            }
            else
            {
                if (level == 1)
                {
                    _image.sprite = Arrow_1;
                }
                else
                {
                    _image.sprite = Arrow_2;
                }

                _image.enabled = true;
                Sequence sequence = DOTween.Sequence();
                sequence.Append(_rect.DOScale(_punchScale, timeToShowUp * 0.75f)).SetEase(Ease.InQuart);
                sequence.Append(_rect.DOScale(_startScale, timeToShowUp * 0.25f)).SetEase(Ease.OutQuart);
            }
        }
        public void HireArrow()
        {
            if (_image == null)
                return;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_rect.DOScale(_punchScale, timeToShowUp * 0.25f)).SetEase(Ease.InQuart);
            sequence.Append(_rect.DOScale(Vector3.zero, timeToShowUp * 0.75f)).SetEase(Ease.OutQuart);
            sequence.OnComplete(() => _image.enabled = false);
        }
        public void HireArrow(Action method)
        {
            if (_image == null)
                return;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_rect.DOScale(_punchScale, timeToShowUp * 0.25f)).SetEase(Ease.InQuart);
            sequence.Append(_rect.DOScale(Vector3.zero, timeToShowUp * 0.75f)).SetEase(Ease.OutQuart);
            sequence.OnComplete(
                () => { _image.enabled = false; method(); });
        }
    }
}