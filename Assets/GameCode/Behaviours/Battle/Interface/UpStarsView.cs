using DG.Tweening;
using Legacy.Client;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using static EZCameraShake.CameraShaker;

public class UpStarsView : MonoBehaviour
{
    [Header("ShakeCamera Settings")]
    [SerializeField] ShakeSettings ShakeSettings;

    [Space]

    [SerializeField] private TextMeshProUGUI starsCount;
    [SerializeField] private GameObject additionalStar;
    [SerializeField] private Transform starEndPosition;
    [SerializeField] private Transform thirdStarEndPosition;
    [SerializeField] private AnimationCurve curve;
    private int previousCount = 0;
    private Sequence mySequence;

    void Start()
    {
        additionalStar.transform.position = starEndPosition.position;
        additionalStar.SetActive(false);
        mySequence = DOTween.Sequence();
    }

    public void SetStars(int starsCount, bool isEnemy)
    {
        if (previousCount != starsCount)
        {
            additionalStar.SetActive(true);
            PlayStarAnimation(!isEnemy, starsCount);
        }
        previousCount = starsCount;
    }

    public void PlayStarAnimation(bool isEnemy, int starsCount)
    {
        SetStartPosition(isEnemy);

        var endPos = starEndPosition.position;
        var centerscreenPosition = BattleInstanceInterface.instance.canvas.GetComponent<RectTransform>().position;

        ShakeSettings.Shake();

        ScaleFromBegin();
        TweenPosition(starsCount, endPos, centerscreenPosition);
    }

    private void ScaleFromBegin()
    {
        var tweenScale = additionalStar.transform.DOScale(1.65f, 0.9f).SetLoops(2, LoopType.Yoyo);
    }

    private void TweenPosition(int starsCount, Vector3 endPos, Vector3 centerscreenPosition)
    {
        var tweenPosition = additionalStar.transform.DOPath(new Vector3[] { centerscreenPosition, endPos }, 2f, PathType.CatmullRom).SetEase(curve);

        tweenPosition.OnComplete(() =>
        {
            TextChanges(starsCount);
            ScaleSmallPunk();
        });

        tweenPosition.OnUpdate(() =>
        {
            additionalStar.SetActive(true);
            var aimedEndPosition = Vector3.SqrMagnitude(additionalStar.transform.position - starEndPosition.position) <= 0.0001f;
            if (aimedEndPosition)
            {
                tweenPosition.Complete();
            }

            var aimedCenterScreenPosition = Vector3.SqrMagnitude(additionalStar.transform.position - centerscreenPosition) <= 0.1;
            if (aimedCenterScreenPosition)
            {
                BeginRotateFromCenter();
            }
        });
    }

    private void TextChanges(int starsCount)
    {
        this.starsCount.gameObject.GetComponent<Animator>().Play("TextPunk");
        this.starsCount.text = starsCount.ToString();
    }

    private void BeginRotateFromCenter()
    {
        additionalStar.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 360), 0.4f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
    }

    private void ScaleSmallPunk()
    {
        var tweenEndScalePunk = additionalStar.transform.DOScale(1.3f, 0.2f).SetLoops(2, LoopType.Yoyo);

        tweenEndScalePunk.OnUpdate(() =>
        {
            if (tweenEndScalePunk.isBackwards)
                tweenEndScalePunk.Complete();
        });

        tweenEndScalePunk.OnComplete(() =>
        {
            DisableOnEndAnimation();

        });
    }

    private void DisableOnEndAnimation()
    {
        additionalStar.SetActive(false);
    }

    private void SetStartPosition(bool isEnemy)
    {
        var flags = StaticColliders.instance.GetFlags(isEnemy);
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(flags.transform.position);
        Vector2 result;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(BattleInstanceInterface.instance.canvas.GetComponent<RectTransform>(), screenPoint, BattleInstanceInterface.instance.UICamera, out result);

        Reset(result);
    }

    private void Reset(Vector2 result)
    {
        var temp = additionalStar.transform.DOComplete(true);
        var temp1 = additionalStar.transform.DOKill(true);
        additionalStar.GetComponent<RectTransform>().localPosition = result;
        additionalStar.transform.localRotation = Quaternion.identity;
        additionalStar.transform.localScale = Vector3.one;
    }

    private Vector3 GetCenterScreenPosition()
    {
        Vector2 result;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(BattleInstanceInterface.instance.canvas.GetComponent<RectTransform>(), Vector3.zero, BattleInstanceInterface.instance.UICamera, out result);
        return result;
    }

    private IEnumerator LerpPostionCoroutine()
    {
        yield return null;
    }
}
