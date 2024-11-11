using System.Collections;
using UnityEngine;

namespace Legacy.Client
{
    public class ParabolaBullet : Bullet
    {
        [SerializeField] [Range(0, 1)] protected float forwardMaximaShift   = 0.5f;
        [SerializeField] [Range(0, 1)] protected float sideMaximaShift      = 0.5f;
        [Space]
        [SerializeField] protected float maximaHightMult = 1;

        protected override IEnumerator BulletFlight()
        {
            var dist = Vector3.Distance(startPosition, targetTrans.position);
            var time = 0f;
            var flightTime = dist / speed;
            var mid = CalculateMiddlePos(dist);

            while (time < flightTime)
            {
                var progress    = time / flightTime;
                var startPos    = Vector3.Lerp(startPosition, mid, progress);
                var finisPos    = Vector3.Lerp(mid, targetTrans.position, progress);

                position        = Vector3.Lerp(startPos, finisPos, progress);
                transform.LookAt(finisPos);

                yield return null;
                time += Time.deltaTime;
            }
            OnBeforeDie();
        }

        private Vector3 CalculateMiddlePos(float dist)
        {
            var mid         = Vector3.Lerp(startPosition, targetTrans.position, forwardMaximaShift);
            var hight       = dist * maximaHightMult;
            var sideShift   = (hight * 0.5f) - (hight * sideMaximaShift);
            mid = mid.AddCoords(0, hight, sideShift);

            return mid;
        }
    }
}
