using Legacy.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class RangeHitEffect : MonoBehaviour
    {
        public GameObject Bullet = null;
        public GameObject SecondBullet = null;
        public GameObject ChargeObject = null;
        public GameObject Explosion = null;

        public Transform HitStartPosition = null;
        public Transform SecondHitStartPosition = null;

        public Transform Target = null;
        private float Speed = 0f;
        internal float SecondBulletDelay = 0f;

        private void OnEnable()
        {
            if (Explosion)
            {
                Explosion.SetActive(false);
            }
        }

        public void Hit(Transform target, float speed)
        {
            Charge(false);
            Speed = speed;
            if (HitStartPosition)
            {
                //if (Bullet.GetComponent<Bullet>().Timer <= 0)
                    SpawnBullet(Bullet, Target, Speed, HitStartPosition);
            }
            if (SecondHitStartPosition && SecondBullet)
            {
                //if (SecondBullet.GetComponent<Bullet>().Timer <= 0)
                    StartCoroutine(BulletWithDelay(target));
            }
        }

        IEnumerator BulletWithDelay(Transform transform)
        {
            yield return new WaitForSeconds(SecondBulletDelay);
            SpawnBullet(SecondBullet, transform, Speed, SecondHitStartPosition);
        }

        private void SpawnBullet(GameObject bullet, Transform target, float speed, Transform start)
        {
            var bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent == null)
                Debug.LogError("No bullet component in " + bullet.name + ".!!!!!!!!!!!!!!!!!!!");

            var dmgPointTrans = target.GetComponent<DamageEffect>().GetDamagePoint();

            bulletComponent.Init(start.position, dmgPointTrans, speed, Explosion);
        }

        public void Charge(bool Switch)
        {
            if (ChargeObject != null)
            {
                ChargeObject.SetActive(Switch);
            }
            var ascalia = GetComponent<AscaliaBehaviour>();
            if (ascalia != null)
            {
                ascalia.ChargeBow(Switch);
            }
        }
    }
}
