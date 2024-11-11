using UnityEngine;
using DG.Tweening;

public class UnexpectedPoonk : MonoBehaviour
{
    private Tweener sequence;
    protected void OnEnable()
    {
        DoJump();
    }

    private void OnDisable()
    {
        if (sequence != null)
            sequence.Kill();
    }

    private void DoJump()
    {
        if (sequence != null)
            sequence.Kill();
        float jumpValue = 0.1f;
        float jumpPower = 0.05f*2;
        int jumps = 3;
        float jumpDuration = 0.5f;

        Vector3 shakeStrength = new Vector3(0, jumpPower);
        sequence = transform.DOShakeScale(2f, shakeStrength, 5, 100);
        sequence.SetDelay(Random.Range(2, 4));
        sequence.onComplete += OnJump;
    }

    private void OnJump()
    {
        sequence.Kill();
        DoJump();
    }
}
