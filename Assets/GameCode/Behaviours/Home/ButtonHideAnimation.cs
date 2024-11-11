using UnityEngine;
using Legacy.Client;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class ButtonHideAnimation : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField, Range(0.0f, 1.0f)] float duration = 0.15f;

    internal void Enable(bool v , float speed=0f)
    {
        if (speed == 0f)
            speed = duration;
       var sequence = DOTween.Sequence();
            sequence.Append(image.DOFade(1, speed));
            sequence.Append(image.DOFade(0, speed));
        sequence.SetLoops(-1);
    }


}
