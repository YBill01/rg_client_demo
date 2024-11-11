using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class RewardGlowRotateTween : MonoBehaviour
{
    private Tweener punchTween;
    private TweenerCore<Quaternion, Vector3, QuaternionOptions> rotateTween;
    protected void OnEnable()
    {
        punchTween = transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 3f, 2, 1f);
        punchTween.SetEase(Ease.Linear);
        punchTween.SetLoops(-1, LoopType.Incremental);

        rotateTween = transform.DORotate(new Vector3(0, 0, 360f), 3.4f, RotateMode.FastBeyond360);
        rotateTween.SetEase(Ease.Linear);
        rotateTween.SetLoops(-1, LoopType.Incremental);
    }

    private void OnDisable()
    {
        if (punchTween != null)
            punchTween.Kill();
        if (rotateTween != null)
            rotateTween.Kill();
    }
}
